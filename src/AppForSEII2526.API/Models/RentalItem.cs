namespace AppForSEII2526.Models
{
    [PrimaryKey(nameof(CarId), nameof(RentalId))]
    public class RentalItem
{
    public int Quantity { get; set; }

    public Car Car { get; set; }
    public Rental Rental { get; set; }
    public int CarId { get; set; }
    public int RentalId { get; set; }
    }
}

