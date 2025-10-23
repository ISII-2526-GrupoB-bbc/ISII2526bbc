using AppForSEII2526.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Formats.Asn1;

namespace AppForSEII2526.API.DTOs.CochesDTO
{
    public class CocheParaAlquilarDTO
    {
        // Constructor con parámetros
        public CocheParaAlquilarDTO(int id, string model, string fuelType, string manufacturer, decimal rentalPrice, string color)
        {
            Id = id;
            ModelName = model;
            FuelType = fuelType;
            Color = color;
            Manufacturer = manufacturer;
            RentalPrice = rentalPrice;
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

        [Required(ErrorMessage = "El precio de alquiler es obligatorio.")]
        [Range(0, 200000, ErrorMessage = "El precio debe ser un valor positivo.")]
        [Display(Name = "Precio de alquiler (€)")]
        public decimal RentalPrice { get; set; }
    }
}
