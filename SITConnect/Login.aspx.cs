using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        // Timeout in minutes when account is locked out
        int accountLockoutTimeout = 1;


        public class MyObject {
            public string success { get; set; }
            public List<string> errorMessage { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["successMsg"] != null)
            {
                Page.Items["successMsg"] = Session["successMsg"].ToString();
                Session["successMsg"] = null;
            }
        }

        protected void btn_register(object sender, EventArgs e)
        {
            Response.Redirect("~/Registration");
        }

        protected void btn_submit_click(object sender, EventArgs e)
        {
            if (validateCaptcha())
            {
                // using EmailAddress for userid, and session
                string userid = tb_EmailAddress.Text.ToString().Trim();
                string pwd = tb_Password.Text.ToString().Trim();

                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(userid);
                string dbSalt = getDBSalt(userid);

                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        if (checkLockedOut())
                        {
                            Page.Items["errorMsg"] = "Too many failed login attempts! Your account has been temporarily locked. Please try again later.";
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Not locked out");
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);

                            // SUCCESSFUL LOGIN
                            if (userHash.Equals(dbHash))
                            {
                                // Reset failed attempts count
                                resetAttempt();
                                // Create session with userid for authorization and identification
                                Session["UserID"] = userid;
                                // Create new GUID and save into the session
                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;
                                // Create cookie with above GUID value
                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                Response.Redirect("~/UserProfile", false);
                            }
                            // LOGIN FAILED, ERROR MSG
                            else
                            {
                                failedAttempt();
                                if (checkLockedOut())
                                {
                                    Page.Items["errorMsg"] = "Too many failed login attempts! Your account has been temporarily locked. Please try again later.";
                                }
                                else
                                {
                                    Page.Items["errorMsg"] = "Invalid login credentials. Please try again.";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally
                {
                    if (Page.Items["errorMsg"] == null)
                    {
                        // if account doesnt exist, it wont return the above errorMsg, this is to ensure
                        // the secrecy of the existance of accounts
                        Page.Items["errorMsg"] = "Invalid login credentials. Please try again.";
                    }
                }
            }
            tb_Password.Text = "";
        }

        protected string getDBHash(string userid)
        {
            string hash = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                hash = reader["PasswordHash"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return hash;
        }

        protected string getDBSalt(string userid)
        {
            string salt = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                salt = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return salt;
        }

        // for Google reCAPTCHA v3
        public bool validateCaptcha()
        {
            bool results = true;

            // user submits recaptcha form and gets a response POST param
            // captchaResponse consists of user click pattern(Behaviour analytics)
            string captchaResponse = Request.Form["g-recaptcha-response"];

            // send a GET request to Google along with response and secret key
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=6LeAKSQaAAAAACuQ3ZzCeUZdqD9BjhEvLNCWuKIR &response="+captchaResponse);

            try
            {
                // receive Response in JSON format from Google server
                using (WebResponse webResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        // create jsonObj to handle response(e.g. success/error), deserialise JSON
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);
                        // convert string to boolean
                        results = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return results;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        // For ACCOUNT LOCKOUT functionality
        // Check whether account is currently locked out
        public bool checkLockedOut()
        {
            bool results = false;
            if (getAttempts() >= 3)
            {
                TimeSpan timeSpan = DateTime.Now - Convert.ToDateTime(getLastAttempt());
                System.Diagnostics.Debug.WriteLine("TimeSpan: "+timeSpan);
                if (timeSpan.Minutes < accountLockoutTimeout)
                {
                    System.Diagnostics.Debug.WriteLine("Account locked out");
                    results = true;
                }
            }
            return results;
        }

        // Called after a failed login attempt, adds 1 to attempt count and the current datetime
        protected void failedAttempt()
        {
            System.Diagnostics.Debug.WriteLine("Failed attempt + 1");
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET Attempts=@Attempts, LastAttempt=@LastAttempt WHERE Email=@UserID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@UserID", tb_EmailAddress.Text.ToString());
                            cmd.Parameters.AddWithValue("@Attempts", getAttempts() + 1);
                            cmd.Parameters.AddWithValue("@LastAttempt", DateTime.Now);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Resets failed attempt count and last attempt datetime, called after successful login
        protected void resetAttempt()
        {
            System.Diagnostics.Debug.WriteLine("Reset attempt count");
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET Attempts=@Attempts, LastAttempt=@LastAttempt WHERE Email=@UserID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@UserID", tb_EmailAddress.Text.ToString());
                            cmd.Parameters.AddWithValue("@Attempts", 0);
                            cmd.Parameters.AddWithValue("@LastAttempt", "");
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Get number of failed attempts, for checkLockedOut()
        protected int getAttempts()
        {
            int attempts = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Attempts FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", tb_EmailAddress.Text.ToString());

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Attempts"] != null)
                        {
                            if (reader["Attempts"] != DBNull.Value)
                            {
                                attempts = Convert.ToInt32(reader["Attempts"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return attempts;
        }

        // Gets datetime of last failed login attempt, for checkLockedOut()
        protected string getLastAttempt()
        {
            string lastAttempt = "";
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LastAttempt FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", tb_EmailAddress.Text.ToString());

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LastAttempt"] != null)
                        {
                            if (reader["LastAttempt"] != DBNull.Value)
                            {
                                lastAttempt = reader["LastAttempt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return lastAttempt;
        }
    }
}