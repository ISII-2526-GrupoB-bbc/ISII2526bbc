using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReseñarDTOs;
using AppForSEII2526.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.CarsController_test.ReviewControllerTest
{
    public class PostReview_test : AppForSEII25264SqliteUT
    {
        // CONSTANTES PARA EL SEED

        private const string UserName = "juanUser";
        private const string Name = "Juan";
        private const string Surname = "García";
        private const string address = "Calle Falsa 123";
        private const string _model1 = "Model S";
        private const string _model2 = "Mustang";

        public PostReview_test()
        {// ===== SEED =====

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
            ApplicationUser user = new ApplicationUser("1",Name, Surname, UserName,address);
            _context.ApplicationUsers.Add(user);

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
                Description = "Reseña para",
                Car = car1,
                Review = review,
                CarId = car1.Id,
                ReviewId = review.Id
            };
            var line2 = new ReviewItem
            {
                Rating = 4,
                Description = "Reseña para",
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

            public static IEnumerable<object[]> TestCasesForCreateReviews()
        {
            var reviewNoITem = new ReseñarForCreateDTO("Juan", "García", "juanUser", "España", "Novato", new List<ReseñarItemDTO>());
            var reviewItems = new List<ReseñarItemDTO>() { new ReseñarItemDTO("Model S", "Tesla", "Negro", 5, "Reseña para") };
            var reviewDriverType = new ReseñarForCreateDTO("Juan", "García", "juanUser", "España", "Sin Carnet", reviewItems);
            var reviewApplicationUser = new ReseñarForCreateDTO("Juan", "García", "usuarioInexistente", "España", "Novato", reviewItems);
            var reviewCarNotExist = new ReseñarForCreateDTO("Juan", "García", "juanUser", "España", "Novato",
                new List<ReseñarItemDTO>() { new ReseñarItemDTO("Citroen C15", "MarcaX", "Azul", 4, "Reseña para") });
            var reviewCarDescriptionfailed = new ReseñarForCreateDTO("Juan", "García", "juanUser", "España", "Novato",
               new List<ReseñarItemDTO>() { new ReseñarItemDTO("Citroen C15", "MarcaX", "Azul", 4, "Coche para test") });

            var allTests = new List<object[]>()
            {
                new object[] { reviewNoITem, "Error! You must include at least one car to be reviewed",  },
                new object[] { reviewDriverType, "Error! DriverType must be 'Novato' or 'Experto'", },
                new object[] { reviewApplicationUser, "Error! UserName is not registered", },
                new object[] { reviewCarNotExist, "Error! The car Citroen C15 does not exist, so you cannot create a review for this car", },
                new object[] { reviewCarDescriptionfailed, "Error! La reseña debe empezar por Reseña para" }
            };
            return allTests;

        }
        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesForCreateReviews))]
        public async Task CreateReviewErrortest(ReseñarForCreateDTO reviewDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;

            var controller = new ReviewsController(_context, logger);

            // Act
            var result = await controller.CreateReview(reviewDTO);

            //Assert
            //we check that the response type is BadRequest and obtain the error returned
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            //we check that the expected error message and actual are the same
            Assert.StartsWith(errorExpected, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReviewSuccesstest()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;

            var controller = new ReviewsController(_context, logger);

            var reviewDTO = new ReseñarForCreateDTO(
                Name,
                Surname,
                UserName,
                "España",
                "Novato",
                new List<ReseñarItemDTO>()
                {
                    new ReseñarItemDTO(_model1, "Tesla", "Negro", 5, "Reseña para")
                });

            //Me creo el reviewDetail esperado
            var expectedReviewDetailDTO = new ReseñarDetailDTO("Juan", "García", "España", "Novato", DateTime.Now, reviewItems: new List<ReseñarItemDTO>
                {
                    new ReseñarItemDTO("Model S", "Tesla", "Negro", 5, "Reseña para"),
                });

            // Act
            var result = await controller.CreateReview(reviewDTO);

            //Assert
            //we check that the response type is BadRequest and obtain the error returned
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualReviewDetailDTO = Assert.IsType<ReseñarDetailDTO>(createdResult.Value);

            //lo comparo con el actual
            Assert.Equal(expectedReviewDetailDTO, actualReviewDetailDTO);
        }
    }
}
