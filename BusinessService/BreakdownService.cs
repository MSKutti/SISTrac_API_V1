using BusinessEntity;
using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using BusinessService;
using Microsoft.Extensions.Configuration;

namespace BusinessService
{
    public class BreakdownService : IBreakdownService
    {
        #region Declrations
        string WaAPIKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("WHGRPDETAIL")["WhaApiKey"];
        string WaMesURL = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("WHGRPDETAIL")["WhaMsgURL"];
        #endregion

        private readonly string _ConnectionString;
        
        public BreakdownService(IConfiguration _configuration)
        {
            _ConnectionString = _configuration.GetConnectionString("DBConnection");
        }
        public BreakdownEntity GetBreakdownData(BreakdownEntity BreakdownEntity)
        {
            DataTable dt = new DataTable();

            using(OracleConnection con = new OracleConnection(_ConnectionString))
            {
                try
                {
                    var cmd = new OracleCommand("APG_TM25800.GET_BREAKDOWN_REPAIR_DATA", con) { CommandType = CommandType.StoredProcedure };
                    con.Open();
                    cmd.Parameters.Add("IN_CORCD", OracleDbType.NVarchar2, BreakdownEntity.CORCD, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_BIZCD", OracleDbType.NVarchar2, BreakdownEntity.BIZCD, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_LOCCD", OracleDbType.NVarchar2, BreakdownEntity.LOCCD, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_STATUS", OracleDbType.NVarchar2, BreakdownEntity.Status, ParameterDirection.Input);
                    cmd.Parameters.Add("OUT_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                   
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        BreakdownEntity.BIZCD = dr["BIZCD"].ToString();
                        BreakdownEntity.CORCD = dr["CORCD"].ToString();
                        BreakdownEntity.OID= dr["OID"].ToString();
                        BreakdownEntity.LOCCD = dr["LOCCD"].ToString();
                        BreakdownEntity.LOCNM = dr["LOCNM"].ToString();
                        BreakdownEntity.SYMPTOM = dr["SYMPTOM"].ToString();
                        BreakdownEntity.RWHO = dr["RWHO"].ToString();
                        BreakdownEntity.RDATE = Convert.ToString(dr["RDATE"]);
                        BreakdownEntity.RTIME = dr["RTIME"].ToString();
                        BreakdownEntity.RYMD = dr["RYMD"].ToString();
                        BreakdownEntity.ACT_WHO = dr["ACT_WHO"].ToString();
                        BreakdownEntity.ACT_WHAT = dr["ACT_WHAT"].ToString();
                        BreakdownEntity.ACT_DATE = dr["ACT_DATE"].ToString();
                        BreakdownEntity.ACT_YMD = dr["ACT_YMD"].ToString();
                        BreakdownEntity.Status = dr["STATUS"].ToString();
                        BreakdownEntity.STATUS_DESC = dr["STATUS_DESC"].ToString();
                        BreakdownEntity.UR_WHAT = dr["UR_WHAT"].ToString();
                        BreakdownEntity.IMG_PATH_BR = dr["IMG_PATH"].ToString();
                        //BreakdownEntity.IMG_PATH_UR = dr["IMG_PATH"].ToString();
                        //BreakdownEntity.BYTE_IMG=  (byte[])dr["BRKDN_IMG"];
                    }

                    if (BreakdownEntity.IMG_PATH_BR != "")
                    {

                        if (BreakdownEntity.Status == "R")
                        {
                            BreakdownEntity.IMG_PATH_UR = BreakdownEntity.IMG_PATH_BR + "UR.jpg";
                        }

                        BreakdownEntity.IMG_PATH_BR = BreakdownEntity.IMG_PATH_BR + "BR.jpg";

                    }

                    if (BreakdownEntity.LOCNM == null)
                    {
                        var cmds = new OracleCommand("USP_LOCATIONDETAILS", con) { CommandType = CommandType.StoredProcedure };
                        con.Open();
                        cmds.Parameters.Add("PUBCD", OracleDbType.NVarchar2, BreakdownEntity.PUBCD, ParameterDirection.Input);
                        cmds.Parameters.Add("LOCATION_DETAIL_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        OracleDataAdapter das = new OracleDataAdapter(cmds);
                        das.Fill(dt);

                        foreach(DataRow drs in dt.Rows)
                        {
                            BreakdownEntity.LOCCD = drs["LOCCD"].ToString();
                            BreakdownEntity.LOCNM = drs["LOCNM"].ToString();
                        }

                        BreakdownEntity.IMG_PATH_BR = "";
                    }
                   

                    //BreakdownEntity.IMG_B64 = Convert.ToBase64String(BreakdownEntity.BYTE_IMG);

                    return BreakdownEntity;
                }
                catch (Exception ex)
                {
                    BreakdownEntity brentity = new BreakdownEntity();
                    brentity.Message = ex.Message;
                    return brentity;
                }
            }
        }

        public BreakdownEntity SetBreakReport(BreakdownEntity breakdownEntity)
        {
            string WhatsAppContent = null;
           

            using (OracleConnection con =new OracleConnection(_ConnectionString))
            {
                try
                {
                    var cmd = new OracleCommand("APG_TM25800.SET_BREAKDOWN_REPAIR_DATA", con) { CommandType = CommandType.StoredProcedure };
                    con.Open();
                    cmd.Parameters.Add("IN_CORCD", OracleDbType.Varchar2, breakdownEntity.CORCD, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_BIZCD", OracleDbType.Varchar2, breakdownEntity.BIZCD, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_LOCCD", OracleDbType.Varchar2, breakdownEntity.LOCCD, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_OID", OracleDbType.Varchar2, breakdownEntity.OID, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_WHAT", OracleDbType.Varchar2, breakdownEntity.ACT_WHAT, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_WHO", OracleDbType.Varchar2, breakdownEntity.ACT_WHO, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_STATUS", OracleDbType.Varchar2, breakdownEntity.Status, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_DURATION_TIME", OracleDbType.Varchar2, breakdownEntity.BREAKDOWN_TIME, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_UR_WHAT", OracleDbType.Varchar2, breakdownEntity.UR_WHAT, ParameterDirection.Input);

                    cmd.Parameters.Add("IN_CONCLUSION", OracleDbType.Varchar2, breakdownEntity.CONCLUSION, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_C_MEASURE", OracleDbType.Varchar2, breakdownEntity.C_MEASURE, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_ACT_TAKEN", OracleDbType.Varchar2, breakdownEntity.ACT_TAKEN, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_ACT_PLAN", OracleDbType.Varchar2, breakdownEntity.ACT_PLAN, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_BYTE_IMG", OracleDbType.Blob, breakdownEntity.BYTE_IMG, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_BYTE_UR_IMG", OracleDbType.Blob, breakdownEntity.BYTE_UR_IMG, ParameterDirection.Input);
                    var Responce = cmd.ExecuteNonQuery();

                    if (Responce != 0)
                    {
                        breakdownEntity.Message = "Saved successfully";
                        string Whatsapp_con_mes = null;
                        string Whatsapp_Details_of_work = null;
                       // breakdownEntity = GetWhatsAppGroupCode(breakdownEntity);   // THIS IS USING FOR GET WHATSAPP GROUP CODE 

                        if (breakdownEntity.Status =="C")
                        {
                            if(breakdownEntity.ACT_TAKEN != "" || breakdownEntity.ACT_PLAN != "" || breakdownEntity.CONCLUSION !=  "" || breakdownEntity.C_MEASURE != "")
                            {
                                 Whatsapp_Details_of_work = "\n\n" + "*Details Of Work*" + "\n----------------------------";
                                 Whatsapp_con_mes = "\n ----------------------------\n";

                                if (breakdownEntity.ACT_PLAN !="")
                                {
                                    Whatsapp_Details_of_work = Whatsapp_Details_of_work + "\n" + " *Action Plan :* " + breakdownEntity.ACT_PLAN+"\n"; 
                                }
                                if(breakdownEntity.ACT_TAKEN != "")
                                {
                                    Whatsapp_Details_of_work = Whatsapp_Details_of_work + "\n" +" *Action Taken :* " + breakdownEntity.ACT_TAKEN;
                                }
                                if(breakdownEntity.C_MEASURE != "")
                                {                                    
                                    Whatsapp_con_mes = Whatsapp_con_mes+ " *Counter Measure :* "+ breakdownEntity.C_MEASURE +"\n\n"; 
                                }
                                if(breakdownEntity.CONCLUSION != "")
                                {
                                    Whatsapp_con_mes = Whatsapp_con_mes+ " *Conclusion :* "+ breakdownEntity.CONCLUSION;
                                }                             
                            }

                            WhatsAppContent = "\n[ *Repair Completed* ] [ *Alert* ]" + "\n\n*Location*\n" + "-----------------\n" + breakdownEntity.LOCNM + "\n\n" + "*Status :* " + breakdownEntity.STATUS_DESC + "\n\n*Breakdown*\n" + "--------------------" + "\nReported By (Mobile #) : " + breakdownEntity.phoneno + "\nDate : " + breakdownEntity.ReportedOn + "\n\n*Repair Completed*\n" + "-----------------------------" + "\nCarried By (Mobile #) : " + breakdownEntity.ACT_WHO + "\n" + "Date : " + breakdownEntity.date + "   " + breakdownEntity.time + "\n\n" + "*Breakdown Duration :* \n" + breakdownEntity.BREAKDOWN_TIME +"\n\n"+"*Reason :* "+ breakdownEntity.ACT_WHAT+ Whatsapp_Details_of_work+Whatsapp_con_mes;
                        }
                        else if(breakdownEntity.Status=="R")
                        {
                            WhatsAppContent = "\n[ *Under Repair* ] [ *Alert* ]" + "\n\n*Location*\n" + "-----------------\n" + breakdownEntity.LOCNM + "\n\n" + "*Status :* " + breakdownEntity.STATUS_DESC + "\n\n*Breakdown*\n" + "--------------------" + "\nReported By (Mobile #) : " + breakdownEntity.phoneno + "\nDate : "+ breakdownEntity.ReportedOn + "\n\n*Under Repair*\n" + "----------------------" + "\nCarried By (Mobile #) : " + breakdownEntity.ACT_WHO + "\n" +"Date : "+ breakdownEntity.date + "   " + breakdownEntity.time + "\n\n" + "*Description :* " + breakdownEntity.ACT_WHAT;
                        }
                        else
                        {
                            WhatsAppContent = "\n[ *Breakdown* ] [ *Alert* ]" + "\n\n*Location*\n" + "-----------------\n" + breakdownEntity.LOCNM + "\n\n*Reported By* (Mobile #) : " + breakdownEntity.ACT_WHO + "\n\n " + breakdownEntity.date + "  | |  " + breakdownEntity.time + "\n\n" + "*Status :* " + breakdownEntity.STATUS_DESC + "\n\n" + "*Description :* " + breakdownEntity.ACT_WHAT;
                        }


                        breakdownEntity.WhatsAppContent = WhatsAppContent;

                        if (breakdownEntity.grp_Cd != null)
                        {
                            breakdownEntity = PostJsonToGivenUrl(breakdownEntity);
                        }
                        else
                        {
                            breakdownEntity = PostJsonToGivenUrl(breakdownEntity);
                           
                            breakdownEntity.Message = "Contact Admin. ERR0001";
                        }
                    }

                    return breakdownEntity;
                    
                }
                catch(Exception ex)
                {
                    BreakdownEntity brentity = new BreakdownEntity();
                    brentity.Message = ex.Message;
                    return brentity;
                }
            }
        }

        public BreakdownEntity GetWhatsAppGroupCode(BreakdownEntity BrkDEntity)
        {
            try
            {
                DataTable dt = new DataTable();
                using (var connection = new OracleConnection(_ConnectionString))
                {
                    var command = new OracleCommand("USP_GET_WA_GRPCD", connection) { CommandType = CommandType.StoredProcedure };
                    connection.Open();
                    command.Parameters.Add("IN_GRPCD", OracleDbType.Varchar2, "G16", ParameterDirection.Input);
                    command.Parameters.Add("GRPCD_DETAIL_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    OracleDataAdapter Da = new OracleDataAdapter(command);
                    Da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        BrkDEntity.grp_Cd = row["wagrid"].ToString();
                    }
                    return BrkDEntity;
                }
            }
            catch (Exception ex)
            {
                BrkDEntity.Message = ex.Message;
                return BrkDEntity;
            }
        }

        public BreakdownEntity PostJsonToGivenUrl(BreakdownEntity BrkEntity)
        {
            try
            {
                string resultOfPost = string.Empty;
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.CreateHttp(WaMesURL);
                httpRequest.Method = "POST";
                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";

                using (StreamWriter writer = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    //string json = File.ReadAllText("WAGrp.json");
                    string json = File.ReadAllText("optin.json");
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    //JToken jToken = jsonObj.SelectToken("recipientIds[0]");
                    //jToken.Replace(jsonObject.grp_Cd);

                    JToken jToken = jsonObj.SelectToken("receiverMobileNo");
                    //jToken.Replace(BrkEntity.phoneno);
                      jToken.Replace("919886308258,916382210064,919515561977,919177367170,916281171453,918147073944");
                    //jToken.Replace("916382210064,919886308258");

                    JToken jToken1 = jsonObj.SelectToken("message[0]");
                    jToken1.Replace(BrkEntity.WhatsAppContent);

                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    //File.WriteAllText("WAGrp.json", output);
                    File.WriteAllText("optin.json", output);
                    writer.Write(output);
                }

                httpRequest.Headers.Add("x-api-key", WaAPIKey);
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    // resultOfPost = streamReader.ReadToEnd();
                    BrkEntity.whatsappresponce = streamReader.ReadToEnd();
                }

                return BrkEntity;
            }
            catch (Exception ex)
            {
                BrkEntity.Message = ex.Message;
                return BrkEntity;
            }
        }

    }
}
