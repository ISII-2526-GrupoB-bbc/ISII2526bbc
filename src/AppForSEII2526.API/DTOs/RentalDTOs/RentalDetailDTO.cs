using AppForSEII2526.Models;

namespace AppForSEII2526.API.DTOs.RentalDTOs
{
    public class RentalDetailDTO : RentalForCreateDTO
    {
        public decimal TotalPrice { get; set; }

        public RentalDetailDTO(
            int id,
            string customerName,
            string customerSurname,
            string deliveryAddress,
            PaymentMethod paymentMethod,
            DateTime rentalDateFrom,
            DateTime rentalDateTo,
            DateTime rentalDate,
            decimal totalPrice,
            IList<RentalItemDTO> rentalItems
        ) : base(customerName, customerSurname, deliveryAddress, paymentMethod, rentalDateFrom, rentalDateTo, rentalItems)
        {
            Id = id;
            RentalDate = rentalDate;
            TotalPrice = totalPrice;
        }

        public RentalDetailDTO(string name, string surname, string deliveryCarDealer, PaymentMethod paymentMethod, DateTime startDate, DateTime endDate, DateTime rentingDate, int totalPrice, List<RentalItemDTO> rentalItemDTOs)
        {
            Name = name;
            Surname = surname;
            DeliveryCarDealer = deliveryCarDealer;
            PaymentMethod = paymentMethod;
            StartDate = startDate;
            EndDate = endDate;
            RentingDate = rentingDate;
            TotalPrice = totalPrice;
            RentalItemDTOs = rentalItemDTOs;
        }

        public int Id { get; set; }

        public DateTime RentalDate { get; set; }
        public string DeliveryCarDealer { get; }
        public DateTime RentingDate { get; }
        public List<RentalItemDTO> RentalItemDTOs { get; }

        public override bool Equals(object? obj)
        {
            if (obj is RentalDetailDTO dto)
            {
                return base.Equals(obj) &&
                   TotalPrice == dto.TotalPrice &&
                   Id == dto.Id &&
                   RentalDate.Date == dto.RentalDate.Date;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, RentalDate);
        }
    }
}
