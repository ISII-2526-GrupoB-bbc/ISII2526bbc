using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.CochesDTO;   
using AppForSEII2526.API.DTOs.ComprarDTOs;   
using AppForSEII2526.API.DTOs.RentalDTOs;
using AppForSEII2526.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.PurchasesController_test
{
    // Esta clase contiene las pruebas unitarias para el método Get_Details_Purchase del PurchasesController.
    // Heredo de AppForSEII25264SqliteUT para tener un DbContext SQLite en memoria preparado para los tests.
    public class Get_Details_Purchase_test : AppForSEII25264SqliteUT
    {
        // En el constructor preparo los datos de prueba que se insertan en la BD en memoria.
        public Get_Details_Purchase_test()
        {
            // ===== Seed de datos para la compra de prueba =====

            // 1) Modelos y coches
            // Creo dos modelos: "Model S" y "Mustang".
            var model1 = new Model { Id = 1, Name = "Model S" };
            var model2 = new Model { Id = 2, Name = "Mustang" };
            // Los añado al contexto.
            _context.Models.AddRange(model1, model2);

            // Defino el primer coche, asociado a model1.
            var car1 = new Car
            {
                Id = 1,
                CarClass = "Sedan",
                FuelType = "Eléctrico",
                Color = "Negro",
                Description = "Coche premium eléctrico",
                Manufacturer = "Tesla",
                PurchasingPrice = 75000m,
                QuantityForPurchasing = 3,
                QuantityForRenting = 1,
                RentingPrice = 300m,
                Model = model1
            };

            // Defino el segundo coche, asociado a model2.
            var car2 = new Car
            {
                Id = 2,
                CarClass = "Deportivo",
                FuelType = "Gasolina",
                Color = "Rojo",
                Description = "Clásico americano potente",
                Manufacturer = "Ford",
                PurchasingPrice = 55000m,
                QuantityForPurchasing = 2,
                QuantityForRenting = 1,
                RentingPrice = 250m,
                Model = model2
            };

            // Inserto ambos coches en el contexto.
            _context.Cars.AddRange(car1, car2);

            // 2) Compra con dos líneas
            // Creo una compra de ejemplo con dos líneas: una de car1 y otra de car2.
            var purchase = new Purchase
            {
                Id = 1,
                PurchasingDate = new DateTime(2024, 3, 10),
                Name = "Juan",
                Surname = "Pérez",
                DeliveryCarDealer = "Concesionario Centro",
                PaymentMethod = PaymentMethod.CreditCard,  // Uso el enum de método de pago.
                PurchaseItems = new List<PurchaseItem>()   // Inicializo la lista de líneas de compra.
            };

            // Primera línea: 2 unidades del coche car1.
            var line1 = new PurchaseItem
            {
                PurchaseId = purchase.Id,   // FK a la compra.
                CarId = car1.Id,            // FK al coche.
                Quantity = 2,               // Cantidad comprada de este coche.
                purchase = purchase,        // Propiedad de navegación a Purchase.
                car = car1                  // Propiedad de navegación a Car.
            };

            // Segunda línea: 1 unidad del coche car2.
            var line2 = new PurchaseItem
            {
                PurchaseId = purchase.Id,
                CarId = car2.Id,
                Quantity = 1,
                purchase = purchase,
                car = car2
            };

            // Añado las líneas a la colección de la compra.
            purchase.PurchaseItems.Add(line1);
            purchase.PurchaseItems.Add(line2);

            // Inserto la compra (con sus líneas) en el contexto.
            _context.Purchases.Add(purchase);
            // Persiste todo el seed en la BD en memoria.
            _context.SaveChanges();
        }

        // ========= CASO OK: la compra existe =========

        // Esta prueba comprueba que cuando pido el detalle de una compra existente (id=1)
        // el controlador devuelve 200 OK y un DTO con toda la información correcta.
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task Get_Details_Purchase_OK_test()
        {
            // Arrange
            // Creo un logger falso (mock) para no depender de un logger real.
            var logger = new Mock<ILogger<PurchasesController>>().Object;
            // Instancio el PurchasesController con el contexto en memoria y el logger simulado.
            var controller = new PurchasesController(_context, logger);

            // Act
            // Llamo al método que quiero probar, pidiendo el detalle de la compra con id=1.
            var result = await controller.Get_Details_Purchase(1);

            // Assert
            // Compruebo que la respuesta sea un OkObjectResult (HTTP 200).
            var ok = Assert.IsType<OkObjectResult>(result);

            // Extraigo el valor del OkObjectResult y verifico que sea del tipo DTO de detalle de compra.
            var actual = Assert.IsType<ComprarForDetailDTO>(ok.Value);

            var expected = new ComprarForDetailDTO(     //Creo el objeto EXPECTED para compararlo en el equals
               id: 1,
               purchasingDate: new DateTime(2024, 3, 10),
               name:"Juan",
               surname:"Pérez",
               address:"Concesionario Centro",
               paymentMethod: PaymentMethod.CreditCard,
               cochesComprados: new List<ComprarForItemDTO>
               {
                    new ComprarForItemDTO(1, new Model { Id = 1, Name = "Model S" }, "Negro","Coche premium eléctrico" ,75000m, 2),
                    new ComprarForItemDTO(2, new Model { Id = 2, Name = "Mustang" }, "Rojo","Clásico americano potente" ,55000m, 1)
               });

            actual.CochesComprados = actual.CochesComprados.OrderBy(ri => ri.Id).ToList();      //Ordeno las listas de RentalItems para compararlas
            expected.CochesComprados = expected.CochesComprados.OrderBy(ri => ri.Id).ToList();

            Assert.Equal(expected, actual); //Comprobación final con el equals de RentalDetailDTO
        }

        // ========= CASO NotFound: la compra NO existe =========

        // Esta prueba comprueba que si pido una compra que no existe,
        // el controlador responde correctamente con 404 NotFound.
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task Get_Details_Purchase_NotFound_test()
        {
            // Arrange
            // Igual que antes: logger simulado y controlador con el contexto de pruebas.
            var logger = new Mock<ILogger<PurchasesController>>().Object;
            var controller = new PurchasesController(_context, logger);

            // Act: uso un id que NO está en el seed (999).
            var result = await controller.Get_Details_Purchase(999);

            // Assert: el controlador debe devolver un NotFoundResult.
            var notFound = Assert.IsType<NotFoundResult>(result);
        }
    }
}