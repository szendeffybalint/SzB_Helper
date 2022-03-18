using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SzbHelper_framework
{
    public class Helper
    {
        /// <summary>
        /// Languages what helper use 
        /// </summary>
        public enum languages { hungarian, english }
        /// <summary>
        /// convert HUNGARIAN/ ENGLISH:  XXXX.JANUÁR -> XXXX.01.01
        /// 
        /// </summary>
        /// <param name="date"> F.E: 2022.január </param>
        /// <returns></returns>
        public string getdate(string date, languages lan)
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
        /// <summary>
        /// Kill process (like task explorer ..:D)
        /// </summary>
        /// <param name="processname">name of the process (.exe not needed)</param>
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
        public string getpapertype(double pagex, double pagey, bool nonstd_longer = false)
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
        /// <summary>
        /// return true / false  depends on the num is between or not
        /// </summary>
        /// <param name="num">number</param>
        /// <param name="lower">to down</param>
        /// <param name="upper">to up</param>
        /// <param name="inclusive"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Only on windows --> System.Drawing.Imaging.ImageFormat..
        /// </summary>
        /// <param name="kep"> </param>
        /// <param name="imageFormat"> </param>
        /// <returns></returns>
        public static string Base64Encode(Bitmap kep)
        {
            System.IO.MemoryStream ms = new MemoryStream();
            kep.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] byteImage = ms.ToArray();
            var SigBase64 = Convert.ToBase64String(byteImage); // Get Base64

            return SigBase64;
        }
        /// <summary>
        /// not 100% it works on HUNGARIAN
        /// </summary>
        /// <param name="number"> number </param>
        /// <param name="lan"> default is english</param>
        /// <returns></returns>
        public static string NumberToWords(int number, languages lan = languages.english)
        {
            string words = "";

            if (languages.english == lan)
            {
                if (number == 0)
                    return "zero";

                if (number < 0)
                    return "minus " + NumberToWords(Math.Abs(number));

                if ((number / 1000000) > 0)
                {
                    words += NumberToWords(number / 1000000) + " million ";
                    number %= 1000000;
                }

                if ((number / 1000) > 0)
                {
                    words += NumberToWords(number / 1000) + " thousand ";
                    number %= 1000;
                }

                if ((number / 100) > 0)
                {
                    words += NumberToWords(number / 100) + " hundred ";
                    number %= 100;
                }

                if (number > 0)
                {
                    if (words != "")
                        words += "and ";

                    var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                    var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                    if (number < 20)
                        words += unitsMap[number];
                    else
                    {
                        words += tensMap[number / 10];
                        if ((number % 10) > 0)
                            words += "-" + unitsMap[number % 10];
                    }
                }
            }
            else if (languages.hungarian == lan)
            {
                if (number == 0)
                    return "nulla";

                if (number < 0)
                    return "minusz " + NumberToWords(Math.Abs(number), lan);

                if ((number / 1000000) > 0)
                {
                    if ((number % 1000000 == 0))
                    {
                        words += NumberToWords(number / 1000000, lan) + "millió";
                        number %= 1000000;
                    }
                    else
                    {
                        words += NumberToWords(number / 1000000, lan) + "millió-";
                        number %= 1000000;
                    }
                }

                if ((number / 1000) > 0)
                {
                    if ((number % 1000 == 0))
                    {
                        words += NumberToWords(number / 1000, lan) + "ezer";
                        number %= 1000;
                    }
                    else
                    {
                        words += NumberToWords(number / 1000, lan) + "ezer-";
                        number %= 1000;
                    }
                }

                if ((number / 100) > 0)
                {
                    words += NumberToWords(number / 100, lan) + "száz";
                    number %= 100;
                }

                if (number > 0)
                {
                    if (words != "")
                        words += "";

                    var unitsMap = new[] { "nulla", "egy", "kettõ", "három", "négy", "öt", "hat", "hét", "nyolc", "kilenc", "tíz",
                        "tizenegy", "tizenekttõ", "tizenhárom", "tizennégy", "tizenöt", "tizenhat", "tizenhét", "tizennyolc", "tizenkilenc" ,
                    "huszonegy", "huszonekttõ", "huszonhárom", "huszonnégy", "huszonöt", "huszonhat", "huszonhét", "huszonnyolc", "huszonkilenc"};
                    var tensMap = new[] { "nulla", "tíz", "húsz", "harminc", "negyven", "ötven", "hatvan", "hetven", "nyolcvan", "kilencven" };

                    if (number < 30)
                        words += unitsMap[number];
                    else
                    {
                        words += tensMap[number / 10];
                        if ((number % 10) > 0)
                            words += "" + unitsMap[number % 10];
                    }
                }
            }
            return words;
        }
        /// <summary>
        /// return the prime factors of the num (in dictinary)
        /// <br></br>
        /// primefactor.value == count fo that prime
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Dictionary<int, int> primeFactors(int num)
        {
            int prime = 2;
            Dictionary<int, int> primeFactors = new Dictionary<int, int>();
            primeFactors.Add(prime, 0);
            while (num > 1)
            {
                if (num % prime == 0)
                {
                    num /= prime;
                    primeFactors[prime]++;
                }
                else
                {
                    bool isPrime = false;
                    while (!isPrime)
                    {
                        isPrime = true;
                        prime++;
                        int limit = (int)(Math.Floor(Math.Sqrt(prime)));
                        for (int k = 2; k <= limit && isPrime; k++)
                        {
                            if (prime % k == 0)
                                isPrime = false;
                        }
                    }
                    primeFactors.Add(prime, 0);
                }
            }
            return primeFactors;
        }
        /// <summary>
        /// <br></br>
        /// Everything converted to string in properties --> no objects
        /// <br></br><br></br>
        /// Obviusly class/struct creation is needed
        /// <br></br>
        /// Return object list [you have to convert the list into the class/struct list]      
        /// </summary>
        /// <param name="url">url where the json coming from</param>
        /// <param name="classname">Name of the class what you want</param>            
        public List<object> LoadJsonFromWeb(string url, string classname)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            string jsonValue = "";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                jsonValue = reader.ReadToEnd();
            }
            List<object> lista = JsonConvert.DeserializeObject<List<object>>(jsonValue);
            return createListFromJson(lista, classname);
        }
        //Inside voids
        /// <summary>
        /// Create Object list from Json JObject
        /// </summary>
        /// <param name="lista">list of Jobjects</param>
        /// <param name="classname">Name of the class/struct what you created</param>
        /// <returns></returns>
        private List<object> createListFromJson(List<object> lista, string classname)
        {
            List<object> returnlista = new List<object>();
            var myclass = CreateByTypeName(classname);
            foreach (JObject item in lista)
            {
                var myitem = CreateByTypeName(classname);
                foreach (PropertyInfo prop in myclass.GetType().GetProperties())
                {
                    PropertyInfo propinfo = myitem.GetType().GetProperty(prop.Name, BindingFlags.Public | BindingFlags.Instance);
                    if (null != propinfo && propinfo.CanWrite)
                    {
                        List<JProperty> proplista = item.Properties().ToList();
                        foreach (JProperty jprop in proplista)
                        {
                            if (jprop.Name.ToString().ToUpper() == prop.Name.ToUpper())
                            {
                                object data = jprop.Value.ToString();
                                propinfo.SetValue(myitem, data);
                                break;
                            }
                        }
                    }
                }
                returnlista.Add(myitem);
            }
            return returnlista;
        }
        /// <summary>
        /// Create Object by the string name of class / struct
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private static object CreateByTypeName(string typeName)
        {
            // scan for the class type
            var type = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from t in assembly.GetTypes()
                        where t.Name == typeName  // you could use the t.FullName as well
                        select t).FirstOrDefault();

            if (type == null)
                throw new InvalidOperationException("Type not found");

            return Activator.CreateInstance(type);
        }
        /// <summary>
        /// Create dictionary
        /// </summary>
        /// <param name="lan"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Convert the date with dictionary
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private string date_convert(string date, Dictionary<string, string> dictionary)
        {
            date = date.ToUpper();
            foreach (var dictkeys in dictionary)
            {
                if (date.Contains(dictkeys.Key))
                    date = date.Replace(dictkeys.Key, dictkeys.Value);
            }
            return date;
        }
    }
}
