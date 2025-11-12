namespace AppForSEII2526.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(30)]
        public string CarClass { get; set; }

        [Required, StringLength(30)]
        public string FuelType { get; set; }

        [Required, StringLength(30)]
        public string Color { get; set; }

        [StringLength(50)]
        public string? Description { get; set; }

        [Required, StringLength(30)]
        public string Manufacturer { get; set; }

        [Range(typeof(decimal), "0", "2000000", ErrorMessage = "El precio de compra no puede ser negativo.")]
        public decimal PurchasingPrice { get; set; }

        [Range(typeof(decimal), "0", "20000000", ErrorMessage = "La cantidad para compra no puede ser negativa.")]
        public decimal QuantityForPurchasing { get; set; }

        [Range(typeof(decimal), "0", "20000000", ErrorMessage = "La cantidad para alquiler no puede ser negativa.")]
        public decimal QuantityForRenting { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El precio de alquiler no puede ser negativo.")]
        public decimal RentingPrice { get; set; }

        [JsonIgnore] // Evita el bucle infinito al serializar
        public Model Model { get; set; }
        public IList<RentalItem> RentalItems { get; set; }
        public IList<PurchaseItem> PurchaseItems { get; set; }
        public IList<ReviewItem> ReviewItems { get; set; }
    }
}