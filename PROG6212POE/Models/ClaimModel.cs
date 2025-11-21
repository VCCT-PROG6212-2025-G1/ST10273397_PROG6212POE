using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212POE.Models
{
    public class ClaimModel
    {
        [Key]
        public int ClaimId { get; set; }

        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int HoursWorked { get; set; }

        [Required]
        public float HourlyRate { get; set; }

        [NotMapped]
        public float TotalAmount => HoursWorked * HourlyRate;

        public string ClaimStatus { get; set; } = "Pending";

        public string AdditionalNotes { get; set; }

        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        public string SuppDocName { get; set; } = ""; // Original file name (for display)
        public string SuppDocPath { get; set; } = ""; // Stored file name on server (for download)

        public ClaimModel() 
        { }

        public ClaimModel(int id, int uid, string uname, string title, int hrsWorked, float hrlyRate, string status, string addNotes, string suppDocName, string suppDocPath) 
        { 
            ClaimId = id;
            UserId = uid;
            UserName = uname;
            Title = title;
            HoursWorked = hrsWorked;
            HourlyRate = hrlyRate;
            ClaimStatus = status;
            AdditionalNotes = addNotes;
            SuppDocName = suppDocName;
            SuppDocPath = suppDocPath;
        }
    }
}
