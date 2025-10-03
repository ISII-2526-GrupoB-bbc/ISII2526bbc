public class Car
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string CarClass { get; set; }

    [Required, StringLength(50)]
    public string Description { get; set; }

    [Required, StringLength(30)]
    public string Manufacturer { get; set; }

    public int PurchasingPrice { get; set; }
    public int QuantityForPurchasing { get; set; }
    public int QuantityForRenting { get; set; }
    public int RentalIntems { get; set; }
    public int RentingPrice { get; set; }
    public int ReviewItems { get; set; }

    public Model Model { get; set; }
    public IList<RentalItem> RentalItems { get; set; }
    public IList<PurchaseItem> PurchaseItems { get; set; }
    public IList<ReviewItem> ReviewItems { get; set; }
}