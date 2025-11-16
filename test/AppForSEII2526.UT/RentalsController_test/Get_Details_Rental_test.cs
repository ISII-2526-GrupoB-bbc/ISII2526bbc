using AppForSEII2526.API.Controllers;
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

namespace AppForSEII2526.UT.RentalsController_test
{
    public class Get_Details_Rental_test : AppForSEII25264SqliteUT
    {
        public Get_Details_Rental_test()
        {
            // ===== Seed de datos =====

            // USUARIO
            var user = new ApplicationUser(
                 id: "user1",
                name: "Juan",
                surname: "García",
                userName: "juanUser",
                address: "Calle Falsa 123"
            );
            _context.ApplicationUsers.Add(user);

            // MODELOS
            var model1 = new Model { Id = 1, Name = "Model S" };
            var model2 = new Model { Id = 2, Name = "Mustang" };
            _context.Models.AddRange(model1, model2);

            
            // COCHES
            var car1 = new Car
            {
                Id = 1,
                CarClass = "Sedan",
                FuelType = "Eléctrico",
                Color = "Negro",
                Description = "Coche para test",
                Manufacturer = "Tesla",
                RentingPrice = 300m,
                Model = model1,
                QuantityForRenting = 10
            };

            var car2 = new Car
            {
                Id = 2,
                CarClass = "Coupé",
                FuelType = "Gasolina",
                Color = "Rojo",
                Description = "Coche para test",
                Manufacturer = "Ford",
                RentingPrice = 250m,
                Model = model2,
                QuantityForRenting = 10
            };

            _context.Cars.AddRange(car1, car2);


            // RENTAL + ITEMS
            var rental = new Rental(
                "ConcesionarioCentro",
                new DateTime(2024, 3, 15),     // RentingDate = fecha creación
                new DateTime(2024, 3, 20),     // EndDate
                PaymentMethod.PayPal,
                new DateTime(2024, 3, 16),     // StartDate
                new List<RentalItem>(),
                user                            // <-- usuario
            )
            {
                Id = 1,
                RentingPrice = 0,
                ApplicationUser = user          
            };
            rental.DeliveryCarDealer = "ConcesionarioCentro";

            var ri1 = new RentalItem(car1.Id, 2, rental);  // 2 unidades del Tesla
            var ri2 = new RentalItem(car2.Id, 1, rental);  // 1 unidad del Mustang

            rental.RentalItems.Add(ri1);
            rental.RentalItems.Add(ri2);

            _context.Rentals.Add(rental);

            _context.SaveChanges();
        }

        // =========== TEST OK ===========
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task Get_Details_Rental_OK_test()
        {
            var logger = new Mock<ILogger<RentalsController>>().Object;
            var controller = new RentalsController(_context, logger);

            var result = await controller.Get_Details_Rental(1);

            // 200 OK
            var ok = Assert.IsType<OkObjectResult>(result);
            var detail = Assert.IsType<RentalDetailDTO>(ok.Value);

            // CAMPOS BÁSICOS
            Assert.Equal(1, detail.Id);
            Assert.Equal("juanUser", detail.CustomerName);
            Assert.Equal("García", detail.CustomerSurname);
            Assert.Equal("ConcesionarioCentro", detail.DeliveryCarDealer);
            Assert.Equal(PaymentMethod.PayPal, detail.PaymentMethod);
            Assert.Equal(new DateTime(2024, 3, 16), detail.StartDate);
            Assert.Equal(new DateTime(2024, 3, 20), detail.EndDate);
            Assert.Equal(new DateTime(2024, 3, 15), detail.RentingDate);

            // RENTAL ITEMS
            Assert.Equal(2, detail.RentalItems.Count);

            var items = detail.RentalItems.OrderBy(i => i.Id).ToList();

            // ITEM 1 (Car 1)
            Assert.Equal(1, items[0].Id);
            Assert.Equal(1, items[0].ModelId);
            Assert.Equal("Model S", items[0].ModelName);
            Assert.Equal("Tesla", items[0].Manufacturer);
            Assert.Equal(300m, items[0].RentingPrice);
            Assert.Equal(2, items[0].Quantity);

            // ITEM 2 (Car 2)
            Assert.Equal(2, items[1].Id);
            Assert.Equal(2, items[1].ModelId);
            Assert.Equal("Mustang", items[1].ModelName);
            Assert.Equal("Ford", items[1].Manufacturer);
            Assert.Equal(250m, items[1].RentingPrice);
            Assert.Equal(1, items[1].Quantity);
        }

        // =========== TEST NOT FOUND ===========
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task Get_Details_Rental_NotFound_test()
        {
            var logger = new Mock<ILogger<RentalsController>>().Object;
            var controller = new RentalsController(_context, logger);

            var result = await controller.Get_Details_Rental(999);

            // NotFound
            var notFound = Assert.IsType<NotFoundResult>(result);
        }
    }
}
  
