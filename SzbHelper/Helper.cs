using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SzbHelper
{
    public class Helper
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        public enum languages {hungarian,english}
        /// <summary>
        /// convert HUNGARIAN/ ENGLISH:  XXXX.JANUÁR -> XXXX.01.01
        /// 
        /// </summary>
        /// <param name="date"> F.E: 2022.január </param>
        /// <returns></returns>
        public string getdate(string date,languages lan)
        {
            if (date != null)
            {
                try
                {
                    Dictionary<string, string> translator = createMonths(lan);
                    date = date.Replace(" ", "");
                    
                    date = date_convert(date, translator);

                    if (date.Length == 6)
                        date = date.Substring(0, 4) + "." + date.Substring(4, date.Length);

                    if (date.Length == 7)
                        date += ".01";

                    if (date.Length != 10) 
                        return null;
                }
                catch (Exception)
                {
                    date = "1111.11.11";
                }

            }
            return date;
        }
        private static Dictionary<string, string> createMonths(languages lan)
        {
            Dictionary<string, string> language_months = new Dictionary<string, string>();
            switch (lan)
            {
                case languages.hungarian:
                    language_months.Add("január".ToUpper(), "01");                
                    language_months.Add("február".ToUpper(), "02");
                    language_months.Add("március".ToUpper(), "03");
                    language_months.Add("április".ToUpper(), "04");
                    language_months.Add("május".ToUpper(), "05");
                    language_months.Add("június".ToUpper(), "06");
                    language_months.Add("július".ToUpper(), "07");
                    language_months.Add("júli".ToUpper(), "07");
                    language_months.Add("augusztus".ToUpper(), "08");                    
                    language_months.Add("szeptember".ToUpper(), "09");                    
                    language_months.Add("október".ToUpper(), "10");                    
                    language_months.Add("november".ToUpper(), "11");                   
                    language_months.Add("december".ToUpper(), "12");
                    break;
                case languages.english:
                    language_months.Add("january".ToUpper(), "01");
                    language_months.Add("januar".ToUpper(), "01");
                    language_months.Add("februar".ToUpper(), "02");
                    language_months.Add("february".ToUpper(), "02");
                    language_months.Add("March".ToUpper(), "03");
                    language_months.Add("april".ToUpper(), "04");
                    language_months.Add("may".ToUpper(), "05");
                    language_months.Add("june".ToUpper(), "06");
                    language_months.Add("july".ToUpper(), "07");
                    language_months.Add("august".ToUpper(), "08");
                    language_months.Add("september".ToUpper(), "09");
                    language_months.Add("october".ToUpper(), "10");
                    language_months.Add("november".ToUpper(), "11");
                    language_months.Add("december".ToUpper(), "12");
                    break;
                default:
                    break;
            }
            language_months.Add("jan".ToUpper(), "01");
            language_months.Add("feb".ToUpper(), "02");
            language_months.Add("febr".ToUpper(), "02");
            language_months.Add("mar".ToUpper(), "03");
            language_months.Add("marc".ToUpper(), "03");
            language_months.Add("apr".ToUpper(), "04");
            language_months.Add("ápr".ToUpper(), "04");
            language_months.Add("máj".ToUpper(), "05");
            language_months.Add("jun".ToUpper(), "06");
            language_months.Add("jún".ToUpper(), "06");
            language_months.Add("júl".ToUpper(), "07");
            language_months.Add("jul".ToUpper(), "07");
            language_months.Add("aug".ToUpper(), "08");
            language_months.Add("szept".ToUpper(), "09");
            language_months.Add("sept".ToUpper(), "09");
            language_months.Add("okt".ToUpper(), "10");
            language_months.Add("nov".ToUpper(), "11");
            language_months.Add("dec".ToUpper(), "12");
            return language_months;
        }
        private string date_convert(string date, Dictionary<string, string> dictionary)
        {
            date = date.ToUpper();
            foreach (var dictkeys in dictionary)
            {
                if(date.Contains(dictkeys.Key))
                    date = date.Replace(dictkeys.Key, dictkeys.Value);
            }
            return date;
        }
        /// <summary>
        /// File path where the new doc have to be (with extension)
        /// </summary>
        /// <param name="filePath"> path of file (For example: c:/temp/proba.pdf </param>
        /// <param name="oldpath"> don't need anything </param>
        public static void dircreate(string filePath, string oldpath = "")
        {//mindketto ugyanazt beleirni
            oldpath = oldpath == "" ? filePath : oldpath;
            string directoryName = Path.GetDirectoryName(filePath);
            string directoryName2 = Path.GetDirectoryName(oldpath);
            if (Directory.Exists(directoryName2))
            {
                string dirname = directoryName2;
            }
            else
            {
                bool vege = false;
                try
                {
                    Directory.CreateDirectory(directoryName);
                    if (directoryName == directoryName2)
                        vege = true;

                }
                catch (Exception)
                { }
                if (Directory.Exists(directoryName) && vege == false)
                    dircreate(oldpath, oldpath);
                else if (vege == false)
                    dircreate(directoryName, oldpath);
                else if (vege == true)
                { }
            }
        }
        /// <summary>
        /// Only on windows --> System.Runtime.InteropServices.Marshal.ReleaseComObject..
        /// </summary>
        /// <param name="obj"></param>
        public static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
        public static void killProcess(string processname)
        {
            foreach (System.Diagnostics.Process Proc in (from p in System.Diagnostics.Process.GetProcesses()
                                                         where p.ProcessName == processname
                                                         select p))
                Proc.Kill();
        }
        /// <summary>
        /// return the paper type 'A0' ... 'A4'
        /// </summary>
        /// <param name="pagex">page width</param>
        /// <param name="pagey">page height</param>
        /// <param name="nonstd_longer"> if not standard uses the bigger num (width / height)</param>
        /// <returns></returns>
        public string getpapertype(double pagex, double pagey,bool nonstd_longer = false)
        {
            string pagetype = "";
            //basic parameters
            if (Between(pagex, 2100, 2800) && Between(pagey, 2900, 4000) || Between(pagey, 2100, 2800) && Between(pagex, 2900, 4000))
                pagetype = "A0";
            else if (Between(pagex, 1560, 2100) && Between(pagey, 2100, 2800) || Between(pagey, 1560, 2100) && Between(pagex, 2100, 2800))
                pagetype = "A1";
            else if (Between(pagex, 1050, 1560) && Between(pagey, 1560, 2100) || Between(pagey, 1050, 1560) && Between(pagex, 1560, 2100))
                pagetype = "A2";           
            else if (Between(pagex, 650, 1050) && Between(pagey, 1050, 1560) || Between(pagey, 650, 1050) && Between(pagex, 1050, 1560))
                pagetype = "A3";
            else if (Between(pagex, 350, 750) && Between(pagey, 650, 1050) || Between(pagey, 350, 750) && Between(pagex, 650, 1050))
                pagetype = "A4";
            else
            {
                if (nonstd_longer)
                {
                    if (pagey > pagex)
                    {
                        pagex = pagey;
                    }
                    if (pagex > 600)
                    {
                        pagetype = "A4";
                        if (pagex > 1000)
                        {
                            pagetype = "A3";
                            if (pagex > 1500)
                            {
                                pagetype = "A2";
                                if (pagex > 2000)
                                {
                                    pagetype = "A1";
                                    if (pagex > 2750)
                                    {
                                        pagetype = "A0";
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (pagey < pagex)
                    {
                        pagex = pagey;
                    }
                    if (pagex > 300)
                    {
                        pagetype = "A4";
                        if (pagex > 750)
                        {
                            pagetype = "A3";
                            if (pagex > 1100)
                            {
                                pagetype = "A2";
                                if (pagex > 1600)
                                {
                                    pagetype = "A1";
                                    if (pagex > 2100)
                                    {
                                        pagetype = "A0";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return pagetype;
        }
        public bool Between(double num, double lower, double upper, bool inclusive = false)
        {
            return inclusive
                ? lower <= num && num <= upper
                : lower < num && num < upper;
        }
        /// <summary>
        /// extracts and returns field headers from a CSV file
        /// </summary>
        /// <param name="csvFile">path of csv file</param>
        /// <param name="allowBlankFieldNames"> obvius</param>
        /// <returns></returns>
        public static string[] ReadFieldHeaders(string csvFile, bool allowBlankFieldNames = false)
        {
            string[] fields = null;

            using (var stream = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(stream))
                {
                    //read header
                    string nextLine = sr.ReadLine();
                    if (nextLine != null)
                    {
                        fields = nextLine.Split(',');
                        TrimValues(fields);
                        if (!allowBlankFieldNames)
                        {
                            for (int n = fields.Length - 1; n >= 0; --n)
                            {
                                if (string.IsNullOrEmpty(fields[n])) throw new Exception(string.Format("Blank field names are not allowed (column number {0})", (n + 1)));
                            }
                        }
                    }
                }
            }

            return fields;
        }
        /// <summary>
        /// Trims whitespace from an array of field values
        /// </summary>
        /// <param name="values"></param>
        public static void TrimValues(string[] values)
        {
            if (values == null) return;
            for (int n = values.Length - 1; n >= 0; --n)
            {
                values[n] = values[n].Trim();
            }
        }
        /// <summary>
        /// Returns zero-based index of a given field name in an array of field names
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="fieldName"></param>
        /// <param name="ignoreCase"> Ignore upper - lower </param>
        /// <returns></returns>
        public static int IndexOfField(string[] fields, string fieldName, bool ignoreCase)
        {
            if (fields == null || string.IsNullOrEmpty(fieldName)) return -1;
            int index;
            for (index = fields.Length - 1; index >= 0; --index)
            {
                if (ignoreCase)
                {
                    if (string.Compare(fields[index], fieldName, StringComparison.OrdinalIgnoreCase) == 0) return index;
                }
                else
                {
                    if (string.Compare(fields[index], fieldName, StringComparison.Ordinal) == 0) return index;
                }
            }
            return index;
        }
    }
}
