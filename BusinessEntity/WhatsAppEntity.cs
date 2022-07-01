using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity
{
  public  class WhatsAppEntity
    {
        public string receiverMobileNo { get; set; }
       
        public List<LoginEntity> message { get; set; } = new List<LoginEntity>();
    }
}
