using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace statistika_net4.statistika
{
    public class csv
    {
        public void writeToCsvFile(String filePath, DataTable dt) 
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, true, System.Text.Encoding.Default);
            String currentRow  = "";
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                currentRow = currentRow + dt.Columns[j].ColumnName + ";";
            }
            sw.WriteLine(currentRow);
            currentRow = "";
            for (int i = 0; i < dt.Rows.Count; i++) 
            {
                for(int j=0; j<dt.Columns.Count; j++)
                {
                    currentRow = currentRow + dt.Rows[i][j] + ";";
                }
                sw.WriteLine(currentRow);
                currentRow = "";
            }
            sw.Close();
        }


        public void writeToCsvFile(String filePath, List<DataTable> tables)
        {
            String dirPath2 = Path.GetDirectoryName(filePath);
            String dirPath1 = Path.GetDirectoryName(dirPath2);
            if (!Directory.Exists(dirPath1)) 
            {
                Directory.CreateDirectory(dirPath1);
            }
            if (!Directory.Exists(dirPath2))
            {
                Directory.CreateDirectory(dirPath2);
            }
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, true, System.Text.Encoding.Default);
            foreach (DataTable dt1 in tables)
            {
                String currentRow = "";
                for (int j = 0; j < dt1.Columns.Count; j++)
                {
                    currentRow = currentRow + dt1.Columns[j].ColumnName + ";";
                }
                sw.WriteLine(currentRow);
                currentRow = "";
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    for (int j = 0; j < dt1.Columns.Count; j++)
                    {
                        currentRow = currentRow + dt1.Rows[i][j] + ";";
                    }
                    sw.WriteLine(currentRow);
                    currentRow = "";
                }
                sw.WriteLine(); sw.WriteLine();
            }
            sw.Close();
        }

    }
}