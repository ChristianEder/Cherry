using Cherry.BDD.Sample.WPF.Behavior.Users.Contracts;
using Cherry.BDD.Sample.WPF.Services.Authentication;

namespace Cherry.BDD.Sample.WPF.Behavior.Users
{
    public class AnonymousUser : IAnonymousUser
    {
        private readonly IAuthentication _authentication;

        public AnonymousUser(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        public bool Is
        {
            get
            {
                return !_authentication.IsLoggedIn;
            }
        }
    }
}
