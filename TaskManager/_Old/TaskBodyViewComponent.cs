using Microsoft.AspNetCore.Mvc;
using TaskManager.Components;
using TaskManager.Models.Classes;
using TaskManager.Models.DbClasses;

namespace TaskManager._Old
{
    public class TaskBodyViewComponent : ViewComponent
    {
        private MyContext context = new MyContext();

        public IViewComponentResult Invoke(int TaskId, int counter, Table tableType = Table.Default)
        {
            Assignment? task = context.Assignment.Find(TaskId);
            if (task == null)
                throw new Exception("Id not found.");

            List<Assigned> assigned = context.Assigned.Where(x => x.AssignmentId == task.AssignmentId).ToList();
            List<string> assignedTo = new();
            foreach (Assigned assignedPart in assigned)
            {
                assignedTo.Add(context.Account.Find(assignedPart.AccountId).Username);
            }
            string assignedBy = context.Account.Find(task.AssignedBy).Username;
            int assignedSubTasks = context.SubAssignment.Where(x => x.AssignmentId == TaskId).Count();
            int finishedSubTasks = context.SubAssignment.Where(x => x.AssignmentId == TaskId && x.Finished).Count();
            List<SubAssignment> subTasks = context.SubAssignment.Where(x => x.AssignmentId == TaskId).ToList();
            bool IsLate = task.EndDate != null && DateTime.Now > task.EndDate ? true : false;

            ViewBag.Task = task;
            ViewBag.assignedTo = assignedTo;
            ViewBag.assignedBy = assignedBy;
            ViewBag.assignedSubTasks = assignedSubTasks;
            ViewBag.finishedSubTasks = finishedSubTasks;
            ViewBag.subTasks = subTasks;
            ViewBag.Counter = counter;
            ViewBag.IsLate = IsLate;
            ViewBag.tableType = tableType;
            return View();
        }
    }
}
