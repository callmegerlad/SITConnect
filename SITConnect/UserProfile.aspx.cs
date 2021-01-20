using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class UserProfile : System.Web.UI.Page
    {
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
            if (!Page.IsPostBack)
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
                        this.Form.DefaultButton = btnSubmit.UniqueID;
                        retrieveUserInfo(Session["UserID"].ToString());
                        // user nameplate
                        userAvatar.Text = firstName.Substring(0,1);
                        userName.Text = firstName + " " + lastName;
                        userEmail.Text = emailAddress;

                        // account information
                        tb_FirstName_Edit.Text = firstName;
                        tb_LastName_Edit.Text = lastName;
                        tb_EmailAddress_Edit.Text = emailAddress;
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
                else // invalid session, redirect back to login
                {
                    Response.Redirect("~/Login", false);
                }
            }
        }

        protected void logout(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("~/Login", false);

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
                            firstName = reader["FirstName"].ToString();
                        }

                        if (reader["LastName"] != DBNull.Value)
                        {
                            lastName = reader["LastName"].ToString();
                        }

                        if (reader["Email"] != DBNull.Value)
                        {
                            emailAddress = reader["Email"].ToString();
                        }

                        if (reader["PhoneNumber"] != DBNull.Value)
                        {
                            phoneNumber = reader["PhoneNumber"].ToString().Trim();
                        }

                        if (reader["DOB"] != DBNull.Value)
                        {
                            dob = reader["DOB"].ToString().Trim();
                        }

                        if (reader["CreditCardInfo"] != DBNull.Value)
                        {
                            string str = reader["CreditCardInfo"].ToString();
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
                throw new Exception(ex.ToString());
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
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET FirstName=@FirstName, LastName=@LastName, Email=@Email, PhoneNumber=@PhoneNumber, DOB=@DOB WHERE Email=@UserID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@UserID", Session["UserID"].ToString());
                            cmd.Parameters.AddWithValue("@FirstName", tb_FirstName_Edit.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", tb_LastName_Edit.Text.Trim());
                            cmd.Parameters.AddWithValue("@Email", tb_EmailAddress_Edit.Text.Trim());
                            cmd.Parameters.AddWithValue("@PhoneNumber", tb_PhoneNumber_Edit.Text.Trim());
                            cmd.Parameters.AddWithValue("@DOB", tb_DOB_Edit.Text.Trim());
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            // change session userid
                            Session["UserID"] = tb_EmailAddress_Edit.Text.Trim();
                            Session["updateMsg"] = "Your account information has been successfully saved!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Save Credit Card Info
        protected void saveCardBtn_Click(object sender, EventArgs e)
        {
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
                    throw new Exception(ex.ToString());
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
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        // Decrypt creditCardInfo
        protected string decryptData(byte[] cipherText)
        {
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
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
        }
    }
}