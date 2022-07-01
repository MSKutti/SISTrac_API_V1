using BusinessEntity;
using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;


namespace BusinessService
{
    public class HomeService : IHomeService
    {
        #region Declrations
        string WaAPIKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("WHGRPDETAIL")["WhaApiKey"];
        string WaMesURL = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("WHGRPDETAIL")["WhaMsgURL"];
        #endregion

        private readonly string _connectionString;
        public HomeService(IConfiguration _configuratio)
        {
            _connectionString = _configuratio.GetConnectionString("DBConnection");
        }

        //This code used to get the ChecklistData of the Machine Location 
        public CheckListEntity GetChecklistData(CheckListEntity ChkEntity)
        {
            List<CheckListEntity> CHECKLISTLOGIN = new List<CheckListEntity>();

            DataTable dt = new DataTable();
            using (OracleConnection con = new OracleConnection(_connectionString))
            {
                try
                    {
                        var cmd = new OracleCommand("USP_CHECKLISTDETAILS", con) { CommandType = CommandType.StoredProcedure };
                        con.Open();
                        cmd.Parameters.Add("PUBHCD", OracleDbType.Varchar2, ChkEntity.PUBCD, ParameterDirection.Input);
                        cmd.Parameters.Add("CHECKLIST_DETAIL_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        OracleDataAdapter Da = new OracleDataAdapter(cmd);
                        Da.Fill(dt);
                        
                       foreach (DataRow row in dt.Rows)
                        {
                            CheckListEntity ChklEntity = new CheckListEntity
                            {
                                LocationId = row["loccd"].ToString(),
                                Location = row["locnm"].ToString(),
                                docCD = row["doccd"].ToString(),
                                Period = row["docnm"].ToString(),
                                chkCD = row["chkcd"].ToString(),
                                chktitle = row["chknm"].ToString(),     
                                BUZCD = row["bizcd"].ToString(),
                                CORCD = row["corcd"].ToString(),
                                PUBCD = row["pubcd"].ToString(),
                                chkmethod = row["method"].ToString(),
                                Value_Type=row["value_type"].ToString()
                            };
                            CHECKLISTLOGIN.Add(ChklEntity); 
                        }
                        ChkEntity.LstChkList = CHECKLISTLOGIN;
                    }
                    catch (Exception ex)
                    {
                    CheckListEntity clslog = new CheckListEntity();
                    clslog.Message = ex.Message;
                    return clslog;
                }
                    finally
                    {
                        con.Close();
                    }                
            }
            return ChkEntity;
        }

       
        //This code used to get the location of the checklist
        public HomeEntity GetLocaton(HomeEntity homeentity) 
        {
            DataTable dt = new DataTable();
            using (var connection = new OracleConnection(_connectionString))
            {
                try
                {
                    var command = new OracleCommand("USP_LOCATIONDETAILS", connection) { CommandType = CommandType.StoredProcedure };
                    connection.Open();
                    command.Parameters.Add("PUBHCD", OracleDbType.Varchar2, homeentity.PUBCD, ParameterDirection.Input);
                    command.Parameters.Add("LOCATION_DETAIL_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    OracleDataAdapter Da = new OracleDataAdapter(command);
                    Da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        homeentity.Location = row["LOCNM"].ToString();
                        homeentity.LocCd = row["loccd"].ToString();
                        homeentity.docCd = row["doccd"].ToString();
                        homeentity.PUBCD = row["pubcd"].ToString();
                       
                    }
                    return homeentity;
                }
                catch(Exception ex)
                {
                    HomeEntity clslog = new HomeEntity();
                    clslog.Message = ex.Message;
                    return clslog;
                }

             }   
        }

       // This code used to save all the checklist data
        public CheckListEntity SavechecklistData(CheckListEntity chkEntity)
        {
           // string WhatsAppContentHeader = null;
            string WhatsAppContent = null;
            string WhatsAppContentNumaric = null;
            string WhatsAppContentText = null;
            string WhatsAppContentNumaricText = null;
           // int Whats_Total = 0;
            int Whats_OKs = 0;           
            int Whats_NGs = 0;           
            int Whats_Waits = 0;
            int Whats_Numaric = 0;
            int Whats_Text = 0;
            using (var connection = new OracleConnection(_connectionString))
            {
                try
                {
                    if (chkEntity.LstChkList.Count > 0)
                    {
                        for (int i = 0; i < chkEntity.LstChkList.Count; i++)
                        {
                            var command = new OracleCommand("APG_TM25500.SET_LAST_CHK_DATA", connection) { CommandType = CommandType.StoredProcedure };
                            connection.Open();
                            command.Parameters.Add("IN_CORCD", OracleDbType.Varchar2, chkEntity.LstChkList[i].CORCD, ParameterDirection.Input);
                            command.Parameters.Add("IN_BIZCD", OracleDbType.Varchar2, chkEntity.LstChkList[i].BUZCD, ParameterDirection.Input);
                            command.Parameters.Add("IN_PUBCD", OracleDbType.Varchar2, chkEntity.LstChkList[i].PUBCD, ParameterDirection.Input);
                            command.Parameters.Add("IN_DOCCD", OracleDbType.Varchar2, chkEntity.LstChkList[i].docCD, ParameterDirection.Input);
                            command.Parameters.Add("IN_CHKCD", OracleDbType.Varchar2, chkEntity.LstChkList[i].chkCD, ParameterDirection.Input);
                            command.Parameters.Add("IN_VAL", OracleDbType.Varchar2, chkEntity.LstChkList[i].rdbval, ParameterDirection.Input);
                            command.Parameters.Add("IN_COMM", OracleDbType.Varchar2, chkEntity.LstChkList[i].Description, ParameterDirection.Input);
                            command.Parameters.Add("IN_WHO", OracleDbType.Varchar2, chkEntity.LstChkList[i].phoneno, ParameterDirection.Input);
                            command.Parameters.Add("IN_LOCCD", OracleDbType.Varchar2, chkEntity.LstChkList[i].LocationId, ParameterDirection.Input);

                            var Response = command.ExecuteNonQuery();

                            if (Response != 0)
                            {
                                chkEntity.Message = "Updated Successful";
                                if (chkEntity.LstChkList[i].rdbval == "OK")
                                {
                                    Whats_OKs = Whats_OKs + 1;
 
                                } else if (chkEntity.LstChkList[i].rdbval == "NG")
                                {
                                    Whats_NGs = Whats_NGs + 1;
                                    WhatsAppContent = WhatsAppContent + "\n" + (i + 1) + ") " + chkEntity.LstChkList[i].chktitle + "\n" + "Description : " + chkEntity.LstChkList[i].Description + "\n" + "Status : " + chkEntity.LstChkList[i].rdbval + "\n";

                                }
                                else if (chkEntity.LstChkList[i].rdbval == "WAIT")
                                {
                                    Whats_Waits = Whats_Waits + 1;
                                    WhatsAppContent = WhatsAppContent + "\n" + (i + 1) + ") " + chkEntity.LstChkList[i].chktitle + "\n" + "Description : " + chkEntity.LstChkList[i].Description + "\n" + "Status : " + chkEntity.LstChkList[i].rdbval + "\n";

                                }
                                else if(chkEntity.LstChkList[i].NValue != null)
                                {
                                    Whats_Numaric = Whats_Numaric + 1;
                                    WhatsAppContentNumaric = WhatsAppContentNumaric + "\n" + (i + 1) + ") " + chkEntity.LstChkList[i].chktitle + "\n" + "Description : " + chkEntity.LstChkList[i].Description + "\n" + "Value : " + chkEntity.LstChkList[i].rdbval + "\n";

                                }else
                                {
                                    Whats_Text = Whats_Text + 1;
                                    WhatsAppContentText = WhatsAppContentText + "\n" + (i + 1) + ") " + chkEntity.LstChkList[i].chktitle + "\n" + "Description : " + chkEntity.LstChkList[i].Description + "\n" + "Value : " + chkEntity.LstChkList[i].rdbval + "\n";

                                }

                            }
                           // Whats_Total = Whats_Total+i;
                            connection.Close();
                        }

                        if(Whats_NGs != 0 || Whats_Waits != 0 || Whats_Numaric !=0 || Whats_Text !=0)
                        {
                           // WhatsAppContentHeader = "\n" + "*Reference:*" + "\n-----------";

                            if(Whats_NGs != 0 || Whats_Waits != 0)
                            {
                                WhatsAppContent = "\n"+"*Toggle: (NG & WAIT Only)*" + "\n-------------------------------"+ WhatsAppContent;
                            }

                            if(Whats_Numaric != 0 && Whats_Text != 0)
                            {
                                WhatsAppContentNumaricText= "\n" + "*Numeric:*" + "\n-------------" + WhatsAppContentNumaric + "\n" + "*Text:*" + "\n---------" +WhatsAppContentText;
                            }

                           else if (Whats_Numaric != 0 && Whats_Text == 0)
                            {
                                WhatsAppContentNumaricText = "\n" + "*Numeric:*" + "\n-------------" + "\n" + WhatsAppContentNumaric;
                            }

                           else if (Whats_Numaric == 0 && Whats_Text != 0)
                            {
                                WhatsAppContentNumaricText = "\n" + "*Text:*" + "\n--------" + "\n" + WhatsAppContentText;
                            }
                        }



                        WhatsAppContent = "\n" + "\n~ ~ ~ ~ ~ ~ *CheckList Data* ~ ~ ~ ~ ~ ~" + "\n\n*Checked By* (Mobile #) : " + chkEntity.LstChkList[0].phoneno + "\n\n " + chkEntity.LstChkList[0].date + "  | |  " + chkEntity.LstChkList[0].time + "\n\n" + "*Location :* " + chkEntity.LstChkList[0].Location + "\n\n" + "*Period :* " + chkEntity.LstChkList[0].Period + "\n\n"+"*Total Check Point :* "+ (Whats_OKs+Whats_NGs+Whats_Waits+Whats_Numaric+Whats_Text) + "\n--------------------------" +"\nNumeric   : "+ Whats_Numaric + "\nText      : "+Whats_Text+"\nToggle    : " +(Whats_OKs+Whats_NGs+Whats_Waits) + "\n [ *OK* -> " + Whats_OKs + "] "+" [ *NG* -> " + Whats_NGs + "] "+" [ *Wait* -> " + Whats_Waits + "] "+"\n"+ WhatsAppContentNumaricText + WhatsAppContent;

                        CheckListEntity chklEntity = new CheckListEntity();

                        chklEntity = Finalchaecklistdatainsert(chkEntity);
                        chklEntity = GetWhatsAppGroupCode(chklEntity);
                        chkEntity.Message = chklEntity.Message;
                        chklEntity.WhatsAppContent = WhatsAppContent;
                      //chklEntity.WhatsAppContent = "[ALERT]\nTEST NOTIFICATION FROM EMPULSE DEVELOPMENT TEAM !";
                        chkEntity.phoneno = "91" + chkEntity.LstChkList[0].phoneno;
                      //  string whatsappresponce = null;

                        if (chklEntity.grp_Cd != null)
                        {
                            chkEntity = PostJsonToGivenUrl(chklEntity);
                        }
                        else
                        {
                            chkEntity.Message = "Contact Admin. ERR0001";
                        }
                    }

                    return chkEntity;

                }
                catch (Exception ex)
                {
                    CheckListEntity clslog = new CheckListEntity();
                    clslog.Message = ex.Message;
                    return clslog;
                }
            }

        }
        //This code used to get the WhatApp Group Code of the checklist
        public CheckListEntity GetWhatsAppGroupCode(CheckListEntity chklEntity)
        {
            try
            {
                DataTable dt = new DataTable();
                using (var connection = new OracleConnection(_connectionString))
                {
                    var command = new OracleCommand("USP_WA_GRPCD_DETAILS", connection) { CommandType = CommandType.StoredProcedure };
                    connection.Open();
                    command.Parameters.Add("PUBHCD", OracleDbType.Varchar2, chklEntity.LstChkList[0].PUBCD, ParameterDirection.Input);
                    command.Parameters.Add("GRPCD_DETAIL_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    OracleDataAdapter Da = new OracleDataAdapter(command);
                    Da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                       chklEntity.grp_Cd = row["wagrid"].ToString();
                    }
                    return chklEntity;
                }
            }
            catch (Exception ex)
            {
                CheckListEntity clslog = new CheckListEntity();
                clslog.Message = ex.Message;
                return clslog;
            }
        }

        // This code used to Update all the checklist data
        private CheckListEntity Finalchaecklistdatainsert(CheckListEntity chkEntity)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    var cmd = new OracleCommand("APG_TM25500.SET_CHK_DATA_HIST", connection) { CommandType = CommandType.StoredProcedure };
                    connection.Open();
                    cmd.Parameters.Add("IN_CORCD", OracleDbType.Varchar2, chkEntity.LstChkList[0].CORCD, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_BIZCD", OracleDbType.Varchar2, chkEntity.LstChkList[0].BUZCD, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_PUBCD", OracleDbType.Varchar2, chkEntity.LstChkList[0].PUBCD, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_DOCCD", OracleDbType.Varchar2, chkEntity.LstChkList[0].docCD, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_CHKCD", OracleDbType.Varchar2, chkEntity.LstChkList[0].chkCD, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_VAL", OracleDbType.Varchar2, chkEntity.LstChkList[0].rdbval, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_COMM", OracleDbType.Varchar2, chkEntity.LstChkList[0].Description, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_WHO", OracleDbType.Varchar2, chkEntity.LstChkList[0].phoneno, ParameterDirection.Input);
                    cmd.Parameters.Add("IN_LOCCD", OracleDbType.Varchar2, chkEntity.LstChkList[0].LocationId, ParameterDirection.Input);

                    var Response = cmd.ExecuteNonQuery();

                    if (Response != 0)
                    {
                        chkEntity.Message = "Saved Successful";
                    }
                }
                return chkEntity;
            }
            catch (Exception ex)
            {
                CheckListEntity clslog = new CheckListEntity();
                clslog.Message = ex.Message;
                return clslog;
            }
        }


        //This code used to send Checklist infromation  to the WhatsAPP Group 
        public CheckListEntity PostJsonToGivenUrl(CheckListEntity jsonObject)
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
                    string json = File.ReadAllText("WAGrp.json");
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    JToken jToken = jsonObj.SelectToken("recipientIds[0]");
                    jToken.Replace(jsonObject.grp_Cd);

                    //JToken jToken = jsonObj.SelectToken("receiverMobileNo");
                    //jToken.Replace(jsonObject.phoneno);
                    //jToken.Replace("916281171453,916382210064,918147073944");

                    JToken jToken1 = jsonObj.SelectToken("message[0]");
                    jToken1.Replace(jsonObject.WhatsAppContent);
       
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText("WAGrp.json", output);
                    writer.Write(output);
                }

                httpRequest.Headers.Add("x-api-key", WaAPIKey);
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    // resultOfPost = streamReader.ReadToEnd();
                    jsonObject.whatsappresponce = streamReader.ReadToEnd();
                }

                return jsonObject;
            }
            catch (Exception ex)
            {
                jsonObject.Message = ex.Message;
                return jsonObject;
            }
        }            
    }
}
