using AppForSEII2526.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public ApplicationUser()
    {
    }

    public ApplicationUser(string id, string name, string surname, string userName, string address)
    {
        Id = id;
        Name = name;
        Surname = surname;
        UserName = userName;
        Email = userName;
        Address = address;
    }



    public ApplicationUser(string name, string surname, IList<Rental> rentals, IList<Purchase> purchases, IList<Review> reviews)
    {
        Name = name;
        Surname = surname;
        Rentals = rentals;
        Purchases = purchases;
        Reviews = reviews;
    }

    [Key, StringLength(30)]
    public string Name { get; set; }

    [Required, StringLength(30)]
    public string Surname { get; set; }

    public string Address { get; set; }

    public IList<Rental> Rentals { get; set; }
    public IList<Purchase> Purchases { get; set; }
    public IList<Review> Reviews { get; set; }

}