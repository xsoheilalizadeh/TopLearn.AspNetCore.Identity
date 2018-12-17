using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace App.Services.Identity.Validators
{
    public class AppUserValidator : UserValidator<User>
    {
        public AppUserValidator(IdentityErrorDescriber errors) : base(errors)
        {
        }

        public override async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            var result = await base.ValidateAsync(manager, user);

            this.ValidateUserName(user,result.Errors.ToList());

            return result;
        }


        private void ValidateUserName(User user, List<IdentityError> errors)
        {
            if (user.UserName.Contains("Admin"))
            {
                errors.Add(new IdentityError
                {
                    Code = "InvalidUser",
                    Description = "این کاربر معتبر نیست"
                });
            }
        }
    }
}