using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity
{
    public class CheckListEntity
    {
        public string Location { get; set; }
        public string Period { get; set; }
        public string docCD { get; set; }
        public string chkCD { get; set; }
        public int ID { get; set; }

        public string LocationId { get; set; }

        public string CORCD { get; set; }

        public string PUBCD { get; set; }

        public string BUZCD { get; set; }

        public string Description { get; set; }

        public string chkmethod { get; set; }

        public string chktitle { get; set; }

        public string rdbval { get; set; }

        public string date { get; set; }
        public string time { get; set; }

        public string phoneno { get; set; }
        public string Message { get; set; }
        public string WhatsAppContent { get; set; }
        public string whatsappresponce { get; set; }

        public string grp_Cd { get; set; }
        public string NValue { get; set; }
        public string Value_Type { get; set; }

        public List<CheckListEntity> LstChkList { get; set; } = new List<CheckListEntity>();
    }
}
