using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity
{
    public class BreakdownEntity
    {
        public string PUBCD { get; set; }
        public string CORCD { get; set; }
        public string BIZCD { get; set; }
        public string Status { get; set; }
        public string OID { get; set; }
        public string LOCCD { get; set; }
        public string LOCNM { get; set; }
        public string SYMPTOM { get; set; }
        public string RWHO { get; set; }
        public string RDATE { get; set; }
        public string RTIME { get; set; }
        public string ReportedOn { get; set; }
        public string RYMD { get; set; }
        public string ACT_WHO { get; set; }
        public string ACT_WHAT { get; set; }
        public string ACT_DATE { get; set; }
        public string ACT_YMD { get; set; }
        public string STATUS_DESC { get; set; }
        public string Message { get; set; }
        public string whatsappresponce { get; set; }
        public string WhatsAppContent { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string phoneno { get; set; }
        public string grp_Cd { get; set; }
        public string BREAKDOWN_TIME { get; set; }
        public string UR_WHAT { get; set; }
        public string CONCLUSION { get; set; }
        public string C_MEASURE { get; set; }
        public string ACT_TAKEN { get; set; }
        public string ACT_PLAN { get; set; }
        public byte[] BYTE_IMG { get; set; }
        public byte[] BYTE_UR_IMG { get; set; }
        public string IMG_B64 { get; set; }
        public string IMG_PATH { get; set; }
        public string IMG_PATH_BR { get; set; }
        public string IMG_PATH_UR { get; set; }
        public Double IMG_SIZE { get; set; }
    }
}
