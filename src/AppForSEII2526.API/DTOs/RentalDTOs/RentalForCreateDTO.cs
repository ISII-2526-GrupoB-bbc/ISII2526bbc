using AppForSEII2526.Models;

namespace AppForSEII2526.API.DTOs.RentalDTOs
{
    public class RentalForCreateDTO
    {
        public RentalForCreateDTO(string name, string surname, string address, PaymentMethod paymentMethod, DateTime rentalStartDate, DateTime rentalEndDate, IList<RentalItemDTO> rentalItems)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            DeliveryAddress = address ?? throw new ArgumentNullException(nameof(address));
            PaymentMethod = paymentMethod;
            StartDate = rentalStartDate;
            EndDate = rentalEndDate;
            RentalItems = rentalItems ?? throw new ArgumentNullException(nameof(rentalItems));
        }

        public RentalForCreateDTO()
        {
            RentalItems = new List<RentalItemDTO>();
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }


        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        [Display(Name = "Delivery Address")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Delivery address must have at least 10 characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your address for delivery")]
        public string DeliveryAddress { get; set; }

        [EmailAddress]
        [Required]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your Name and Surname")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Name and Surname must have at least 10 characters")]
        public string Surname { get; set; }

        public IList<RentalItemDTO> RentalItems { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        private int NumberOfDays
        {
            get
            {
                return (EndDate - StartDate).Days;
            }
        }

        [Display(Name = "Total Price")]
        [JsonPropertyName("TotalPrice")]
        public decimal TotalPrice
        {
            get
            {
                return RentalItems.Sum(ri => ri.TotalPrice * ri.Quantity);
            }
        }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

        public override bool Equals(object? obj)
        {
            return obj is RentalForCreateDTO dTO &&
                   CompareDate(StartDate, dTO.StartDate) &&
                   CompareDate(EndDate, dTO.EndDate) &&
                   DeliveryAddress == dTO.DeliveryAddress &&
                   Name == dTO.Name &&
                   Surname == dTO.Surname &&
                   RentalItems.SequenceEqual(dTO.RentalItems) &&
                   PaymentMethod == dTO.PaymentMethod &&
                   TotalPrice == dTO.TotalPrice;
        }
    }
}
