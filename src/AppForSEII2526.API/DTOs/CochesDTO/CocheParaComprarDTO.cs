using AppForSEII2526.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Drawing;
using System.Formats.Asn1;

namespace AppForSEII2526.API.DTOs.CochesDTO
{
    public class CocheParaComprarDTO
    {
        // Constructor con parámetros
        public CocheParaComprarDTO(int id,string modelName,string color,string fuelType,string manufacturer ,decimal purchasingPrice)
        {
            Id = id;
            ModelName = modelName;
            Color = color;
            FuelType = fuelType; 
            Manufacturer = manufacturer;
            PurchasingPrice = purchasingPrice;
        }

        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El modelo del coche es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del modelo no puede tener más de 50 caracteres.")]
        [Display(Name = "Modelo del coche")]
        public string ModelName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color es obligatorio.")]
        [StringLength(30, ErrorMessage = "El color no puede tener más de 30 caracteres.")]
        [Display(Name = "Color del coche")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de gasoil es obligatorio")]
        [Display(Name = "Tipo de gasoil ")]
        public string FuelType { get; set; } = string.Empty;

        [Required(ErrorMessage = "El fabricante es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del fabricante no puede tener más de 50 caracteres.")]
        [Display(Name = "Fabricante")]
        public string Manufacturer { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio de compra es obligatorio.")]
        [Range(0, 200000, ErrorMessage = "El precio debe ser un valor positivo.")]
        [Display(Name = "Precio de compra (€)")]
        public decimal PurchasingPrice { get; set; }




    }
}
