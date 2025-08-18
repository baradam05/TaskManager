using Microsoft.EntityFrameworkCore;

namespace TaskManager.Models.DbClasses
{
    [Keyless]
    public class Assigned
    {
        public int AccountId { get; set; }
        public int AssignmentId { get; set; }

        public Assigned()
        {
            
        }

        public Assigned(int AccountId, int AssignmentId)
        {
            this.AccountId = AccountId;
            this.AssignmentId = AssignmentId;
        }
    }
}
