using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace statistika_net4.statistika
{
    public partial class genikoSinolo : System.Web.UI.Page
    {
        String currentDataBase = "";
        String User_Id = "", Password = "";
        String connstring = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Session["IsPostBack"] = IsPostBack;
            if (this.Session["uname"] == null && this.Session["pass"] == null)
            {
                Response.Redirect("~/statistika/login.aspx");
            }
            else
            {
                User_Id = this.Session["uname"].ToString();
                Password = this.Session["pass"].ToString();
            }
            if (!IsPostBack)
            {
                string script = "$(document).ready(function () { $('[id*=btnSubmit]').click(); });";
                ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);
                Label2.Text = "Γεωτεμάχια:";
                viewGeotemaxiaStats();
                Label3.Text = "Δικαιώματα:";
                viewDikaiomataStats();
            }
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            Label2.Text = "Γεωτεμάχια:";
            viewGeotemaxiaStats();
            Label3.Text = "Δικαιώματα:";
            viewDikaiomataStats();
        }


        private void viewGeotemaxiaStats() 
        {
            currentDataBase = DropDownListDatabase.Text;
            DataTable statsDataTable = new DataTable("statistika");
            statsDataTable.Columns.Add("Ελεγμένα Γεωτεμάχια");
            statsDataTable.Columns.Add("Ποσοστό Ελεγμένων Γεωτεμαχίων");
            statsDataTable.Columns.Add("Μή Ελεγμένα Γεωτεμάχια");
            statsDataTable.Columns.Add("Ποσοστό Μη Ελεγμένων Γεωτεμαχίων");
            statsDataTable.Columns.Add("Συνολικά Γεωτεμάχια");
            connstring = "User Id=" + User_Id + ";Password=" + Password + ";Server=192.168.22.21;Port=5432;Database=" + currentDataBase + ";Pooling=false;Preload Reader=true;CommandTimeout=10000";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            string sqlKaekElegmena = @"select count(*) as elegmenaKaek from ktgis.pst where geometry_verified = '1' and kaek not like '%UN%';";
            DataTable dtKaekElegmena = selectInDatabase(sqlKaekElegmena, conn);
            string sqlKaekMiElegmena = @"select count(*) as miElegmenaKaek from ktgis.pst where (geometry_verified = '0' or geometry_verified is null) and kaek not like '%UN%';";
            DataTable dtKaekMiElegmena = selectInDatabase(sqlKaekMiElegmena, conn);
            conn.Close();
            DataRow drRights = statsDataTable.NewRow();
            int sinoloDikaiomaton = Convert.ToInt32(dtKaekElegmena.Rows[0]["elegmenaKaek"]) + Convert.ToInt32(dtKaekMiElegmena.Rows[0]["miElegmenaKaek"]);
            drRights["Ελεγμένα Γεωτεμάχια"] = dtKaekElegmena.Rows[0]["elegmenaKaek"];
            double posostoEl = (Convert.ToDouble(dtKaekElegmena.Rows[0]["elegmenaKaek"]) * 100) / sinoloDikaiomaton;
            drRights["Ποσοστό Ελεγμένων Γεωτεμαχίων"] = Convert.ToString(Math.Round(posostoEl, 1)) + "%";
            drRights["Μή Ελεγμένα Γεωτεμάχια"] = dtKaekMiElegmena.Rows[0]["miElegmenaKaek"];
            double posostoMi = (Convert.ToDouble(dtKaekMiElegmena.Rows[0]["miElegmenaKaek"]) * 100) / sinoloDikaiomaton;
            drRights["Ποσοστό Μη Ελεγμένων Γεωτεμαχίων"] = Convert.ToString(Math.Round(posostoMi, 1)) + "%";
            drRights["Συνολικά Γεωτεμάχια"] = sinoloDikaiomaton.ToString();
            statsDataTable.Rows.Add(drRights);
            GridViewGeotemaxia.DataSource = statsDataTable;
            GridViewGeotemaxia.DataBind();
            this.Session["genikoSinoloStatsRights"] = statsDataTable;
        }

        private void viewDikaiomataStats() 
        {
            currentDataBase = DropDownListDatabase.Text;
            DataTable statsDataTable = new DataTable("statistika");
            statsDataTable.Columns.Add("Ελεγμένα Δικαιώματα");
            statsDataTable.Columns.Add("Ποσοστό Ελεγμένων");
            statsDataTable.Columns.Add("Μή Ελεγμένα Δικαιώματα");
            statsDataTable.Columns.Add("Ποσοστό Μη Ελεγμένων");
            statsDataTable.Columns.Add("Συνολικά Δικαιώματα");
            connstring = "User Id=" + User_Id + ";Password=" + Password + ";Server=192.168.22.21;Port=5432;Database=" + currentDataBase + ";Pooling=false;Preload Reader=true;CommandTimeout=10000";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            string sqlRightsElegmena = @"select count(distinct(r.g_right_id)) as elegmenaRights from ktinc.prop p left join ktinc.right r on p.g_prop_id = r.g_prop_id left join ktinc.ben_right b on r.g_right_id = b.g_right_id 
                            where p.validation_flag_in_prop_tab = '1' and b.right_status <> '04' and r.g_right_id not like 'UN%' and b.ben_role = '1';";
            DataTable dtRightsElegmena = selectInDatabase(sqlRightsElegmena, conn);
            string sqlRightsMiElegmena = @"select count(distinct(r.g_right_id)) as miElegmenaRights from ktinc.prop p left join ktinc.right r on p.g_prop_id = r.g_prop_id left join ktinc.ben_right b on r.g_right_id = b.g_right_id 
                            where (p.validation_flag_in_prop_tab = '0' or p.validation_flag_in_prop_tab is null) and b.right_status <> '04' and r.g_right_id not like 'UN%' and b.ben_role = '1';";
            DataTable dtRightsMiElegmena = selectInDatabase(sqlRightsMiElegmena, conn);
            conn.Close();
            DataRow drRights = statsDataTable.NewRow();
            int sinoloDikaiomaton = Convert.ToInt32(dtRightsElegmena.Rows[0]["elegmenaRights"]) + Convert.ToInt32(dtRightsMiElegmena.Rows[0]["miElegmenaRights"]);
            drRights["Ελεγμένα Δικαιώματα"] = dtRightsElegmena.Rows[0]["elegmenaRights"];
            double posostoEl = (Convert.ToDouble(dtRightsElegmena.Rows[0]["elegmenaRights"]) * 100) / sinoloDikaiomaton;
            drRights["Ποσοστό Ελεγμένων"] = Convert.ToString(Math.Round(posostoEl, 1)) + "%";
            drRights["Μή Ελεγμένα Δικαιώματα"] = dtRightsMiElegmena.Rows[0]["miElegmenaRights"];
            double posostoMi = (Convert.ToDouble(dtRightsMiElegmena.Rows[0]["miElegmenaRights"]) * 100) / sinoloDikaiomaton;
            drRights["Ποσοστό Μη Ελεγμένων"] = Convert.ToString(Math.Round(posostoMi, 1)) + "%";
            drRights["Συνολικά Δικαιώματα"] = sinoloDikaiomaton.ToString();
            statsDataTable.Rows.Add(drRights);
            GridViewDikaiomaton.DataSource = statsDataTable;
            GridViewDikaiomaton.DataBind();
            this.Session["genikoSinoloStatsDataTable"] = statsDataTable;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            csv csvFile = new csv();
            String fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".csv";
            String thisDayDir = Server.MapPath("~") + "csvFiles\\" + DateTime.Now.ToString("yyyy_MM_dd");
            String excellFilePath = thisDayDir + "\\" + fileName;
            DataTable genikoSinoloStatsDataTable = (DataTable)this.Session["genikoSinoloStatsDataTable"];
            DataTable genikoSinoloStatsRights = (DataTable)this.Session["genikoSinoloStatsRights"];
            List<DataTable> tables = new List<DataTable>(){genikoSinoloStatsRights, genikoSinoloStatsDataTable};
            csvFile.writeToCsvFile(excellFilePath, tables);

            Response.Write("<script>");
            Response.Write("window.open('downloadFile.aspx?fileName=" + fileName + "&excellFilePath=" + excellFilePath.Replace('\\','*')+"' ,'_blank')");
            Response.Write("</script>");

            //Response.Write("<script>window.open ('~/statistika/downloadFile.aspx?fileName=" + fileName + "&excellFilePath=" + excellFilePath + ",'_blank');</script>");
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