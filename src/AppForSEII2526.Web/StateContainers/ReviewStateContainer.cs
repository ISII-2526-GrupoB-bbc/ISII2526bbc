using AppForSEII2526.Web;

namespace AppForSEII2526.Web
{
    public class ReviewStateContainer
    {

        // Se crea un objeto Review al instanciar la clase
        public ReseñarForCreateDTO Review { get; private set; } = new ReseñarForCreateDTO()
        {
            ReviewItems = new List<ReseñarItemDTO>()
        };

        public event Action? OnChange;

        // Agregar coche para reseñar (sin duplicados)
        public void AddCarToReview(CocheParaReviewDTO car)
        {
            if (!Review.ReviewItems.Any(r => r.Model == car.Model))
                Review.ReviewItems.Add(new ReseñarItemDTO()
                {
                    Model = car.Model,
                    Manufacturer = car.Manufacturer,
                    FuelType = car.FuelType,
                    Color = car.Color,
                });
        }


        // Eliminar coche del carrito de reseñas
        public void RemoveCarFromReview(ReseñarItemDTO carReview)
        {
            Review.ReviewItems.Remove(carReview);
        }


        // Borrar todos los coches seleccionados
        public void ClearReviewCart()
        {
            Review.ReviewItems.Clear();
        }

        public void ReviewProcessed()
        {
            Review = new ReseñarForCreateDTO()
            {
                ReviewItems = new List<ReseñarItemDTO>()
            };
            
        }
    }
}
