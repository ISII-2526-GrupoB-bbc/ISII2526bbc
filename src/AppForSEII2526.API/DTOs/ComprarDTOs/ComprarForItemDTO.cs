using AppForSEII2526.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.CochesDTO
{
    public class ComprarForItemDTO
    {
        public ComprarForItemDTO(int id, Model model, string color, string description, decimal purchasingPrice, int quantity)
        {
            Id = id;
            Model = model;
            Color = color;
            Description = description;
            PurchasingPrice = purchasingPrice;
            Quantity = quantity;
        }

        public int Id { get; set; }
        public Model Model { get; set; }
        public string Color { get; set; }

        public string Description { get; set; }  // 👈 NUEVO

        public decimal PurchasingPrice { get; set; }
        public int Quantity { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is not ComprarForItemDTO dto)
                return false;

            // Comparo modelos por su identidad lógica, no por referencia
            bool sameModel =
                (Model == null && dto.Model == null) ||
                (Model != null && dto.Model != null &&
                 Model.Id == dto.Model.Id &&
                 Model.Name == dto.Model.Name);

            return Id == dto.Id &&
                   sameModel &&
                   Color == dto.Color &&
                   Description == dto.Description &&
                   PurchasingPrice == dto.PurchasingPrice &&
                   Quantity == dto.Quantity;
        }

        public override int GetHashCode()
        {
            // Uso las mismas propiedades que en Equals
            return HashCode.Combine(
                Id,
                Model?.Id,
                Model?.Name,
                Color,
                Description,
                PurchasingPrice,
                Quantity);
        }
    }
}

