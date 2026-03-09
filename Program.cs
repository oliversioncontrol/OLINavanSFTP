// See https://aka.ms/new-console-template for more information
// using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using OLINavanSFTP.Data;
using OLINavanSFTP.Handlers;
using OLINavanSFTP.Models;
using Renci.SshNet;

string EscapeDoubleQuotes(string input)
{
    // return input;
    // return input.Replace("\"", "");
    return input.Replace("\"", "\"\"");
}

DateTime startTime = DateTime.Now;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

bool devRun = config.GetSection("IsDev").Get<bool>();
// bool dev = true;

string localFileRoot = "C:\\ScheduledTasks\\NavanSFTP\\";
if (devRun)
{
    localFileRoot = "./";
}

DataContextDapper dapper = new DataContextDapper(config);
LoggingHandler logging = new LoggingHandler(dapper, devRun);

logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
logging.WriteLog("Preparing SFTP Credentials");

string sshKeyLocation = localFileRoot + "navan_ssh";

// string sftpClientURL = "sftp://prod-integration.navan.com";
int port = 22;
string sshPW = "Navan456&";
string username = "onlocationinc";

string sftpClientURL = "prod-integration.navan.com";
string fileRoot = "/data/prod/projects";

// if (args?[0]?.ToLower() == "test")
// {
//     sftpClientURL = "test-integration.navan.com";
//     fileRoot = "/data/test/projects";
// }

var privateKey = new PrivateKeyFile(sshKeyLocation, sshPW);
var keyFiles = new[] { privateKey };

// var methods = new List<AuthenticationMethod>(){
//     new PrivateKeyAuthenticationMethod(username, keyFiles)
// };

PrivateKeyAuthenticationMethod authMethod = new PrivateKeyAuthenticationMethod(username, keyFiles);

// var connection = new ConnectionInfo(sftpClientURL, port, username, methods.ToArray());
var connection = new ConnectionInfo(sftpClientURL, port, username, authMethod);




logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
logging.WriteLog("Getting Job Info");

IEnumerable<JobInfo> jobNumbers = dapper.LoadData<JobInfo>("EXEC dbo.spJobNumbersForNavan_Get");
// string csvHeader = "Projects\n";

// string jobNumberCSV = csvHeader;
// foreach (JobInfo job in jobNumbers)
// {
//     jobNumberCSV += $"{EscapeDoubleQuotes(job.Project)}\n";
// }

// string csvHeader = "Name,ID,Default,Approver(s)\n";

// string jobNumberCSV = csvHeader;
// foreach (JobInfo job in jobNumbers)
// {
//     jobNumberCSV += $"\"{EscapeDoubleQuotes(job.Project)}\",\"{job.JobNumber}\",\"{job.IsDefault}\",\"{job.Approvers}\"\n";
// }

// // string csvHeader = "Projects,Name,ID,Default,Approver(s),Follow Up Responses\n";
string csvHeader = "Custom Field Name,Name,ID,Default,Approver(s)\n";

string jobNumberCSV = csvHeader;
foreach (JobInfo job in jobNumbers)
{
    jobNumberCSV += $"\"projects\",\"{EscapeDoubleQuotes(job.Project)}\",\"{job.JobNumber}\",\"{job.IsDefault}\",\"{job.Approvers}\"\n";
}

// string jobNumberCSV = "Projects\nField\n" + String.Join("\n", jobNumbers);
// string jobNumberCSV = "Projects\nField\n'" + String.Join("'\n'", jobNumbers) + "'";
// string jobNumberCSV = "Projects\nField\n\"" + String.Join("\"\n\"", jobNumbers) + "\"";

Upload(jobNumberCSV, connection, localFileRoot + "Projects.csv");



void Upload(string fileToUpload, ConnectionInfo connection, string fileName)
{
    //TestFile

    logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
    logging.WriteLog("Generating Latest File");
    File.WriteAllText(fileName, fileToUpload);
    // Console.WriteLine(fileName);
    // Console.WriteLine(fileToUpload);
    try
    {
        using (var sftp = new SftpClient(connection))
        {
            //Upload Logic
            MemoryStream memStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(fileToUpload));

            logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
            logging.WriteLog("Uploading File to Navan SFTP");

            sftp.Connect();

            sftp.ChangeDirectory(fileRoot);
            sftp.UploadFile(memStream, fileName, true);

            sftp.Disconnect();

            logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
            logging.WriteLog("Upload Completed Successfully");
        }
    }
    catch (Exception e)
    {
        logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
        logging.WriteLog("Failed To Upload!");
        Console.WriteLine(e.Message);
    }
}