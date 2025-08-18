using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Components
{
    public class DividerViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string name, bool isExpandable = false, string expandableName = "none")
        {
            if (isExpandable && expandableName == "none")
                throw new Exception();

            this.ViewBag.Name = name;
            this.ViewBag.IsExpandable = isExpandable;
            this.ViewBag.ExpandableName = expandableName;
            return View();
        }

    }
}
