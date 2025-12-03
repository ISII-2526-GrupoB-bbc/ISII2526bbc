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
            _logger.LogInformation("CarsController initialized.");
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<CocheParaComprarDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<CocheParaComprarDTO>>> GetCars(string? color = null,string? model=null)
        {
            _logger.LogInformation("Petición GET /api/Cars/GetCars iniciada.");
            try
            {
                var query = _context.Cars.AsQueryable();

                // Aplicar filtro SOLO por color (flujo alternativo)
                if (!string.IsNullOrEmpty(color))
                {
                    _logger.LogDebug("Aplicando filtro de color: {color}", color);
                    query = query.Where(c => c.Color.Contains(color));
                }

                // NUEVO → filtro por modelo si llega
                if (!string.IsNullOrEmpty(model))
                {
                    _logger.LogDebug("Aplicando filtro de modelo: {model}", model);
                    query = query.Where(c => c.Model.Name.Contains(model));
                }

                var cars = await query
                    .Select(c => new CocheParaComprarDTO(
                        c.Id,
                        c.Model.Name,
                        c.Color,
                        c.FuelType,
                        c.Manufacturer,
                        c.PurchasingPrice
                    ))
                    .ToListAsync();

                if (cars == null || !cars.Any())
                {
                    _logger.LogWarning("No se encontraron coches con el color especificado: {color}", color);
                    return NotFound("No se encontraron coches con ese color.");
                }

                    _logger.LogInformation("Se recuperaron {count} coches correctamente.", cars.Count);
                    return Ok(cars);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now}: Error en GetCars() - {ex.Message}");
                return BadRequest("Error al obtener los coches disponibles.");
            }
        }


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<CocheParaReviewDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetCarsForReview(string? filtroManufacturer, string? filtroFuelType)
        {
            var cars = await _context.Cars
               .Include(c => c.Model)
               .Where(c => (c.Manufacturer.Contains(filtroManufacturer) || filtroManufacturer == null) && (c.FuelType.Contains(filtroFuelType) || (filtroFuelType == null)))
               .OrderBy(c => c.Id)
               .Select(c => new CocheParaReviewDTO(c.Id, c.Model.Name, c.CarClass, c.Manufacturer, c.FuelType, c.Color))
               .ToListAsync();
            _logger.LogInformation($"CarsController || Coches para reseñar encontrados con los parametros {filtroManufacturer} y {filtroFuelType}");
            return Ok(cars);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<CocheParaAlquilarDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<CocheParaAlquilarDTO>>> GetCarsRental(decimal? rentingPrice, string? modelName)
        {
            try
            {
                var query = _context.Cars.AsQueryable();

                if (rentingPrice.HasValue)
                    query = query.Where(c => c.RentingPrice == rentingPrice.Value);

                if (!string.IsNullOrEmpty(modelName))
                    query = query.Where(c => c.Model.Name.Contains(modelName));

                var cars = await query
                    .Select(c => new CocheParaAlquilarDTO(
                        c.Id,
                        c.Model.Name,
                        c.FuelType,
                        c.Manufacturer,
                        c.RentingPrice,
                        c.Color
                    ))
                    .ToListAsync();

                if (cars == null || !cars.Any())
                {
                    return NotFound("No se encontraron coches con los filtros proporcionados.");
                }

                return Ok(cars);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener los coches.");
            }
        }



        // Devuelve la lista de fueltypes
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetFuelTypes()
        {
            try
            {
                var fuelTypes = await _context.Cars
                    .Select(c => c.FuelType)
                    .Distinct()
                    .ToListAsync();

                return Ok(fuelTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now}: Error en GetFuelTypes() - {ex.Message}");
                return BadRequest("Error al obtener los tipos de combustible.");
            }
        }

        // Devuelve la lista de modelos
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<string>>> GetModels(string? modelName = null)
        {
            try
            {
                // Obtengo todos los nombres de modelos disponibles
                var query = _context.Cars
                                    .Include(c => c.Model)
                                    .Select(c => c.Model.Name)
                                    .Distinct()
                                    .AsQueryable();

                // Filtro opcional por nombre (si lo envías desde el cliente)
                if (!string.IsNullOrEmpty(modelName))
                {
                    query = query.Where(m => m.Contains(modelName));
                }

                var models = await query.ToListAsync();

                if (!models.Any())
                {
                    return NotFound("No se encontraron modelos.");
                }

                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now}: Error en GetModels() - {ex.Message}");
                return BadRequest("Error al obtener los modelos disponibles.");
            }
        }


    }
}

