using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.RentalDTOs;
using AppForSEII2526.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.RentalsController_test
{
    public class PostRentals_test : AppForSEII25264SqliteUT
    {
        // CONSTANTES PARA EL SEED

        private const string _userName = "juanUser";
        private const string _customerName = "Juan";
        private const string _customerSurname = "García";
        private const string _deliveryDealer = "ConcesionarioCentro";

        private const string _model1 = "Model S";
        private const string _model2 = "Mustang";

        public PostRentals_test()
        {
            // ===== SEED =====

            // Usuario existente
            var user = new ApplicationUser(
                id: "user1",
                name: _customerName,
                surname: _customerSurname,
                userName: _userName,
                address: "Calle Falsa 123"
            );
            _context.ApplicationUsers.Add(user);

            // Modelos
            var m1 = new Model { Id = 1, Name = _model1 };
            var m2 = new Model { Id = 2, Name = _model2 };
            _context.Models.AddRange(m1, m2);

            // Coches
            var car1 = new Car
            {
                Id = 1,
                CarClass = "Sedan",
                FuelType = "Eléctrico",
                Color = "Negro",
                Manufacturer = "Tesla",
                RentingPrice = 300m,
                Model = m1,
                QuantityForRenting = 10
            };

            var car2 = new Car
            {
                Id = 2,
                CarClass = "Coupé",
                FuelType = "Gasolina",
                Color = "Rojo",
                Manufacturer = "Ford",
                RentingPrice = 250m,
                Model = m2,
                QuantityForRenting = 10
            };

            _context.Cars.AddRange(car1, car2);

            //Rental previo (para que el siguiente tenga id=2)

            var dummyRental = new Rental(
                _deliveryDealer,
                DateTime.Today,
                DateTime.Today.AddDays(1),
                PaymentMethod.PayPal,
                DateTime.Today,
                new List<RentalItem>(),
                user
            );
            dummyRental.DeliveryCarDealer = _deliveryDealer;

            _context.Rentals.Add(dummyRental);

            _context.SaveChanges();
        }

        // ===== CASOS DE ERROR =====
        public static IEnumerable<object[]> TestCasesFor_CreateRental()
        {
            // Item válido
            var items = new List<RentalItemDTO>()
            {
                new RentalItemDTO(1,1,"Model S","Tesla",300m,1)
            };

            var emptyItems = new RentalForCreateDTO(
                "juanUser",
                "García",
                "ConcesionarioCentro",
                PaymentMethod.PayPal,
                DateTime.Today.Date.AddDays(2),
                DateTime.Today.Date.AddDays(5),
                new List<RentalItemDTO>()
            );

            var startBeforeToday = new RentalForCreateDTO(  //Fecha de inicio antes que hoy
                "juanUser",
                "García",
                "ConcesionarioCentro",
                PaymentMethod.PayPal,
                DateTime.Today.Date,                
                DateTime.Today.Date.AddDays(5),
                items
            );

            var endBeforeStart = new RentalForCreateDTO(    //EndDate antes que StartDate
                "juanUser",
                "García",
                "ConcesionarioCentro",
                PaymentMethod.PayPal,
                DateTime.Today.Date.AddDays(5),
                DateTime.Today.Date.AddDays(2),     
                items
            );

            var invalidUser = new RentalForCreateDTO(       //Usuario inexistente
                "userInexistente",
                "García",
                "ConcesionarioCentro",
                PaymentMethod.PayPal,
                DateTime.Today.Date.AddDays(2),
                DateTime.Today.Date.AddDays(5),
                items
            );

            var unavailableCar = new RentalForCreateDTO(    //Cantidad solicitada mayor que la disponible
                "juanUser",
                "García",
                "ConcesionarioCentro",
                PaymentMethod.PayPal,
                DateTime.Today.Date.AddDays(2),
                DateTime.Today.Date.AddDays(5),
                new List<RentalItemDTO>()
                {
                    new RentalItemDTO(1,1,"Model S","Tesla",300m,99) 
                }
            );

            return new List<object[]>   //lo que devuelve para cada error
            {
                new object[] { emptyItems, "Error! You must include at least one car" },
                new object[] { startBeforeToday, "Error! Your rental date must start later than today" },
                new object[] { endBeforeStart, "Error! Your rental must end later than it starts" },
                new object[] { invalidUser, "Error! El nombre de usuario no está registrado" },
                new object[] { unavailableCar, "Error! Car" }
            };
        }

        // ==== TEST DE ERRORES ====
        [Theory]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_CreateRental))]
        public async Task CreateRental_Error_test(RentalForCreateDTO rentalDTO, string expectedError)
        {
            var logger = new Mock<ILogger<RentalsController>>().Object;
            var controller = new RentalsController(_context, logger);

            var result = await controller.CreateRental(rentalDTO);

            var badReq = Assert.IsType<BadRequestObjectResult>(result);
            var problem = Assert.IsType<ValidationProblemDetails>(badReq.Value);

            //Obtengo el primer error devuelto por ModelState
            var errorActual = problem.Errors.First().Value[0];

            Assert.StartsWith(expectedError, errorActual);
        }

        // ===== TEST DE ÉXITO =====
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateRental_Success_test()
        {
            var logger = new Mock<ILogger<RentalsController>>().Object;
            var controller = new RentalsController(_context, logger);

            DateTime start = DateTime.Today.AddDays(2);
            DateTime end = DateTime.Today.AddDays(5);
            DateTime rentingDate = DateTime.Today;


            //DTO de entrada
            var rentalDTO = new RentalForCreateDTO(
                "juanUser",
                "García",
                "ConcesionarioCentro",
                PaymentMethod.PayPal,
                start,
                end,
                new List<RentalItemDTO>()
                {
                    new RentalItemDTO(1,1,"Model S","Tesla",300m,1)
                }
            );

            //DTO expected
            var expected = new RentalDetailDTO(
                2,  // id del nuevo rental
                rentalDTO.CustomerName,
                rentalDTO.CustomerSurname,
                rentalDTO.DeliveryCarDealer,
                rentalDTO.PaymentMethod,
                rentalDTO.StartDate,
                rentalDTO.EndDate,
                rentalDTO.RentingDate,
                rentalDTO.RentalItems.Sum(ri =>
                    ri.RentingPrice * ri.Quantity * (decimal)(rentalDTO.EndDate - rentalDTO.StartDate).TotalDays),
                new List<RentalItemDTO>() {
                    new RentalItemDTO(
                        1,       // CarId CORRECTO
                        1,       // ModelId CORRECTO
                        "Model S",
                        "Tesla",
                        300m,
                        1
                    )
                }
            );


            // COMPARACION
            var result = await controller.CreateRental(rentalDTO);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var actual = Assert.IsType<RentalDetailDTO>(created.Value);

            

            Assert.Equal(expected, actual);
        }
    }
}

