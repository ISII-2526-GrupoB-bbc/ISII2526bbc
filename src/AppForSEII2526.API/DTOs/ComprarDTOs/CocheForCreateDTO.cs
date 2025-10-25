using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppForSEII2526.API.DTOs.CochesDTO;

namespace AppForSEII2526.API.DTOs.ComprarDTOs
{
    public class CocheForCreateDTO
    {
        public CocheForCreateDTO(
        string name,
        string surname,
        string address,
        string paymentMethod,
        IList<CocheForItemDTO> cochesComprados)
        {
            Name = name;
            Surname = surname;
            Address = address;
            PaymentMethod = paymentMethod;
            CochesComprados = cochesComprados;
        }


    public CocheForCreateDTO() { }

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

        [Required(ErrorMessage = "Debe seleccionar un método de pago.")]
        [Display(Name = "Método de pago")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Debe indicar los coches comprados.")]
        [Display(Name = "Coches comprados")]
        public IList<CocheForItemDTO> CochesComprados { get; set; }
    }


}
