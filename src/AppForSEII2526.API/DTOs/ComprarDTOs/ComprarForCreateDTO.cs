using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.Models;

namespace AppForSEII2526.API.DTOs.ComprarDTOs
{
    public class ComprarForCreateDTO
    {
        public ComprarForCreateDTO(
        string name,
        string surname,
        string address,
        PaymentMethod paymentMethod,
        IList<ComprarForItemDTO> cochesComprados)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            PaymentMethod = paymentMethod;
            CochesComprados = cochesComprados ?? throw new ArgumentNullException(nameof(cochesComprados));
        }


    public ComprarForCreateDTO()
        {
            CochesComprados = new List<ComprarForItemDTO>();
        }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [Display(Name = "Nombre del cliente")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 80 caracteres.")]
        [Display(Name = "Apellidos del cliente")]
        public string Surname { get; set; } = string.Empty;

        
        [Display(Name = "Dirección de entrega")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "La dirección debe tener al menos 10 caracteres.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe indicar una dirección de entrega.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un método de pago.")]
        [Display(Name = "Método de pago")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required(ErrorMessage = "Debe indicar los coches comprados.")]
        [Display(Name = "Coches comprados")]
        public IList<ComprarForItemDTO> CochesComprados { get; set; }

        [Display(Name = "Precio total de la compra (€)")]
        [JsonPropertyName("TotalPrice")]
        public decimal TotalPrice
        {
            get
            {
                return CochesComprados.Sum(c => c.PurchasingPrice * c.Quantity);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is ComprarForCreateDTO dto &&
                   Name == dto.Name &&
                   Surname == dto.Surname &&
                   Address == dto.Address &&
                   PaymentMethod == dto.PaymentMethod &&
                   CochesComprados.SequenceEqual(dto.CochesComprados) &&
                   TotalPrice == dto.TotalPrice;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Surname, Address, PaymentMethod, CochesComprados, TotalPrice);
        }
    }


}
