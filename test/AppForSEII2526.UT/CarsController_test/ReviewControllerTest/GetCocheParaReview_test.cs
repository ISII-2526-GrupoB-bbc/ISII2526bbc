using AppForSEII2526.Models;
using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.API.Controllers;

namespace AppForSEII2526.UT.CarsController_test.ReviewControllerTest
{
    // Defino la clase de tests y heredo de mi fixture SQLite en memoria para tener un DbContext limpio por test.
    public class GetCocheParaReview_test : AppForSEII25264SqliteUT
    {
        // En el constructor hago el seed de datos que usarán los tests.
        public GetCocheParaReview_test()
        {
            // Crear modelos de prueba
            var model1 = new Model { Id = 1, Name = "Model S" };
            var model2 = new Model { Id = 2, Name = "Mustang" };
            var model3 = new Model { Id = 3, Name = "RAV4" };

            _context.Models.AddRange(model1, model2, model3); // Inserto los modelos en la BD en memoria.

            // Crear coches de prueba
            // Primer coche : Tesla, negro, eléctrico, con precio de compra y stock.
            var car1 = new Car
            {
                Id = 1,
                CarClass = "Sedan",
                FuelType = "Electrico",
                Color = "Negro",
                Description = "Coche premium eléctrico",
                Manufacturer = "Tesla",
                PurchasingPrice = 75000m,
                QuantityForPurchasing = 3,
                QuantityForRenting = 1,
                RentingPrice = 300m,
                Model = model1
            };
            // Segundo coche: Ford Mustang, rojo, gasolina, con precio de compra y stock.
            var car2 = new Car
            {
                Id = 2,
                CarClass = "Deportivo",
                FuelType = "Gasolina",
                Color = "Rojo",
                Description = "Clásico americano potente",
                Manufacturer = "Ford",
                PurchasingPrice = 55000,
                QuantityForPurchasing = 2,
                QuantityForRenting = 1,
                RentingPrice = 250,
                Model = model2
            };
            // Tercer coche: Toyota RAV4, blanco, híbrido; sin stock de compra (no afecta a GetCars, que filtra por color).
            var car3 = new Car
            {
                Id = 3,
                CarClass = "SUV",
                FuelType = "Gasolina",
                Color = "Blanco",
                Description = "SUV familiar",
                Manufacturer = "Toyota",
                PurchasingPrice = 40000m,
                QuantityForPurchasing = 0,
                QuantityForRenting = 5,
                RentingPrice = 120m,
                Model = model3
            };

            _context.Cars.AddRange(car1, car2, car3);

            // Guardar los datos en SQLite
            _context.SaveChanges();

        }
        // ========= Tests para GetCars (por FuelType) =========
        // Preparo los casos de prueba de éxito para GetCars usando MemberData.
        public static IEnumerable<object[]> GetCarsOKCases()
        {
            var carDTOs = new List<CocheParaReviewDTO>() { 
            // Construyo los DTOs esperados tal y como los proyecta el action GetCars (mismo orden de parámetros).
            new CocheParaReviewDTO(1, "Model S", "Sedan", "Tesla", "Electrico", "Negro"),
            new CocheParaReviewDTO(2, "Mustang", "Deportivo", "Ford", "Gasolina", "Rojo"),
            new CocheParaReviewDTO(3, "RAV4", "SUV", "Toyota", "Gasolina", "Blanco"),

            };
            var carDTOsTC1 = new List<CocheParaReviewDTO>() { carDTOs[0], carDTOs[1], carDTOs[2] }; //los resultados que espero obtener con las pruebas
            var carDTOsTC2 = new List<CocheParaReviewDTO>() { carDTOs[0] };
            var carDTOsTC3 = new List<CocheParaReviewDTO>() { carDTOs[1], carDTOs[2] };
            var carDTOsTC4 = new List<CocheParaReviewDTO>() { carDTOs[1] };

            var allTest = new List<object[]> //casos de prueba
            {
                new object[] { null, null, carDTOsTC1 },
                new object[] { "Tesla", null, carDTOsTC2 },
                new object[] { null, "Gasolina", carDTOsTC3 },
                new object[] { "Ford", "Gasolina", carDTOsTC4 },

            };

            return allTest;
        }
        // Test parametrizado para validar que GetCars devuelve 200 OK con la lista esperada.
        [Theory]
        [MemberData(nameof(GetCarsOKCases))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCars_OK_test(string? filtroManufacturer, string? filtroFuelType, IList<CocheParaReviewDTO> expectedCars)
        {
            // Arrange
            var mock = new Mock<ILogger<CarsControllers>>();
            ILogger<CarsControllers> logger = mock.Object;
            var controller = new CarsControllers(_context, logger);
            // Act
            var result = await controller.GetCarsForReview(filtroManufacturer, filtroFuelType); //filtro por el manufacturer y el fueltype
            //Assert
            //we check that the response type is OK 
            var okResult = Assert.IsType<OkObjectResult>(result);
            //and obtain the list of cars
            var carDTOsActual = Assert.IsType<List<CocheParaReviewDTO>>(okResult.Value);
            Assert.Equal(expectedCars, carDTOsActual); //si lo esperado es igual a lo actual, me devuelve el OK

        }
    }
}
