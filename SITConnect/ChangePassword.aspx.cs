using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        // Variables
        // DB
        string MYDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["passwordError"] == null)
            {
                if (!Page.IsPostBack)
                {
                    Response.StatusCode = 403;
                    Response.Flush();
                    Response.End();
                }
            }
            else
            {
                if (Session["passwordError"] != null)
                {
                    Page.Items["passwordError"] = Session["passwordError"].ToString();
                    Session["passwordError"] = null;
                }
            }
        }

        protected void btn_submit_click(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                SITConnect.UserProfile up = new SITConnect.UserProfile();

                string userid = Session["UserID"].ToString();
                string pwd = HttpUtility.HtmlEncode(tb_PreviousPassword.Text.ToString().Trim());
                string new_pwd = HttpUtility.HtmlEncode(tb_NewPassword.Text.ToString().Trim());
                string confirm_new_pwd = HttpUtility.HtmlEncode(tb_NewPasswordConfirm.Text.ToString().Trim());

                if (pwd == new_pwd)
                {
                    Session["passwordError"] = "Your new password cannot be same as current password. Please try again...";
                }
                else
                {
                    // Check if new passwords match
                    if (new_pwd == confirm_new_pwd)
                    {
                        // Check if new password meets password requirements
                        if (up.validatePassword(new_pwd))
                        {
                            string dbHash = up.getDBHash(userid);
                            string dbSalt = up.getDBSalt(userid);
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
                                        if (newPassHash == up.getPasswordHistory("1") || newPassHash == up.getPasswordHistory("2"))
                                        {
                                            Session["passwordError"] = "Your new password must not be the same as any of your recent passwords. Please try again...";
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
                                                            // save current passwordhash to PasswordHistory
                                                            up.savePasswordHistory(currentPassHash);
                                                            up.saveLastPasswordChange();
                                                            Logout();
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
                                        Session["passwordError"] = "Something went wrong while changing your password. Please try again...";
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
                            Session["passwordError"] = "Your new password does not meet the password requirements. Please try again...";
                        }
                    }
                    else
                    {
                        Session["passwordError"] = "Your new passwords do not match. Please try again...";
                    }
                }
            }
            if (Session["UserID"] != null)
            {
                Response.Redirect("~/ChangePassword", false);
            }
            else
            {
                Response.Redirect("~/Login", false);
            }
        }



        // LOGOUT
        protected void logout_btn_Click(object sender, EventArgs e)
        {
            Logout();
            Response.Redirect("~/Default", false);
        }

        protected void Logout()
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
    }
}