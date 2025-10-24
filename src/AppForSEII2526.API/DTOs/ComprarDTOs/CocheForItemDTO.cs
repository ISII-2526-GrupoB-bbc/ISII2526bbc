using AppForSEII2526.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.CochesDTO
{
    public class CocheForItemDTO
    {
        public CocheForItemDTO(int id, Model model, string color, decimal purchasingPrice, int quantity)
        {
            Id = id;
            Model = model;
            Color = color;
            PurchasingPrice = purchasingPrice;
            Quantity = quantity;
        }

        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El modelo del coche es obligatorio.")]
        [Display(Name = "Modelo del coche")]
        public Model Model { get; set; } = new Model();

        [Required(ErrorMessage = "El color es obligatorio.")]
        [StringLength(30, ErrorMessage = "El color no puede tener más de 30 caracteres.")]
        [Display(Name = "Color del coche")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio de compra es obligatorio.")]
        [Range(0, 200000, ErrorMessage = "El precio debe ser un valor positivo.")]
        [Display(Name = "Precio de compra (€)")]
        public decimal PurchasingPrice { get; set; }

        [Required(ErrorMessage = "Debe indicar la cantidad de coches a comprar.")]
        [Range(1, 10, ErrorMessage = "Debe comprar al menos una unidad y como máximo 10.")]
        [Display(Name = "Cantidad de compra")]
        public int Quantity { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CocheForItemDTO dto &&
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
