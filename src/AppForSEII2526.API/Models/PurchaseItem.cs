using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII2526.Models
{
    [PrimaryKey(nameof(CarId), nameof(PurchaseId))]
    public class PurchaseItem
    {
        

        [Required]
        public int Quantity { get; set; }

        public int PurchaseId { get; set; }
        public int CarId { get; set; }
        public Purchase purchase { get; set; }
        public Car car { get; set; }

    }
}
