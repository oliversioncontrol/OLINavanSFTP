
namespace OLINavanSFTP.Models
{
    public partial class JobInfo
    {
        public string Project { get; set; } = "";
        public string JobDescription { get; set; } = "";
        public int JobNumber { get; set; }
        public bool IsDefault { get; set; }
        public string Approvers { get; set; } = "";
    }
}