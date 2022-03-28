using AssistantTrainingCore.Data;
using AssistantTrainingCore.Models;
using AssistantTrainingCore.Repositories;
using AssistantTrainingCore.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;

namespace AssistantTrainingCore.Controllers
{
    [Authorize]
    public class WorkersController : Controller
    {
        private ApplicationDbContext db;
        private readonly IWorkerRepository workerRepository;

        public WorkersController(ApplicationDbContext dbContext, IWorkerRepository workerRepository)
        {
            db = dbContext;
            this.workerRepository = workerRepository;
        }

        public IActionResult Index()
        {
            //GetIndexData();
            return View();
        }

        public ActionResult SelectWorkers([DataSourceRequest] DataSourceRequest request)
        {
            return Json(GetIndexData().ToDataSourceResult(request));
        }

        /// <summary>
        /// https://demos.telerik.com/aspnet-core/spreadstreamprocessing
        /// </summary>
        /// <param name="rowsCount"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GenerateDocument(int rowsCount, string fileType)
        {
            var workers =
                 (from w in db.Workers
                  join gw in db.GroupWorkers on w.ID equals gw.WorkerId
                  join g in db.Groups on gw.GroupId equals g.ID
                  orderby w.LastName, w.FirstMidName
                  select new { FullName = w.FirstMidName + " " + w.LastName, w.IsSuspend, g.GroupName }
                 ).ToList();

            try
            {
                MemoryStream content = new MemoryStream();
                using (ExcelPackage package = new ExcelPackage(content))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets.Add("workers");
                    ws.Cells["A1"].LoadFromCollection(workers, true);

                    package.Save();
                }

                content.Position = 0;

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public ActionResult Excel_Export_Read([DataSourceRequest] DataSourceRequest request)
        {
            var workers =
                (from w in db.Workers
                    join gw in db.GroupWorkers on w.ID equals gw.WorkerId
                    join g in db.Groups on gw.GroupId equals g.ID
                    orderby w.LastName, w.FirstMidName
                    select new {FullName = w.FirstMidName + " " + w.LastName, w.IsSuspend, g.GroupName}
                ).ToList();

            return Json(workers.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        // GET: Workers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var worker = db.Workers.Find(id);
            var groups = workerRepository.GetAllGroups();

            var workerGroup = new WorkerViewModel
            {
                AvailableGroups = groups,

                FirstMidName = worker.FirstMidName,
                LastName = worker.LastName,
                ID = worker.ID,
                Tag = worker.Tag,
                IsSuspend = worker.IsSuspend,
                PostingGroups = new PostingGroup()
                {
                    GroupIDs = db.GroupWorkers.Where(x => x.WorkerId.Equals(worker.ID))
                        .Select(x => x.GroupId.ToString()).ToArray()
                },
                SelectedGroups = db.GroupWorkers.Where(x => x.WorkerId.Equals(worker.ID)).Select(x => x.Group).ToList()
            };

            if (worker == null)
            {
                return NotFound();
            }

            return View(workerGroup);
        }

        // GET: Workers/Create
        public ActionResult Create()
        {
            var workerGroup = new WorkerViewModel();
            var groups = workerRepository.GetAllGroups();
            workerGroup.AvailableGroups = groups;
            workerGroup.CheckBoxGroupValue = new string[] {""};

            workerGroup.ItemsList = groups.Select(x => new InputGroupItemModel
            {
                Value = x.ID.ToString(),
                Label = x.GroupName,
                Enabled = true,
                Encoded = false,
                CssClass = "test",
                HtmlAttributes = new Dictionary<string, object>() {{"data-custom", "custom"}}
            }).ToList<IInputGroupItem>();
            
            return View(workerGroup);
        }

        // POST: Workers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(WorkerViewModel workerGroup)
        {
            if (workerGroup == null || string.IsNullOrWhiteSpace(workerGroup.LastName))
            {
                return BadRequest();
            }

            var worker = new Worker
            {
                TimeOfCreation = DateTime.Now,
                TimeOfModification = DateTime.Now,
                LastName = workerGroup.LastName,
                FirstMidName = workerGroup.FirstMidName,
                Tag = workerGroup.Tag
            };

            db.Workers.Add(worker);
            db.SaveChanges();

            if (workerGroup.CheckBoxGroupValue != null && workerGroup.CheckBoxGroupValue.Length > 0)
            {
                foreach (var item in workerGroup.CheckBoxGroupValue)
                {
                    var groupInstructions = new GroupWorker()
                    {
                        WorkerId = worker.ID,
                        TimeOfCreation = DateTime.Now,
                        TimeOfModification = DateTime.Now,
                        GroupId = Int32.Parse(item)
                    };
                    db.GroupWorkers.Add(groupInstructions);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        // GET: Workers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var worker = db.Workers.Find(id);

            if (worker == null)
            {
                return NotFound();
            }

            var groups = workerRepository.GetAllGroups();

            var workerGroup = new WorkerViewModel
            {
                AvailableGroups = groups,
                FirstMidName = worker.FirstMidName,
                LastName = worker.LastName,
                ID = worker.ID,
                Tag = worker.Tag,
                IsSuspend = worker.IsSuspend,
                PostingGroups = new PostingGroup()
                {
                    GroupIDs = db.GroupWorkers.Where(x => x.WorkerId.Equals(worker.ID))
                        .Select(x => x.GroupId.ToString()).ToArray()
                },
                SelectedGroups = db.GroupWorkers.Where(x => x.WorkerId.Equals(worker.ID)).Select(x => x.Group).ToList()
            };

            workerGroup.ItemsList = groups.Select(x => new InputGroupItemModel
            {
                Value = x.ID.ToString(),
                Label = x.GroupName,
                Enabled = true,
                Encoded = false,
                CssClass = "test",
                HtmlAttributes = new Dictionary<string, object>() {{"data-custom", "custom"}}
            }).ToList<IInputGroupItem>();

            workerGroup.CheckBoxGroupValue = db.GroupWorkers.Where(x => x.WorkerId.Equals(worker.ID))
                .Select(x => x.Group.ID.ToString()).ToArray();

            return View(workerGroup);
        }

        // POST: Workers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WorkerViewModel workerGroup)
        {
            if (workerGroup.ID > 0)
            {
                var worker = new Worker
                {
                    ID = workerGroup.ID,
                    LastName = workerGroup.LastName,
                    FirstMidName = workerGroup.FirstMidName,
                    Tag = workerGroup.Tag,
                    TimeOfModification = DateTime.Now,
                    IsSuspend = workerGroup.IsSuspend
                };

                db.Workers.Attach(worker);
                db.Entry(worker).Property(X => X.FirstMidName).IsModified = true;
                db.Entry(worker).Property(X => X.LastName).IsModified = true;
                db.Entry(worker).Property(X => X.Tag).IsModified = true;
                db.Entry(worker).Property(X => X.TimeOfModification).IsModified = true;
                db.Entry(worker).Property(X => X.IsSuspend).IsModified = true;

                if (workerGroup.CheckBoxGroupValue != null && workerGroup.CheckBoxGroupValue.Any())
                {
                    var wGroups = db.GroupWorkers.Where(w => w.WorkerId == workerGroup.ID).ToList();

                    foreach (var item in workerGroup.CheckBoxGroupValue)
                    {
                        if ((wGroups.FirstOrDefault(x => x.WorkerId.Equals(workerGroup.ID) && x.GroupId.Equals(int.Parse(item))) == null) || !wGroups.Any())
                        {
                            var groupInstructions = new GroupWorker()
                            {
                                WorkerId = worker.ID,
                                TimeOfCreation = DateTime.Now,
                                TimeOfModification = DateTime.Now,
                                GroupId = Int32.Parse(item)
                            };
                            db.GroupWorkers.Add(groupInstructions);
                            db.SaveChanges();
                        }
                    }

                    foreach (var item in wGroups)
                    {
                        if (!workerGroup.CheckBoxGroupValue.Contains(item.GroupId.ToString()))
                        {
                            db.GroupWorkers.Remove(item);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    //Usuń wszystkie

                    var wGroups = db.GroupWorkers.Where(w => w.WorkerId == workerGroup.ID);

                    foreach (var g in wGroups)
                    {
                        db.GroupWorkers.Remove(g);
                    }

                    db.SaveChanges();
                }

                db.SaveChanges();
                return RedirectToAction("Index", "Workers", new {area = ""});
            }

            return View(workerGroup);
        }

        // GET: Workers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var worker = db.Workers.Find(id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // POST: Workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Worker worker = db.Workers.Find(id);
            db.Workers.Remove(worker);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Excel()
        {
            //var workers = db.Workers.Select(w=>new { FullName = w.FirstMidName + " " + w.LastName, IsSuspend = w.IsSuspend}).ToList();

            var workers =
                (from w in db.Workers
                    join gw in db.GroupWorkers on w.ID equals gw.WorkerId
                    join g in db.Groups on gw.GroupId equals g.ID
                    orderby w.LastName, w.FirstMidName
                    select new
                    {
                        FullName = w.FirstMidName + " " + w.LastName, IsSuspend = w.IsSuspend, GroupName = g.GroupName
                    }
                ).ToList();

            try
            {
                MemoryStream content = new MemoryStream();
                using (ExcelPackage package = new ExcelPackage(content))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets.Add("Workers");
                    ws.Cells["A1"].LoadFromCollection(workers, true);

                    package.Save();
                }

                content.Position = 0;

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

            return RedirectToAction("Index");
        }

        public ActionResult SearchByWorker(string srchtermWorkerByWorker)
        {
            var allWorker = db.Workers.ToList();
            var groups = workerRepository.GetAllGroups();

            List<WorkerGroupViewModel> lstWorkerGroups = new List<WorkerGroupViewModel>();

            foreach (var item in allWorker)
            {
                var workerGroup = new WorkerGroupViewModel();

                workerGroup.ID = item.ID;
                workerGroup.FirstMidName = item.FirstMidName;
                workerGroup.LastName = item.LastName;
                workerGroup.FullName = item.LastName + " " + item.FirstMidName;
                workerGroup.Tag = item.Tag;
                workerGroup.SelectedIds = db.GroupWorkers.Where(x => x.WorkerId.Equals(item.ID))
                    .Select(x => x.GroupId.ToString()).ToArray();
                workerGroup.WorkerGroups = groups;
                workerGroup.Items = groups.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GroupName
                });

                lstWorkerGroups.Add(workerGroup);
            }

            return View("Index",
                lstWorkerGroups.Where(x => x.FullName.ToUpper().Contains(srchtermWorkerByWorker.ToUpper())));
        }

        public ActionResult SearchByGroup(string srchtermWorkerByGroup)
        {
            var allWorker = db.Workers.ToList();
            var groups = workerRepository.GetAllGroups();

            List<WorkerGroupViewModel> lstWorkerGroups = new List<WorkerGroupViewModel>();

            foreach (var item in allWorker)
            {
                var workerGroup = new WorkerGroupViewModel();

                var lstGroups = db.GroupWorkers.Where(x => x.WorkerId.Equals(item.ID)).Select(x => x.Group.GroupName)
                    .ToList();
                if (lstGroups.FirstOrDefault(stringToCheck =>
                        stringToCheck.ToUpper().Contains(srchtermWorkerByGroup.ToUpper())) != null)
                {
                    workerGroup.ID = item.ID;
                    workerGroup.FirstMidName = item.FirstMidName;
                    workerGroup.LastName = item.LastName;
                    workerGroup.FullName = item.LastName + " " + item.FirstMidName;
                    workerGroup.Tag = item.Tag;
                    workerGroup.SelectedIds = db.GroupWorkers.Where(x => x.WorkerId.Equals(item.ID))
                        .Select(x => x.GroupId.ToString()).ToArray();
                    workerGroup.WorkerGroups = groups;
                    workerGroup.Items = groups.Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.GroupName
                    });

                    lstWorkerGroups.Add(workerGroup);
                }
            }

            return View("Index", lstWorkerGroups);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        private List<WorkerGroupViewModel> GetIndexData()
        {
            var allWorker = db.Workers.ToList();
            var groups = workerRepository.GetAllGroups();

            List<WorkerGroupViewModel> lstWorkerGroups = new List<WorkerGroupViewModel>();
            int RowNo = 0;
            foreach (var item in allWorker)
            {
                RowNo += 1;

                var workerGroup = new WorkerGroupViewModel
                {
                    RowNo = RowNo,
                    ID = item.ID,
                    FirstMidName = item.FirstMidName,
                    LastName = item.LastName,
                    FullName = item.LastName + " " + item.FirstMidName,
                    Tag = item.Tag,
                    SelectedIds = db.GroupWorkers.Where(x => x.WorkerId.Equals(item.ID))
                        .Select(x => x.GroupId.ToString()).ToArray(),
                    WorkerGroups = groups,
                    Items = groups.Select(x => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = x.GroupName
                    }),
                    IsSuspend = item.IsSuspend,
                    IsSuspendDesc = item.IsSuspend == true ? "Tak" : "Nie"
                };

                workerGroup.GrupsInString = String.Join("\n",
                    workerGroup.Items.Where(x => workerGroup.SelectedIds.Contains(x.Value)).Select(x => x.Text)
                        .ToArray());

                lstWorkerGroups.Add(workerGroup);
            }

            return lstWorkerGroups.OrderBy(x => x.LastName).ToList();
        }
    }
}