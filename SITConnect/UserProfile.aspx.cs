using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class UserProfile : System.Web.UI.Page
    {
        // Variables
        int minimumPasswordAge = 5; // in minutes

        // DB
        string MYDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        // account information
        string firstName;
        string lastName;
        string emailAddress;
        string phoneNumber;
        string dob;
        byte[] creditCardInfo;
        string cardNumber;
        string cardExpDate;
        string cardVerification;

        protected void Page_Load(object sender, EventArgs e)
        {
            // check that userid session, authtoken session and cookie exist
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                // all 3 are not null, now check if session authtoken and cookie authtoken are the same, prevent session fixation
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("~/Login", false);
                }
                else
                {
                    if (!Page.IsPostBack)
                    {
                        this.Form.DefaultButton = HiddenButton.UniqueID;
                        retrieveUserInfo(Session["UserID"].ToString());
                        // user nameplate
                        userAvatar.Text = firstName.Substring(0, 1);
                        userName.Text = firstName + " " + lastName;
                        userEmail.Text = emailAddress;

                        // account information
                        tb_FirstName_Edit.Text = firstName;
                        tb_LastName_Edit.Text = lastName;
                        //tb_EmailAddress_Edit.Text = emailAddress;
                        tb_PhoneNumber_Edit.Text = phoneNumber;
                        tb_DOB_Edit.Text = dob;

                        // billing information
                        tb_CardNumber.Text = cardNumber;
                        tb_CardExpDate.Text = cardExpDate;
                        tb_CardVerification.Text = cardVerification;

                        // set update success msg
                        if (Session["updateMsg"] != null)
                        {
                            Page.Items["updateMsg"] = Session["updateMsg"].ToString();
                            Session["updateMsg"] = null;
                        }
                        if (Session["errorMsg"] != null)
                        {
                            Page.Items["errorMsg"] = Session["errorMsg"].ToString();
                            Session["errorMsg"] = null;
                        }

                        // switch back to original tab before postback
                        if (Session["tabNo"] != null)
                        {
                            Page.Items["tabNo"] = Session["tabNo"].ToString();
                            Session["tabNo"] = null;
                        }
                    }
                }
            }
            else // invalid session, redirect back to login
            {
                Response.Redirect("~/Login", false);
            }
        }

        protected void checkSession()
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Login", true);
            }
        }

        protected void logout(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = String.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = String.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }

            Response.Redirect("~/Login", false);
        }

        protected void retrieveUserInfo(string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT * FROM Account WHERE Email=@userId";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // for encryption n decryption
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }

                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }

                        // information
                        if (reader["FirstName"] != DBNull.Value)
                        {
                            firstName = HttpUtility.HtmlEncode(reader["FirstName"].ToString());
                        }

                        if (reader["LastName"] != DBNull.Value)
                        {
                            lastName = HttpUtility.HtmlEncode(reader["LastName"].ToString());
                        }

                        if (reader["Email"] != DBNull.Value)
                        {
                            emailAddress = HttpUtility.HtmlEncode(reader["Email"].ToString());
                        }

                        if (reader["PhoneNumber"] != DBNull.Value)
                        {
                            phoneNumber = HttpUtility.HtmlEncode(reader["PhoneNumber"].ToString().Trim());
                        }

                        if (reader["DOB"] != DBNull.Value)
                        {
                            dob = HttpUtility.HtmlEncode(reader["DOB"].ToString().Trim());
                        }

                        if (reader["CreditCardInfo"] != DBNull.Value)
                        {
                            string str = HttpUtility.HtmlEncode(reader["CreditCardInfo"].ToString());
                            if (str.Length > 0)
                            {
                                creditCardInfo = Convert.FromBase64String(str);
                                string decrypted = decryptData(creditCardInfo);
                                string[] splitDecrypted = decrypted.Split(',');
                                cardNumber = splitDecrypted[0];
                                cardExpDate = splitDecrypted[1];
                                cardVerification = splitDecrypted[2];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                //Response.Redirect("~/CustomError/Error500", true);
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
            }
            finally
            {
                connection.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Session["tabNo"] = "1";
            update();
            Response.Redirect("~/UserProfile", false);
        }

        protected void update()
        {
            checkSession();
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET FirstName=@FirstName, LastName=@LastName, PhoneNumber=@PhoneNumber, DOB=@DOB WHERE Email=@UserID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@UserID", Session["UserID"].ToString());
                            cmd.Parameters.AddWithValue("@FirstName", HttpUtility.HtmlEncode(tb_FirstName_Edit.Text.Trim()));
                            cmd.Parameters.AddWithValue("@LastName", HttpUtility.HtmlEncode(tb_LastName_Edit.Text.Trim()));
                            cmd.Parameters.AddWithValue("@PhoneNumber", HttpUtility.HtmlEncode(tb_PhoneNumber_Edit.Text.Trim()));
                            cmd.Parameters.AddWithValue("@DOB", HttpUtility.HtmlEncode(tb_DOB_Edit.Text.Trim()));
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            Session["updateMsg"] = "Your account information has been successfully saved!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                //Response.Redirect("~/CustomError/Error500", true);
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
            }
        }

        // Save Credit Card Info
        protected void saveCardBtn_Click(object sender, EventArgs e)
        {
            checkSession();
            Session["tabNo"] = "3";
            string cardNumber = tb_CardNumber.Text.Trim();
            string cardExpDate = tb_CardExpDate.Text.Trim();
            string cardVerification = tb_CardVerification.Text.Trim();

            if (!String.IsNullOrEmpty(cardNumber) && !String.IsNullOrEmpty(cardExpDate) && !String.IsNullOrEmpty(cardVerification))
            {
                string cardInfo = cardNumber.Replace("-", "") + "," + cardExpDate.Replace("/", "") + "," + cardVerification;

                try
                {
                    using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE Account SET CreditCardInfo=@CreditCardInfo WHERE Email=@UserID"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@UserID", Session["UserID"].ToString());
                                cmd.Parameters.AddWithValue("@CreditCardInfo", Convert.ToBase64String(encryptData(cardInfo)));
                                cmd.Connection = con;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                                Session["updateMsg"] = "Your billing information has been successfully saved!";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //throw new Exception(ex.ToString());
                    //Response.Redirect("~/CustomError/Error500", true);
                    Response.StatusCode = 500;
                    Response.Flush();
                    Response.End();
                }
            }
            else
            {
                // if any one of the textboxes are empty, send error msg
                Session["errorMsg"] = "Something went wrong while saving your billing information. Please try again...";
            }
            Response.Redirect("~/UserProfile", false);
        }

        // Encrypt creditCardInfo
        protected byte[] encryptData(string data)
        {
            checkSession();
            retrieveUserInfo(Session["UserID"].ToString());
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                //Response.Redirect("~/CustomError/Error500", true);
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
            }
            finally { }
            return cipherText;
        }

        // Decrypt creditCardInfo
        protected string decryptData(byte[] cipherText)
        {
            checkSession();
            string plainText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;

                // Decryptor to perform the stream transform
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                // Streams used for decryption
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read decrypted bytes from the decrypting stream and place them in a string
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                //Response.Redirect("~/CustomError/Error500", true);
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
            }
            finally { }
            return plainText;
        }


        // CHANGE PASSWORD
        protected void changePassword_Click(object sender, EventArgs e)
        {
            checkSession();
            Session["tabNo"] = "2";
            string userid = Session["UserID"].ToString();
            string pwd = HttpUtility.HtmlEncode(tb_CurrentPassword.Text.ToString().Trim());
            string new_pwd = HttpUtility.HtmlEncode(tb_NewPassword.Text.ToString().Trim());
            string confirm_new_pwd = HttpUtility.HtmlEncode(tb_NewPasswordConfirm.Text.ToString().Trim());

            bool allowPasswordChange = true;

            // Check if password was changed recently, before the minimum password age defined
            string lastPasswordChange = getLastPasswordChange();
            // if password was changed before
            if (lastPasswordChange != "")
            {
                TimeSpan ts = DateTime.Now - Convert.ToDateTime(lastPasswordChange);
                if (ts.TotalMinutes < minimumPasswordAge)
                {
                    System.Diagnostics.Debug.WriteLine("Password change before minimum password age");
                    Session["errorMsg"] = "Please wait a while before changing your password again! Please try again later...";
                    allowPasswordChange = false;
                }
            }

            if (allowPasswordChange)
            {
                // Check if password fields are empty
                if (pwd == new_pwd)
                {
                    Session["errorMsg"] = "Your new password cannot be same as current password. Please try again...";
                }
                else
                {
                    // Check if new passwords match
                    if (new_pwd == confirm_new_pwd)
                    {
                        // Check if new password meets password requirements
                        if (validatePassword(new_pwd))
                        {
                            string dbHash = getDBHash(userid);
                            string dbSalt = getDBSalt(userid);
                            try
                            {
                                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                                {
                                    SHA512Managed hashing = new SHA512Managed();
                                    string pwdWithSalt = pwd + dbSalt;
                                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                    string currentPassHash = Convert.ToBase64String(hashWithSalt);

                                    // SUCCESSFUL LOGIN
                                    if (currentPassHash.Equals(dbHash))
                                    {
                                        // Create new password hash with salt
                                        string new_pwdWithSalt = new_pwd + dbSalt;
                                        byte[] new_hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(new_pwdWithSalt));
                                        string newPassHash = Convert.ToBase64String(new_hashWithSalt);
                                        // Check if new password hash matches 2 prior passwords (PasswordHistory)
                                        // New password matches prior passwords, deny change of password and return error msg
                                        if (newPassHash == getPasswordHistory("1") || newPassHash == getPasswordHistory("2"))
                                        {
                                            Session["errorMsg"] = "Your new password must not be the same as any of your recent passwords. Please try again...";
                                        }
                                        // New password doesnt match prior passwords, safe to change password
                                        else
                                        {
                                            try
                                            {
                                                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                                                {
                                                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET PasswordHash=@PasswordHash WHERE Email=@UserID"))
                                                    {
                                                        using (SqlDataAdapter sda = new SqlDataAdapter())
                                                        {
                                                            cmd.CommandType = CommandType.Text;
                                                            cmd.Parameters.AddWithValue("@UserID", Session["UserID"].ToString());
                                                            cmd.Parameters.AddWithValue("@PasswordHash", newPassHash);
                                                            cmd.Connection = con;
                                                            con.Open();
                                                            cmd.ExecuteNonQuery();
                                                            con.Close();
                                                            Session["updateMsg"] = "Your password has been successfully changed!";
                                                            // save current passwordhash to PasswordHistory
                                                            savePasswordHistory(currentPassHash);
                                                            saveLastPasswordChange();
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                //throw new Exception(ex.ToString());
                                                //Response.Redirect("~/CustomError/Error500", true);
                                                Response.StatusCode = 500;
                                                Response.Flush();
                                                Response.End();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Session["errorMsg"] = "Something went wrong while changing your password. Please try again...";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //throw new Exception(ex.ToString());
                                //Response.Redirect("~/CustomError/Error500", true);
                                Response.StatusCode = 500;
                                Response.Flush();
                                Response.End();
                            }
                        }
                        else
                        {
                            Session["errorMsg"] = "Your new password does not meet the password requirements. Please try again...";
                        }
                    }
                    else
                    {
                        Session["errorMsg"] = "Your new passwords do not match. Please try again...";
                    }
                }
            }
            Response.Redirect("~/UserProfile", false);
        }



        // Validate Password
        public bool validatePassword(string password)
        {
            int score = 0;
            // Password Length
            if (password.Length < 8)
            {
                return false;
            }
            else
            {
                score = 1;
            }
            // Password contain numeral
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            // Password contain lowercase
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            // Password contain uppercase
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            // Password contain special character
            if (Regex.IsMatch(password, "^[0-9]|[a-z]|[A-Z]$"))
            {
                score++;
            }

            if (score == 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        // Get PasswordHistory
        public string getPasswordHistory(string no)
        {
            checkSession();
            string pwdHistory = "";
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT * FROM Account WHERE Email=@userId";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", Session["UserID"].ToString());
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHistory"+no] != DBNull.Value)
                        {
                            pwdHistory = reader["PasswordHistory"+no].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                //Response.Redirect("~/CustomError/Error500", true);
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
            }
            finally
            {
                connection.Close();
            }
            return pwdHistory;
        }


        // Save Current PasswordHash into PasswordHistory
        public void savePasswordHistory(string currentPassword)
        {
            checkSession();
            string ph1 = getPasswordHistory("1");
            // If PasswordHistory1 is empty, meaning no prior password history, then save current password to it
            // If PasswordHistory1 is not empty, shift it to PasswordHistory2, then save current password to PasswordHistory1
            if (ph1 != "")
            {
                // Shift PasswordHistory1 to PasswordHistory2
                try
                {
                    using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE Account SET PasswordHistory2=@PasswordHistory2 WHERE Email=@UserID"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@UserID", Session["UserID"].ToString());
                                cmd.Parameters.AddWithValue("@PasswordHistory2", ph1);
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
                    //throw new Exception(ex.ToString());
                    //Response.Redirect("~/CustomError/Error500", true);
                    Response.StatusCode = 500;
                    Response.Flush();
                    Response.End();
                }
            }
            
            // Save current password to PasswordHistory1
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET PasswordHistory1=@PasswordHistory1 WHERE Email=@UserID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@UserID", Session["UserID"].ToString());
                            cmd.Parameters.AddWithValue("@PasswordHistory1", currentPassword);
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
                //throw new Exception(ex.ToString());
                //Response.Redirect("~/CustomError/Error500", true);
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
            }
        }


        // SAVE LAST PASSWORD CHANGE DATETIME
        public void saveLastPasswordChange()
        {
            checkSession();
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LastPasswordChange=@LastPasswordChange WHERE Email=@UserID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@UserID", Session["UserID"].ToString());
                            cmd.Parameters.AddWithValue("@LastPasswordChange", DateTime.Now.ToString());
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
                //throw new Exception(ex.ToString());
                //Response.Redirect("~/CustomError/Error500", true);
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
            }
        }

        // GET LAST PASSWORD CHANGE
        public string getLastPasswordChange()
        {
            checkSession();
            string lastPasswordChange = "";
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT * FROM Account WHERE Email=@userId";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", Session["UserID"].ToString());
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LastPasswordChange"] != DBNull.Value)
                        {
                            lastPasswordChange = reader["LastPasswordChange"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.ToString());
                //Response.Redirect("~/CustomError/Error500", true);
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
            }
            finally
            {
                connection.Close();
            }
            return lastPasswordChange;
        }





        // DB Hash & Salt
        public string getDBHash(string userid)
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
                //throw new Exception(ex.ToString());
                //Response.Redirect("~/CustomError/Error500", true);
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
            }
            finally { connection.Close(); }
            return hash;
        }

        public string getDBSalt(string userid)
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
                //throw new Exception(ex.ToString());
                //Response.Redirect("~/CustomError/Error500", true);
                Response.StatusCode = 500;
                Response.Flush();
                Response.End();
            }
            finally { connection.Close(); }
            return salt;
        }
    }
}