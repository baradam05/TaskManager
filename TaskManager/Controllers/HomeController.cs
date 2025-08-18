using Microsoft.AspNetCore.Mvc;
using TaskManager.Components;
using TaskManager.Models.Classes;
using TaskManager.Models.DbClasses;
using TaskManager.Models.Dto;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext context = new MyContext();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Secured]
        public IActionResult Index(Table? orderTable = null, string sortCol = null, sortDirection? sortDir = null)
        {
            int loggedId = int.Parse(this.HttpContext.Session.GetString("login"));
            List<Assigned> assigned = this.context.Assigned.Where(x => x.AccountId == loggedId).ToList();
            List<Assignment> assignments = new();
            List<Assignment> finished = new();

            foreach (Assigned assignedItem in assigned)
            {
                int x = assignedItem.AssignmentId;
                Assignment assignment = this.context.Assignment.Find(assignedItem.AssignmentId);
                if(assignment.Finished == null)
                    assignments.Add(assignment);
                else
                    finished.Add(assignment);
            }

            TableSortDto? tableSort = orderTable == null ? new TableSortDto() : new TableSortDto((Table)orderTable,sortCol,(sortDirection)sortDir);

            this.ViewBag.Assignments = assignments;
            this.ViewBag.Finished = finished;
            this.ViewBag.Role = this.context.Account.Find(loggedId).LeaderId == null ? "User" : "Leader";

            this.ViewBag.TableSort = tableSort;
            return View();
        }

        [Secured]
        public IActionResult SubAssignmentChange(int assignmentId,string isEditing, List<int> completedSubIds)
        {            
            List<SubAssignment> subAssignments = context.SubAssignment.Where(x => assignmentId == x.AssignmentId).ToList();

            foreach (SubAssignment subAssignment in subAssignments)
            {
                if (completedSubIds.Contains(subAssignment.SubAssignmentId) && !subAssignment.Finished)
                    subAssignment.Finished = true;
                else if (!completedSubIds.Contains(subAssignment.SubAssignmentId) && subAssignment.Finished)
                    subAssignment.Finished = false;
            }
            Assignment assignment = this.context.Assignment.Find(assignmentId);
            assignment.Finished = subAssignments.Count == completedSubIds.Count ? DateTime.Now : null;
        
            context.SaveChanges();

            if(bool.Parse(isEditing))
                return RedirectToAction("Manage", "Assignment");
            return RedirectToAction("Index");

        }
    }
}
