namespace AppForSEII2526.Models
{
    [PrimaryKey(nameof(CarId), nameof(RentalId))]
    public class RentalItem
{
    public int Quatity { get; set; }

    public Car CarId { get; set; }
    public Rental RentalId { get; set; }
    }
}

