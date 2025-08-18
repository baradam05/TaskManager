using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.Classes;
using TaskManager.Models.DbClasses;

namespace TaskManager._Old
{
    public class SubTaskViewComponent : ViewComponent
    {
        MyContext context = new MyContext();
        public IViewComponentResult Invoke(int SubTaskId)
        {
            SubAssignment SubAssignment = context.SubAssignment.Find(SubTaskId);
            ViewBag.SubAssignment = SubAssignment;
            return View();
        }
    }
}
 