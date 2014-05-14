using Cherry.BDD.Sample.WPF.Behavior.Users.Contracts;
using Cherry.BDD.Sample.WPF.Services.Authentication;

namespace Cherry.BDD.Sample.WPF.Behavior.Users
{
    public class Administrator : IAdministrator
    {
        private readonly IAuthentication _authentication;

        public Administrator(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        public bool Is
        {
            get
            {
                return _authentication.IsLoggedIn && _authentication.CurrentUser.IsAdministrator;
            }
        }
    }
}