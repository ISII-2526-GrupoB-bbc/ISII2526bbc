namespace AppForSEII2526.Models
{
    using AppForSEII2526.API.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Rental
{


        protected Rental() { }
        public Rental(
            string deliveryCarDealer,
            DateTime rentingDate,
            DateTime endDate,
            PaymentMethod paymentMethod,
            DateTime startDate,
            List<RentalItem> rentalItems,
            ApplicationUser? user)
        {
            DeliveryCarDealer = deliveryCarDealer;
            RentingDate = rentingDate;
            EndDate = endDate;
            PaymentMethod = paymentMethod;
            StartDate = startDate;
            RentalItems = rentalItems;
            ApplicationUser = user;
        }

        [Key]
    public int Id { get; set; }
    private string DeliveryAddress { get; set; }
    private ApplicationUser? User { get; set; }
    public string DeliveryCarDealer { get; set; }

    public decimal RentingPrice { get; set; }

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



