using AppForSEII2526.Web;

namespace AppForSEII2526.Web
{
    public class RentalStateContainer
    {

        //creamos una instancia de rental cuando una instancia de RentalStateContainer se crea
        public RentalForCreateDTO Rental { get; private set; } = new RentalForCreateDTO()
        {
            RentalItems = new List<RentalItemDTO>()
        };

        //calculo el precio total en funcion de los dias y los items
        public decimal TotalPrice
        {
            get
            {
                int numberOfDays = (Rental.EndDate - Rental.StartDate).Days;
                return Convert.ToDecimal(Rental.RentalItems.Sum(ri => ri.RentingPrice * numberOfDays));
            }
        }

        //Avisar a la UI de que el estado ha cambiado
        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();



        //Añadir un coche al alquiler
        public void AddCarToRental(CocheParaAlquilarDTO car)
        {
            //Evitamos añadir el mismo coche dos veces
            if (!Rental.RentalItems.Any(ri => ri.Id == car.Id))
                //Lo añadimos a la lista
                Rental.RentalItems.Add(new RentalItemDTO()
                {
                    ModelId = car.Id,                   //ID del modelo
                    ModelName = car.ModelName,          //Nombre del modelo
                    Manufacturer = car.Manufacturer,    //Fabricante
                    RentingPrice = car.RentingPrice,    //Precio de alquiler
                    Quantity = 1                        //Se añade una unidad
                }
            );

        }

        //Eliminar un coche de la lista de coches seleccionados
        public void RemoveRentalItemToRent(RentalItemDTO item)
        {
            Rental.RentalItems.Remove(item);
            NotifyStateChanged();

        }

        //Vaciar todos los coches seleccionados
        public void ClearRentingCart()
        {
            Rental.RentalItems.Clear();
            NotifyStateChanged();

        }

        //Cuando el alquiler se ha procesado correctamente, se reinicia el estado para empezar uno nuevo
        public void RentalProcessed()
        {
            Rental = new RentalForCreateDTO()
            {
                RentalItems = new List<RentalItemDTO>()
            };

            NotifyStateChanged();
        }
    }
}
