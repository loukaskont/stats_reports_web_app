using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace statistika_net4.statistika
{
    public partial class teampadAnatheseisPerUser : System.Web.UI.Page
    {
        String User_Id = "", Password = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Session["uname"] == null && this.Session["pass"] == null)
            {
                Response.Redirect("~/statistika/login.aspx");
            }
            else
            {
                User_Id = this.Session["uname"].ToString();
                Password = this.Session["pass"].ToString();
            }
            String connstring = "User Id=" + User_Id + ";Password=" + Password + ";Server=192.168.12.25;Port=5432;Database=geookoutjjils;Pooling=false;Preload Reader=true;CommandTimeout=10000";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            String sqlSelectTeampadStats = "select full_name, a.anatheseis from teampad_users_anatheseis a left join teampad_users b on a.teampad_id = b.teampad_id where full_name <> '';";
            DataTable dtTeampadStats = selectInDatabase(sqlSelectTeampadStats, conn);
            dtTeampadStats.Columns["full_name"].ColumnName = "Ονοματεπώνυμο";
            dtTeampadStats.Columns["anatheseis"].ColumnName = "Αναθέσεις";
            GridView1.DataSource = dtTeampadStats;
            GridView1.DataBind();
            conn.Close();
        }

        private DataTable selectInDatabase(String sql, NpgsqlConnection connection)
        {
            DataTable returnDatatable = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, connection);
            DataSet ds = new DataSet();
            da.Fill(ds);
            returnDatatable = ds.Tables[0];
            this.Session["anatethimenaAnaUser"] = returnDatatable;
            return returnDatatable;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            csv csvFile = new csv();
            String fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".csv";
            String thisDayDir = Server.MapPath("~") + "\\csvFiles\\" + DateTime.Now.ToString("yyyy_MM_dd");
            String excellFilePath = thisDayDir + "\\" + fileName;
            DataTable anatethimenaAnaUser = (DataTable)this.Session["anatethimenaAnaUser"];
            List<DataTable> tables = new List<DataTable>() { anatethimenaAnaUser };
            csvFile.writeToCsvFile(excellFilePath, tables);
            Response.Write("<script>");
            Response.Write("window.open('downloadFile.aspx?fileName=" + fileName + "&excellFilePath=" + excellFilePath.Replace('\\', '*') + "' ,'_blank')");
            Response.Write("</script>");
            /*System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ";");
            response.TransmitFile(excellFilePath);
            response.Flush();
            response.End();*/
        }



    }
}
