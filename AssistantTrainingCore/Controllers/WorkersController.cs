using AssistantTrainingCore.Data;
using AssistantTrainingCore.Models;
using AssistantTrainingCore.Repositories;
using AssistantTrainingCore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;

namespace AssistantTrainingCore.Controllers
{
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
            GetIndexData();
            return View();
        }

        // GET: Workers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var worker = db.Workers.Find(id);

            var workerGroup = new WorkerViewModel();
            var groups = workerRepository.GetAllGroups();

            workerGroup.AvailableGroups = groups;

            workerGroup.FirstMidName = worker.FirstMidName;
            workerGroup.LastName = worker.LastName;
            workerGroup.ID = worker.ID;
            workerGroup.Tag = worker.Tag;
            workerGroup.IsSuspend = worker.IsSuspend;
            workerGroup.PostingGroups = new PostingGroup() { GroupIDs = db.GroupWorkers.Where(x => x.WorkerId.Equals(worker.ID)).Select(x => x.GroupId.ToString()).ToArray() };
            workerGroup.SelectedGroups = db.GroupWorkers.Where(x => x.WorkerId.Equals(worker.ID)).Select(x => x.Group).ToList();

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

            return View(workerGroup);
        }

        // POST: Workers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WorkerViewModel workerGroup)
        {
            if (ModelState.IsValid)
            {
                var worker = new Worker();
                worker.TimeOfCreation = DateTime.Now;
                worker.TimeOfModification = DateTime.Now;
                worker.LastName = workerGroup.LastName;
                worker.FirstMidName = workerGroup.FirstMidName;
                worker.Tag = workerGroup.Tag;

                db.Workers.Add(worker);
                db.SaveChanges();

                if (workerGroup.PostingGroups != null && workerGroup.PostingGroups.GroupIDs != null && workerGroup.PostingGroups.GroupIDs.Count() > 0)
                {
                    foreach (var item in workerGroup.PostingGroups.GroupIDs)
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

            return View(workerGroup);
        }

        // GET: Workers/Edit/5
        public ActionResult Edit(int? id)
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
                PostingGroups = new PostingGroup() { GroupIDs = db.GroupWorkers.Where(x => x.WorkerId.Equals(worker.ID)).Select(x => x.GroupId.ToString()).ToArray() },
                SelectedGroups = db.GroupWorkers.Where(x => x.WorkerId.Equals(worker.ID)).Select(x => x.Group).ToList()
            };

            if (worker == null)
            {
                return NotFound();
            }
            return View(workerGroup);
        }

        // POST: Workers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WorkerViewModel workerGroup)
        {
            if (ModelState.IsValid)
            {
                var worker = new Worker();
                worker.ID = workerGroup.ID;
                worker.LastName = workerGroup.LastName;
                worker.FirstMidName = workerGroup.FirstMidName;
                worker.Tag = workerGroup.Tag;
                worker.TimeOfModification = DateTime.Now;
                worker.IsSuspend = workerGroup.IsSuspend;

                db.Workers.Attach(worker);
                db.Entry(worker).Property(X => X.FirstMidName).IsModified = true;
                db.Entry(worker).Property(X => X.LastName).IsModified = true;
                db.Entry(worker).Property(X => X.Tag).IsModified = true;
                db.Entry(worker).Property(X => X.TimeOfModification).IsModified = true;
                db.Entry(worker).Property(X => X.IsSuspend).IsModified = true;

                if (workerGroup.PostingGroups != null && workerGroup.PostingGroups.GroupIDs != null && workerGroup.PostingGroups.GroupIDs.Count() > 0)
                {
                    var wGroups = db.GroupWorkers.Where(w => w.WorkerId == workerGroup.ID).ToList();

                    foreach (var item in workerGroup.PostingGroups.GroupIDs)
                    {
                        if ((wGroups.Where(x => x.WorkerId.Equals(workerGroup.ID) && x.GroupId.Equals(Int32.Parse(item))).FirstOrDefault() == null) || wGroups.Count() == 0)
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
                        if (!workerGroup.PostingGroups.GroupIDs.Contains(item.GroupId.ToString()))
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
                return RedirectToAction("Index", "Workers", new { area = "" });
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
              select new { FullName = w.FirstMidName + " " + w.LastName, IsSuspend = w.IsSuspend, GroupName = g.GroupName }
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
                workerGroup.SelectedIds = db.GroupWorkers.Where(x => x.WorkerId.Equals(item.ID)).Select(x => x.GroupId.ToString()).ToArray();
                workerGroup.WorkerGroups = groups;
                workerGroup.Items = groups.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GroupName
                });

                lstWorkerGroups.Add(workerGroup);
            }
            return View("Index", lstWorkerGroups.Where(x => x.FullName.ToUpper().Contains(srchtermWorkerByWorker.ToUpper())));
        }

        public ActionResult SearchByGroup(string srchtermWorkerByGroup)
        {
            var allWorker = db.Workers.ToList();
            var groups = workerRepository.GetAllGroups();

            List<WorkerGroupViewModel> lstWorkerGroups = new List<WorkerGroupViewModel>();

            foreach (var item in allWorker)
            {
                var workerGroup = new WorkerGroupViewModel();

                var lstGroups = db.GroupWorkers.Where(x => x.WorkerId.Equals(item.ID)).Select(x => x.Group.GroupName).ToList();
                if (lstGroups.FirstOrDefault(stringToCheck => stringToCheck.ToUpper().Contains(srchtermWorkerByGroup.ToUpper())) != null)
                {
                    workerGroup.ID = item.ID;
                    workerGroup.FirstMidName = item.FirstMidName;
                    workerGroup.LastName = item.LastName;
                    workerGroup.FullName = item.LastName + " " + item.FirstMidName;
                    workerGroup.Tag = item.Tag;
                    workerGroup.SelectedIds = db.GroupWorkers.Where(x => x.WorkerId.Equals(item.ID)).Select(x => x.GroupId.ToString()).ToArray();
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
                var workerGroup = new WorkerGroupViewModel();

                workerGroup.ID = item.ID;
                workerGroup.FirstMidName = item.FirstMidName;
                workerGroup.LastName = item.LastName;
                workerGroup.FullName = item.LastName + " " + item.FirstMidName;
                workerGroup.Tag = item.Tag;
                workerGroup.SelectedIds = db.GroupWorkers.Where(x => x.WorkerId.Equals(item.ID)).Select(x => x.GroupId.ToString()).ToArray();
                workerGroup.WorkerGroups = groups;
                workerGroup.Items = groups.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GroupName
                });
                workerGroup.IsSuspend = item.IsSuspend;
                workerGroup.IsSuspendDesc = item.IsSuspend == true ? "Tak" : "Nie";

                RowNo += 1;
                workerGroup.RowNo = RowNo;

                lstWorkerGroups.Add(workerGroup);
            }

            return lstWorkerGroups.OrderBy(x => x.LastName).ToList();
        }
    }
}