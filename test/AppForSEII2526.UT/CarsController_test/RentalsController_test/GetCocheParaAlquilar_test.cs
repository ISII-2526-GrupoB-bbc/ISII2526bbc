using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//PRUEBA DEL MÉTODO GetCars3 -> Filtro por precio de alquiler

namespace AppForSEII2526.UT.CarsController_test.RentalsController_test
{
    public class GetCocheParaAlquilar_test: AppForSEII25264SqliteUT
    {
        public GetCocheParaAlquilar_test()
        {
            // Creacion de modelos para las pruebas
            var model1 = new Model { Id = 1, Name = "Qashqai" };
            var model2 = new Model { Id = 2, Name = "R8" };
            var model3 = new Model { Id = 3, Name = "Model S" };
            var model4 = new Model { Id = 4, Name = "Mustang" };

            // Creacion de coches para las pruebas
            var car1 = new Car
            {
                Id = 1,
                CarClass = "Todoterreno",
                FuelType = "Eléctrico",
                Color = "Blanco",
                Description = "Coche premium eléctrico",
                Manufacturer = "Nissan",
                PurchasingPrice = 75000m,
                QuantityForPurchasing = 3,
                QuantityForRenting = 1,
                RentingPrice = 35000m,
                Model = model1
            };

            var car2 = new Car
            {
                Id = 2,
                CarClass = "Deportivo",
                FuelType = "Gasolina",
                Color = "Rojo",
                Description = "Deportivo",
                Manufacturer = "Audi",
                PurchasingPrice = 40000m,
                QuantityForPurchasing = 2,
                QuantityForRenting = 1,
                RentingPrice = 55000m,
                Model = model2
            };

            var car3 = new Car
            {
                Id = 3,
                CarClass = "Sedan",
                FuelType = "Eléctrico",
                Color = "Blanco",
                Description = "Coche premium eléctrico",
                Manufacturer = "Tesla",
                PurchasingPrice = 75000m,
                QuantityForPurchasing = 3,
                QuantityForRenting = 1,
                RentingPrice = 30000m,
                Model = model3
            };

            var car4 = new Car
            {
                Id = 4,
                CarClass = "Deportivo",
                FuelType = "Gasolina",
                Color = "Rojo",
                Description = "Clásico americano potente",
                Manufacturer = "Ford",
                PurchasingPrice = 55000m,
                QuantityForPurchasing = 2,
                QuantityForRenting = 1,
                RentingPrice = 25000m,
                Model = model4
            };


            _context.Cars.AddRange(car1, car2, car3, car4); //Inserto los coches

            //Guardo los datos en SQLite
            _context.SaveChanges();
        }

        //====== Test para getCars3 ======

        // Estos son los casos de prueba con éxito
        public static IEnumerable<object[]> GetCarsRental_OK_Cases()  //Este metodo prepara los datos de entrada y salida esperados
        {
            // Construyo los DTOs esperados tal y como los proyecta el action GetCars (mismo orden de parámetros).
            var nissan = new CocheParaAlquilarDTO(1, "Qashqai", "Eléctrico", "Nissan", 35000, "Blanco");
            var audi = new CocheParaAlquilarDTO(2, "R8", "Gasolina", "Audi", 55000, "Rojo");
            var tesla = new CocheParaAlquilarDTO(3, "Model S", "Eléctrico", "Tesla", 30000, "Blanco");
            var ford = new CocheParaAlquilarDTO(4, "Mustang", "Gasolina", "Ford", 25000, "Rojo");

            //Creo una lista con los coches esperados, ordenada por Id
            //Esta lista representa lo que debe devolver el controlador sin aplicar ningun filtro
            var all = new List<CocheParaAlquilarDTO> { nissan, audi, tesla, ford }.OrderBy(c => c.Id).ToList(); 

            //Esta lista contiene solo el coche Ford, el unico con precio 25000
            //Esta lista es el resultado esperado de aplicar un filtro de 25000
            var only25000 = new List<CocheParaAlquilarDTO> { ford };

            return new List<object[]>
            {
                new object[] { null, null, all },         //Si no aplico ningun filtro (null), devuelvo todos (all)
                new object[] { null, 25000m, only25000 }, //Si aplico filtro 25000, devuelve el Ford (only25000)
            };
        }




        //TEST PARAMETRIZADO: 200 OK + Lista Esperada

        //Este metodo comprueba que GetCars3:
            //Devuelve 200 OK cuando todo va bien
            //Devuelve la lista esperada de coches según el filtro
            //Los datos de salida coinciden campo por campo con lo esperado

        [Theory]
        [MemberData(nameof(GetCarsRental_OK_Cases))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]

        //Este es el metodo del test
        public async Task GetCarsRental_OK_test(string? modelName, decimal? rentingPrice, IList<CocheParaAlquilarDTO> expected)
        {
            var logger = new Mock<ILogger<CarsControllers>>().Object;
            var controller = new CarsControllers(_context, logger);             //Se instancia el controlado real para simular una ejecución real

            var result = await controller.GetCarsRental(rentingPrice, modelName);               //Llamo a GetCarsRental con el parámetro de filtro -> result puede ser Ok, NotFound...

            var ok = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<List<CocheParaAlquilarDTO>>(ok.Value);

            // Ordenamos por Id para comparación determinista
            actual = actual.OrderBy(x => x.Id).ToList();
            expected = expected.OrderBy(x => x.Id).ToList();

            Assert.Equal(expected, actual);     //Comprobacion usando el EQUALS de CocheParaAlquilarDTO
        }

        // Test NotFound: cuando no hay coincidencias por precio
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCarsRental_NotFound_test()
        {
            var logger = new Mock<ILogger<CarsControllers>>().Object;
            var controller = new CarsControllers(_context, logger);

            var result = await controller.GetCarsRental(99999m, "modelo_que_no_existe");

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No se encontraron coches con los filtros proporcionados.", notFound.Value);
        }
    }
}







