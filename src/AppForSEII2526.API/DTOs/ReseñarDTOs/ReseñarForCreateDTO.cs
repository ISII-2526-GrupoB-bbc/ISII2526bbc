using AppForSEII2526.API.DTOs.ReseñarDTOs;

namespace AppForSEII2526.API.DTOs.ReseñarDTOs
{
    public class ReseñarForCreateDTO
    {
        [StringLength(20, ErrorMessage = "Name cannot be any longer than 20 characters, neither shorter than 2.", MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Surname cannot be any longer than 100 characters, neither shorter than 4.", MinimumLength = 4)]
        public string Surname { get; set; }

        public string UserName { get; set; }

        [StringLength(30, ErrorMessage = "Country cannot be any longer than 30 characters, neither shorter than 3.", MinimumLength = 3)]
        public string Country { get; set; }

        [StringLength(30, ErrorMessage = "DriverType cannot be any longer than 30 characters, neither shorter than 3.", MinimumLength = 3)]
        public string DriverType { get; set; }

        public IList<ReseñarItemDTO> ReviewItems { get; set; }


        public ReseñarForCreateDTO(string name, string surname, string userName, string country, string driverType, IList<ReseñarItemDTO> reviewItems)
        {
            Name = name ?? throw new ArgumentNullException(nameof(Name));
            Surname = surname;
            UserName = userName ?? throw new ArgumentNullException(nameof(UserName));
            Country = country ?? throw new ArgumentNullException(nameof(Country));
            DriverType = driverType ?? throw new ArgumentNullException(nameof(DriverType));
            ReviewItems = reviewItems ?? throw new ArgumentNullException(nameof(ReviewItems));
        }

        public override bool Equals(object? obj)
        {
            return obj is ReseñarForCreateDTO dTO &&
                   Name == dTO.Name &&
                   Surname == dTO.Surname &&
                   UserName == dTO.UserName &&
                   Country == dTO.Country &&
                   DriverType == dTO.DriverType &&
                   ReviewItems.SequenceEqual(dTO.ReviewItems);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Surname, UserName, Country, DriverType, ReviewItems);
        }
    }
}