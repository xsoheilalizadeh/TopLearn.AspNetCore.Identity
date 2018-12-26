using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace App.DTOs.Account
{
    public class RegisterAccount
    {
        public RegisterAccount()
        {
            RegisteredOn = DateTime.Now;
        }

        [Required]
        [Remote("ValidateUserName",ErrorMessage = "نام کاربری قبلا ثبت شده است.")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Remote("ValidateEmail", ErrorMessage = "ایمیل وارد شده قبلا ثبت شده است.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage = "تکرار رمز عبور اشتباه می باشد.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }    

        public DateTime RegisteredOn { get; set; }
    }
}