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


        //DETAILS: obtiene detalles de un alquiler
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<ActionResult> GetDetailsRental(int id)  //Se elige el alquiler por su ID
        {

            //MODIFICACION EN EL EXAMEN: Si el id es menor que 0, devuelvo NotFound
            if (id < 0)
            {
                return NotFound("ERROR: El id no puede ser menor que 0");
            }


            var rental = await _context.Rentals         //1. Busco el alquiler por ID, con todas las relaciones necesarias
                .Where(r => r.Id == id)
                .Include(r => r.ApplicationUser) 
                .Include(r => r.RentalItems) 
                    .ThenInclude(ri => ri.Car) 
                        .ThenInclude(c => c.Model)     
                .Select(r => new RentalDetailDTO(       //2. Convierto la entridad a DTO para enviar al cliente
                    r.Id, 
                    r.ApplicationUser.UserName, 
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
             .FirstOrDefaultAsync();                    //Devuelve null si no existe


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
        public async Task<ActionResult> CreateRental(RentalForCreateDTO rentalForCreate)
        {
            //VALIDACIONES INICIALES

            // 1. Fehca de inicio debe ser después a hoy
            if (rentalForCreate.StartDate <= DateTime.Today)
                ModelState.AddModelError("RentalDateFrom",
            "Error! Your rental date must start later than today");

            // 2. Compruebo que la fecha de fin del alquiler es posterior a la de inicio
            if (rentalForCreate.StartDate >= rentalForCreate.EndDate)
                ModelState.AddModelError("RentalDateFrom&RentalDateTo",
            "Error! Your rental must end later than it starts");

            // 3. Compruebo que hay al menos un coche seleccionado para alquilar
            if (rentalForCreate.RentalItems.Count == 0)
                ModelState.AddModelError("RentalItems",
            "Error! You must include at least one car to be rented");

            // Si hay errores -> devolver BadRequest
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // VALIDAR USUSARIO
            var user = _context.ApplicationUsers
                .FirstOrDefault(u => u.UserName == rentalForCreate.UserName);

            if (user == null)
            {
                var problemDetails = new ValidationProblemDetails();
                problemDetails.Errors.Add("RentalAplicationUser",
                    new string[] { "Error! El nombre de usuario no está registrado" });

                return BadRequest(problemDetails);
            }

            // OBTENER COCHES Y VER DISPONIBILIDAD
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
                    NumberOfRentedItems = c.RentalItems.Count(ri =>         //Numero de coches ya alquilados en el rango de fechas
                        ri.Rental.StartDate <= rentalForCreate.EndDate
                            && ri.Rental.EndDate >= rentalForCreate.StartDate)
                }).ToList();


            // CREAR ENTIDAD RENTAL
            var rental = new Rental(
                rentalForCreate.DeliveryCarDealer, 
                rentalForCreate.RentingDate = DateTime.Now, 
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

                if ((car == null) || ((car.NumberOfRentedItems + item.Quantity) >= car.QuantityForRenting))     //Si no existe o no hay cantidad suficiente
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

            // GUARDO EN LA BASE DE DATOS
            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            //CREAR EL DTO (detalle) QUE DEVUELVE LA API
            var rentalDetail = new RentalDetailDTO(
                rental.Id,
                rental.ApplicationUser.UserName,
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



