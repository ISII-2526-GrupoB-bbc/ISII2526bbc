public class Rental
{
    [Key]
    public int Id { get; set; }

    public string DeliveryCarDealer { get; set; }
    public int PaymentMethod { get; set; }
    public int TotalPrice { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }
    [DataType(DataType.Date)]
    public DateTime RentingDate { get; set; }
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    public ApplicationUser User {  get; set; }
    public IList<RentalItem> RentalItems { get; set; }
}
