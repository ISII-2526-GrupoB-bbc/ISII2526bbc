using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.API.DTOs.RentalDTOs;
using AppForSEII2526.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        //Devuelve todos los detalles de un alquiler: METODO DETAILS
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<ActionResult> Get_Details_Rental(int id)  //Se elige el alquiler por su ID
        {
            var rental = await _context.Rentals
                .Where(r => r.Id == id)
                .Include(r => r.ApplicationUser) 
                .Include(r => r.RentalItems) 
                    .ThenInclude(ri => ri.Car) 
                        .ThenInclude(c => c.Model)     
                .Select(r => new RentalDetailDTO(
                    r.Id, 
                    r.ApplicationUser.Name, 
                    r.ApplicationUser.Surname, 
                    r.DeliveryCarDealer, 
                    r.PaymentMethod, 
                    r.StartDate, 
                    r.EndDate, 
                    r.RentingDate,
                    r.RentingPrice,
                    r.RentalItems.Select(ri => 
                        new RentalItemDTO(
                            ri.CarId, 
                            ri.Car.Model.Id, 
                            ri.Car.Model.Name,
                            ri.Car.Manufacturer, 
                            ri.Car.RentingPrice, 
                            ri.Quantity))
                    .ToList<RentalItemDTO>()))
             .FirstOrDefaultAsync();


            if (rental == null)
            {
                _logger.LogError($"Error: Rental with id {id} does not exist");
                return NotFound();
            }


            return Ok(rental);
        }


        //METODO POST: Crear un nuevo alquiler
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> Create_Rental(RentalForCreateDTO rentalForCreate)
        {
            //VALIDACIONES BÁSICAS
            //Compruebo que la fecha de inicio es posterior a la de hoy
            if (rentalForCreate.EndDate <= DateTime.Today)
                ModelState.AddModelError("RentalDateFrom", "Error! Your rental date must start later than today");

            //Compruebo que la fecha de fin del alquiler es posterior a la de inicio
            if (rentalForCreate.StartDate >= rentalForCreate.EndDate)
                ModelState.AddModelError("RentalDateFrom&RentalDateTo", "Error! Your rental must end later than it starts");

            //Compruebo que hay al menos un coche seleccionado para alquilar
            if (rentalForCreate.RentalItems.Count == 0)
                ModelState.AddModelError("RentalItems", "Error! You must include at least one car to be rented");

            var user = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == rentalForCreate.CustomerName);
            if (user == null)
                ModelState.AddModelError("RentalAplicationUser", "Error! El nombre de usuario no está registrado");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            //LISTA DE MODELOS SOLICITADOS PARA ALQUILAR
            var carModelNames = rentalForCreate.RentalItems.Select(ri => ri.ModelName).ToList();

            var cars = _context.Cars
                .Include(c => c.RentalItems)
                .ThenInclude(ri => ri.Rental)
                .Where(c => carModelNames.Contains(c.Model.Name))
                .Select(c => new
                {
                    c.Id,
                    ModelId = c.Model.Id,
                    ModelName = c.Model.Name,
                    c.RentingPrice,
                    c.QuantityForRenting,
                    NumberOfRentedItems = c.RentalItems.Count(ri => 
                        ri.Rental.StartDate <= rentalForCreate.EndDate
                            && ri.Rental.EndDate >= rentalForCreate.StartDate)
                }).ToList();


            var rental = new Rental(
                rentalForCreate.DeliveryCarDealer, 
                rentalForCreate.RentingDate, 
                rentalForCreate.EndDate, 
                rentalForCreate.PaymentMethod, 
                rentalForCreate.StartDate, 
                new List<RentalItem>(), 
                user);

            var numDays = (rental.EndDate - rental.StartDate).TotalDays;


            //PROCESAR CADA COCHE SELECCIONADO
            foreach (var item in rentalForCreate.RentalItems)
            {
                var car = cars.FirstOrDefault(c => c.ModelName == item.ModelName);

                if ((car == null) || ((car.NumberOfRentedItems + item.Quantity) >= car.QuantityForRenting))
                {
                    ModelState.AddModelError("RentalItems", $"Error! Car '{item.ModelName}' is not available.");
                }
                else
                {
                    rental.RentalItems.Add(new RentalItem(car.Id, item.Quantity, rental));
                    rental.RentingPrice += car.RentingPrice * item.Quantity * (decimal)numDays;
                }
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            //CREAR EL DTO QUE DEVUELVE LA API
            var rentalDetail = new RentalDetailDTO(
                rental.Id,
                rental.ApplicationUser.Name,
                rental.ApplicationUser.Surname,
                rental.DeliveryCarDealer,
                rental.PaymentMethod,
                rental.StartDate,
                rental.EndDate,
                rental.RentingDate,
                rental.RentingPrice,
                rental.RentalItems.Select(ri =>
                    new RentalItemDTO(
                        ri.CarId,
                        ri.Car.Model.Id,
                        ri.Car.Model.Name,
                        ri.Car.Manufacturer,
                        ri.Car.RentingPrice,
                        ri.Quantity
                    )).ToList()
            );
            return CreatedAtAction("Get_Details_Rental", new { id = rental.Id }, rentalDetail);
        }
    }
}



