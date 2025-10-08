using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII2526.Models
{
    [PrimaryKey(nameof(CarId), nameof(PurchaseId))]
    public class PurchaseItem
    {
        

        [Required]
        public int Quantity { get; set; }

        public Purchase PurchaseId { get; set; }
        public Car CarId { get; set; }



    }
}
