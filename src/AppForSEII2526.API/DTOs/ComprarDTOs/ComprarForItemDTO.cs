using AppForSEII2526.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.CochesDTO
{
    public class ComprarForItemDTO
    {
        public ComprarForItemDTO(int id, Model model, string color, decimal purchasingPrice, int quantity)
        {
            Id = id;
            Model = model;
            Color = color;
            PurchasingPrice = purchasingPrice;
            Quantity = quantity;
        }

        
        public int Id { get; set; }

        
        public Model Model { get; set; }
        
        public string Color { get; set; }
        public decimal PurchasingPrice { get; set; }
        public int Quantity { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ComprarForItemDTO dto &&
                   Id == dto.Id &&
                   Equals(Model, dto.Model) &&
                   Color == dto.Color &&
                   PurchasingPrice == dto.PurchasingPrice &&
                   Quantity == dto.Quantity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Model, Color, PurchasingPrice, Quantity);
        }
    }
}
