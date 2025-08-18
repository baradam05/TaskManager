using TaskManager.Models.DbClasses;

namespace TaskManager.Models.Dto
{
    public class TableRowDto
    {
        public Assignment assignment { get; set; }
        public List<SubAssignment> subAssignments { get; set; }
        public List<string> assignees { get; set; }
        public string assignedBy { get; set; }

        public TableRowDto(Assignment assignment, List<SubAssignment> subAssignments, string assignedBy)
        {
            this.assignment = assignment;
            this.subAssignments = subAssignments;
            this.assignedBy = assignedBy;
        }

        public TableRowDto(Assignment assignment, List<SubAssignment> subAssignments, string assignedBy, List<string> assignees)
        {
            this.assignment = assignment;
            this.subAssignments = subAssignments;
            this.assignedBy = assignedBy;
            this.assignees = assignees;
        }
    }
}
