using System.ComponentModel.DataAnnotations;
using TaskManager.Models.DbClasses;

namespace TaskManager.Models.Dto
{
    public class AssignmentDto
    {
        public DateTime? StartDate { get; set; }
        public int? StartHours { get; set; }
        public int? StartMinutes { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int EndHours { get; set; }
        [Required]
        public int EndMinutes { get; set; }
        public List<int> AssignedToIds { get; set; } = new();
        public List<SubAsignmentDto> SubAsignments { get; set; } = new();

        public int IsEditing { get; set; } //-1: not editing, number: id of the assignment being edited

        public AssignmentDto()
        {
            EndDate = DateTime.Now;
            EndHours = DateTime.Now.Hour + 1;
            EndMinutes = 0;
            IsEditing = -1;
        }

        public AssignmentDto(Assignment assignment, List<SubAssignment> subAssignments, List<Account> assignToAccounts)
        {
            StartDate = assignment.StartDate;
            StartHours = assignment.StartDate == null ? null : assignment.StartDate.Value.Hour;
            StartMinutes = assignment.StartDate == null ? null : assignment.StartDate.Value.Minute;

            EndDate = assignment.EndDate;
            EndHours= assignment.EndDate.Hour;
            EndMinutes = assignment.EndDate.Minute;

            foreach (SubAssignment subAssignment in subAssignments) 
                SubAsignments.Add(new SubAsignmentDto() { SubAssignment = subAssignment.Assignment, id = subAssignment.SubAssignmentId, finished = subAssignment.Finished });

            foreach (Account account in assignToAccounts)
                AssignedToIds.Add((int)account.AccountId);

            IsEditing = assignment.AssignmentId;
        }
    }
}
