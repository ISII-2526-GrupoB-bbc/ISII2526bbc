using AppForSEII2526.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Drawing;
using System.Formats.Asn1;

namespace AppForSEII2526.API.DTOs.CochesDTO
{
    public class CocheParaReviewDTO
    {
        public int Id { get; set; }

        [StringLength(20, ErrorMessage = "Model name cannot be any longer than 20 characters, neither shorter than 1.", MinimumLength = 1)]
        public string Model { get; set; }

        [StringLength(20, ErrorMessage = "CarClass cannot be any longer than 20 characters, neither shorter than 1.", MinimumLength = 1)]
        public string CarClass { get; set; }

        [StringLength(20, ErrorMessage = "Manufacturer cannot be any longer than 20 characters, neither shorter than 1.", MinimumLength = 1)]
        public string Manufacturer { get; set; }

        [StringLength(15, ErrorMessage = "FuelType cannot be any longer than 15 characters, neither shorter than 1.", MinimumLength = 1)]
        public string FuelType { get; set; }

        [StringLength(20, ErrorMessage = "Color cannot be any longer than 20 characters, neither shorter than 1.", MinimumLength = 1)]
        public string Color { get; set; }

        public CocheParaReviewDTO(int id, string model, string carClass, string manufacturer, string fuelType, string color)
        {
            Id = id;
            Model = model;
            CarClass = carClass;
            Manufacturer = manufacturer;
            FuelType = fuelType;
            Color = color;
        }

        public override bool Equals(object? obj)
        {
            return obj is CocheParaReviewDTO dTO &&
                   Id == dTO.Id &&
                   Model == dTO.Model &&
                   CarClass == dTO.CarClass &&
                   Manufacturer == dTO.Manufacturer &&
                   FuelType == dTO.FuelType &&
                   Color == dTO.Color;
        }
    }
}
