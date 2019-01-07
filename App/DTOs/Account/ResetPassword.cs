using System.ComponentModel.DataAnnotations;

namespace App.DTOs.Account
{
    public class ResetPassword
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } 
    }
}