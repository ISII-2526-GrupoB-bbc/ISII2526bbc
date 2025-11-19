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
        public async Task<ActionResult> Create_Purchase(ComprarForCreateDTO compra)
        {
            // --- Validaciones de entrada (además de DataAnnotations) ---

            // Compruebo que venga al menos un coche en la compra.
            if (compra.CochesComprados == null || compra.CochesComprados.Count == 0)
                ModelState.AddModelError(nameof(compra.CochesComprados), "Error! Debe incluir al menos un coche a comprar.");

            // Verifico que cada coche tenga una cantidad válida (>= 1).
            if (compra.CochesComprados?.Any(i => i.Quantity <= 0) == true)
                ModelState.AddModelError("Quantity", "Error! Cada coche debe tener una cantidad >= 1.");

            // Compruebo que el método de pago recibido es un valor válido del enum PaymentMethod.
            if (!Enum.IsDefined(typeof(PaymentMethod), compra.PaymentMethod))
                ModelState.AddModelError(nameof(compra.PaymentMethod), "Error! Método de pago no válido.");

            // Si alguna de las validaciones anteriores ha fallado, devuelvo 400 BadRequest
            // con los detalles de todos los errores.
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // --- Cargar los coches referenciados por Id desde BD ---

            // Del DTO de entrada extraigo los Ids de los coches que quiere comprar el usuario.
            var carIds = compra.CochesComprados.Select(i => i.Id).Distinct().ToList();

            // Consulto en base de datos esos coches, incluyendo su Model para poder mostrarlo luego.
            var cars = await _context.Cars
                .Include(c => c.Model)
                .Where(c => carIds.Contains(c.Id))
                .ToListAsync();

            // --- Crear la entidad Purchase a partir del DTO ---

            // Construyo una nueva entidad Purchase usando los datos de la petición.
            var purchase = new Purchase
            {
                Name = compra.Name,
                Surname = compra.Surname,
                DeliveryCarDealer = compra.Address,       // Dirección de entrega del concesionario
                PaymentMethod = compra.PaymentMethod,     // Enum de método de pago
                PurchasingDate = DateTime.Now,           // Fecha de la compra (momento actual)
                PurchasingPrice = 0m,                    // El total lo iré acumulando más abajo
                PurchaseItems = new List<PurchaseItem>() // Inicializo la lista de líneas de compra
            };

            // --- Validación de stock y composición de líneas ---

            // Recorro cada coche que el usuario quiere comprar.
            foreach (var item in compra.CochesComprados)
            {
                // Busco el coche real en la lista que he traído de la base de datos.
                var car = cars.FirstOrDefault(c => c.Id == item.Id);

                // Si no existe en BD, añado un error y sigo con el siguiente.
                if (car == null)
                {
                    ModelState.AddModelError("CochesComprados", $"Error! El coche con Id {item.Id} no existe.");
                    continue;
                }

                // (Opcional) Compruebo que el color enviado coincide con el color del coche en el concesionario.
                if (!string.IsNullOrWhiteSpace(item.Color) &&
                    !string.Equals(item.Color, car.Color, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("CochesComprados",
                        $"Error! El color enviado ('{item.Color}') no coincide con el del coche '{car.Model.Name}' en el concesionario ('{car.Color}').");
                    continue;
                }

                // Verifico que haya stock suficiente para la cantidad solicitada.
                if (car.QuantityForPurchasing < item.Quantity)
                {
                    ModelState.AddModelError("CochesComprados",
                        $"Error! No hay stock suficiente para '{car.Model.Name}'. Disponible: {car.QuantityForPurchasing}, solicitado: {item.Quantity}.");
                    continue;
                }

                // Si hay stock, descuento la cantidad solicitada del stock de compra.
                car.QuantityForPurchasing -= item.Quantity;

                // Creo una nueva línea de compra (PurchaseItem) asociando el coche y la cantidad.
                purchase.PurchaseItems.Add(new PurchaseItem
                {
                    CarId = car.Id,          // Relación con el coche
                    Quantity = item.Quantity // Cantidad comprada de ese coche
                });

                // Aseguro que el precio unitario que devuelvo al cliente es el actual del coche.
                item.PurchasingPrice = car.PurchasingPrice;

                // Acumulo el precio total de la compra (precio * cantidad).
                purchase.PurchasingPrice += car.PurchasingPrice * item.Quantity;
            }

            // Si durante el procesamiento de las líneas se ha detectado algún error,
            // devuelvo 400 con todos los problemas encontrados.
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // --- Persistencia (compra + decremento de stock de coches) ---

            // Añado la nueva compra al DbContext para que EF Core la rastree.
            _context.Purchases.Add(purchase);

            try
            {
                // Intento guardar los cambios en base de datos:
                // - inserción de Purchase
                // - inserción de PurchaseItems
                // - actualización del stock de los coches
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Si ocurre cualquier error al escribir en BD (conflictos, problemas de conexión, etc.),
                // lo registro en el log y devuelvo un 409 Conflict al cliente.
                _logger.LogError(ex, "Error al guardar la compra");
                return Conflict("Error al guardar la compra. Inténtelo de nuevo más tarde.");
            }

            // --- Construcción del DTO de respuesta (detalle) ---

            // Una vez guardada la compra, vuelvo a construir los items de detalle,
            // pero usando la información final (coches consolidados, cantidades, precios).
            var detailItems = purchase.PurchaseItems.Select(pi =>
            {
                var car = cars.First(c => c.Id == pi.CarId);

                return new ComprarForItemDTO(
                    id: car.Id,
                    model: car.Model,
                    color: car.Color,
                    purchasingPrice: car.PurchasingPrice,
                    quantity: pi.Quantity
                );
            }).ToList();

            // El método de pago de la respuesta lo tomo directamente del DTO de entrada.
            var pm = compra.PaymentMethod;

            // Construyo el DTO de detalle que enviaré al cliente como respuesta.
            var detail = new ComprarForDetailDTO(
                id: purchase.Id,
                purchasingDate: purchase.PurchasingDate,
                name: purchase.Name,
                surname: purchase.Surname,
                address: purchase.DeliveryCarDealer,
                paymentMethod: pm,
                cochesComprados: detailItems
            );

            // Devuelvo 201 Created, indicando además la ruta para consultar el detalle
            // de esta misma compra (Get_Details_Purchase) y el cuerpo del DTO de detalle.
            return CreatedAtAction(nameof(Get_Details_Purchase), new { id = purchase.Id }, detail);
        }


    }
}
