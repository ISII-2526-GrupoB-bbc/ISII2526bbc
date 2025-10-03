using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    [Key, StringLength(30)]
    public string Name { get; set; }

    [Required, StringLength(30)]
    public string Surname { get; set; }
}