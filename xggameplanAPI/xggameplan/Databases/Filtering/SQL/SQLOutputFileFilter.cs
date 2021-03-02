using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;
using xggameplan.common.Databases;

namespace xggameplan.OutputFiles.Filtering.SQL
{
    /// <summary>
    /// Filters output file data using SQL, writes to CSV
    /// </summary>
    public class SQLOutputFileFilter : IOutputFileFilter<SQLOutputFileFilterSettings>
    {
        public void Filter(SQLOutputFileFilterSettings settings)
        {            
            List<string> oldFiles = new List<string>();
            List<string> newFiles = new List<string>();

            try
            {
                // Force files to have .txt extension for query to work
                foreach (string file in Directory.GetFiles(settings.OutputFileFolder))
                {
                    string newFile = Path.Combine(settings.OutputFileFolder, Path.GetFileNameWithoutExtension(file) + ".txt");
                    if (file != newFile)
                    {
                        if (File.Exists(newFile))
                        {
                            File.Delete(newFile);
                        }
                        oldFiles.Add(file);
                        newFiles.Add(newFile);
                        File.Move(file, newFile);
                    }
                }

                // Open database
                string connectionString = GetConnectionString(settings.OutputFileFolder);
                //OleDbDatabase database = new OleDbDatabase(connectionString);            
                OdbcDatabase database = new OdbcDatabase(connectionString);
                database.Open();

                // Execute query (OleDb)                        
                /*
                using (OleDbDataReader reader = database.ExecuteReader(CommandType.Text, settings.SQL, CommandBehavior.Default, null))
                {
                    //DataTable dataTable = new DataTable();
                    //dataTable.Load(reader);
                    SaveResults(reader, settings);
                    reader.Close();
                } 
                */

                // Execute query (ODBC)
                using (OdbcDataReader reader = database.ExecuteReader(CommandType.Text, settings.SQL, CommandBehavior.Default, null))
                {
                    //DataTable dataTable = new DataTable();
                    //dataTable.Load(reader);
                    SaveResults(reader, settings);
                    reader.Close();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                // Rename files back from .txt to original
                for (int index = 0; index < oldFiles.Count; index++)
                {                    
                    File.Move(newFiles[index], oldFiles[index]);
                }
            }
        }

        private string GetConnectionString(string dataFolder)
        {
            // OleDB
            // Added Mode=Read, ReadOnly
            //return string.Format("Provider = Microsoft.ACE.OLEDB.12.0; Data Source = {0}; Extended Properties =\"Text;HDR=Yes;FORMAT=Delimited\"", dataFolder);   // Not registered on local machine
            //return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Mode=Read;ReadOnly=1;Read Only=1;Extended Properties=\"text;HDR=Yes;FMT=Delimited\"", dataFolder);

            // ODBC           
            //return string.Format("Driver = Microsoft Access Text Driver(*.txt, *.csv); Dbq = {0}; Extensions = asc,csv,tab,txt;", dataFolder);    // 64 bit odbc
            // Added read only
            return "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=[data_folder]; ReadOnly=true; Extensions = asc, csv, tab, txt; ".Replace("[data_folder]", dataFolder);   // Microsoft Text Driver (Standard) - Cannot update, database or object is read-only
            //return string.Format("Driver = Microsoft Access Text Driver(*.txt, *.csv); Dbq = {0}; Extensions = asc,csv,tab,txt; ", dataFolder);     // 64 bit odbc
        }

        /// <summary>
        /// Saves results to CSV filter
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="settings"></param>
        //private void SaveResults(OleDbDataReader reader, SQLOutputFileFilterSettings settings)
        //{
        //    Directory.CreateDirectory(Path.GetDirectoryName(settings.ResultsFile));
        //    if (File.Exists(settings.ResultsFile))
        //    {
        //        File.Delete(settings.ResultsFile);
        //    }

        //    using (StreamWriter writer = new StreamWriter(settings.ResultsFile))
        //    {
        //        // Write header
        //        writer.WriteLine(GetHeaderLine(reader, settings));

        //        // Write data
        //        while (reader.Read())
        //        {
        //            writer.WriteLine(GetDataLine(reader, settings));
        //        }
        //        writer.Flush();
        //        writer.Close();                
        //    }
        //}

        private void SaveResults(OdbcDataReader reader, SQLOutputFileFilterSettings settings)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settings.ResultsFile));
            if (File.Exists(settings.ResultsFile))
            {
                File.Delete(settings.ResultsFile);
            }

            using (StreamWriter writer = new StreamWriter(settings.ResultsFile, true))
            {
                // Write header
                writer.WriteLine(GetHeaderLine(reader, settings));

                // Write data
                while (reader.Read())
                {
                    writer.WriteLine(GetDataLine(reader, settings));
                }
                writer.Flush();
                writer.Close();
            }
        }

        private string GetHeaderLine(OdbcDataReader reader, SQLOutputFileFilterSettings settings)
        {
            StringBuilder line = new StringBuilder("");
            for (int fieldIndex = 0; fieldIndex < reader.FieldCount; fieldIndex++)
            {
                if (fieldIndex > 0)
                {
                    line.Append(settings.Delimiter);
                }
                line.Append(reader.GetName(fieldIndex));
            }
            return line.ToString();
        }

        /// <summary>
        /// Returns header line for CSV
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        //private string GetHeaderLine(OleDbDataReader reader, SQLOutputFileFilterSettings settings)
        //{
        //    StringBuilder line = new StringBuilder("");
        //    for (int fieldIndex = 0; fieldIndex < reader.FieldCount; fieldIndex++)
        //    {
        //        if (fieldIndex > 0)
        //        {
        //            line.Append(settings.Delimiter);
        //        }               
        //        line.Append(reader.GetName(fieldIndex));                
        //    }
        //    return line.ToString();
        //}

        ///// <summary>
        ///// Returns data line for CSV
        ///// </summary>
        ///// <param name="reader"></param>
        ///// <param name="settings"></param>
        ///// <returns></returns>
        //private string GetDataLine(OleDbDataReader reader, SQLOutputFileFilterSettings settings)
        //{
        //    StringBuilder line = new StringBuilder("");
        //    for (int fieldIndex =0; fieldIndex < reader.FieldCount; fieldIndex++)
        //    {
        //        if (fieldIndex > 0)
        //        {
        //            line.Append(settings.Delimiter);
        //        }
        //        if (reader.IsDBNull(fieldIndex))
        //        {
        //            line.Append("null");
        //        }
        //        else
        //        {
        //            line.Append(reader.GetValue(fieldIndex).ToString());        // TODO: Improve this
        //        }
        //    }
        //    return line.ToString();
        //}

        private string GetDataLine(OdbcDataReader reader, SQLOutputFileFilterSettings settings)
        {
            StringBuilder line = new StringBuilder("");
            for (int fieldIndex = 0; fieldIndex < reader.FieldCount; fieldIndex++)
            {
                if (fieldIndex > 0)
                {
                    line.Append(settings.Delimiter);
                }
                if (reader.IsDBNull(fieldIndex))
                {
                    line.Append("null");
                }
                else
                {
                    line.Append(reader.GetValue(fieldIndex).ToString());        // TODO: Improve this
                }
            }
            return line.ToString();
        }
    }
}