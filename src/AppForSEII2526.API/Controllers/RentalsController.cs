using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.API.DTOs.RentalDTOs;
using AppForSEII2526.Models;           // Entidades de dominio: Car, Model, Purchase, PurchaseItem, PaymentMethod
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
             .Select(r => new RentalDetailDTO(r.ApplicationUser.Name, r.ApplicationUser.Surname, r.DeliveryCarDealer, r.PaymentMethod, r.StartDate, r.EndDate, r.RentingDate, r.TotalPrice, r.RentalItems
                        .Select(ri => new RentalItemDTO(ri.CarId, ri.Car.Model, ri.Car.Manufacturer, ri.Car.RentingPrice, ri.Quantity)).ToList<RentalItemDTO>()))
             .FirstOrDefaultAsync();


            if (rental == null)
            {
                _logger.LogError($"Error: Rental with id {id} does not exist");
                return NotFound();
            }


            return Ok(rental);
        }
    }
}


