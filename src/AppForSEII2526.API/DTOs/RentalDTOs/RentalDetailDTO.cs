namespace AppForMovies.API.DTOs.RentalDTOs
{
    public class RentalDetailDTO : RentalForCreateDTO
    {
        public decimal TotalPrice { get; set; }

        public RentalDetailDTO(
            int id,
            string customerName,
            string customerSurname,
            string deliveryAddress,
            Rental.PaymentMethod paymentMethod,
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
        public int Id { get; set; }

        public DateTime RentalDate { get; set; }

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
