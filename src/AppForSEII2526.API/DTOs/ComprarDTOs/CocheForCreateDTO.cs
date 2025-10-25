using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.Models;

namespace AppForSEII2526.API.DTOs.ComprarDTOs
{
    public class CocheForCreateDTO
    {
        public CocheForCreateDTO(
            string name,
            string surname,
            string address,
            DateTime purchasingDate,
            int quantityForPurchasing,
            List<CocheForItemDTO> cochesComprados)
        {
            Name = name;
            Surname = surname;
            Address = address;
            PurchasingDate = purchasingDate;
            QuantityForPurchasing = quantityForPurchasing;
            CochesComprados = cochesComprados;
        }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        [Display(Name = "Nombre del cliente")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Los apellidos del cliente son obligatorios.")]
        [StringLength(80, ErrorMessage = "Los apellidos no pueden tener más de 80 caracteres.")]
        [Display(Name = "Apellidos del cliente")]
        public string Surname { get; set; } 

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(100, ErrorMessage = "La dirección no puede tener más de 100 caracteres.")]
        [Display(Name = "Dirección de entrega")]
        public string Address { get; set; } 

        [Required(ErrorMessage = "La fecha de compra es obligatoria.")]
        [Display(Name = "Fecha de compra")]
        public DateTime PurchasingDate { get; set; }

        [Required(ErrorMessage = "El precio total de la compra es obligatorio.")]
        [Range(0, 2000000, ErrorMessage = "El precio total debe ser un valor positivo.")]
        [Display(Name = "Precio total (€)")]
        public int QuantityForPurchasing { get; set; }

        [Required(ErrorMessage = "Debe indicar los coches comprados.")]
        [Display(Name = "Coches comprados")]
        public List<CocheForItemDTO> CochesComprados { get; set; } = new List<CocheForItemDTO>();

        public CocheForCreateDTO() { }
    }
}
