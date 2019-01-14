using System.ComponentModel.DataAnnotations;

namespace App.DTOs.Manager.User
{
    public class EditUser
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public int Age { get; set; }    
    }
}