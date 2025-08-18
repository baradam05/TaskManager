using Microsoft.AspNetCore.Mvc;
using TaskManager.Components;
using TaskManager.Models.Classes;
using TaskManager.Models.DbClasses;
using TaskManager.Models.Dto;

namespace TaskManager.Controllers
{
    public class AssignmentController : Controller
    {
        MyContext context = new();

#region Assignment create/edit
        [Secured]
        [Admin]
        [HttpGet]
        public IActionResult Index(int? assignmentId = null)
        {
            Assignment assignment = new();
            List<SubAssignment> subAssignments = new List<SubAssignment>();
            List<Account> accounts = new List<Account>();
            if (assignmentId != null) 
            { 
                assignment = context.Assignment.Find(assignmentId);
                subAssignments = context.SubAssignment.Where(x => x.AssignmentId == assignmentId).ToList();
                foreach(Assigned assigned in this.context.Assigned.Where(x => x.AssignmentId == assignmentId).ToList())
                {
                    accounts.Add(this.context.Account.Find(assigned.AccountId));
                }
            }
            AssignmentDto assignmentDto = assignmentId != null ? new AssignmentDto(assignment, subAssignments, accounts) : new AssignmentDto();

            this.ViewBag.Users = this.context.Account.Where(x => x.LeaderId == int.Parse(this.HttpContext.Session.GetString("login"))).ToList();
            this.ViewBag.NoSubAssignments = false;
            this.ViewBag.NoAssignedUsers = false;
            this.ViewBag.IsNew = assignmentId == null ? true : false;
            return View(assignmentDto);
        }

        [Secured]
        [Admin]
        [HttpPost]
        public IActionResult Index(AssignmentDto assignmentDto)
        {
            bool throwError = false;

            if (assignmentDto.SubAsignments.Count == 0 || assignmentDto.AssignedToIds.Count == 0)
                throwError = true;

            if (throwError)
            {
                this.ViewBag.NoSubAssignments = assignmentDto.SubAsignments.Count == 0;
                this.ViewBag.NoAssignedUsers = assignmentDto.AssignedToIds.Count == 0;
                this.ViewBag.Users = this.context.Account.Where(x => x.LeaderId == int.Parse(this.HttpContext.Session.GetString("login"))).ToList();
                return View(assignmentDto);
            }

            Assignment assignment; 

            #region Assignment

            if (assignmentDto.IsEditing == -1)
            {
                assignment = new Assignment
                {
                    AssignedBy = int.Parse(this.HttpContext.Session.GetString("login")),
                    AssignedDate = DateTime.Now,
                    StartDate = assignmentDto.StartDate?.Date.AddHours(assignmentDto.StartHours ?? 0).AddMinutes(assignmentDto.StartMinutes ?? 0),
                    EndDate = assignmentDto.EndDate.Date.AddHours(assignmentDto.EndHours).AddMinutes(assignmentDto.EndMinutes)
                }; 

                this.context.Assignment.Add(assignment);
                this.context.SaveChanges();
            }
            else
            {
                assignment = this.context.Assignment.FirstOrDefault(a => a.AssignmentId == assignmentDto.IsEditing);

                assignment.StartDate = assignmentDto.StartDate?.Date.AddHours(assignmentDto.StartHours ?? 0).AddMinutes(assignmentDto.StartMinutes ?? 0);
                assignment.EndDate = assignmentDto.EndDate.Date.AddHours(assignmentDto.EndHours).AddMinutes(assignmentDto.EndMinutes);
            }

            #endregion

            #region AssignedTo

            List<Assigned> existingAssigned = this.context.Assigned.Where(x => x.AssignmentId == assignment.AssignmentId).ToList();

            foreach (int accountId in assignmentDto.AssignedToIds)
            {
                if (!existingAssigned.Any(x => x.AccountId == accountId))
                {
                    this.context.Assigned.Add(new Assigned
                    {
                        AccountId = accountId,
                        AssignmentId = assignment.AssignmentId
                    });
                }
            }

            foreach (Assigned assigned in existingAssigned)
            {
                if (!assignmentDto.AssignedToIds.Contains(assigned.AccountId))
                    this.context.Assigned.Remove(assigned);
            }

            #endregion

            #region SubAssignments

            List<SubAssignment> existingSubs = this.context.SubAssignment.Where(x => x.AssignmentId == assignment.AssignmentId).ToList();
            List<int> submittedIds = assignmentDto.SubAsignments.Where(x => x.id != null && x.id != 0).Select(x => x.id.Value).ToList();

            foreach (var existing in existingSubs)
            {
                if (!submittedIds.Contains(existing.SubAssignmentId))
                    this.context.SubAssignment.Remove(existing);
            }

            foreach (SubAsignmentDto subDto in assignmentDto.SubAsignments)
            {
                if (subDto.id == null || subDto.id == 0)
                {
                    SubAssignment newSub = new SubAssignment(
                        assignment.AssignmentId,
                        subDto.SubAssignment
                    );
                    newSub.Finished = subDto.finished;
                    this.context.SubAssignment.Add(newSub);
                }
                else
                {
                    SubAssignment existing = existingSubs.FirstOrDefault(x => x.SubAssignmentId == subDto.id.Value);
                    existing.Assignment = subDto.SubAssignment;
                    existing.Finished = subDto.finished;
                }
            }

            #endregion

            this.context.SaveChanges();
            return RedirectToAction("Manage");
        }
        #endregion

#region Manage assignments
        [Secured]
        [Admin]
        [HttpGet]
        public IActionResult Manage(Table? orderTable = null, string sortCol = null, sortDirection? sortDir = null)
        {
            int loggedInId = int.Parse(this.HttpContext.Session.GetString("login"));
            List<Account> accounts = this.context.Account.Where(x => x.LeaderId == loggedInId).ToList();
            List<Assignment> assignmentList = this.context.Assignment.Where(x => x.AssignedBy == int.Parse(this.HttpContext.Session.GetString("login"))).ToList();

            TableSortDto? tableSort = orderTable == null ? null : new TableSortDto((Table)orderTable, sortCol, (sortDirection)sortDir);

            this.ViewBag.Assignment = assignmentList;
            this.ViewBag.Accounts = accounts;

            this.ViewBag.TableSort = tableSort;
            return View();
        }
        [Secured]
        [Admin]
        [HttpPost]
        public IActionResult Manage(FilterDto filter)
        {
            int loggedInId = int.Parse(this.HttpContext.Session.GetString("login"));

            List<Account> accounts = this.context.Account.Where(x => x.LeaderId == loggedInId).ToList();
            List<Assignment> assignmentList = this.context.Assignment.Where(x => x.AssignedBy == loggedInId).ToList();
            List<Assigned> assignedList = this.context.Assigned.ToList();

            //status
            if(filter.status != 0)
            {
                if (filter.status == -1)
                {
                    assignmentList = assignmentList.Where(x => x.EndDate < DateTime.Now && x.Finished == null).ToList();
                }
                else if (filter.status == 1)
                {
                    assignmentList = assignmentList.Where(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now && x.Finished == null).ToList();
                }
                else if (filter.status == 2)
                {
                    assignmentList = assignmentList.Where(x => x.Finished != null).ToList();
                }
            }

            //assigned to
            if (filter.assignedToId.Count > 0)
            {
                assignmentList = assignmentList.Where(x => assignedList.Any(y => y.AssignmentId == x.AssignmentId && filter.assignedToId.Contains(y.AccountId))).ToList();
            }

            //start date
            if (filter.StartFrom != null)
            {
                assignmentList = assignmentList.Where(x => x.StartDate >= filter.StartFrom).ToList();
            }
            if (filter.StartTo != null)
            {
                assignmentList = assignmentList.Where(x => x.StartDate <= filter.StartTo).ToList();
            }

            //end date
            if (filter.EndFrom != null)
            {
                assignmentList = assignmentList.Where(x => x.EndDate >= filter.EndFrom).ToList();
            }
            if (filter.EndTo != null)
            {
                assignmentList = assignmentList.Where(x => x.EndDate <= filter.EndTo).ToList();
            }

            //assigned at
            if (filter.AssignedAtFrom != null)
            {
                assignmentList = assignmentList.Where(x => x.AssignedDate >= filter.AssignedAtFrom).ToList();
            }
            if (filter.AssignedAtTo != null)
            {
                assignmentList = assignmentList.Where(x => x.AssignedDate <= filter.AssignedAtTo).ToList();
            }

            this.ViewBag.Assignment = assignmentList;
            this.ViewBag.Accounts = accounts;

            return View(filter);
        }
        #endregion

        [Secured]
        [Admin]
        public IActionResult Edit(int id)
        {
            return RedirectToAction("Index",new { assignmentId = id, isEditing = true });
        }

        [Secured]
        [Admin]
        public IActionResult Delete(int id)
        {
            var deleteAssignment = this.context.Assignment.Find(id);

            this.context.Assignment.Remove(deleteAssignment);
            this.context.SaveChanges();

            return RedirectToAction("Manage");
        }

    }
}
