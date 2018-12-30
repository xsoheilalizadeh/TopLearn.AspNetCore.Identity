using System.ComponentModel.DataAnnotations;

namespace App.DTOs.Account
{
    public class VerifyCode
    {
        [Required]
        public string Code { get; set; }

        public string Provider { get; set; }

        public bool RememberMe { get; set; }

        public bool BrowserRemember { get; set; }

        public string ReturnTo { get; set; }
    }
}