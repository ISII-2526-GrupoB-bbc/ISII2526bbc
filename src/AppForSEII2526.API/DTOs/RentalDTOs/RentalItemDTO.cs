using AppForSEII2526.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.CochesDTO
{
    public class RentalItemDTO
    {
        public RentalItemDTO(int id, Model model, string manufacturer, decimal rentalPrice, int quantity)
        {
            Id = id;
            Model = model;
            Manufacturer = manufacturer;
            TotalPrice = rentalPrice;
            Quantity = quantity;
        }


        public int Id { get; set; }
        public Model Model { get; set; }
        public string Manufacturer { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RentalItemDTO dto &&
                   Id == dto.Id &&
                   Equals(Model, dto.Model) &&
                   Manufacturer == dto.Manufacturer &&
                   TotalPrice == dto.TotalPrice &&
                   Quantity == dto.Quantity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Model, Manufacturer, TotalPrice, Quantity);
        }
    }
}
