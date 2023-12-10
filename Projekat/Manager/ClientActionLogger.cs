using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class ClientActionLogger
    {
        string logFile = @"..\..\..\Manager\ActionLog.txt";
        public void LogAction(string user, string action, bool check)
        {
            string text;
            if (check)
            {
                text = $"{DateTime.Now} \t User {user} performed {action} action successfully!";
            }
            else
            {
                text = $"{DateTime.Now} \t User {user} performed {action} action unsuccessfully!";
            }

            StreamWriter sw = File.AppendText(logFile);
            sw.WriteLine(text);
            sw.Close();

        }
    }
}
