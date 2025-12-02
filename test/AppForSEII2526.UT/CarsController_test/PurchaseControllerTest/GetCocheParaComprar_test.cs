using AppForSEII2526.API.Controllers;          // Importo el namespace del controlador que voy a testear (CarsControllers).
using AppForSEII2526.API.DTOs.CochesDTO;       // Importo los DTOs que devuelve el endpoint (CocheParaComprarDTO).
using AppForSEII2526.Models;

namespace AppForSEII2526.UT.CarsController_test.PurchaseControllerTest
{
    // Defino la clase de tests y heredo de mi fixture SQLite en memoria para tener un DbContext limpio por test.
    public class GetCocheParaComprar_test : AppForSEII25264SqliteUT
    {
        // En el constructor hago el seed de datos que usarán los tests.
        public GetCocheParaComprar_test()
        {
            // ===== Seed de datos =====
            // Creo tres modelos base para asociarlos a los coches.
            var model1 = new Model { Id = 1, Name = "Model S" };
            var model2 = new Model { Id = 2, Name = "Mustang" };
            var model3 = new Model { Id = 3, Name = "RAV4" };
            _context.Models.AddRange(model1, model2, model3); // Inserto los modelos en la BD en memoria.

            // Primer coche: Tesla, negro, eléctrico, con precio de compra y stock.
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
                PurchasingPrice = 55000m,   
                QuantityForPurchasing = 2,
                QuantityForRenting = 1,
                RentingPrice = 250m,
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

            // Inserto los coches y persisto el seed.
            _context.Cars.AddRange(car1, car2, car3);
            _context.SaveChanges();
        }

        // ========= Tests para GetCars (por color) =========

        // Preparo los casos de prueba de éxito para GetCars usando MemberData.
        public static IEnumerable<object[]> GetCarsOKCases()
        {
            // Construyo los DTOs esperados tal y como los proyecta el action GetCars (mismo orden de parámetros).
            var tesla = new CocheParaComprarDTO(1, "Model S", "Negro", "Eléctrico", "Tesla", 75000m);
            var ford = new CocheParaComprarDTO(2, "Mustang", "Rojo", "Gasolina", "Ford", 55000m);
            var toy = new CocheParaComprarDTO(3, "RAV4", "Blanco", "Híbrido", "Toyota", 40000m);

            // Caso sin filtro: espero los tres, y ordeno por Id para comparar de forma determinista.
            var all = new List<CocheParaComprarDTO> { tesla, ford, toy }.OrderBy(d => d.Id).ToList();
            // Caso con filtro de color "Negro": sólo debería devolver el Tesla.
            var onlyBlack = new List<CocheParaComprarDTO> { tesla };

            // Devuelvo los dos casos: (color=null) y (color="Negro"), con sus listas esperadas.
            return new List<object[]>
            {
                new object[] { null,    all },
                new object[] { "Negro", onlyBlack },
            };
        }

        // Test parametrizado para validar que GetCars devuelve 200 OK con la lista esperada.
        [Theory]
        [MemberData(nameof(GetCarsOKCases))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCarsOKtest(string? color, IList<CocheParaComprarDTO> expected)
        {
            // Creo un logger simulado para el controlador.
            var logger = new Mock<ILogger<CarsControllers>>().Object;
            // Instancio el controlador con el contexto en memoria y el logger fake.
            var controller = new CarsControllers(_context, logger);

            // Ejecuto el endpoint con el filtro de color que toque en cada caso.
            var result = await controller.GetCars(color);

            // Verifico que el resultado sea 200 OK y que el Value sea la lista de DTOs.
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var actual = Assert.IsType<List<CocheParaComprarDTO>>(ok.Value);

            // Ordeno por Id para comparar de forma estable (el action no impone un orden).
            actual = actual.OrderBy(x => x.Id).ToList();
            // Comparo tamaño de listas.
            Assert.Equal(expected.Count, actual.Count);
            // Comparo elemento a elemento todos los campos relevantes del DTO.
            Assert.Equal(expected, actual);
        }

        // Test específico para validar el nuevo filtro por modelo en GetCars.
        [Fact]
        public async Task GetCarsFilterByModelOKTest()
        {
            // Arrange
            var logger = new Mock<ILogger<CarsControllers>>().Object;
            var controller = new CarsControllers(_context, logger);

            // Act
            var actionResult = await controller.GetCars(null, "Model S");

            // Assert
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            var cars = Assert.IsAssignableFrom<IEnumerable<CocheParaComprarDTO>>(ok.Value);

            Assert.Single(cars);
            Assert.Equal("Model S", cars.First().ModelName);
        }


        // Test específico para validar el filtro combinado por color y modelo en GetCars.
        [Fact]
        public async Task GetCarsFilterByColorAndModelOKTest()
        {
            var logger = new Mock<ILogger<CarsControllers>>().Object;
            var controller = new CarsControllers(_context, logger);

            var actionResult = await controller.GetCars("Rojo", "Mustang");

            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            var cars = Assert.IsAssignableFrom<IEnumerable<CocheParaComprarDTO>>(ok.Value);

            Assert.Single(cars);
            Assert.Equal("Mustang", cars.First().ModelName);
            Assert.Equal("Rojo", cars.First().Color);
        }



        // Test para el caso en que no hay coincidencias y el endpoint debe devolver NotFound.
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCarsNotFoundtest()
        {
            // Creo el logger simulado y el controlador.
            var logger = new Mock<ILogger<CarsControllers>>().Object;
            var controller = new CarsControllers(_context, logger);

            // Llamo al endpoint con un color inexistente en el seed.
            var result = await controller.GetCars("Azul");

            // Verifico que devuelve 404 NotFound y el mensaje definido en el controller.
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No se encontraron coches con ese color.", notFound.Value);
        }
    }
}
