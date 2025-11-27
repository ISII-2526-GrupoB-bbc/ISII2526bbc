using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class PurchaseStateContainer
    {
        // Creamos una instancia de compra cuando se crea el contenedor de estado
        public ComprarForCreateDTO Purchase { get; private set; } = new ComprarForCreateDTO()
        {
            // Lista de coches que el usuario quiere comprar
            CochesComprados = new List<ComprarForItemDTO>()
        };

        // Calculo el precio total en función de los coches seleccionados y sus cantidades
        public decimal TotalPrice
        {
            get
            {
                return Convert.ToDecimal(
                Purchase.CochesComprados.Sum(ci => ci.PurchasingPrice * ci.Quantity)
            );
            }
        }

        // Avisar a la UI de que el estado ha cambiado
        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        // Añadir un coche a la compra
        public void AddCarToPurchase(CocheParaComprarDTO car)
        {
            // Evitamos añadir el mismo coche dos veces
            if (!Purchase.CochesComprados.Any(ci => ci.Id == car.Id))
            {
                // Lo añadimos a la lista de coches comprados
                Purchase.CochesComprados.Add(new ComprarForItemDTO
                {
                    Id = car.Id,
                    Model = new Model { Name = car.ModelName },
                    Color = car.Color,
                    PurchasingPrice = car.PurchasingPrice,
                    Quantity = 1
                   
                });
            }

          
        }

        // Eliminar un coche de la lista de coches seleccionados
        public void RemovePurchaseItem(ComprarForItemDTO item)
        {
            Purchase.CochesComprados.Remove(item);
            NotifyStateChanged();
        }

        // Vaciar todos los coches seleccionados para comprar
        public void ClearPurchaseCart()
        {
            Purchase.CochesComprados.Clear();
            NotifyStateChanged();
        }

        // Cuando la compra se ha procesado correctamente, se reinicia el estado
        public void PurchaseProcessed()
        {
            Purchase = new ComprarForCreateDTO()
            {
                CochesComprados = new List<ComprarForItemDTO>()
            };

            NotifyStateChanged();
        }
    }
}
