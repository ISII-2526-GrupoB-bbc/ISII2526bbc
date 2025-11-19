using System;

namespace AppForSEII2526.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
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

    [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime Created { get; set; }

    public IList<ReviewItem> ReviewItems { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public Review() { }
     

        }

}


