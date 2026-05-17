using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.API.DTOs.ReseñarDTOs;
using AppForSEII2526.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(ApplicationDbContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogInformation("Controller 'ReviewsController' inicializado");
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ReseñarDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetDetailsReview(int id)
        {
            if (_context.Reviews == null)
            {
                _logger.LogError("Error: Review table does not exist");
                return NotFound();
            }

            var review = await _context.Reviews
    .Where(p => p.Id == id)
    .Include(p => p.ReviewItems)
        .ThenInclude(pi => pi.Car)
        .ThenInclude(c => c.Model)
    .Select(p => new ReseñarDetailDTO(
        p.Id,
        p.UserName,
        "",
        p.UserName,
        p.Country,
        p.DriverType,
        p.Created,
        p.ReviewItems
            .Select(pi => new ReseñarItemDTO(
                pi.Car.Model.Name,
                pi.Car.Manufacturer,
                pi.Car.Color,
                pi.Rating,
                pi.Description))
            .ToList()))
    .FirstOrDefaultAsync();

            if (review == null) //Si el id de la review no existe lanzo un error
            {
                _logger.LogError($"Error: Review with id {id} does not exist");
                return NotFound();
            }


            return Ok(review);
        }

        [HttpPost] //operación de creación
        [Route("[action]")] //operación de tipo acción
        [ProducesResponseType(typeof(ReseñarDetailDTO), (int)HttpStatusCode.Created)] //devuelve OK cuando consigue meter en la base de datos el código
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)] //devuelve BadRequest cuando hay un error durante la comprobación de la petición
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)] //devuelve Conflict cuando hay un error al añadir a la base de datos
        public async Task<ActionResult> CreateReview(ReseñarForCreateDTO reseñaForCreate)
        {
            if (reseñaForCreate.ReviewItems.Count == 0) //compruebo que he seleccionado algún coche para comprar.
            {
                ModelState.AddModelError("ReviewItems", "Error! You must include at least one car to be reviewed");
                _logger.LogError($"ReviewsController || Error! You must include at least one car to be reviewed");
            }

            if (reseñaForCreate.DriverType != "Novato" && reseñaForCreate.DriverType != "Experto") //compruebo que el tipo de conductor es correcto
            {
                ModelState.AddModelError("DriverType", "Error! DriverType must be 'Novato' or 'Experto'");
                _logger.LogError($"ReviewsController || Error! DriverType must be 'Novato' or 'Experto'");
            }
            foreach (var item in reseñaForCreate.ReviewItems)
            {
                if (item.Description != null && !item.Description.StartsWith("Reseña para"))
                {
                    ModelState.AddModelError("Description", "Error! La reseña debe empezar por Reseña para");
                    _logger.LogError($"Description || Error! La reseña debe empezar por Reseña para");
                }

            }

            if (ModelState.ErrorCount > 0) //si tengo algún error acumulado, devuelve BadRequest
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            var reviewCars = reseñaForCreate.ReviewItems.Select(pi => pi.Model).ToList<string>(); //hago una lista con los modelos de los coches que le he pasado


            var cars = _context.Cars
                .Include(c => c.Model)
                .Where(c => reviewCars.Contains(c.Model.Name))
                .Select(c => new
                {
                    c.Id,
                    c.Model.Name
                })
                .ToList();

            Review review = new Review
            {
                Country = reseñaForCreate.Country,
                Created = DateTime.Now,
                DriverType = reseñaForCreate.DriverType,
                ReviewItems = new List<ReviewItem>(),
                UserName = string.IsNullOrWhiteSpace(reseñaForCreate.UserName)
                ? "Invitado"
        :       reseñaForCreate.UserName,
            };

            foreach (var item in reseñaForCreate.ReviewItems)
            {
                var car = cars.FirstOrDefault(c => c.Name == item.Model);

                if (car == null)
                {
                    ModelState.AddModelError("ReviewItems", $"Error! The car {item.Model} does not exist, so you cannot create a review for this car");
                    _logger.LogError($"ReviewsController || Error! The car {item.Model} does not exist, so you cannot create a review for this car");
                }
                else
                {
                    review.ReviewItems.Add(new ReviewItem
                    {
                        CarId = car.Id,
                        Description = item.Description,
                        Rating = item.Rating,
                        Review = review
                    });
                }
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Add(review);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Review", $"Error! There was an error while saving your review, plese, try again later");
                _logger.LogError($"ReviewsController || Error! {ex.Message}");
                return Conflict("Error" + ex.Message);
            }

            var reviewDetail = new ReseñarDetailDTO(
    review.Id,
    review.UserName,
    "",
    review.UserName,
    review.Country,
    review.DriverType,
    review.Created,
    reseñaForCreate.ReviewItems
);

            return CreatedAtAction(nameof(GetDetailsReview), new { id = review.Id }, reviewDetail);

        }
    }
}