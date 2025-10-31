using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.API.DTOs.RentalDTOs;
using AppForSEII2526.Models;           // Entidades de dominio: Car, Model, Purchase, PurchaseItem, PaymentMethod
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;


namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RentalsController> _logger;

        public RentalsController(ApplicationDbContext context, ILogger<RentalsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        //Devuelve todos los detalles de un alquiler:
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Get_Details_Rental(int id)
        {
            if (_context.Rentals == null)
            {
                _logger.LogError("Error: Rental table does not exist");
                return NotFound();
            }

            var rental = await _context.Rentals
             .Where(r => r.Id == id)
                 .Include(r => r.ApplicationUser) 
                 .Include(r => r.RentalItems) 
                    .ThenInclude(ri => ri.Car) 
                        .ThenInclude(c => c.Model)     
             .Select(r => new RentalDetailDTO(r.ApplicationUser.Name, r.ApplicationUser.Surname, r.DeliveryCarDealer, r.PaymentMethod, r.StartDate, r.EndDate, r.RentingDate, r.RentingPrice, r.RentalItems
                        .Select(ri => new RentalItemDTO(ri.CarId, ri.Car.Model, ri.Car.Manufacturer, ri.Car.RentingPrice, ri.Quantity)).ToList<RentalItemDTO>()))
             .FirstOrDefaultAsync();


            if (rental == null)
            {
                _logger.LogError($"Error: Rental with id {id} does not exist");
                return NotFound();
            }


            return Ok(rental);
        }


        //Crea un nuevo alquiler a partir de los coches seleccionados:
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> Create_Rental(RentalForCreateDTO rentalForCreate)
        {
            //Compruebo que la fecha de inicio es posterior a hoy
            if (rentalForCreate.StartDate <= DateTime.Today)
                ModelState.AddModelError("RentalDateFrom", "Error! Your rental date must start later than today");

            //Compruebo que la fecha de fin del alquiler es posterior a la de inicio
            if (rentalForCreate.StartDate >= rentalForCreate.EndDate)
                ModelState.AddModelError("RentalDateFrom&RentalDateTo", "Error! Your rental must end later than it starts");

            //Compruebo que hay al menos un coche seleccionado para alquilar
            if (rentalForCreate.RentalItems.Count == 0)
                ModelState.AddModelError("RentalItems", "Error! You must include at least one car to be rented");

            //if (!_context.ApplicationUser.Any(au=>au.UserName == rentalFromCreate.CustomerUserName))
            var user = _context.ApplicationUsers.FirstOrDefault(au => au.UserName == rentalForCreate.Name);
            if (user == null)
                ModelState.AddModelError("RentalAplicationUser", "Error! UserName is not registered");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var carModels = rentalForCreate.RentalItems.Select(ri => ri.Model.Name).ToList<string>();

            var cars = _context.Cars.Include(r => r.RentalItems)
                .ThenInclude(ri => ri.Car)
                .ThenInclude(c => c.Model)
                .Where(r => carModels.Contains(r.Model.Name))
                .Select(c => new
                {
                    c.Id,
                    c.Model.Name,
                    c.RentingPrice,
                    c.QuantityForRenting,
                    NumberOfRentedItems = c.RentalItems.Count(ri => ri.Rental.StartDate <= rentalForCreate.EndDate
                            && ri.Rental.EndDate >= rentalForCreate.StartDate)
                }
                ).ToList();
            Rental rental = new Rental(rentalForCreate.DeliveryAddress, rentalForCreate.RentingDate, rentalForCreate.EndDate, rentalForCreate.PaymentMethod, rentalForCreate.StartDate, new List<RentalItem>(), user);

            rental.RentingPrice = 0;
            var numDays = (rental.EndDate - rental.StartDate).TotalDays;

            foreach (var item in rentalForCreate.RentalItems)
            {
                var car = cars.FirstOrDefault(c => c.Name == item.Model.Name);
                //Comprobar si hay cantidad suficiente para alquilar
                if ((car == null) || ((car.NumberOfRentedItems + item.Quantity) >= car.QuantityForRenting))
                {
                    ModelState.AddModelError("RentalItems", $"Error! Car '{item.Model}' is not available for being rented from {rentalForCreate.StartDate.ToShortDateString()} to {rentalForCreate.EndDate.ToShortDateString()}");
                }
                else
                {
                    // rental does not exist in the database yet and does not have a valid Id, so we must relate rentalitem to the object rental
                    rental.RentalItems.Add(new RentalItem(car.Id, item.Quantity, rental));
                    item.RentingPrice = car.RentingPrice;
                    rental.RentingPrice += car.RentingPrice * item.Quantity * (decimal)numDays;
                }
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Add(rental);

            try
            {
                //we store in the database both rental and its rentalitems
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Rental", $"Error! There was an error while saving your rental, plese, try again later");
                return Conflict("Error" + ex.Message);

            }

            var rentalDetail = new RentalDetailDTO(rental.ApplicationUser.Name, rental.ApplicationUser.Surname, rental.DeliveryCarDealer, rental.PaymentMethod, rental.StartDate, rental.EndDate, rental.RentingDate, rental.RentingPrice, rentalForCreate.RentalItems.ToList());

            return CreatedAtAction("Get_Details_Rental", new { id = rental.Id }, rentalDetail);
        }

    }

}



