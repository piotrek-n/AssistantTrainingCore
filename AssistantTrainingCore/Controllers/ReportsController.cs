using AssistantTrainingCore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AssistantTrainingCore.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        //private IGridMvcHelper gridMvcHelper;
        private const string GRID_PARTIAL_PATH = "~/Views/Reports/_ReportMainGrid.cshtml";

        public ReportsController()
        {
            //this.gridMvcHelper = new GridMvcHelper();
        }

        // GET: Reports
        public ActionResult Index()
        {
            var appReports = new ReportsIndexData();
            List<SelectListItem> lstItems = new List<SelectListItem>();
            lstItems.Add(new SelectListItem()
            {
                Value = "0",
                Text = "Wyczyść"
            });
            lstItems.Add(new SelectListItem()
            {
                Value = "1",
                Text = "SZKOLENIA"
            });
            lstItems.Add(new SelectListItem()
            {
                Value = "2",
                Text = "INSTRUKCJE"
            });
            lstItems.Add(new SelectListItem()
            {
                Value = "3",
                Text = "PRACOWNICY"
            });

            appReports.Items = lstItems;

            return View(appReports);
        }
    }
}