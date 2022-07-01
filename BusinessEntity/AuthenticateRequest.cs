using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity
{
    public class AuthenticateRequest
    {
        public string EmailID { get; set; }
        public string Password { get; set; }
    }
    public class AppSettings
    {
        public string Secret { get; set; }
    }
}
