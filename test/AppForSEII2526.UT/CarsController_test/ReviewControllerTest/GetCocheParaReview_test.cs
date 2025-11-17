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
                FuelType = "Híbrido",
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
        public static IEnumerable<object[]> GetCars_OK_Cases()
        {
            // Construyo los DTOs esperados tal y como los proyecta el action GetCars (mismo orden de parámetros).
            var tesla = new CocheParaReviewDTO(1, "Model S", "Negro", "Sedan", "Tesla", "Eléctrico");
            var ford = new CocheParaReviewDTO(2, "Mustang", "Rojo", "Deportivo", "Ford", "Gasolina");
            var toy = new CocheParaReviewDTO(3, "RAV4", "Blanco", "SUV", "Toyota", "Híbrido");
            // Caso sin filtro: espero los tres, y ordeno por Id para comparar de forma determinista.
            var all = new List<CocheParaReviewDTO> { tesla, ford, toy }.OrderBy(d => d.Id).ToList();
            // Caso con filtro de color "Negro": sólo debería devolver el Tesla.
            var onlyGasolina = new List<CocheParaReviewDTO> { ford };
            // Devuelvo los dos casos: (color=null) y (color="Negro"), con sus listas esperadas.
            return new List<object[]>
            {
                new object[] { null,    all },
                new object[] { "Gasolina", onlyGasolina },
            };
        }
        // Test parametrizado para validar que GetCars devuelve 200 OK con la lista esperada.
        [Theory]
        [MemberData(nameof(GetCars_OK_Cases))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCars_OK_test(string? fueltype, IList<CocheParaReviewDTO> expected)
        {
            // Creo un logger simulado para el controlador.
            var logger = new Mock<ILogger<CarsControllers>>().Object;
            // Instancio el controlador con el contexto en memoria y el logger fake.
            var controller = new CarsControllers(_context, logger);

            // Ejecuto el endpoint con el filtro de fueltype que toque en cada caso.
            var result = await controller.GetCars2(fueltype);

            // Verifico que el resultado sea 200 OK y que el Value sea la lista de DTOs.
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var actual = Assert.IsType<List<CocheParaReviewDTO>>(ok.Value);

            // Ordeno por Id para comparar de forma estable (el action no impone un orden).
            actual = actual.OrderBy(x => x.Id).ToList();
            // Comparo tamaño de listas.
            Assert.Equal(expected.Count, actual.Count);
            // Comparo elemento a elemento todos los campos relevantes del DTO.
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].Id, actual[i].Id);
                Assert.Equal(expected[i].ModelName, actual[i].ModelName);
                Assert.Equal(expected[i].Color, actual[i].Color);
                Assert.Equal(expected[i].FuelType, actual[i].FuelType);
                Assert.Equal(expected[i].Manufacturer, actual[i].Manufacturer);
                Assert.Equal(expected[i].CarClass, actual[i].CarClass);
            }

        }
        // Test para el caso en que no hay coincidencias y el endpoint debe devolver NotFound.
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCars_NotFound_test()
        {
            // Creo el logger simulado y el controlador.
            var logger = new Mock<ILogger<CarsControllers>>().Object;
            var controller = new CarsControllers(_context, logger);

            // Llamo al endpoint con un color inexistente en el seed.
            var result = await controller.GetCars("Gasolina");

            // Verifico que devuelve 404 NotFound y el mensaje definido en el controller.
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No se encontraron coches con ese color.", notFound.Value);
        }

    }
}
