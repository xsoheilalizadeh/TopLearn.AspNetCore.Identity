using System.ComponentModel.DataAnnotations;

namespace App.DTOs.Account
{
    public class ExternalLoginConfirm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }    
    }
}