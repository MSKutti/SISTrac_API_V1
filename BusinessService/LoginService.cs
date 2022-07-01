using BusinessEntity;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Configuration;
using System.Collections.Generic;

namespace BusinessService
{
    public class LoginService : ILoginService
    {
        #region Declrations
        string WaAPIKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("WHGRPDETAIL")["WhaApiKey"];
        string WaMesURL = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("WHGRPDETAIL")["WhaMsgURL"];
        #endregion

        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly DynamicParameters dynamicParameters = new DynamicParameters();
        private readonly string _connectionString;
        private string RandomNumber;
      

        public LoginService(IWebHostEnvironment hostingEnvironment, IConfiguration _configuratio)
        {
            _hostingEnvironment = hostingEnvironment;

            _connectionString = _configuratio.GetConnectionString("DBConnection");
        
        }

        //This code used to get the OTP  
        public LoginEntity GetOtp(LoginEntity loginEntity)
        {
            try
            {
                LoginEntity Loginresponce = new LoginEntity();

                Random rnd = new Random();
                RandomNumber = (rnd.Next(1000, 9999)).ToString();

                Loginresponce.phoneno = "91" + loginEntity.phoneno;
                Loginresponce.OTP = RandomNumber;
                Loginresponce.message = "\n\n*OTP* for SISTrac Login is *"+RandomNumber+"*";

                var whatsappresponce = PostJsonToGivenUrl(WaMesURL, Loginresponce);

                loginEntity.message = whatsappresponce;
                loginEntity.OTP = RandomNumber;
                loginEntity.phoneno = Loginresponce.phoneno;

                return loginEntity;
            }
            catch (Exception ex)
            {
                LoginEntity clslog = new LoginEntity();
                clslog.message = ex.Message;
                return clslog;
            }
        }

        //This code used to Save Log details  
        public LoginEntity SaveOtp(LoginEntity loginEntity)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                try
                {
                    var command = new OracleCommand("USP_LOGDETAILS", connection) { CommandType = CommandType.StoredProcedure };
                    connection.Open();
                    command.Parameters.Add("Log_Phoneno", OracleDbType.Varchar2, loginEntity.phoneno, ParameterDirection.Input);
                    command.Parameters.Add("Log_Otp", OracleDbType.Varchar2, loginEntity.OTP, ParameterDirection.Input);
                    command.Parameters.Add("PUBHCD", OracleDbType.Varchar2, loginEntity.PUBCD, ParameterDirection.Input);
                    var Response = command.ExecuteNonQuery();

                    if (Response != 0)
                    {
                        loginEntity.message = "Saved Successful";
                    }                           
                    return loginEntity;
                }
                catch (Exception ex)
                {
                    LoginEntity clslog = new LoginEntity();
                    clslog.message = ex.Message;
                    return clslog;
                }
            }
         }
        //This code used to send OTP to the WhatsAPP 
        string PostJsonToGivenUrl(string url, LoginEntity jsonObject)
        {
            try
            {
                string resultOfPost = string.Empty;

                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.CreateHttp(url);
                httpRequest.Method = "POST";
                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";

                using (StreamWriter writer = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    string json = File.ReadAllText("optin.json"); //get the root path  and read all the content inside the file
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    JToken jToken = jsonObj.SelectToken("receiverMobileNo");
                    jToken.Replace(jsonObject.phoneno);

                    JToken jToken1 = jsonObj.SelectToken("message[0]");
                    jToken1.Replace(jsonObject.message);
                    
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText("optin.json", output);

                    writer.Write(output);
                }
                httpRequest.Headers.Add("x-api-key", WaAPIKey);

                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    resultOfPost = streamReader.ReadToEnd();
                }
                return resultOfPost;
            }
            catch(Exception ex)
            {
               return ex.Message.ToString();
            }
        }






        public LoginEntity GetLocaton(LoginEntity homeentity)
        {
            List<LoginEntity> login = new List<LoginEntity>();
            
            DataTable dt = new DataTable();
            using (var connection = new OracleConnection(_connectionString))
            {
                try
                {
                    var command = new OracleCommand("APG_TM25700.GET_PRT_HIST_DET_DLY", connection) { CommandType = CommandType.StoredProcedure };
                    connection.Open();
                    command.Parameters.Add("IN_CORCD", OracleDbType.Varchar2, homeentity.CORCD, ParameterDirection.Input);
                    command.Parameters.Add("IN_BIZCD", OracleDbType.Varchar2, homeentity.BUZCD, ParameterDirection.Input);
                    command.Parameters.Add("IN_PUBCD", OracleDbType.Varchar2, homeentity.PUBCD, ParameterDirection.Input);
                    command.Parameters.Add("IN_DATE", OracleDbType.Varchar2, homeentity.Date, ParameterDirection.Input);
                    command.Parameters.Add("OUT_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    

                    OracleDataAdapter Da = new OracleDataAdapter(command);
                    Da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        LoginEntity ChklEntity = new LoginEntity
                        {
                            chkdesc = row["CHKNM"].ToString(),
                            method = row["METHOD"].ToString(),
                            Locationid = row["LOCCD"].ToString(),
                            Location = row["LOCNM"].ToString(),
                            chkcount = row["CHK_SEQ"].ToString(),
                            D00 = row["VAL"].ToString(),
                            who = row["WHO"].ToString()

                        };
                        login.Add(ChklEntity);
                    }
                    homeentity.LstLogin = login;
                    return homeentity;
                }
                catch (Exception ex)
                {
                    LoginEntity clslog = new LoginEntity();
                    clslog.message = ex.Message;
                    return clslog;
                }

            }
        }

    }
}
