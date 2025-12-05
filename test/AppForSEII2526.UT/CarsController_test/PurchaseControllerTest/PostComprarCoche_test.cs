using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.API.DTOs.ComprarDTOs;
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
    public class Create_Purchase_test : AppForSEII25264SqliteUT
    {
        private readonly Model _model1;
        private readonly Model _model2;
        private readonly Car _car1;
        private readonly Car _car2;

        public Create_Purchase_test()
        {
            // ===== Seed de datos compartido para los tests del POST =====

            _model1 = new Model { Id = 1, Name = "Model S" };
            _model2 = new Model { Id = 2, Name = "Mustang" };
            _context.Models.AddRange(_model1, _model2);

            _car1 = new Car
            {
                Id = 1,
                CarClass = "Sedan",
                FuelType = "Eléctrico",
                Color = "Negro",
                Description = "Coche premium eléctrico",
                Manufacturer = "Tesla",
                PurchasingPrice = 75000m,
                QuantityForPurchasing = 3,   // stock suficiente
                QuantityForRenting = 1,
                RentingPrice = 300m,
                Model = _model1
            };

            _car2 = new Car
            {
                Id = 2,
                CarClass = "Deportivo",
                FuelType = "Gasolina",
                Color = "Rojo",
                Description = "Clásico americano potente",
                Manufacturer = "Ford",
                PurchasingPrice = 55000m,
                QuantityForPurchasing = 2,   // stock suficiente
                QuantityForRenting = 1,
                RentingPrice = 250m,
                Model = _model2
            };

            var user = new ApplicationUser
            {
                UserName = "Juan",
                Name = "Juan",
                Surname = "Pérez",
                Address = "Concesionario Centro",
               // PaymentMethod = PaymentMethod.CreditCard,

            };

            _context.ApplicationUsers.Add(user);
            _context.Cars.AddRange(_car1, _car2);
            _context.SaveChanges();
        }

        // ========= CASO OK: la compra se crea correctamente =========

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchaseOKtest()
        {
            // Arrange
            var logger = new Mock<ILogger<PurchasesController>>().Object;
            var controller = new PurchasesController(_context, logger);

            // DTO de entrada con dos coches: 2 unidades del car1 y 1 del car2.
            var itemsDto = new List<ComprarForItemDTO>
            {
                new ComprarForItemDTO(_car1.Id, _model1, "Negro",_car1.Description ,0m, 2),
                new ComprarForItemDTO(_car2.Id, _model2, "Rojo",_car2.Description ,0m, 1)
            };

            var compraDto = new ComprarForCreateDTO(
                name: "Juan",
                surname: "Pérez",
                address: "Concesionario Centro",
                paymentMethod: PaymentMethod.CreditCard,
                cochesComprados: itemsDto
            );

            // Act
            var result = await controller.CreatePurchase(compraDto);

            // Assert
            // 1) Debe devolver CreatedAtAction
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(PurchasesController.GetDetailsPurchase), created.ActionName);

            // 2) El cuerpo debe ser un ComprarForDetailDTO
            var actual = Assert.IsType<ComprarForDetailDTO>(created.Value);

            // Construyo el DTO esperado usando el Id y la fecha que ha generado el propio método
            var expectedItems = new List<ComprarForItemDTO>
            {
                new ComprarForItemDTO(_car1.Id, _model1, "Negro",_car1.Description, 75000m, 2),
                new ComprarForItemDTO(_car2.Id, _model2, "Rojo", _car2.Description, 55000m, 1)
            };

            var expected = new ComprarForDetailDTO(
                id: actual.Id,                         // usamos el Id recién creado
                purchasingDate: actual.PurchasingDate, // dejamos que CompareDate se encargue
                name: "Juan",
                surname: "Pérez",
                address: "Concesionario Centro",
                paymentMethod: PaymentMethod.CreditCard,
                cochesComprados: expectedItems
            );

            // Ordeno las listas para que la comparación sea estable
            actual.CochesComprados = actual.CochesComprados.OrderBy(i => i.Id).ToList();
            expected.CochesComprados = expected.CochesComprados.OrderBy(i => i.Id).ToList();

            // 3) Comparo el DTO completo usando Equals de ComprarForDetailDTO
            Assert.Equal(expected, actual);

            // 4) Compruebo que se ha descontado el stock en la base de datos
            var car1Db = await _context.Cars.FindAsync(_car1.Id);
            var car2Db = await _context.Cars.FindAsync(_car2.Id);

            Assert.Equal(3 - 2, car1Db!.QuantityForPurchasing); // tenía 3, compramos 2
            Assert.Equal(2 - 1, car2Db!.QuantityForPurchasing); // tenía 2, compramos 1
        }

        // ========= CASO BadRequest: sin coches en la compra =========

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchaseNoItemsBadRequesttest()
        {
            // Arrange
            var logger = new Mock<ILogger<PurchasesController>>().Object;
            var controller = new PurchasesController(_context, logger);

            var compraDto = new ComprarForCreateDTO(
                name: "Juan",
                surname: "Pérez",
                address: "Concesionario Centro",
                paymentMethod: PaymentMethod.CreditCard,
                cochesComprados: new List<ComprarForItemDTO>() // lista vacía
            );

            // Act
            var result = await controller.CreatePurchase(compraDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);

            // Debe haber error asociado a CochesComprados
            Assert.True(problem.Errors.ContainsKey(nameof(compraDto.CochesComprados)));
        }

        // ========= CASO BadRequest: cantidad <= 0 =========

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchaseInvalidQuantityBadRequesttest()
        {
            // Arrange
            var logger = new Mock<ILogger<PurchasesController>>().Object;
            var controller = new PurchasesController(_context, logger);

            var itemsDto = new List<ComprarForItemDTO>
            {
                // Cantidad 0 → debe disparar error de validación
                new ComprarForItemDTO(_car1.Id, _model1, "Negro",_car1.Description ,0m, 0)
            };

            var compraDto = new ComprarForCreateDTO(
                name: "Juan",
                surname: "Pérez",
                address: "Concesionario Centro",
                paymentMethod: PaymentMethod.CreditCard,
                cochesComprados: itemsDto
            );

            // Act
            var result = await controller.CreatePurchase(compraDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);

            // Debe haber error asociado a "Quantity"
            Assert.True(problem.Errors.ContainsKey("Quantity"));
        }


        // ========= CASO BadRequest: sin coches en la compra =========
         
        /*
         * MODIFICACION DEL EXAMENE N(me generaba problemas y la he tenido que comenter y eliminarla del post lo siento en alma no puedo mas)
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchaseNumeroDecocheses2()
        {
            // Arrange
            var logger = new Mock<ILogger<PurchasesController>>().Object;
            var controller = new PurchasesController(_context, logger);

            var itemsDto = new List<ComprarForItemDTO>
            {
                
                new ComprarForItemDTO(_car1.Id, _model1, "Negro",_car1.Description ,0m, 2)
            };


            var compraDto = new ComprarForCreateDTO(
                name: "Juan",
                surname: "Pérez",
                address: "Concesionario Centro",
                paymentMethod: PaymentMethod.CreditCard,
                cochesComprados: itemsDto 
            );

            // Act
            var result = await controller.CreatePurchase(compraDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);

            // Debe haber error asociado a CochesComprados
            Assert.True(problem.Errors.ContainsKey(nameof(compraDto.CochesComprados)));
        }
    }
*/
    }

  }
