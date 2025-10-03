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
}
