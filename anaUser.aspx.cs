using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace statistika_net4.statistika
{
    public partial class anaUser : System.Web.UI.Page
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
            if (!IsPostBack)
            {
                string script = "$(document).ready(function () { $('[id*=btnSubmit]').click(); });";
                ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);
                if (DateTime.Now.AddDays(-1).DayOfWeek == DayOfWeek.Sunday)
                {
                    Calendar1.SelectedDate = DateTime.Now.AddDays(-3);
                }
                if (DateTime.Now.AddDays(-1).DayOfWeek == DayOfWeek.Saturday)
                {
                    Calendar1.SelectedDate = DateTime.Now.AddDays(-2);
                }
                if (DateTime.Now.AddDays(-1).DayOfWeek != DayOfWeek.Saturday && DateTime.Now.AddDays(-1).DayOfWeek != DayOfWeek.Sunday)
                {
                    Calendar1.SelectedDate = DateTime.Now.AddDays(-1);
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            loadStats();
        }


        private void viewDikaiomataAnaUser() 
        {
            String currentDate = Calendar1.SelectedDate.ToString("yyyy-MM-dd");
            currentDataBase = DropDownListDatabase.Text;
            connstring = "User Id=" + User_Id + ";Password=" + Password + ";Server=192.168.22.21;Port=5432;Database=" + currentDataBase + ";Pooling=false;Preload Reader=true;CommandTimeout=10000";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            string sqlDikaiomataAnaUser = @"select g.fname as userName, count(distinct(r.g_right_id)) as rightsCount from ktinc.prop p 
                    left join ktinc.right r on p.g_prop_id = r.g_prop_id
                    left join ktinc.ben_right b on r.g_right_id = b.g_right_id
                    left join history_inc.right h on b.g_right_id = h.g_right_id
                    left join admin.geousers g on trim(split_part(h.upduser,'/',1)) = g.uname
                    where to_char(h.upddate, 'YYYY-MM-DD') = '" + currentDate + @"'
                    and r.g_right_id not like 'UN%'
                    and b.right_status <> '04'
                    and p.validation_flag_in_prop_tab = 1
                    and g.fname <> ''
                    group by g.fname;";
            DataTable dtsqlDikaiomataAnaUser = selectInDatabase(sqlDikaiomataAnaUser, conn);
            conn.Close();
            dtsqlDikaiomataAnaUser.Columns["userName"].ColumnName = "Χρήστης";
            dtsqlDikaiomataAnaUser.Columns["rightsCount"].ColumnName = "Δικαιώματα";
            GridViewDikaiomata.DataSource = dtsqlDikaiomataAnaUser;
            GridViewDikaiomata.DataBind();
            this.Session["dtsqlDikaiomataAnaUser"] = dtsqlDikaiomataAnaUser;
        }

        private void viewGeotemaxiaAnaUser() 
        {
            String currentDate = Calendar1.SelectedDate.ToString("yyyy-MM-dd");
            currentDataBase = DropDownListDatabase.Text;
            connstring = "User Id=" + User_Id + ";Password=" + Password + ";Server=192.168.22.21;Port=5432;Database=" + currentDataBase + ";Pooling=false;Preload Reader=true;CommandTimeout=10000";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            string sqlKaekAnaUserElegmena = @"select u.fname as userName, count(distinct(p.kaek)) as elegmena
                from history_gis.pst p 
                left join admin.geousers u on trim(split_part(p.upduser,'/',1)) = u.uname
                where kaek <> '' and 
                to_char(p.upddate, 'YYYY-MM-DD') = '" + currentDate + @"' 
                and p.geometry_verified = 1
                and u.fname is not null
                group by u.fname;";
            string sqlKaekAnaUserXoriki = @"select u.fname as userName, count(distinct(p.kaek)) as xorikiMetavoli
                from history_gis.pst p 
                left join admin.geousers u on trim(split_part(p.upduser,'/',1)) = u.uname
                where kaek <> '' and 
                to_char(p.upddate, 'YYYY-MM-DD') = '" + currentDate + @"' 
                and u.fname is not null
                group by u.fname;";
            DataTable dtKaekKaekAnaUserElegmena = selectInDatabase(sqlKaekAnaUserElegmena, conn);
            DataTable dtKaekKaekAnaUserXoriki = selectInDatabase(sqlKaekAnaUserXoriki, conn);
            dtKaekKaekAnaUserXoriki.Columns.Add("elegmena");
            addElegmenaToXorikiMetavoli(dtKaekKaekAnaUserXoriki, dtKaekKaekAnaUserElegmena);
            conn.Close();
            dtKaekKaekAnaUserXoriki.Columns["userName"].ColumnName = "Χρήστης";
            dtKaekKaekAnaUserXoriki.Columns["xorikiMetavoli"].ColumnName = "χωρική μεταβολή";
            dtKaekKaekAnaUserXoriki.Columns["elegmena"].ColumnName = "Ελεγμένα";
            GridViewGeotemaxia1.DataSource = dtKaekKaekAnaUserXoriki;
            GridViewGeotemaxia1.DataBind();
            this.Session["dtKaekKaekAnaUserXoriki"] = dtKaekKaekAnaUserXoriki;
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


        private void addElegmenaToXorikiMetavoli(DataTable xor, DataTable eleg) 
        {
            for (int i = 0; i < xor.Rows.Count; i++) 
            {
                for (int j = 0; j < eleg.Rows.Count; j++)
                {
                    if (xor.Rows[i]["userName"].ToString() == eleg.Rows[j]["userName"].ToString())
                    {
                        xor.Rows[i]["elegmena"] = eleg.Rows[j]["elegmena"].ToString();
                    }
                }
                if (xor.Rows[i]["elegmena"].ToString() == "") 
                {
                    xor.Rows[i]["elegmena"] = "0";
                }
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            csv csvFile = new csv();
            String fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".csv";
            String thisDayDir = Server.MapPath("~") + "\\csvFiles\\" + DateTime.Now.ToString("yyyy_MM_dd");
            String excellFilePath = thisDayDir + "\\" + fileName;
            DataTable dtsqlDikaiomataAnaUser = (DataTable)this.Session["dtsqlDikaiomataAnaUser"];
            DataTable dtKaekKaekAnaUserXoriki = (DataTable)this.Session["dtKaekKaekAnaUserXoriki"];
            List<DataTable> tables = new List<DataTable>() { dtKaekKaekAnaUserXoriki, dtsqlDikaiomataAnaUser };
            csvFile.writeToCsvFile(excellFilePath, tables);
            Response.Write("<script>");
            Response.Write("window.open('downloadFile.aspx?fileName=" + fileName + "&excellFilePath=" + excellFilePath.Replace('\\', '*') + "' ,'_blank')");
            Response.Write("</script>");
        }


        private void loadStats() 
        {
            LabelCurrentDate.Text = "Στατιστικά για τις: " + Calendar1.SelectedDate.ToString("dd-MM-yyyy");
            Label2.Text = "Γεωτεμάχια:";
            viewGeotemaxiaAnaUser();
            Label3.Text = "Δικαιώματα:";
            viewDikaiomataAnaUser();
        }



    }
}