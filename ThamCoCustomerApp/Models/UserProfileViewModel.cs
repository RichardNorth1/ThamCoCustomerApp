using System.ComponentModel.DataAnnotations;

namespace ThamCoCustomerApp.Models
{
    public class UserProfileViewModel
    {
        [Required(ErrorMessage = "Profile Image is required")]
        public string ProfileImage { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Forename is required")]
        public string Forename { get; set; }

        public string FullName => Forename + " " + Surname;

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Telephone is required")]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "Street Address is required")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "County is required")]
        public string County { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        public string PostalCode { get; set; }

        public double Balance { get; set; }
    }
}
