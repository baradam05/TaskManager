using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.Classes;
using TaskManager.Models.DbClasses;
using TaskManager.Models.Dto;

namespace TaskManager.Components
{
    public class TableViewComponent : ViewComponent
    {
        MyContext context = new();
        public IViewComponentResult Invoke(List<Assignment> assignments, Table tableType = Table.Default, TableSortDto tableSort = null)
        {
            int loggedInId = int.Parse(this.HttpContext.Session.GetString("login"));
            List<Assigned> assignedToLogged = this.context.Assigned.Where(x => x.AccountId == loggedInId).ToList();
            List<TableRowDto> tableRows = new();

            for (int i = 0; i < assignments.Count; i++)
            {
                TableRowDto tableRow;
                Account assignedByUser = this.context.Account.FirstOrDefault(x => x.AccountId == assignments[i].AssignedBy);
                List<SubAssignment> subs = this.context.SubAssignment.Where(x => x.AssignmentId == assignments[i].AssignmentId).ToList();

                if (tableType == Table.Edit)
                {
                    List<Account> assignees = this.context.Account.Where(acc => this.context.Assigned.Any(asg => asg.AccountId == acc.AccountId && asg.AssignmentId == assignments[i].AssignmentId)).ToList();
                    tableRow = new TableRowDto(assignments[i], subs, assignedByUser.Username, assignees.Select(x => x.Username).ToList());
                }
                else
                    tableRow = new TableRowDto(assignments[i], subs, assignedByUser.Username);

                tableRows.Add(tableRow);
            }           

            if (tableSort != null && tableSort.sortDir != sortDirection.None)
                tableRows = Sort(tableRows, tableSort);



            this.ViewBag.TableRows = tableRows;
            this.ViewBag.TableType = tableType;
            this.ViewBag.TableSort = tableSort;



            if (tableType == Table.Edit)
                return View("Edit");
            else if (tableType == Table.Done)
                return View("Done");

            return View();
        }

        public List<TableRowDto> Sort(List<TableRowDto> tableRows, TableSortDto tableSort)
        {
            if (tableRows == null || tableSort == null || tableSort.sortCol == null)
                return tableRows;

            bool asc = tableSort.sortDir == sortDirection.Ascending;

            switch (tableSort.sortCol)
            {
                case "assignedDate":
                    return asc
                        ? tableRows.OrderBy(x => x.assignment.AssignedDate).ToList()
                        : tableRows.OrderByDescending(x => x.assignment.AssignedDate).ToList();

                case "startDate":
                    return asc
                        ? tableRows.OrderBy(x => x.assignment.StartDate).ToList()
                        : tableRows.OrderByDescending(x => x.assignment.StartDate).ToList();

                case "endDate":
                    return asc
                        ? tableRows.OrderBy(x => x.assignment.EndDate).ToList()
                        : tableRows.OrderByDescending(x => x.assignment.EndDate).ToList();

                case "finishedDate":
                    return asc
                        ? tableRows.OrderBy(x => x.assignment.Finished).ToList()
                        : tableRows.OrderByDescending(x => x.assignment.Finished).ToList();

                case "finished":
                    return asc
                        ? tableRows.OrderBy(x => x.subAssignments.Count(sa => sa.Finished)).ToList()
                        : tableRows.OrderByDescending(x => x.subAssignments.Count(sa => sa.Finished)).ToList();


                case "assignedTo":
                    return asc
                        ? tableRows
                            .OrderBy(x => x.assignees?.Count ?? 0)
                            .ThenBy(x => x.assignees is null ? "" : string.Join(", ", x.assignees))
                            .ToList()
                        : tableRows
                            .OrderByDescending(x => x.assignees?.Count ?? 0)
                            .ThenByDescending(x => x.assignees is null ? "" : string.Join(", ", x.assignees))
                            .ToList();

                case "assignedBy":
                    return asc
                        ? tableRows.OrderBy(x => x.assignedBy ?? string.Empty).ToList()
                        : tableRows.OrderByDescending(x => x.assignedBy ?? string.Empty).ToList();

                default:
                    return tableRows;
            }
        }


    }
    public enum Table
    {
        Default = 0,
        Edit = 1,
        Done = 2
    }
}