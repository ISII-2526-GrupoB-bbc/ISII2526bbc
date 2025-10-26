using AppForSEII2526.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.CarsController_test
{
    public class GetCocheParaComprar_test : AppForSEII25264SqliteUT
    {
        public GetCocheParaComprar_test(){
            // Crear modelos de prueba
            var model1 = new Model { Id = 1, Name = "Model S" };
            var model2 = new Model { Id = 2, Name = "Mustang" };

            _context.Models.AddRange(model1, model2);

            // Crear coches de prueba
            var car1 = new Car
            {
                Id = 1,
                CarClass = "Sedan",
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
                Description = "Clásico americano potente",
                Manufacturer = "Ford",
                PurchasingPrice = 55000,
                QuantityForPurchasing = 2,
                QuantityForRenting = 1,
                RentingPrice = 250,
                Model = model2
            };

            _context.Cars.AddRange(car1, car2);

            // Guardar los datos en SQLite
            _context.SaveChanges();

        }
    }
}
