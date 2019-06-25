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
    public partial class monthly_general_stats : System.Web.UI.Page
    {
        DataTable tableForCsv;
        String User_Id = "", Password = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string script = "$(document).ready(function () { $('[id*=btnSubmit]').click(); });";
                ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);
            }
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            selectToStatTable();
        }

        
        private void selectToStatTable() 
        {
            String currentDataBase = DropDownListDatabase.SelectedValue;
            String connstring = "User Id=" + User_Id + ";Password=" + Password + ";Server=192.168.22.21;Port=5432;Database=geoutils;Pooling=false;Preload Reader=true;CommandTimeout=10000";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            String startDate = CalendarStart.SelectedDate.ToString("yyyy-MM-dd");
            String endDate = CalendarEnd.SelectedDate.ToString("yyyy-MM-dd");
            LabelApoDate.Text = CalendarStart.SelectedDate.ToString("dd-MM-yyyy");
            LabelEosDate.Text = CalendarEnd.SelectedDate.ToString("dd-MM-yyyy");
            String selectStats_geoktima_general = @"select to_char(stat_date, 'dd-mm-yyyy') as statDate,
                    elegmena_geotemaxia,mi_elegmena_geotemaxia,pososto_elegmenon_geot || '%' as pososto_elegmenon_geot,pososto_mi_elegmenon_geot || '%' as pososto_mi_elegmenon_geot,elegmena_dikaiomata,mi_elegmena_dikaiomata,pososto_elegmena_dikaiom || '%' as pososto_elegmena_dikaiom,
                    pososto_mi_elegmena_dikaiom || '%' as pososto_mi_elegmena_dikaiom,sinolika_geotemaxia,sinolika_dikaiomata,stat_date,gkt_name
                    from geoutils.public.stats_geoktima_general where stat_date between '" + startDate + "' and '" + endDate + "' and gkt_name = '" + currentDataBase + "';";
            DataTable generalStats = selectInDatabase(selectStats_geoktima_general, conn);
            generalStats.Columns.Remove("gkt_name");
            generalStats.Columns.Remove("stat_date");
            generalStats.Columns["statDate"].SetOrdinal(0);
            generalStats.Columns["statDate"].ColumnName = "Ημερομηνία";
            generalStats.Columns["elegmena_geotemaxia"].ColumnName = "Ελεγμένα Γεωτεμάχια";
            generalStats.Columns["mi_elegmena_geotemaxia"].ColumnName = "Μη ελεγμένα Γεωτεμάχια";
            generalStats.Columns["pososto_elegmenon_geot"].ColumnName = "Ποσοστό Ελεγμένων Γεωτεμαχίων";
            generalStats.Columns["pososto_mi_elegmenon_geot"].ColumnName = "Ποσοστό Μη Ελεγμένων Γεωτεμαχίων";
            generalStats.Columns["elegmena_dikaiomata"].ColumnName = "Ελεγμένα Δικαιώματα";
            generalStats.Columns["mi_elegmena_dikaiomata"].ColumnName = "Μη Ελεγμένα Δικαιώματα";
            generalStats.Columns["pososto_elegmena_dikaiom"].ColumnName = "Ποσοστό Ελεγμένων Δικαιωμάτων";
            generalStats.Columns["pososto_mi_elegmena_dikaiom"].ColumnName = "Ποσοστό Μη Ελεγμένων Δικαιωμάτων";
            generalStats.Columns["sinolika_geotemaxia"].ColumnName = "Συνολικά Γεωτεμάχια";
            generalStats.Columns["sinolika_dikaiomata"].ColumnName = "Συνολικά Δικαιώματα";
            DataView dv = generalStats.DefaultView;
            dv.Sort = "Ημερομηνία asc";
            DataTable generalStatsSorted = dv.ToTable();
            GridView1.DataSource = generalStatsSorted;
            GridView1.DataBind();
            conn.Close();
            this.Session["generalStatsSorted"] = generalStatsSorted;
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



        protected void Button2_Click1(object sender, EventArgs e)
        {
            csv excell = new csv();
            String projectDirPath = Server.MapPath("~");
            String fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".csv";
            String thisDayDir = Server.MapPath("~") + "\\csvFiles\\" + DateTime.Now.ToString("yyyy_MM_dd");
            String excellFilePath = thisDayDir + "\\" + fileName;
            DataTable generalStatsSorted = (DataTable)this.Session["generalStatsSorted"];
            excell.writeToCsvFile(excellFilePath, generalStatsSorted);
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