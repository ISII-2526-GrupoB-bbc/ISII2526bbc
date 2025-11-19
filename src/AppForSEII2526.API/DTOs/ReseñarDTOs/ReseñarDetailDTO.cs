using AppForSEII2526.API.DTOs.ReseñarDTOs;
using AppForSEII2526.Models;
using Humanizer;
using System;
using System.Drawing;

namespace AppForSEII2526.API.DTOs.ReseñarDTOs
{
    public class ReseñarDetailDTO
    {
        [StringLength(20, ErrorMessage = "Name cannot be any longer than 20 characters, neither shorter than 2.", MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Surname cannot be any longer than 100 characters, neither shorter than 4.", MinimumLength = 4)]
        public string Surname { get; set; }

        [StringLength(30, ErrorMessage = "Country cannot be any longer than 30 characters, neither shorter than 3.", MinimumLength = 3)]
        public string Country { get; set; }

        [StringLength(30, ErrorMessage = "DriverType cannot be any longer than 30 characters, neither shorter than 3.", MinimumLength = 3)]
        public string DriverType { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Created { get; set; }

        public IList<ReseñarItemDTO> ReviewItems { get; set; }

        public ReseñarDetailDTO(string name, string surname, string country, string driverType, DateTime created, IList<ReseñarItemDTO> reviewItems)
        {
            Name = name;
            Surname = surname;
            Country = country;
            DriverType = driverType;
            Created = created;
            ReviewItems = reviewItems;
        }

        public override bool Equals(object? obj)
        {
            return obj is ReseñarDetailDTO dTO &&
                   Name == dTO.Name &&
                   Surname == dTO.Surname &&
                   Country == dTO.Country &&
                   DriverType == dTO.DriverType &&
                   Created.Date == dTO.Created.Date &&
                   ReviewItems.SequenceEqual(dTO.ReviewItems);
        }
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Name);
            hash.Add(Surname);
            hash.Add(Country);
            hash.Add(DriverType);
            hash.Add(Created);

            foreach (var item in ReviewItems)
            {
                hash.Add(item);
            }

            return hash.ToHashCode();
        }
      
    }

}




