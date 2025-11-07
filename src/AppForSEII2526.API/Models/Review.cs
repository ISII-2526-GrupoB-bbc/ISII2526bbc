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

    [DataType(DataType.Date)]
    public DateTime Created { get; set; }
    public IList<ReviewItem> ReviewItems { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
        public Review() { }
        public Review(string country, DateTime created, string driverType, IList<ReviewItem> reviewItems, ApplicationUser applicationUser)
        {
            Country = country;
            Created = created;
            DriverType = driverType;
            ReviewItems = reviewItems;
            ApplicationUser = applicationUser;
        }

        }

}


