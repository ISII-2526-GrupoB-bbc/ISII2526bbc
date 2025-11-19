using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReseñarDTOs;
using AppForSEII2526.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.CarsController_test.ReviewControllerTest
{
    namespace AppForSEII2526.UT.CarsController_test.RentalsController_test
    {
        public class Get_Details_Review_test : AppForSEII25264SqliteUT
        {
            public Get_Details_Review_test()
            {
                // ===== Seed de datos para la reseña de prueba =====
                // USUARIO
                var user = new ApplicationUser(
                     id: "user1",
                    name: "Juan",
                    surname: "García",
                    userName: "juanUser",
                    address: "Calle Falsa 123"
                );
                _context.ApplicationUsers.Add(user);

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


                // REVIEW + ITEMS DE REVIEW
                var review = new Review
                {
                    Id = 1,
                    UserName = "juanUser",
                    Country = "España",
                    DriverType = "Novato",
                    Created = DateTime.Now,
                    ReviewItems = new List<ReviewItem>(),
                };
                var line1 = new ReviewItem
                {
                    Rating = 5,
                    Description = "Coche para test",
                    Car = car1,
                    Review = review,
                    CarId = car1.Id,
                    ReviewId = review.Id
                };
                var line2 = new ReviewItem
                {
                    Rating = 4,
                    Description = "Coche para test",
                    Car = car2,
                    Review = review,
                    CarId = car2.Id,
                    ReviewId = review.Id
                };
                //Añado las lineas a la coleccion de la Reseña
                review.ReviewItems.Add(line1);
                review.ReviewItems.Add(line2);
                // vincular la navegación
                review.ApplicationUser = user;
                //Añado la Reseña al contexto
                _context.Add(review);
                //Persisto todo el seed en la BD en memoria.
                _context.SaveChanges();

            }
            // Esta prueba comprueba que cuando pido el detalle de una compra existente (id=1)
            // el controlador devuelve 200 OK y un DTO con toda la información correcta.
            [Fact]
            [Trait("Database", "WithoutFixture")]
            [Trait("LevelTesting", "Unit Testing")]
            public async Task Get_Details_Review_OK_test()
            {
                // Arrange
                // Creo un logger falso (mock) para no depender de un logger real.
                var logger = new Mock<ILogger<ReviewsController>>().Object;
                // Instancio el PurchasesController con el contexto en memoria y el logger simulado.
                var controller = new ReviewsController(_context, logger);
                // Act
                // Llamo al método que quiero probar, pidiendo el detalle de la compra con id=1.
                var result = await controller.Get_Details_Review(1);
                // Assert
                // Compruebo que el resultado es un OkObjectResult (código 200).
                var okReview = Assert.IsType<OkObjectResult>(result);
                // Extraigo el valor del OkObjectResult y verifico que sea del tipo DTO de detalle de review.
                var actual = Assert.IsType<ReseñarDetailDTO>(okReview.Value);
                

                var expectedReview = new ReseñarDetailDTO("Juan", "García", "España", "Novato", DateTime.Now, reviewItems: new List<ReseñarItemDTO>
                {
                    new ReseñarItemDTO("Model S", "Tesla", "Negro", 5, "Coche para test"),
                    new ReseñarItemDTO("Mustang", "Ford", "Rojo", 4, "Coche para test")

                });

                Assert.Equal(expectedReview, actual); //Comprobación final con el equals de RentalDetailDTO

            }
            // ========= CASO NotFound: la compra NO existe =========

            // Esta prueba comprueba que si pido una compra que no existe,
            // el controlador responde correctamente con 404 NotFound.
            [Fact]
            [Trait("Database", "WithoutFixture")]
            [Trait("LevelTesting", "Unit Testing")]
            public async Task Get_Details_Review_NotFound_test()
            {
                // Arrange
                // Igual que antes: logger simulado y controlador con el contexto de pruebas.
                var logger = new Mock<ILogger<ReviewsController>>().Object;
                var controller = new ReviewsController(_context, logger);

                // Act: uso un id que NO está en el seed (999).
                var result = await controller.Get_Details_Review(999);

                // Assert: el controlador debe devolver un NotFoundResult.
                var notFound = Assert.IsType<NotFoundResult>(result);
            }

        }
    }
}

