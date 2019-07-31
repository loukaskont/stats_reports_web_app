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
    public partial class monthly_user_stats : System.Web.UI.Page
    {
        String currentDataBase = "";
        String User_Id = "", Password = "";
        String connstring = "";
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
            if (CalendarStart.SelectedDate.ToString("yyyy-MM-dd") == "0001-01-01")
            {
                CalendarStart.SelectedDate = DateTime.Now.AddDays(-20);
                CalendarStart.VisibleDate = CalendarStart.SelectedDate;
                CalendarEnd.SelectedDate = DateTime.Now.AddDays(1);
                CalendarEnd.VisibleDate = CalendarEnd.SelectedDate;
                selectToStatTable();
            }
        }



        private void selectToStatTable()
        {
            String currentDataBase = DropDownListDatabase.SelectedValue;
            String connstring = "User Id=" + User_Id + ";Password=" + Password + ";Server=192.168.23.28;Port=5432;Database=geouyuuyiitils;Pooling=false;Preload Reader=true;CommandTimeout=10000";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            String startDate = CalendarStart.SelectedDate.ToString("yyyy-MM-dd");
            String endDate = CalendarEnd.SelectedDate.ToString("yyyy-MM-dd");
            LabelApoDate.Text = CalendarStart.SelectedDate.ToString("dd-MM-yyyy");
            LabelEosDate.Text = CalendarEnd.SelectedDate.ToString("dd-MM-yyyy");
            String selectStats_geoktima_general = @"select to_char(stat_date, 'dd-mm-yyyy') as statDate,* from geoutils.public.stats_geot_per_user where stat_date between '" + startDate + "' and '" + endDate + "' and \"dataBaseName\" = '" + currentDataBase + "';";
            DataTable generalStats = selectInDatabase(selectStats_geoktima_general, conn);
            DataTable monthlygeneralStats = createStatsCompareTable(generalStats);
            GridView1.DataSource = monthlygeneralStats;
            GridView1.DataBind();
            conn.Close();
            this.Session["monthlygeneralStats"] = monthlygeneralStats;
        }

        private DataTable createStatsCompareTable(DataTable generalStats)
        {
            DataTable usersDT = new DataTable();
            usersDT.Columns.Add("Ημερομηνία");
            DateTime startDate = CalendarStart.SelectedDate;
            DateTime endDate = CalendarEnd.SelectedDate;
            DataView view = new DataView(generalStats);
            DataTable distinctUserNames = view.ToTable(true, "uname");
            for (int i = 0; i < distinctUserNames.Rows.Count; i++)
            {
                usersDT.Columns.Add(distinctUserNames.Rows[i]["uname"].ToString());
            }
            while (startDate.ToString("yyyy-MM-dd") != endDate.ToString("yyyy-MM-dd"))
            {
                DataRow dayRow = usersDT.NewRow();
                dayRow[0] = startDate.ToString("dd-MM-yyyy");
                for (int i = 1; i < usersDT.Columns.Count; i++)
                {
                    DataRow[] userStats = generalStats.Select("uname = '" + usersDT.Columns[i].ColumnName + "' and stat_date = '" + startDate.ToString("yyyy-MM-dd") + "'");
                    if (userStats.Length > 0)
                    {
                        String hor_metavoli = userStats[0]["hor_metavoli_geot"].ToString();
                        String elegmena = userStats[0]["elegmena_geot"].ToString();
                        String rights = userStats[0]["rights"].ToString();
                        if (hor_metavoli == "") { hor_metavoli = "0"; }
                        if (elegmena == "") { elegmena = "0"; }
                        if (rights == "") { rights = "0"; }
                        String kaek_rights = " ΚΑΕΚ = " + hor_metavoli + "    Ελεγμένα = " + elegmena + "    Δικαιώματα = " + rights;
                        dayRow[i] = kaek_rights;
                    }
                    else 
                    {
                        dayRow[i] = "-";
                    }
                }
                usersDT.Rows.Add(dayRow);
                startDate = startDate.AddDays(1);
            }
            return usersDT;
        }

        private DataTable selectInDatabase(String sql, NpgsqlConnection connection)
        {
            DataTable returnDatatable = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, connection);
            DataSet ds = new DataSet();
            da.Fill(ds);
            returnDatatable = ds.Tables[0];
            return returnDatatable;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            csv excell = new csv();
            String fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".csv";
            String thisDayDir = Server.MapPath("~") + "\\csvFiles\\" + DateTime.Now.ToString("yyyy_MM_dd");
            String excellFilePath = thisDayDir + "\\" + fileName;
            DataTable generalStats = (DataTable)this.Session["monthlygeneralStats"];
            excell.writeToCsvFile(excellFilePath, generalStats);
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            selectToStatTable();
        }





    }
}
