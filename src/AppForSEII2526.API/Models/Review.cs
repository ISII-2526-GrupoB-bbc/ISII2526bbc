using System;
namespace AppForSEII2526.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string UserName { get; set; }

        [Required, StringLength(50)]
        public string Country { get; set; }

        [Required]
        public string DriverType { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }
        public IList<ReviewItem> ReviewItems { get; set; }

    }
}
