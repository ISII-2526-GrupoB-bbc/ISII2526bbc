using AppForSEII2526.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppForSEII2526.API.DTOs.RentalDTOs
{
    public class RentalForCreateDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string CustomerSurname { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 10)]
        public string DeliveryCarDealer { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public DateTime RentingDate { get; set; }

        [Required]
        public string UserName { get; set; }

        public IList<RentalItemDTO> RentalItems { get; set; } = new List<RentalItemDTO>();

        [JsonPropertyName("TotalPrice")]
        public decimal RentingPrice =>
            RentalItems.Sum(ri => ri.RentingPrice * ri.Quantity);

        public RentalForCreateDTO() { }

        public RentalForCreateDTO(
            string customerName,
            string customerSurname,
            string deliveryCarDealer,
            PaymentMethod paymentMethod,
            DateTime startDate,
            DateTime endDate,
            IList<RentalItemDTO> rentalItems)
        {
            CustomerName = customerName;
            CustomerSurname = customerSurname;
            DeliveryCarDealer = deliveryCarDealer;
            PaymentMethod = paymentMethod;
            StartDate = startDate;
            EndDate = endDate;
            RentalItems = rentalItems ?? new List<RentalItemDTO>();
        }
    }
}

