using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.API.DTOs.ReseñarDTOs;
using AppForSEII2526.API.DTOs.ReviewDTO;
using AppForSEII2526.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;

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
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ReseñarDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Get_Details_Review(int id)
        {
            if (_context.Reviews == null)
            {
                _logger.LogError("Error: Review table does not exist");
                return NotFound();
            }

            var review = await _context.Reviews
             .Where(p => p.Id == id)
                 .Include(p => p.ApplicationUser) //join table ApplicationUser
                 .Include(p => p.ReviewItems) //join table ReviewItem
                    .ThenInclude(pi => pi.Car) //then join table Car
                        .ThenInclude(c => c.Model) //then join table Model     
             .Select(p => new ReseñarDetailDTO(p.ApplicationUser.Name, p.Country, p.DriverType, p.ReviewItems
                        .Select(pi => new ReseñarItemDTO(pi.Car.Model.Name, pi.Car.Manufacturer, pi.Car.Color, pi.Rating, pi.Description)).ToList<ReseñarItemDTO>(), p.Created))
             .FirstOrDefaultAsync();


            if (review == null)
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
        public async Task<ActionResult> Create_Review(ReseñarForCreateDTO reseñaForCreate)
        {
            if (reseñaForCreate.ReviewItems.Count == 0) //compruebo que he seleccionado algún coche para comprar.
            {
                ModelState.AddModelError("ReviewItems", "Error! You must include at least one car to be reviewed");
            }

            if (reseñaForCreate.DriverType != "Novato" && reseñaForCreate.DriverType != "Experto") //compruebo que el tipo de conductor es correcto
            {
                ModelState.AddModelError("DriverType", "Error! DriverType must be 'Novato' or 'Experto'");
            }

            var user = _context.ApplicationUsers.FirstOrDefault(au => au.Name == reseñaForCreate.Name); //compruebo que el usuario que compra existe en la base de datos
            if (user == null)
            {
                ModelState.AddModelError("ReviewApplicationUser", "Error! UserName is not registered");
            }

            if (ModelState.ErrorCount > 0) //si tengo algún error acumulado, devuelve BadRequest
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            var reviewCars = reseñaForCreate.ReviewItems.Select(pi => pi.Modelo).ToList<string>();

            var cars = _context.Cars
                .Include(c => c.Model)
                .Where(c => reviewCars.Contains(c.Model.Name))
                .Select(c => new
                {
                    c.Id,
                    c.Model.Name
                })
                .ToList();

            Review review = new Review(reseñaForCreate.Country, DateTime.Now, reseñaForCreate.DriverType, new List<ReviewItem>(), user);

            foreach (var item in reseñaForCreate.ReviewItems)
            {
                var car = cars.FirstOrDefault(c => c.Name == item.Modelo);

                if (car == null)
                {
                    ModelState.AddModelError("ReviewItems", $"Error! The car {item.Modelo} does not exist, so you cannot create a review for this car");
                }
                else
                {
                    review.ReviewItems.Add(new ReviewItem(car.Id, item.Description, item.Rating, review));
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
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Review", $"Error! There was an error while saving your review, plese, try again later");
                return Conflict("Error" + ex.Message);
            }

            var reviewDetail = new ReseñarDetailDTO(review.ApplicationUser.Name,review.Country, review.DriverType, reseñaForCreate.ReviewItems,review.Created);

            return CreatedAtAction("Get_Details_Review", new { id = review.Id }, reviewDetail);
        }
    }
}