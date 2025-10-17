

namespace AppForSEII2526.Models
{
    using AppForSEII2526.API.Models;
    using System.ComponentModel.DataAnnotations;
    public class Rental
{
    [Key]
    public int Id { get; set; }

    public string DeliveryCarDealer { get; set; }
    public int TotalPrice { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }
    [DataType(DataType.Date)]
    public DateTime RentingDate { get; set; }
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Display(Name = "Payment Method")]
    public PaymentMethod PaymentMethod { get; set; }

    public ApplicationUser ApplicationUser {  get; set; }
    public IList<RentalItem> RentalItems { get; set; }
    }

    public enum PaymentMethod
    {
        CreditCard,
        PayPal,
        Cash
    }
}



