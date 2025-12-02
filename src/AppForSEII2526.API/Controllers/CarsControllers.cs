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
        public async Task<ActionResult<IEnumerable<CocheParaComprarDTO>>> GetCars(string? color = null)
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
        public async Task<ActionResult> GetCars2(string? filtroManufacturer, string? filtroFuelType)
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
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<ActionResult<IEnumerable<CocheParaAlquilarDTO>>> GetCars3(decimal? RentingPrice = null)
        {
            _logger.LogInformation("Petición GET /api/Cars/GetCars3 iniciada.");    //Log de inicio de la petición

            try
            {
                var query = _context.Cars.AsQueryable();

                // Aplico filtro por RentingPrice (precio de alquiler)
                if (RentingPrice.HasValue)
                {
                    _logger.LogDebug("Aplicando filtro de RentingPrice: {price}", RentingPrice.Value);  //Log para depurar el valor del filtro
                    query = query.Where(c => c.RentingPrice == RentingPrice.Value);
                }
                //Si quiero que me de los coches con precio menor o igual, cambiar "==" por "<="

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
                    _logger.LogWarning("No se encontraron coches con RentingPrice = {price}", RentingPrice);
                    return NotFound("No se encontraron coches con ese precio de alquiler.");
                }

                _logger.LogInformation("Se recuperaron {count} coches correctamente.", cars.Count);
                return Ok(cars);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now}: Error en GetCars3() - {ex.Message}");
                return BadRequest("Error al obtener los coches disponibles.");
            }
        }
    }





}

