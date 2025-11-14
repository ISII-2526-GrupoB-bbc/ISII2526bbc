using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace AppForSEII2526.Models
{
    [PrimaryKey(nameof(CarId), nameof(PurchaseId))]
    public class PurchaseItem
    {
        

        [Required]
        public int Quantity { get; set; }

        public int PurchaseId { get; set; }
        public int CarId { get; set; }

        [JsonIgnore]
        public Purchase purchase { get; set; }
        public Car car { get; set; }

    }
}
