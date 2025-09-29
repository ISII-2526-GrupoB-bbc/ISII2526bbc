using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII2526.Models
{
    public class PurchaseItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        //Faltarian las relaciones con las clases Car y con las clases Purchase 
       
    }
}
