using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.API.DTOs.ComprarDTOs;
using AppForSEII2526.Models;           // Entidades de dominio: Car, Model, Purchase, PurchaseItem, PaymentMethod
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PurchasesController> _logger;

        public PurchasesController(ApplicationDbContext context, ILogger<PurchasesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // -----------------------------------------------------------
        // GET api/purchases/get_details_purchase?id=123
        // Devuelve el detalle completo de una compra existente.
        // -----------------------------------------------------------
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ComprarForDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Get_Details_Purchase(int id)
        {
            // Validación defensiva: comprobar que existe el DbSet
            if (_context.Purchases == null)
            {
                _logger.LogError("Error: Purchases table does not exist");
                return NotFound();
            }

            // Cargamos la compra + sus líneas (PurchaseItems).
            // OJO: en tu modelo PurchaseItem no hay navegación a Car,
            // por eso NO hay ThenInclude(pi => pi.Car) aquí.
            var purchase = await _context.Purchases
                .Where(p => p.Id == id)
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync();

            if (purchase == null)
            {
                _logger.LogError($"Error: Purchase with id {id} does not exist");
                return NotFound();
            }

            // Reunimos los Ids de coches presentes en las líneas
            // (si tu FK se llamara 'Card', cambia pi.CarId por pi.Card).
            var carIds = purchase.PurchaseItems
                .Select(pi => pi.CarId /* o pi.Card */)
                .Distinct()
                .ToList();

            // Traemos esos coches con su Model para poder proyectar al DTO
            var cars = await _context.Cars
                .Include(c => c.Model)
                .Where(c => carIds.Contains(c.Id))
                .ToListAsync();

            // Diccionario para resolver rápido un coche por Id
            var carById = cars.ToDictionary(c => c.Id);

            // Proyección de cada línea de compra a ComprarForItemDTO
            var items = purchase.PurchaseItems.Select(pi =>
            {
                var car = carById[pi.CarId /* o pi.Card */];
                return new ComprarForItemDTO(
                    id: car.Id,
                    model: car.Model,         // Pasamos el Model completo porque así lo pide tu DTO
                    color: car.Color,
                    purchasingPrice: car.PurchasingPrice, // Precio actual del coche
                    quantity: pi.Quantity
                );
            }).ToList();

            // La entidad Purchase guarda PaymentMethod como string.
            // Convertimos a enum para encajar con el DTO.
            if (!Enum.TryParse<PaymentMethod>(purchase.PaymentMethod, ignoreCase: true, out var pm))
                pm = default; // En caso de valor inesperado, cae al valor por defecto del enum.

            // Construimos el DTO de detalle usando el constructor que definiste
            var detail = new ComprarForDetailDTO(
                id: purchase.Id,
                purchasingDate: purchase.PurchasingDate,
                name: purchase.Name,
                surname: purchase.Surname,
                address: purchase.DeliveryCarDealer,
                paymentMethod: pm,
                cochesComprados: items
            );

            return Ok(detail);
        }

        // -----------------------------------------------------------
        // POST api/purchases/create_purchase
        // Crea una nueva compra a partir de los coches seleccionados.
        // -----------------------------------------------------------
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ComprarForDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> Create_Purchase(ComprarForCreateDTO compra)
        {
            // --- Validaciones de entrada (además de DataAnnotations) ---

            // Debe venir al menos un coche
            if (compra.CochesComprados == null || compra.CochesComprados.Count == 0)
                ModelState.AddModelError(nameof(compra.CochesComprados), "Error! Debe incluir al menos un coche a comprar.");

            // Cada coche debe tener cantidad >= 1
            if (compra.CochesComprados?.Any(i => i.Quantity <= 0) == true)
                ModelState.AddModelError("Quantity", "Error! Cada coche debe tener una cantidad >= 1.");

            // Método de pago debe ser válido (enum)
            if (!Enum.IsDefined(typeof(PaymentMethod), compra.PaymentMethod))
                ModelState.AddModelError(nameof(compra.PaymentMethod), "Error! Método de pago no válido.");

            // Si hay errores de validación, cortamos y devolvemos 400 con el detalle
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // --- Cargar los coches referenciados por Id desde BD ---

            // Tu DTO trae el Id del coche en cada item
            var carIds = compra.CochesComprados.Select(i => i.Id).Distinct().ToList();

            // Traemos los coches con su Model (para mostrar luego en la respuesta)
            var cars = await _context.Cars
                .Include(c => c.Model)
                .Where(c => carIds.Contains(c.Id))
                .ToListAsync();

            // --- Crear la entidad Purchase a partir del DTO ---

            var purchase = new Purchase
            {
                Name = compra.Name,
                Surname = compra.Surname,
                DeliveryCarDealer = compra.Address,          // Dirección de entrega
                PaymentMethod = compra.PaymentMethod.ToString(), // Guardamos como string en la entidad
                PurchasingDate = DateTime.Now,               // Momento de la compra
                PurchasingPrice = 0m,                        // Se calcula más abajo
                PurchaseItems = new List<PurchaseItem>()
            };

            // --- Validación de stock y composición de líneas ---

            foreach (var item in compra.CochesComprados)
            {
                // Buscar el coche en la lista cargada
                var car = cars.FirstOrDefault(c => c.Id == item.Id);

                if (car == null)
                {
                    // El coche no existe en BD
                    ModelState.AddModelError("CochesComprados", $"Error! El coche con Id {item.Id} no existe.");
                    continue;
                }

                // (Opcional) coherencia del color con lo que llega desde la UI
                if (!string.IsNullOrWhiteSpace(item.Color) &&
                    !string.Equals(item.Color, car.Color, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("CochesComprados",
                        $"Error! El color enviado ('{item.Color}') no coincide con el del coche '{car.Model.Name}' en el concesionario ('{car.Color}').");
                    continue;
                }

                // Comprobación de stock real contra QuantityForPurchasing
                if (car.QuantityForPurchasing < item.Quantity)
                {
                    ModelState.AddModelError("CochesComprados",
                        $"Error! No hay stock suficiente para '{car.Model.Name}'. Disponible: {car.QuantityForPurchasing}, solicitado: {item.Quantity}.");
                    continue;
                }

                // Decrementamos el stock en memoria (se persistirá al guardar)
                car.QuantityForPurchasing -= item.Quantity;

                // Añadimos la línea (sin usar constructores especiales)
                purchase.PurchaseItems.Add(new PurchaseItem
                {
                    CarId = car.Id,       // Si tu FK se llama 'Card', usa: Card = car.Id
                    Quantity = item.Quantity
                });

                // Por coherencia de precios, fijamos el unitario que responderemos
                item.PurchasingPrice = car.PurchasingPrice;

                // Acumulamos el total de la compra
                purchase.PurchasingPrice += car.PurchasingPrice * item.Quantity;
            }

            // Si detectamos errores en alguna línea, devolvemos 400
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // --- Persistencia (compra + decremento de stock de coches) ---
            _context.Purchases.Add(purchase);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Cualquier problema al escribir en BD se reporta como 409 Conflict
                _logger.LogError(ex, "Error al guardar la compra");
                return Conflict("Error al guardar la compra. Inténtelo de nuevo más tarde.");
            }

            // --- Construcción del DTO de respuesta (detalle) ---

            // Reproyectamos las líneas con los datos “congelados” del momento de la compra
            var detailItems = purchase.PurchaseItems.Select(pi =>
            {
                var car = cars.First(c => c.Id == pi.CarId /* o pi.Card */);
                return new ComprarForItemDTO(
                    id: car.Id,
                    model: car.Model,
                    color: car.Color,
                    purchasingPrice: car.PurchasingPrice,
                    quantity: pi.Quantity
                );
            }).ToList();

            // En la respuesta podemos devolver el enum directamente (lo traemos del DTO de entrada)
            var pm = compra.PaymentMethod;

            var detail = new ComprarForDetailDTO(
                id: purchase.Id,
                purchasingDate: purchase.PurchasingDate,
                name: purchase.Name,
                surname: purchase.Surname,
                address: purchase.DeliveryCarDealer,
                paymentMethod: pm,
                cochesComprados: detailItems
            );

            // 201 Created con ubicación GET y el cuerpo del detalle
            return CreatedAtAction(nameof(Get_Details_Purchase), new { id = purchase.Id }, detail);
        }
    }
}
