using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;

namespace SITConnect
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                if (Session["errorMsg"] != null)
                {
                    Page.Items["errorMsg"] = Session["errorMsg"].ToString();
                    Session["errorMsg"] = null;
                }
            }
            else
            {
                Response.Redirect("~/UserProfile", false);
            }
        }

        protected void btn_login_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Login");
        }

        private bool validatePassword(string password)
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

        protected void btn_submit_click(object sender, EventArgs e)
        {
            if (validatePassword(tb_Password.Text)) 
            {
                string pwd = tb_Password.Text.ToString().Trim();

                // Generate random "salt"
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];

                // Fills array of bytes with a cryptographcally strong sequence of random values
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);

                SHA512Managed hashing = new SHA512Managed();

                string pwdWithSalt = pwd + salt;
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                finalHash = Convert.ToBase64String(hashWithSalt);

                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;

                createAccount();
                if (Session["errorMsg"] != null)
                {
                    Response.Redirect("~/Registration", false);
                }
                else
                {
                    Session["successMsg"] = "Successfully registered. We're happy to have you, please log in!";
                    Response.Redirect("~/Login", false);
                }
            }
            else
            {
                Session["errorMsg"] = "Password not strong enough!";
            }
        }

        protected void createAccount()
        {
            string cardNumber = HttpUtility.HtmlEncode(tb_CardNumber.Text.Trim());
            string cardExpDate = HttpUtility.HtmlEncode(tb_CardExpDate.Text.Trim());
            string cardVerification = HttpUtility.HtmlEncode(tb_CardVerification.Text.Trim());
            try
            {
                if (!String.IsNullOrEmpty(cardNumber) && !String.IsNullOrEmpty(cardExpDate) && !String.IsNullOrEmpty(cardVerification))
                {
                    string cardInfo = cardNumber.Replace("-", "") + "," + cardExpDate.Replace("/", "") + "," + cardVerification;
                    using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@FirstName,@LastName,@Email,@PhoneNumber,@DOB,@CreditCardInfo,@PasswordHash,@PasswordSalt,@DateTimeRegistered,@IV,@Key,@Attempts,@LastAttempt,@PasswordHistory1,@PasswordHistory2,@LastPasswordChange)"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@FirstName", HttpUtility.HtmlEncode(tb_FirstName.Text.Trim()));
                                cmd.Parameters.AddWithValue("@LastName", HttpUtility.HtmlEncode(tb_LastName.Text.Trim()));
                                cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(tb_EmailAddress.Text.Trim()));
                                cmd.Parameters.AddWithValue("@PhoneNumber", HttpUtility.HtmlEncode(tb_PhoneNumber.Text.Trim()));
                                cmd.Parameters.AddWithValue("@DOB", HttpUtility.HtmlEncode(tb_DOB.Text.Trim()));
                                cmd.Parameters.AddWithValue("@CreditCardInfo", Convert.ToBase64String(encryptData(cardInfo)));
                                cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                                cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                                cmd.Parameters.AddWithValue("@DateTimeRegistered", DateTime.Now);
                                cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                                cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                                cmd.Parameters.AddWithValue("@Attempts", DBNull.Value);
                                cmd.Parameters.AddWithValue("@LastAttempt", DBNull.Value);
                                cmd.Parameters.AddWithValue("@PasswordHistory1", DBNull.Value);
                                cmd.Parameters.AddWithValue("@PasswordHistory2", DBNull.Value);
                                cmd.Parameters.AddWithValue("@LastPasswordChange", DateTime.Now.ToString());
                                cmd.Connection = con;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                    }
                }
                else
                {
                    // if any one of the textboxes are empty, send error msg
                    Session["errorMsg"] = "Credit Card Information is in an invalid format. Please try again...";
                }
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("SQL EXCEPTION NUMBER: " + ex.Number);
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    // Violation of unique index / unique constraint of email
                    Session["errorMsg"] = "The email address is already in use. Please try another email.";
                }
                else
                {
                    //throw new Exception(ex.ToString());
                    Response.Redirect("~/CustomError/Error500", true);
                }
            }
        }


        // Encrypt creditCardInfo
        protected byte[] encryptData(string data)
        {
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
                Response.Redirect("~/CustomError/Error500", true);
            }
            finally { }
            return cipherText;
        }
    }
}