using Microsoft.AspNetCore.Identity;

namespace ManejoPresupuesto.Servicios
{
    public class MensajesDeErrorIdentity: IdentityErrorDescriber
    {
        //Identity Error -
        public override IdentityError DefaultError()
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }

        //Identity Error -
        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }

        //Identity Error -
        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }

        //Identity Error -
        public override IdentityError InvalidToken()
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }


        //Identity Error -
        public override IdentityError LoginAlreadyAssociated()
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }


        //Identity Error -
        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }


        //Identity Error -
        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }


        //Identity Error -
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }


        //Identity Error -
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }
        /////////////////////////////////////
        ///
        //Identity Error -
        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }

        //Identity Error -
        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }

        //Identity Error -
        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }

        //Identity Error -
        public override IdentityError UserLockoutNotEnabled()
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }

        //Identity Error -
        public override IdentityError UserAlreadyInRole(string role)
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }

        //Identity Error -
        public override IdentityError UserNotInRole(string role)
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }

        //Identity Error -
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }

        //Identity Error -
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }
        ///////////
        ///
        //Identity Error -
        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }
        //Identity Error -
        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            { Code = nameof(DefaultError), Description = $"Ha ocurrido un error." };

        }

    }
}
