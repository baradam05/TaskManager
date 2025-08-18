using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.DbClasses
{
    public class SubAssignment
    {
        [Key]
        public int SubAssignmentId { get; set; }
        public int AssignmentId { get; set; }
        public string Assignment { get; set; }
        public bool Finished { get; set; }
        public SubAssignment() {}

        public SubAssignment(int AssignmentId, string Assignment)
        {
            this.AssignmentId = AssignmentId;
            this.Assignment = Assignment;
            this.Finished = false;
        }
    }
}
