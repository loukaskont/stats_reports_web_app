using Newtonsoft.Json;
using Npgsql;
using statistika_net4.statistika;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Statistika.statistika
{
    public partial class Teampad : System.Web.UI.Page
    {
        String User_Id = "", Password = "";
        public String baseUrl = "http://geo.team-pad.com/api/yourId/sheets/";
        Dictionary<String, String> workspacesSheets = new Dictionary<String, String>();
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
            if (searchForWorkSpaces())
            {
                foreach (KeyValuePair<string, string> entry in workspacesSheets)
                {
                    try
                    {
                        showWorkspace(baseUrl + entry.Value, entry.Key);
                    }
                    catch
                    {
                        String webSiteRootPath = Server.MapPath("~");
                        Process process = new Process();
                        process.StartInfo.FileName = webSiteRootPath + "TeamPadClient.exe";
                        process.Start();
                        Label1.Text = "Ενημερώνονται οι κωδικοί των teampad sheets. Η διαδικασία θα διαρκέσει μερικά δευτερόλεπτα.";
                        process.WaitForExit();
                        Page.Response.Redirect(Page.Request.Url.ToString(), true);
                    }
                }
            }
        }

        private void showWorkspace(String url, String workSpace)
        {
            String currentDate = DateTime.Now.AddDays(-1).ToString("yyy-MM-dd");
            String currentDataBase = "geoutils";
            String connstring = "User Id=" + User_Id + ";Password=" + Password + ";Server=192.168.65.26;Port=5432;Database=" + currentDataBase + ";Pooling=false;Preload Reader=true;CommandTimeout=10000";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            String sqlSelectTeampadStats = "select stat_date,workspace_name,oloklir_mix,oloklir_nom,mi_oloklir_mix,mi_oloklir_nom,olokliromena,mi_olokliromena from stats_teampad where stat_date = '" + currentDate + "' and workspace_name = '" + workSpace + "';";
            DataTable dtTeampadStats = selectInDatabase(sqlSelectTeampadStats, conn);
            if (dtTeampadStats.Rows.Count == 0) 
            {
                teampad teampad = new teampad();
                teampad.insertStats();
                dtTeampadStats = selectInDatabase(sqlSelectTeampadStats, conn);
            }
            conn.Close();
            GridView GridView1 = new GridView();
            Label titleLabel = new Label();
            titleLabel.Text = "Workspace: " + workSpace + ":";
            Panel1.Controls.Add(titleLabel);
            Panel1.Controls.Add(GridView1);
            Panel1.Controls.Add(new LiteralControl("<br/><br/>"));
            DataTable dtTeampadStatsForView = new DataTable();
            dtTeampadStatsForView.Columns.Add("Τύπος");
            dtTeampadStatsForView.Columns.Add("Μηχανικοί");
            dtTeampadStatsForView.Columns.Add("Νομικοί");
            dtTeampadStatsForView.Columns.Add("Σύνολο");
            DataRow oloklirRow = dtTeampadStatsForView.NewRow();
            oloklirRow["Τύπος"] = "ΟΛΟΚΛΗΡΩΜΕΝΑ";
            oloklirRow["Μηχανικοί"] = dtTeampadStats.Rows[0]["oloklir_mix"];
            oloklirRow["Νομικοί"] = dtTeampadStats.Rows[0]["oloklir_nom"];
            oloklirRow["Σύνολο"] = dtTeampadStats.Rows[0]["olokliromena"];
            DataRow mi_oloklirRow = dtTeampadStatsForView.NewRow();
            mi_oloklirRow["Τύπος"] = "ΜΗ ΟΛΟΚΛΗΡΩΜΕΝΑ";
            mi_oloklirRow["Μηχανικοί"] = dtTeampadStats.Rows[0]["mi_oloklir_mix"];
            mi_oloklirRow["Νομικοί"] = dtTeampadStats.Rows[0]["mi_oloklir_nom"];
            mi_oloklirRow["Σύνολο"] = dtTeampadStats.Rows[0]["mi_olokliromena"];
            dtTeampadStatsForView.Rows.Add(oloklirRow);
            dtTeampadStatsForView.Rows.Add(mi_oloklirRow);
            GridView1.DataSource = dtTeampadStatsForView;
            GridView1.DataBind();
            this.Session["statistikaTable" + workSpace] = dtTeampadStatsForView;
        }


        private DataTable getStatsTable(String url, String workSpace) 
        {
            DataTable dt = getDataTableFromUrl(url);
            int olokliromena = getCountOfColumn(dt, dt.Columns[22].ColumnName, "'True'", " = ");
            int mixanikoiMiOlokliromena = getCountOfColumn(dt, "" + dt.Columns[16].ColumnName + " = 'ΜΗΧΑΝΙΚΟΣ' and " + dt.Columns[22].ColumnName + " <> 'True'");
            int nomikoiMiOlokliromena = getCountOfColumn(dt, "" + dt.Columns[16].ColumnName + " = 'ΝΟΜΙΚΟΣ' and " + dt.Columns[22].ColumnName + " <> 'True'");
            int perilipsi_aitisi = getCountOfColumn(dt, "" + dt.Columns[16].ColumnName + " = 'ΠΕΡΙΛΗΨΗ-ΑΙΤΗΣΗ'");
            int miOlokliromena = getCountOfColumn(dt, "" + dt.Columns[22].ColumnName + " <> 'True'");
            int mixanikoi_oloklir = getCountOfColumn(dt, "" + dt.Columns[22].ColumnName + " = 'True' and " + dt.Columns[16].ColumnName + " = 'ΜΗΧΑΝΙΚΟΣ'");
            int nomikoi_oloklir = getCountOfColumn(dt, "" + dt.Columns[22].ColumnName + " = 'True' and " + dt.Columns[16].ColumnName + " = 'ΝΟΜΙΚΟΣ'");
            List<String> descriptions = new List<String>() { "ΟΛΟΚΛΗΡΩΜΕΝΑ", "ΜΗ ΟΛΟΚΛΗΡΩΜΕΝΑ" };
            List<String> values = new List<String>() { (mixanikoi_oloklir + nomikoi_oloklir).ToString(), (mixanikoiMiOlokliromena + nomikoiMiOlokliromena).ToString(), mixanikoiMiOlokliromena.ToString(), nomikoiMiOlokliromena.ToString(), mixanikoi_oloklir.ToString(), nomikoi_oloklir.ToString() };
            DataTable statistikaTable = new DataTable();
            statistikaTable.Columns.Add("Τύπος");
            statistikaTable.Columns.Add("Μηχανικοί");
            statistikaTable.Columns.Add("Νομικοί");
            statistikaTable.Columns.Add("Σύνολο");
            createStatistikaRows(descriptions, values, statistikaTable);
            return statistikaTable;
        }


        public String getHtmlCode(string Url)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();
            return result;
        }

        private DataTable getDataTableFromUrl(String url)
        {
            String jsonStream = getHtmlCode(url);
            DataTable returnDt = (DataTable)JsonConvert.DeserializeObject(jsonStream, (typeof(DataTable)));
            return returnDt;
        }

        private void createStatistikaRows(List<String> descriptions, List<String> values, DataTable statistikaTable)
        {
            DataRow drolokliromena = statistikaTable.NewRow();
            drolokliromena[0] = descriptions[0];
            drolokliromena[1] = values[4];
            drolokliromena[2] = values[5];
            drolokliromena[3] = values[0];
            statistikaTable.Rows.Add(drolokliromena);
            DataRow drmioloklir = statistikaTable.NewRow();
            drmioloklir[0] = descriptions[1];
            drmioloklir[1] = values[2];
            drmioloklir[2] = values[3];
            drmioloklir[3] = values[1];
            statistikaTable.Rows.Add(drmioloklir);
        }

        private int getCountOfColumn(DataTable dt, String columnName, String value, String selectOperator)
        {
            int returnCount = 0;
            DataRow[] selectedRows = dt.Select(columnName + selectOperator + value);
            if (selectedRows.Length > 0)
            {
                returnCount = selectedRows.Length;
            }
            return returnCount;
        }

        private int getCountOfColumn(DataTable dt, String selectStr)
        {
            int returnCount = 0;
            DataRow[] selectedRows = dt.Select(selectStr);
            if (selectedRows.Length > 0)
            {
                returnCount = selectedRows.Length;
            }
            return returnCount;
        }

        private DataTable getFirstRows(DataTable dt, int rowsCount)
        {
            return dt.AsEnumerable().Take(rowsCount).CopyToDataTable();
        }

        private DataTable getDataFromUrlLimit(String TeamPadUrl)
        {
            DataTable returnDataTable = new DataTable();
            String jsonStream = "";
            int pageIndex = 1;
            Boolean readNextPage = true;
            while (readNextPage)
            {
                jsonStream = getHtmlCode(TeamPadUrl + pageIndex);
                pageIndex++;
                if (jsonStream == "")
                {
                    readNextPage = false;
                }
                else
                {
                    DataTable thisPageDt = (DataTable)JsonConvert.DeserializeObject(jsonStream, (typeof(DataTable)));
                    if (returnDataTable.Rows.Count == 0)
                    {
                        returnDataTable = thisPageDt.Copy();
                    }
                    else
                    {
                        returnDataTable.Merge(thisPageDt, true, MissingSchemaAction.Ignore);
                    }
                }
                jsonStream = "";
            }
            return returnDataTable;
        }

        private Boolean searchForWorkSpaces()
        {
            String connstring = "User Id=" + User_Id + ";Password=" + Password + ";Server=192.168.24.45;Port=5432;Database=geoutils;Pooling=false;Preload Reader=true;CommandTimeout=10000";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            String sql = "select * from geoutils.public.teampad_stats_sheets;";
            DataTable dt = selectInDatabase(sql, conn);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                workspacesSheets.Add(dt.Rows[i]["sheet_name"].ToString(), dt.Rows[i]["sheet_code"].ToString());
            }
            return true;
        }


        private String getSheetCodeFromHtml(String html) 
        {
            String newSheetCode = "";
            for (int i = 0; i < html.Length-19; i++)
            {
                if (html.Substring(i, 19) == "ΛΙΣΤΕΣ ΓΙΑ ΠΑΡΑΔΟΣΗ")
                {
                    int j = i;
                    while (html.Substring(j, 5) != "href=") 
                    {
                        j--;
                    }
                    j = j + 7;
                    while (html[j] != '\"') 
                    {
                        newSheetCode = newSheetCode + html[j];
                        j++;
                    }
                }
            }
            return newSheetCode;
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
            List<DataTable> tables = new List<DataTable>();
            foreach (KeyValuePair<string, string> entry in workspacesSheets)
            {
                String workSpace = entry.Key;
                DataTable statistikaTable = (DataTable)this.Session["statistikaTable" + workSpace];
                tables.Add(statistikaTable);
            }
            csv csvFile = new csv();
            String fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".csv";
            String thisDayDir = Server.MapPath("~") + "\\csvFiles\\" + DateTime.Now.ToString("yyyy_MM_dd");
            if (!Directory.Exists(thisDayDir))
            {
                Directory.CreateDirectory(thisDayDir);
            }
            String excellFilePath = thisDayDir + "\\" + fileName;
            csvFile.writeToCsvFile(excellFilePath, tables);
            Response.Write("<script>");
            Response.Write("window.open('downloadFile.aspx?fileName=" + fileName + "&excellFilePath=" + excellFilePath.Replace('\\', '*') + "' ,'_blank')");
            Response.Write("</script>");
        }


    }
}
