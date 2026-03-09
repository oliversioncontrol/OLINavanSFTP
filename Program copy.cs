// // See https://aka.ms/new-console-template for more information
// using Microsoft.Extensions.Configuration;
// using OLINavanSFTP.Data;
// using OLINavanSFTP.Handlers;
// using Renci.SshNet;

// bool dev = false;
// // bool dev = true;

// string localFileRoot = "C:\\ScheduledTasks\\NavanSFTP\\";
// if (dev)
// {
//     localFileRoot = "./";
// }

// DateTime startTime = DateTime.Now;

// IConfiguration config = new ConfigurationBuilder()
//     .AddJsonFile("appsettings.json")
//     .Build();

// DataContextDapper dapper = new DataContextDapper(config);
// LoggingHandler logging = new LoggingHandler(dapper, dev);

// logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
// logging.WriteLog("Preparing SFTP Credentials");

// string sshKeyLocation = localFileRoot + "navan_ssh";

// // string sftpClientURL = "sftp://prod-integration.navan.com";
// int port = 22;
// string sshPW = "Navan456&";
// string username = "onlocationinc";

// string sftpClientURL = "prod-integration.navan.com";
// string fileRoot = "/data/prod/projects";

// // if (args?[0]?.ToLower() == "test")
// // {
// //     sftpClientURL = "test-integration.navan.com";
// //     fileRoot = "/data/test/projects";
// // }

// var privateKey = new PrivateKeyFile(sshKeyLocation, sshPW);
// var keyFiles = new[] { privateKey };

// // var methods = new List<AuthenticationMethod>(){
// //     new PrivateKeyAuthenticationMethod(username, keyFiles)
// // };

// PrivateKeyAuthenticationMethod authMethod = new PrivateKeyAuthenticationMethod(username, keyFiles);

// // var connection = new ConnectionInfo(sftpClientURL, port, username, methods.ToArray());
// var connection = new ConnectionInfo(sftpClientURL, port, username, authMethod);




// logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
// logging.WriteLog("Getting Job Info");

// IEnumerable<string> jobNumbers = dapper.LoadData<string>("EXEC dbo.spJobNumbersForNavan_Get");

// string jobNumberCSV = "Projects\nField\n" + String.Join("\n", jobNumbers);
// // string jobNumberCSV = "Projects\nField\n'" + String.Join("'\n'", jobNumbers) + "'";
// // string jobNumberCSV = "Projects\nField\n\"" + String.Join("\"\n\"", jobNumbers) + "\"";

// Upload(jobNumberCSV, connection, localFileRoot + "Projects.csv");



// void Upload(string fileToUpload, ConnectionInfo connection, string fileName)
// {
//     //TestFile

//     logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
//     logging.WriteLog("Generating Latest File");
//     File.WriteAllText(fileName, fileToUpload);
//     // Console.WriteLine(fileName);
//     // Console.WriteLine(fileToUpload);
//     try
//     {
//         using (var sftp = new SftpClient(connection))
//         {
//             //Upload Logic
//             MemoryStream memStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(fileToUpload));

//             logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
//             logging.WriteLog("Uploading File to Navan SFTP");

//             sftp.Connect();

//             sftp.ChangeDirectory(fileRoot);
//             sftp.UploadFile(memStream, fileName, true);

//             sftp.Disconnect();

//             logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
//             logging.WriteLog("Upload Completed Successfully");
//         }
//     }
//     catch (Exception e)
//     {
//         logging.WriteLog((DateTime.Now - startTime).TotalSeconds.ToString());
//         logging.WriteLog("Failed To Upload!");
//         Console.WriteLine(e.Message);
//     }
// }