
namespace OLINavanSFTP.Models
{
    public partial class EmailToSend
    {
        public string EmailTo { get; set; }
        public string EmailBody { get; set; }
        public string EmailSubject { get; set; }

        public EmailToSend()
        {
            if (EmailTo == null)
            {
                EmailTo = "";
            }
            if (EmailBody == null)
            {
                EmailBody = "";
            }
            if (EmailSubject == null)
            {
                EmailSubject = "";
            }
        }
    }
}