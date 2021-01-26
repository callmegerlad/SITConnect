using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class SiteMaster : MasterPage
    {
        // Variables
        public int maximumPasswordAge = 15; // in minutes

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                if (Page.Title != "Change Password")
                {
                    checkMaximumPasswordAge();
                }
            }
        }

        protected void checkMaximumPasswordAge()
        {
            SITConnect.UserProfile up = new SITConnect.UserProfile();
            TimeSpan ts = DateTime.Now - Convert.ToDateTime(up.getLastPasswordChange());
            if (ts.Minutes >= maximumPasswordAge)
            {
                System.Diagnostics.Debug.WriteLine("Password past maximum password age! Need to change password to continue.");
                Session["passwordError"] = "Your password has expired past the maximum password age! Please change your password to continue...";
                Response.Redirect("~/ChangePassword", false);
            }
            System.Diagnostics.Debug.WriteLine("Minutes since last password change: "+ts.Minutes.ToString());
        }
    }
}