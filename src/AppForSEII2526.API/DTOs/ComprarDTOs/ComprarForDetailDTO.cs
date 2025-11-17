using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppForSEII2526.API.DTOs.CochesDTO;
using AppForSEII2526.Models;

namespace AppForSEII2526.API.DTOs.ComprarDTOs
{
    public class ComprarForDetailDTO : ComprarForCreateDTO
    {
        public ComprarForDetailDTO(
        int id,
        DateTime purchasingDate,
        string name,
        string surname,
        string address,
        PaymentMethod paymentMethod,
        IList<ComprarForItemDTO> cochesComprados)
        : base(name, surname, address, paymentMethod, cochesComprados)
        {
            Id = id;
            PurchasingDate = purchasingDate;
        }


        

        [Required]
        [Display(Name = "Identificador de la compra")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fecha de compra")]
        public DateTime PurchasingDate { get; set; }

        [Display(Name = "Cantidad total de coches comprados")]
        public int QuantityForPurchasing
        {
            get
            {
                int total = 0;
                foreach (var coche in CochesComprados)
                {
                    total += coche.Quantity;
                }
                return total;
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is ComprarForDetailDTO dto &&
                   Id == dto.Id &&
                   Name == dto.Name &&
                   Surname == dto.Surname &&
                   Address == dto.Address &&
                   PaymentMethod == dto.PaymentMethod &&
                   CompareDate(PurchasingDate, dto.PurchasingDate) &&
                   CochesComprados.SequenceEqual(dto.CochesComprados);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, PurchasingDate);
        }

        // Igual que en el ejemplo de películas: comparar fechas con un margen de minutos
        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }
    }


}
