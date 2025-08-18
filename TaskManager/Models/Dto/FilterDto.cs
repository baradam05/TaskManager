using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using TaskManager.Models.DbClasses;

namespace TaskManager.Models.Dto
{
    public class FilterDto
    {
        public int status { get; set; } // -1: late; 0: all; 1: in progress; 2: finished
        public List<int> assignedToId { get; set; } = new();
        public DateTime? StartFrom { get; set; }
        public DateTime? StartTo { get; set; }
        public DateTime? EndFrom { get; set; }
        public DateTime? EndTo { get; set; }
        public DateTime? AssignedAtFrom { get; set; }
        public DateTime? AssignedAtTo { get; set; }
    }

}
