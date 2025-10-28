using AppForSEII2526.API.DTOs.RentalDTOs;
using AppForSEII2526.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateRental(RentalForCreateDTO rentalForCreate)
        {
            //La fecha de inicio del alquiler debe ser posterior a la fecha actual
            if (rentalForCreate.StartDate <= DateTime.Today)
                ModelState.AddModelError("RentalDateFrom", "Error! Your rental date must start later than today");

            //La fecha de fin del alquiler debe ser posterior a la fecha de inicio
            if (rentalForCreate.StartDate >= rentalForCreate.EndDate)
                ModelState.AddModelError("RentalDateFrom&RentalDateTo", "Error! Your rental must end later than it starts");

            //Hay que seleccionar al menos un coche para alquilar
            if (rentalForCreate.RentalItems.Count == 0)
                ModelState.AddModelError("RentalItems", "Error! You must include at least one car to be rented");

            //Comprobar que el usuario está registrado
            var user = _context.ApplicationUsers.FirstOrDefault(au => au.UserName == rentalForCreate.Name);
            if (user == null)
                ModelState.AddModelError("RentalApplicationUser", "Error! UserName is not registered");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            var carManufacturer = rentalForCreate.RentalItems.Select(ri => ri.Manufacturer).ToList<string>();

            var movies = _context.Cars.Include(m => m.RentalItems)
                .ThenInclude(ri => ri.Rental)
                .Where(m => carManufacturer.Contains(m.Manufacturer))

                //we use an anonymous type https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types
                .Select(c => new {
                    c.Id,
                    c.CarClass,
                    c.FuelType,
                    c.Manufacturer,
                    c.RentingPrice,
                    c.Color,
                    //we count the number of rentalItems that are within the rental period
                    NumberOfRentedItems = c.RentalItems.Count(ri => ri.Rental.StartDate <= rentalForCreate.EndDate
                            && ri.Rental.EndDate >= rentalForCreate.StartDate)
                })
                .ToList();


            Rental rental = new Rental(rentalForCreate.Name, rentalForCreate.Surname,
                user, rentalForCreate.DeliveryAddress, DateTime.Now,
                (AppForSEII2526.API.Models.PaymentMethod)rentalForCreate.PaymentMethod,
rentalForCreate.StartDate, rentalForCreate.EndDate, new List<RentalItem>());


            rental.TotalPrice = 0;
            var numDays = (rental.EndDate - rental.StartDate).TotalDays;


            foreach (var item in rentalForCreate.RentalItems)
            {
                var car = car.FirstOrDefault(m => m.Title == item.Title);
                //we must check that there is enough quantity to be rented in the database
                if ((car == null) || (car.NumberOfRentedItems >= car.QuantityForRenting))
                {
                    ModelState.AddModelError("RentalItems", $"Error! Movie titled '{item.Title}' is not available for being rented from {rentalForCreate.StartDate.ToShortDateString()} to {rentalForCreate.EndDate.ToShortDateString()}");
                }
                else
                {
                    // rental does not exist in the database yet and does not have a valid Id, so we must relate rentalitem to the object rental
                    rental.RentalItems.Add(new RentalItem(car.Id, rental, car.RentingPrice, car.Description));
                    item.RentingPrice = car.RentingPrice;
                }
            }
            rental.TotalPrice = rental.RentalItems.Sum(ri => ri.RentingPrice * numDays);


            //if there is any problem because of the available quantity of movies or because the movie does not exist
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

            //it returns rentalDetail
            var rentalDetail = new RentalDetailDTO(rental.Id, rental.customerName,
                rental.Name, rental.Surname,
                rental.Address, rentalForCreate.PaymentMethod,
                rental.StartDate, rental.EndDate,
                rentalForCreate.RentalItems);

            return CreatedAtAction("GetRental", new { id = rental.Id }, rentalDetail);
        }
    }

}
    
