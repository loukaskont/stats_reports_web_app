using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace statistika_net4.statistika
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            String uname = TextBoxUname.Text;
            String pass = TextBoxPassword.Text;
            String currentDataBase = "geoutils";
            String connstring = "User Id=" + uname + ";Password=" + pass + ";Server=192.168.22.21;Port=5432;Database=" + currentDataBase + ";Pooling=false;Preload Reader=true;CommandTimeout=10000";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            try
            {
                conn.Open();
                this.Session["uname"] = uname;
                this.Session["pass"] = pass;
                Response.Redirect("~/About.aspx");
            }
            catch
            {
            }
            finally
            {
                conn.Close();
                LabelLogMessage.Text = "authentication failed for user " + uname;
            }
        }
    }
}