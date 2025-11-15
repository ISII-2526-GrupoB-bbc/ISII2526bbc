namespace AppForSEII2526.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

   
    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string Surname { get; set; }

        [Required, StringLength(50)]
        public string DeliveryCarDealer { get; set; }

        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        [DataType(DataType.Date)]
        public DateTime PurchasingDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal PurchasingPrice { get; set; }

        //Relacion con PurchaseItem
        public IList<PurchaseItem> PurchaseItems { get; set; }
        public ApplicationUser ApplicationUser { get; set; }


       

    } 
    
   
}
