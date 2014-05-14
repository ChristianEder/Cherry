using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cherry.BDD.Sample.WPF.Services.Authentication
{
    public interface IAuthentication
    {
        bool IsLoggedIn { get; }
        User CurrentUser { get; }
    }

    public class User
    {
        public bool IsAdministrator { get; private set; }
    }
}
