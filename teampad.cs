using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Npgsql;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Net;

namespace statistika_net4.statistika
{
    public class teampad
    {
        public String baseUrl = "http://geo.team-pad.com/api/yourId/sheets/";
        Dictionary<String, String> workspacesSheets = new Dictionary<String, String>();
        String geoutilsConnstring = "User Id=its;Password=1;Server=192.168.24.24;Port=5432;Database=geoutils;Pooling=false;Preload Reader=true;CommandTimeout=10000";

        public void insertStats()
        {
            if (searchForWorkSpaces())
            {
                foreach (KeyValuePair<string, string> entry in workspacesSheets)
                {
                    try
                    {
                        insertWorkspaceStats(baseUrl + entry.Value, entry.Key);
                    }
                    catch
                    {
                        String appDirPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Process process = new Process();
                        process.StartInfo.FileName = appDirPath + "TeamPadClient.exe";
                        process.Start();
                        process.WaitForExit();
                    }
                }
            }
        }



        private void insertWorkspaceStats(String url, String workSpace)
        {
            String currentDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            DataTable statistikaTable = getStatsTable(url, workSpace);
            int oloklirMix = Convert.ToInt32(statistikaTable.Rows[0][1].ToString());
            int oloklirNom = Convert.ToInt32(statistikaTable.Rows[0][2].ToString());
            int oloklir = Convert.ToInt32(statistikaTable.Rows[0][3].ToString());
            int miOloklir_mix = Convert.ToInt32(statistikaTable.Rows[1][1].ToString());
            int miOloklirNom = Convert.ToInt32(statistikaTable.Rows[1][2].ToString());
            int miOloklir = Convert.ToInt32(statistikaTable.Rows[1][3].ToString());
            String insertTeamPadStats = @"insert into stats_teampad(stat_date,workspace_name,oloklir_mix,oloklir_nom,mi_oloklir_mix,mi_oloklir_nom,olokliromena,mi_olokliromena) 
                                        values('" + currentDate + "','" + workSpace + "'," + oloklirMix + "," + oloklirNom + "," + miOloklir_mix + ", " + miOloklirNom + "," + oloklir + "," + miOloklir + ");";
            NpgsqlConnection conn = new NpgsqlConnection(geoutilsConnstring);
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(insertTeamPadStats, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
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
            String connstring = "User Id=its;Password=1;Server=192.168.27.55;Port=5432;Database=geoutils;Pooling=false;Preload Reader=true;CommandTimeout=10000";
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
            for (int i = 0; i < html.Length - 19; i++)
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

    }
}
