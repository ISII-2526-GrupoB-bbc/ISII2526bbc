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

        [Range(0, int.MaxValue, ErrorMessage = "El precio de compra no puede ser negativo.")]
        public int PurchasingPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad para compra no puede ser negativa.")]
        public int QuantityForPurchasing { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad para alquiler no puede ser negativa.")]
        public int QuantityForRenting { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El precio de alquiler no puede ser negativo.")]
        public int RentingPrice { get; set; }
       

        public Model Model { get; set; }
        public IList<RentalItem> RentalItems { get; set; }
        public IList<PurchaseItem> PurchaseItems { get; set; }
        public IList<ReviewItem> ReviewItems { get; set; }
    }
}