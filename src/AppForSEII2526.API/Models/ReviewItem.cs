using System;
namespace AppForSEII2526.Models
{
public class ReviewItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CarId { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime Reviewed { get; set; }

    public Car Car { get; set; }
    public Review Review { get; set; }
    }
}



