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
        public async Task<ActionResult> GetDetailsPurchase(int id)
        {
            // Primero compruebo que el DbSet de Purchases está disponible.
            // Si no, lo considero un error grave de configuración y devuelvo 404.
            if (_context.Purchases == null)
            {
                _logger.LogError("Error: Purchases table does not exist");
                return NotFound();
            }

            // Busco en base de datos la compra cuyo Id coincide con el parámetro.
            // Incluyo también sus líneas (PurchaseItems) porque las necesito para el detail.
            var purchase = await _context.Purchases
                .Where(p => p.Id == id)
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync();

            // Si no encuentro ninguna compra con ese Id, registro el error y devuelvo 404.
            if (purchase == null)
            {
                _logger.LogError($"Error: Purchase with id {id} does not exist");
                return NotFound();
            }

            // A partir de las líneas de compra, obtengo los Ids de los coches comprados.
            // Uso Distinct para no repetir Ids si el mismo coche aparece en varias líneas.
            var carIds = purchase.PurchaseItems
                .Select(pi => pi.CarId)
                .Distinct()
                .ToList();

            // Cargo desde base de datos todos los coches involucrados en la compra,
            // incluyendo su Model para poder mostrar esa información en el DTO.
            var cars = await _context.Cars
                .Include(c => c.Model)
                .Where(c => carIds.Contains(c.Id))
                .ToListAsync();

            // Creo un diccionario para acceder a los coches por su Id de forma rápida (O(1)).
            var carById = cars.ToDictionary(c => c.Id);

            // Recorro cada línea de compra y la transformo en un ComprarForItemDTO.
            // De esta forma construyo la lista de "cochesComprados" que irán dentro del detalle.
            var items = purchase.PurchaseItems.Select(pi =>
            {
                // Recupero el coche asociado a esta línea a partir del diccionario.
                var car = carById[pi.CarId];

                // Con los datos del coche y la cantidad de la línea construyo el DTO del item.
                return new ComprarForItemDTO(
                    id: car.Id,
                    model: car.Model,                 // Incluyo el Model del coche (ya filtrado por JSON).
                    color: car.Color,
                    description: car.Description,
                    purchasingPrice: car.PurchasingPrice, // Precio unitario actual del coche.
                    quantity: pi.Quantity                 // Cantidad comprada de ese coche.
                );
            }).ToList();

            // Construyo el DTO de detalle de la compra a partir de la entidad Purchase
            // y de la lista de items que acabo de generar.
            var detail = new ComprarForDetailDTO(
                id: purchase.Id,
                purchasingDate: purchase.PurchasingDate,
                name: purchase.Name,
                surname: purchase.Surname,
                address: purchase.DeliveryCarDealer,
                paymentMethod: purchase.PaymentMethod, // Enum PaymentMethod ya guardado en la entidad.
                cochesComprados: items
            );

            // Devuelvo 200 OK junto con el DTO de detalle de la compra.
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
        public async Task<ActionResult> CreatePurchase(ComprarForCreateDTO compra)
        {
            // ==== VALIDACIONES BASICAS ====

            if (compra.CochesComprados == null || compra.CochesComprados.Count == 0)
                ModelState.AddModelError(nameof(compra.CochesComprados), "Error! Debe incluir al menos un coche a comprar.");

            if (compra.CochesComprados?.Any(i => i.Quantity <= 0) == true)
                ModelState.AddModelError("Quantity", "Error! Cada coche debe tener una cantidad >= 1.");

            if (!Enum.IsDefined(typeof(PaymentMethod), compra.PaymentMethod))
                ModelState.AddModelError(nameof(compra.PaymentMethod), "Error! Método de pago no válido.");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            // ==== VALIDACIÓN USUARIO ====

            var userName = compra.Name?.Trim();

            if (string.IsNullOrWhiteSpace(userName))
            {
                ModelState.AddModelError("PurchaseAplicationUser", "El nombre es obligatorio");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Busca por UserName (IMPORTANTE)
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                var pd = new ValidationProblemDetails();
                pd.Errors.Add("PurchaseAplicationUser", new[] { "Error! El nombre de usuario no está registrado" });
                return BadRequest(pd);
            }


            // ==== CARGA DE COCHES ====

            var carIds = compra.CochesComprados.Select(i => i.Id).Distinct().ToList();

            var cars = await _context.Cars
                .Include(c => c.Model)
                .Where(c => carIds.Contains(c.Id))
                .ToListAsync();


            // ==== CREAR PURCHASE ====

            var purchase = new Purchase
            {
                Name = compra.Name,
                Surname = compra.Surname,
                DeliveryCarDealer = compra.Address,
                PaymentMethod = compra.PaymentMethod,
                PurchasingDate = DateTime.Now,
                PurchasingPrice = 0m,
                PurchaseItems = new List<PurchaseItem>()
            };


            // ==== PROCESAR ITEMS ====

            foreach (var item in compra.CochesComprados)
            {
                var car = cars.FirstOrDefault(c => c.Id == item.Id);

                if (car == null)
                {
                    ModelState.AddModelError("CochesComprados", $"Error! El coche con Id {item.Id} no existe.");
                    continue;
                }

                if (car.QuantityForPurchasing < item.Quantity)
                {
                    ModelState.AddModelError("CochesComprados",
                        $"Error! No hay stock suficiente para '{car.Model.Name}'. Disponible: {car.QuantityForPurchasing}, solicitado: {item.Quantity}.");
                    continue;
                }

                car.QuantityForPurchasing -= item.Quantity;

                purchase.PurchaseItems.Add(new PurchaseItem
                {
                    CarId = car.Id,
                    Quantity = item.Quantity
                });

                item.PurchasingPrice = car.PurchasingPrice;
                purchase.PurchasingPrice += car.PurchasingPrice * item.Quantity;
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            // ==== GUARDAR ====

            _context.Purchases.Add(purchase);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la compra");
                return Conflict("Error al guardar la compra. Inténtelo de nuevo más tarde.");
            }


            // ==== RESPUESTA ====

            var detailItems = purchase.PurchaseItems.Select(pi =>
            {
                var car = cars.First(c => c.Id == pi.CarId);

                return new ComprarForItemDTO(
                    car.Id,
                    car.Model,
                    car.Color,
                    car.Description,
                    car.PurchasingPrice,
                    pi.Quantity
                );
            }).ToList();

            var detail = new ComprarForDetailDTO(
                purchase.Id,
                purchase.PurchasingDate,
                purchase.Name,
                purchase.Surname,
                purchase.DeliveryCarDealer,
                compra.PaymentMethod,
                detailItems
            );

            return CreatedAtAction(nameof(GetDetailsPurchase), new { id = purchase.Id }, detail);
        }
    }
  }
