using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        // used to enable your controllers to access the database
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CarsController> _logger;

        public CarsController(ApplicationDbContext context, ILogger<CarsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<CocheParaReviewDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<CocheParaReviewDTO>>> GetCars(string? FuelType = null)
        {
            try
            {
                var query = _context.Cars.AsQueryable();

                // Aplicar filtro SOLO por color (flujo alternativo)
                if (!string.IsNullOrEmpty(FuelType))
                    query = query.Where(c => c.FuelType.Contains(FuelType));

                var cars = await query
                    .Select(c => new CocheParaReviewDTO(
                        c.Id,
                        c.Model.Name,
                        c.Color,
                        c.CarClass,
                        c.Manufacturer,
                        c.FuelType
                    ))
                    .ToListAsync();

                if (cars == null || !cars.Any())
                    return NotFound("No se encontraron coches con ese tipo de combustible.");

                return Ok(cars);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now}: Error en GetCars() - {ex.Message}");
                return BadRequest("Error al obtener los coches disponibles.");
            }
        }
    }
}
