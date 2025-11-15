using AppForSEII2526.Models;

namespace AppForSEII2526.API.DTOs.RentalDTOs
{
    public class RentalDetailDTO
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public string DeliveryCarDealer { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RentingDate { get; set; }
        public decimal RentingPrice { get; set; }
        public IList<RentalItemDTO> RentalItems { get; set; }

        public RentalDetailDTO(
            int id,
            string customerName,
            string customerSurname,
            string deliveryCarDealer,
            PaymentMethod paymentMethod,
            DateTime startDate,
            DateTime endDate,
            DateTime rentingDate,
            decimal rentingPrice,
            IList<RentalItemDTO> rentalItems
        )
        {
            Id = id;
            CustomerName = customerName;
            CustomerSurname = customerSurname;
            DeliveryCarDealer = deliveryCarDealer;
            PaymentMethod = paymentMethod;
            StartDate = startDate;
            EndDate = endDate;
            RentingDate = rentingDate;
            RentingPrice = rentingPrice;
            RentalItems = rentalItems;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not RentalDetailDTO other)
                return false;

            return Id == other.Id &&
                   CustomerName == other.CustomerName &&
                   CustomerSurname == other.CustomerSurname &&
                   DeliveryCarDealer == other.DeliveryCarDealer &&
                   PaymentMethod == other.PaymentMethod &&
                   StartDate.Date == other.StartDate.Date &&
                   EndDate.Date == other.EndDate.Date &&
                   RentingDate.Date == other.RentingDate.Date &&
                   RentingPrice == other.RentingPrice &&
                   RentalItems.SequenceEqual(other.RentalItems);
        }
    }
}
