using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppForSEII2526.Models;

namespace AppForSEII2526.UT.CarsController_test
{
    public class GetCocheParaAlquilar_test: AppForSEII25264SqliteUT
    {
        public GetCocheParaAlquilar_test()
        {
            // Creacion de modelos para las pruebas
            var model1 = new Model { Id = 1, Name = "Nissan Qashqai" };
            var model2 = new Model { Id = 2, Name = "Audi R8" };

            // Creacion de coches para las pruebas
            var car1 = new Car
            {
                Id = 1,
                CarClass = "Todoterreno",
                FuelType = "Eléctrico",
                Color = "Negro",
                Description = "Coche premium eléctrico",
                Manufacturer = "Tesla",
                PurchasingPrice = 75000,
                QuantityForPurchasing = 3,
                QuantityForRenting = 1,
                RentingPrice = 300,
                Model = model1
            };

            var car2 = new Car
            {
                Id = 2,
                CarClass = "Deportivo",
                FuelType = "Gasolina",
                Color = "Rojo",
                Description = "Deportivo aleman",
                Manufacturer = "Ford",
                PurchasingPrice = 40000,
                QuantityForPurchasing = 2,
                QuantityForRenting = 1,
                RentingPrice = 5500,
                Model = model2
            };

            _context.Cars.AddRange(car1, car2);

            //Guardo los datos en SQLite
            _context.SaveChanges();
        }
    }
}
