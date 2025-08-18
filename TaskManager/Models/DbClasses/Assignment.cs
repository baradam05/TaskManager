using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.DbClasses
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }
        public DateTime AssignedDate { get; set; }
        public int AssignedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? Finished { get; set; }
    }
}
