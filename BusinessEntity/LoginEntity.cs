using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity
{
   public class LoginEntity
    {
        public string phoneno { get; set; }

        public string OTP { get; set; }

        public string CORCD { get; set; }

        public string PUBCD { get; set; }

        public string BUZCD { get; set; }

        public List<LoginEntity> LstLogin { get; set; } = new List<LoginEntity>();
               
        public string message { get; set; }

        public string messagecode { get; set; }

        public string status { get; set; }
        public string Location { get; set; }
        public string Locationid { get; set; }
        public string chkmethod { get; set; }
        public string chkcount { get; set; }
        public string chktime { get; set; }
        public string chkdesc { get; set; }
       
        public string val { get; set; }
        public string who { get; set; }

        
        public string Date { get; set; }
        public string chknm { get; set; }
        public string method { get; set; }
        public string D00 { get; set; }
        public string D01 { get; set; }
        public string D02 { get; set; }
        public string D03 { get; set; }
        public string D04 { get; set; }
        public string D05 { get; set; }
        public string D06 { get; set; }
        public string D07 { get; set; }
        public string D08 { get; set; }
        public string D09 { get; set; }
        public string D10 { get; set; }
       
       


        public int ID { get; set; }
    }
}
