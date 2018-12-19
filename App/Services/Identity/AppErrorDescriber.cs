using Microsoft.AspNetCore.Identity;

namespace App.Services.Identity
{
    public class AppErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = nameof(DefaultError),
                Description = "خطایی رخ داده است."
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = "ایمیل وارد شده تکراری می باشد."
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = "نام کاربری وارد شده تکراری می باشد."
            };
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {   
                Code = nameof(InvalidUserName),
                Description = "نام کاربری نامعتبر می باشد."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "رمز عبور شما باید شامل اعداد باشد"
            };
        }
    }
}