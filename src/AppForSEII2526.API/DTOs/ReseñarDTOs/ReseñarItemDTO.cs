namespace AppForSEII2526.API.DTOs.ReseñarDTOs
{
    public class ReseñarItemDTO
    {
        public ReseñarItemDTO(string modelo, string manufacturer, string color, int rating, string description = "")
        {
            Modelo = modelo;
            Manufacturer = manufacturer;
            Color = color;
            Rating = rating;
            Description = description;
        }
        public string Modelo { get; set; }
        public string Manufacturer { get; set; }
        public string Color { get; set; }
        public int Rating { get; set; }
        public string? Description { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReseñarItemDTO dTO &&
                   Modelo == dTO.Modelo &&
                   Manufacturer == dTO.Manufacturer &&
                   Color == dTO.Color &&
                   Rating == dTO.Rating &&
                   Description == dTO.Description;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Modelo, Manufacturer, Color, Rating, Description);

        }
    }
}
