using System;
namespace AppForSEII2526.Models
{
    using System.ComponentModel.DataAnnotations;
    [PrimaryKey(nameof(CarId), nameof(ReviewId))]
    public class ReviewItem
{

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime Reviewed { get; set; }

    public Car Car { get; set; }
    public Review Review { get; set; }
    public int CarId { get; set; }
    public int ReviewId { get; set; }
        }
}



