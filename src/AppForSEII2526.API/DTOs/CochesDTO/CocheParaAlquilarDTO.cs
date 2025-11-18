using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.CochesDTO
{
    public class CocheParaAlquilarDTO
    {
        public CocheParaAlquilarDTO(int id, string modelName, string fuelType, string manufacturer, decimal rentingPrice, string color)
        {
            Id = id;
            ModelName = modelName;
            FuelType = fuelType;
            Manufacturer = manufacturer;
            RentingPrice = rentingPrice;
            Color = color;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "El modelo del coche es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del modelo no puede tener más de 50 caracteres.")]
        [Display(Name = "Modelo del coche")]
        public string ModelName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color es obligatorio.")]
        [StringLength(30, ErrorMessage = "El color no puede tener más de 30 caracteres.")]
        [Display(Name = "Color del coche")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de combustible es obligatorio.")]
        [Display(Name = "Tipo de combustible")]
        public string FuelType { get; set; } = string.Empty;

        [Required(ErrorMessage = "El fabricante es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del fabricante no puede tener más de 50 caracteres.")]
        [Display(Name = "Fabricante")]
        public string Manufacturer { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio de alquiler es obligatorio.")]
        [Range(0, 200000, ErrorMessage = "El precio debe ser un valor positivo.")]
        [Display(Name = "Precio de alquiler (€)")]
        public decimal RentingPrice { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CocheParaAlquilarDTO dto &&
               Id == dto.Id &&
               ModelName == dto.ModelName &&
               FuelType == dto.FuelType &&
               Manufacturer == dto.Manufacturer &&
               RentingPrice == dto.RentingPrice &&
               Color == dto.Color;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ModelName, FuelType, Manufacturer, RentingPrice, Color);
        }
    }
    }

