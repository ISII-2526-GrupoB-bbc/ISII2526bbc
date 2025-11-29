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

namespace AppForSEII2526.UT.CarsController_test.RentalsController_test
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

            var result = await controller.GetDetailsRental(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<RentalDetailDTO>(ok.Value);

            var expected = new RentalDetailDTO(     //Creo el objeto EXPECTED para compararlo en el equals
                id: 1,
                customerName: "juanUser",
                customerSurname: "García",
                deliveryCarDealer: "ConcesionarioCentro",
                paymentMethod: PaymentMethod.PayPal,
                startDate: new DateTime(2024, 3, 16),
                endDate: new DateTime(2024, 3, 20),
                rentingDate: new DateTime(2024, 3, 15),
                rentingPrice: 0m,
                rentalItems: new List<RentalItemDTO>
                {
                    new RentalItemDTO(1, 1, "Model S", "Tesla", 300m, 2),
                    new RentalItemDTO(2, 2, "Mustang", "Ford", 250m, 1)
                });

            actual.RentalItems = actual.RentalItems.OrderBy(ri => ri.Id).ToList();      //Ordeno las listas de RentalItems para compararlas
            expected.RentalItems = expected.RentalItems.OrderBy(ri => ri.Id).ToList();

            Assert.Equal(expected, actual); //Comprobación final con el equals de RentalDetailDTO
        }

        // =========== TEST NOT FOUND ===========
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task Get_Details_Rental_NotFound_test()
        {
            var logger = new Mock<ILogger<RentalsController>>().Object;
            var controller = new RentalsController(_context, logger);

            var result = await controller.GetDetailsRental(999);

            // NotFound
            var notFound = Assert.IsType<NotFoundResult>(result);
        }

        // =========== TEST NOT FOUND (ID < 0) ===========      MODIFICACION PARA EL EXAMEN
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task Get_Details_Rental_NotFound_IdNegativo_test()
        {
            var logger = new Mock<ILogger<RentalsController>>().Object;
            var controller = new RentalsController(_context, logger);

            var result = await controller.GetDetailsRental(-1);

            // NotFound
            var notFoundNegativo = Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
  
