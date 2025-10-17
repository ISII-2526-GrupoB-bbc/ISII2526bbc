using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsControllers : ControllerBase
    {
        //Un controlador para la base de datos
        private readonly ApplicationDbContext _context;
        //Un log de problemas
        private readonly ILogger<CarsControllers> _logger;

        public CarsControllers(ApplicationDbContext context, ILogger<CarsControllers> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<CocheParaComprarDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<CocheParaComprarDTO>>> GetCars(string? color = null)
        {
            try
            {
                var query = _context.Cars.AsQueryable();

                // Aplicar filtro SOLO por color (flujo alternativo)
                if (!string.IsNullOrEmpty(color))
                    query = query.Where(c => c.Color.Contains(color));

                var cars = await query
                    .Select(c => new CocheParaComprarDTO(
                        c.Id,
                        c.Model.Name,
                        c.Color,
                        c.CarClass,
                        c.Manufacturer,
                        c.PurchasingPrice
                    ))
                    .ToListAsync();

                if (cars == null || !cars.Any())
                    return NotFound("No se encontraron coches con ese color.");

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
