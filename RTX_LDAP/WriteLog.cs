using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace RTX_LDAP
{
    class WriteLog
    {
        public class LogManager
        {
            // 写日志
            public static void WriteLog(string logFile, string msg)
            {
                string LogPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\logs\";
                try
                {
                    System.IO.StreamWriter sw = System.IO.File.AppendText(
                        LogPath + logFile + " " +
                        DateTime.Now.ToString("yyyyMMdd") + ".Log"
                        );
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + msg);
                    sw.Close();
                }
                catch
                { }
            }

            /// <summary>
            /// 写日志
            /// </summary>
            public static void WriteLog(LogFile logFile, string msg)
            {
                WriteLog(logFile.ToString(), msg);
            }
        }

        /// <summary>
        /// 日志类型
        /// </summary>
        public enum LogFile
        {
            Trace,
            Error,
        }
    }
}
