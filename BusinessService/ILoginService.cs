using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessEntity;

namespace BusinessService
{
   public interface ILoginService
    {
        LoginEntity GetOtp(LoginEntity loginEntity);
        LoginEntity SaveOtp(LoginEntity loginEntity);
        LoginEntity GetLocaton(LoginEntity loginEntity);
    }
}
