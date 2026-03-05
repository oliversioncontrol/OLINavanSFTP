using System.Text;
using OLINavanSFTP.Data;
using OLINavanSFTP.Models;
using System.Text.Json;

namespace OLINavanSFTP.Handlers
{
    public class LoggingHandler
    {
        private readonly string apiToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxNTkiLCJ1c2VyIjoiZHRyaXBvZGkiLCJ1c2VyVHlwZSI6IkUiLCJpc0FkbWluIjoidHJ1ZSIsIm5iZiI6MTY2NjEyNDgzMSwiZXhwIjoxOTczOTA0NDMxLCJpYXQiOjE2NjYxMjQ4MzF9.D7k-g5Ql50_1cs3KtdDQq2bOqt_-svHiKLKxDk3rwvf_USdCqREHBP-BxjzszRyYjYwQHynFk0cNc3HZc2AARg";
        private readonly DataContextDapper _dapper;
        private readonly string _rootFolder = "C:\\ScheduledTasks\\NavanSFTP\\logs\\";
        public LoggingHandler(DataContextDapper dapper, bool dev)
        {
            _dapper = dapper;
            if (dev)
            {
                _rootFolder = "logs/";
            }
        }

        public void WriteLog(string logText, bool failure = false)
        {
            try
            {
                Console.WriteLine(logText);
                string dateStamp = DateTime.Now.ToString("yyyy-MM-dd");//DateTime.Now.ToShortDateString();

                string logLocation = _rootFolder + dateStamp + ".txt";
                //Console.WriteLine(logLocation);
                // Console.WriteLine(dateStamp);
                if (!System.IO.File.Exists(logLocation))
                {
                    System.IO.File.WriteAllText(logLocation, logText);
                }
                else
                {
                    using StreamWriter openFile = new(logLocation, append: true);

                    openFile.WriteLine(logText + "\n");

                    openFile.Close();
                }

                if (failure)
                {
                    SendFailureEmail(logText);
                }
            }
            catch
            {
                SendFailureEmail(logText);
            }
        }

        public void SendFailureEmail(string logText)
        {
            string emailUrl = "https://onlocationapi.azurewebsites.net/api/Email";

            EmailToSend emailToSend = new EmailToSend()
            {
                EmailBody = "Error in Navan SFTP Load: " + logText,
                EmailSubject = "Error in Navan SFTP Load",
                // EmailTo = "dtripodi@abeerconsulting.com;abeer@abeerconsulting.com"
                EmailTo = "dtripodi@abeerconsulting.com"
            };

            string jsonContent = JsonSerializer.Serialize(emailToSend);

            HttpClient client = new HttpClient();
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = client.PostAsync(new Uri(emailUrl), content).Result;

            Console.WriteLine(JsonSerializer.Serialize(response));
        }

    }
}