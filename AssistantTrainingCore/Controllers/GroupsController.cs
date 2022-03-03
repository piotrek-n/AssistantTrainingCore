using AssistantTrainingCore.Data;
using AssistantTrainingCore.Models;
using AssistantTrainingCore.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace AssistantTrainingCore.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;

        public GroupsController(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public ActionResult SelectGroups([DataSourceRequest] DataSourceRequest request)
        {
            var result =
                db.Groups.Select(x => x).AsEnumerable().Select((x, index) => new GroupViewModel()
                {
                    RowNo = index + 1,
                    GroupName = x.GroupName,
                    ID = x.ID,
                    //Instructions = x.Instructions,
                    Tag = x.Tag,
                    TimeOfCreation = x.TimeOfCreation,
                    TimeOfModification = x.TimeOfModification
                }).ToList();

            return Json(result.ToDataSourceResult(request));
        }

        public ActionResult SelectGroupDetails([DataSourceRequest] DataSourceRequest request, int id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }
            var groupDetails = new GroupDetails
            {
                ID = group.ID,
                GroupName = group.GroupName
            };


            var items = db.Instructions.GroupBy(c => c.Number)
                               .Select(g => new
                               {
                                   Key = g.Key,
                                   ID = db.Instructions.FirstOrDefault(x => x.Number == g.Key && x.Version == g.Max(x => x.Version)).ID,
                                   maxVersion = g.Max(x => x.Version)
                               }).Select(x => x.ID).ToList();

            var lst = (from ig in db.InstructionGroups
                       join i in db.Instructions on ig.InstructionId equals i.ID
                       where ig.GroupId == id && items.Contains(i.ID)
                       select i).ToList();

            if (lst != null && lst.Count > 0)
            {
                groupDetails.Instructions = new List<InstructionInGroup>();
                foreach (var item in lst)
                {
                    groupDetails.Instructions.Add(new InstructionInGroup() { Name = item.Name, Number = item.Number, Version = item.Version, ID = item.ID });
                }
            }

            return Json(groupDetails.Instructions.ToDataSourceResult(request));
        }

        public ActionResult Excel_Export_Read([DataSourceRequest] DataSourceRequest request)
        {
            var result =
                db.Groups.Select(x => x).AsEnumerable().Select((x, index) => new GroupViewModel()
                {
                    RowNo = index + 1,
                    GroupName = x.GroupName,
                    ID = x.ID,
                    Tag = x.Tag,
                    TimeOfCreation = x.TimeOfCreation,
                    TimeOfModification = x.TimeOfModification
                }).ToList();
            return Json(result.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        // GET: Groups
        public ActionResult Index()
        {
            return View(new List<GroupViewModel>());
        }

        // GET: Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }
            var gd = new GroupDetails
            {
                ID = group.ID,
                GroupName = group.GroupName
            };


            var items = db.Instructions.GroupBy(c => c.Number)
                               .Select(g => new
                               {
                                   Key = g.Key,
                                   ID = db.Instructions.FirstOrDefault(x => x.Number == g.Key && x.Version == g.Max(x => x.Version)).ID,
                                   maxVersion = g.Max(x => x.Version)
                               }).Select(x => x.ID).ToList();

            var lst = (from ig in db.InstructionGroups
                       join i in db.Instructions on ig.InstructionId equals i.ID
                       where ig.GroupId == id && items.Contains(i.ID)
                       select i).ToList();

            if (lst != null && lst.Count > 0)
            {
                gd.Instructions = new List<InstructionInGroup>();
                foreach (var item in lst)
                {
                    gd.Instructions.Add(new InstructionInGroup() { Name = item.Name, Number = item.Number, Version = item.Version, ID = item.ID });
                }
            }

            return View(gd);
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("ID,GroupName,TimeOfCreation,TimeOfModification,Tag")] Group group)
        {
            if (ModelState.IsValid)
            {
                group.TimeOfCreation = DateTime.Now;
                group.TimeOfModification = DateTime.Now;

                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("ID,GroupName,TimeOfCreation,TimeOfModification,Tag")] Group group)
        {
            if (ModelState.IsValid)
            {
                group.TimeOfModification = DateTime.Now;

                db.Groups.Attach(group);
                db.Entry(group).Property(X => X.GroupName).IsModified = true;
                db.Entry(group).Property(X => X.Tag).IsModified = true;
                db.Entry(group).Property(X => X.TimeOfModification).IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SearchByGroup(string srchtermWorkerByGroup)
        {
            var result =
                db.Groups.Where(x => x.GroupName.ToUpper().Contains(srchtermWorkerByGroup.ToUpper())).Select(x => x).AsEnumerable().Select((x, index) => new GroupViewModel()
                {
                    RowNo = index + 1,
                    GroupName = x.GroupName,
                    ID = x.ID,
                    //Instructions = x.Instructions,
                    Tag = x.Tag,
                    TimeOfCreation = x.TimeOfCreation,
                    TimeOfModification = x.TimeOfModification
                }).ToList();

            return View("Index", result);
        }

        public ActionResult Excel()
        {
            var groups = db.Groups.Select(x => new { Name = x.GroupName }).ToList();

            try
            {
                MemoryStream content = new MemoryStream();
                using (ExcelPackage package = new ExcelPackage(content))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets.Add("Groups");
                    ws.Cells["A1"].LoadFromCollection(groups, true);

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

        public ActionResult TrainedUsers(int id)
        {
            var workers = (from t in db.Trainings
                           join w in db.Workers on t.WorkerId equals w.ID
                           where t.InstructionId.Equals(id) && !t.DateOfTraining.ToString().Equals("1900-01-01 00:00:00.000") //> new DateTime(2000, 1, 1)
                           select w).ToList();

            return View(workers);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}