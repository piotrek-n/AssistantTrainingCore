using AssistantTrainingCore.Data;
using AssistantTrainingCore.Models;
using AssistantTrainingCore.Repositories;
using AssistantTrainingCore.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;

namespace AssistantTrainingCore.Controllers
{
    [Authorize]
    public class InstructionsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWorkerRepository workerRepository;
        private readonly UserManager<IdentityUser> userManager;

        public InstructionsController(ApplicationDbContext dbContext, IWorkerRepository workerRepository, UserManager<IdentityUser> userManager)
        {
            db = dbContext;
            this.workerRepository = workerRepository;
            this.userManager = userManager;
        }

        public ActionResult SelectWorkersByGroup([DataSourceRequest] DataSourceRequest request, string groupName)
        {
            var itemExt = request as DataSourceRequestExt;
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
            var r = lstWorkerGroups.Where(wg => wg.WorkerGroups.Any(g => g.GroupName == groupName) && wg.IsSuspend == false).GroupBy(p => p.ID).Select(g => g.FirstOrDefault()).ToList(); ;
            return Json(r.ToDataSourceResult(request));
        }

        public ActionResult Select([DataSourceRequest] DataSourceRequest request)
        {
            var newInstructions = db.Instructions.GroupBy(c => c.Number)
                                .Select(g => new
                                {
                                    Key = g.Key,
                                    ID = db.Instructions.FirstOrDefault(x => x.Number == g.Key && x.Version == g.Max(x => x.Version)).ID,
                                    maxVersion = g.Max(x => x.Version)
                                }).ToList();

            //var newInstructions =
            //    (from i in db.Instructions
            //     group i by i.Number
            //        into groupedI
            //     let maxVersion = groupedI.Max(v => v.Version)
            //     select new InstructionLatestVersion
            //     {
            //         Key = groupedI.Key,
            //         maxVersion = maxVersion
            //         ,ID = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).ID
            //     }).AsEnumerable();


            var test = db.Instructions.ToList();

            var allInstructions =
                db.Instructions.ToList().Where(x => newInstructions.Any(ni => ni.ID == x.ID)).OrderByDescending(ins => ins.TimeOfCreation).ToList();
            var groups = workerRepository.GetAllGroups();

            var lstInstructionGroups = new List<InstructionIndexData>();
            var RowNo = 0;
            foreach (var item in allInstructions)
            {
                var instructioGroup = new InstructionIndexData();

                RowNo += 1;
                instructioGroup.RowNo = RowNo; //+ ((request.Page - 1) * request.PageSize);
                instructioGroup.ID = item.ID;
                instructioGroup.Name = item.Name;
                instructioGroup.Number = item.Number;
                instructioGroup.Version = item.Version;
                instructioGroup.UserName = item.CreatedByUserId;
                if (item.CreatedByUserId != null && db.Users.Find(item.CreatedByUserId) != null)
                    instructioGroup.UserName = db.Users.Find(item.CreatedByUserId).UserName;
                instructioGroup.TimeOfCreation = item.TimeOfCreation.ToShortDateString();

                var lstGroupIds = (from InstructionGroups in db.InstructionGroups
                                   where
                                       InstructionGroups.GroupId != null &&
                                       InstructionGroups.InstructionId == item.ID
                                   select new
                                   {
                                       val = InstructionGroups.GroupId ?? 0
                                   }).Select(x => x.val.ToString()).ToList();
                instructioGroup.SelectedIds = lstGroupIds.ToArray();

                instructioGroup.Items = groups.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GroupName
                });

                instructioGroup.GrupsInString = String.Join("\n", instructioGroup.Items.Where(x => instructioGroup.SelectedIds.Contains(x.Value)).Select(x => x.Text).ToArray());

                lstInstructionGroups.Add(instructioGroup);
            }
            var result = lstInstructionGroups.ToDataSourceResult(request);
            //result.Total = total;

            return Json(result);
        }

        // GET: Instructions
        public ActionResult Index()
        {
            return View(new List<InstructionIndexData>());
        }

        // GET: Instructions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) { return BadRequest(); }
            var instruction = db.Instructions.Find(id);

            if (instruction == null)
            {
                return NotFound();
            }
            ViewData["IdInstruction"] = id;
            var instructionGroupViewModel = new InstructionDetailsData();
            var idsGroups = new List<int>();

            idsGroups = (from InstructionGroups in db.InstructionGroups
                         where
                             InstructionGroups.GroupId != null &&
                             InstructionGroups.InstructionId == id
                         select new
                         {
                             val = InstructionGroups.GroupId ?? 0
                         }).Select(x => x.val).ToList();

            var groups = workerRepository.GetGroupsById(idsGroups);

            instructionGroupViewModel.Name = instruction.Name;
            instructionGroupViewModel.Version = instruction.Version;
            instructionGroupViewModel.Groups = groups;
            instructionGroupViewModel.GroupWithNumbers = groups.Select((x, i) => new GroupViewModel
            {
                GroupName = x.GroupName,
                RowNo = i + 1,
                ID = x.ID,
                //Instructions = x.Instructions,
                Tag = x.Tag,
                TimeOfCreation = x.TimeOfCreation,
                TimeOfModification = x.TimeOfModification
            });

            instructionGroupViewModel.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });

            var lst = (from w in db.Workers
                       join wg in db.GroupWorkers on w.ID equals wg.WorkerId
                       join gi in db.InstructionGroups on wg.GroupId equals gi.GroupId
                       join i in db.Instructions on gi.InstructionId equals i.ID
                       join t in db.Trainings
                           on new { InstructionId = i.ID, wg.WorkerId }
                           equals new { t.InstructionId, t.WorkerId } into t_join
                       from t in t_join.DefaultIfEmpty()
                       where i.ID == id
                       select new InstructionVsTrainingData
                       {
                           WorkerLastName = w.LastName,
                           WorkerFirstMidName = w.FirstMidName,
                           WorkerFullName = w.LastName + " " + w.FirstMidName,
                           WorkerIsSuspendedDesc = (w.IsSuspend == true ? "Tak" : "Nie"),
                           InstructionName = i.Name,
                           GroupId = gi.GroupId,
                           InstructionVersion = i.Version,
                           InstructionNumber = i.Number,
                           DateOfTraining = (DateTime?)t.DateOfTraining,
                           TrainingName = t.TrainingName.Number
                       }).Select(x => x).AsEnumerable().GroupBy(p => new { p.WorkerFullName, p.InstructionNumber }).Select(x => x.First()).Select((w, i) =>
                       new InstructionVsTrainingData
                       {
                           WorkerLastName = w.WorkerLastName,
                           WorkerFirstMidName = w.WorkerFirstMidName,
                           WorkerFullName = w.WorkerFullName,
                           WorkerIsSuspendedDesc = w.WorkerIsSuspendedDesc,
                           InstructionName = w.InstructionName,
                           GroupId = w.GroupId,
                           InstructionVersion = w.InstructionVersion,
                           InstructionNumber = w.InstructionNumber,
                           DateOfTraining = w.DateOfTraining,
                           TrainingName = w.TrainingName,
                           RowNo = i + 1
                       }).ToList();

            instructionGroupViewModel.instructionVsTrainingList = lst.ToList();
            if (instructionGroupViewModel == null)
            {
                return NotFound();
            }
            return View(instructionGroupViewModel);
        }

        // GET: Instructions/Create
        public ActionResult Create()
        {
            var instructionGroup = new InstructionIndexData();
            var groups = workerRepository.GetAllGroups();

            instructionGroup.CheckBoxGroupValue = new string[] { "" };

            instructionGroup.ItemsList = groups.Select(x => new InputGroupItemModel
            {
                Value = x.ID.ToString(),
                Label = x.GroupName,
                Enabled = true,
                Encoded = false,
                CssClass = "test",
                HtmlAttributes = new Dictionary<string, object>() { { "data-custom", "custom" } }
            }).ToList<IInputGroupItem>();

            return View(instructionGroup);
        }

        // POST: Instructions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InstructionIndexData instructionGroupViewModel)
        {
            if (ModelState.IsValid || instructionGroupViewModel.SelectedIds == null)
            {
                //db.InstructionGroupViewModels.Add(instructionGroupViewModel);
                Instruction instruction = new Instruction
                {
                    Number = instructionGroupViewModel.Number,
                    Name = instructionGroupViewModel.Name,
                    Version = instructionGroupViewModel.Version,
                    Reminder = instructionGroupViewModel.Reminder,
                    TimeOfCreation = DateTime.Now,
                    TimeOfModification = DateTime.Now
                };
                var currentUserId = userManager.GetUserId(User); // User.Identity.GetUserId();
                instruction.CreatedByUserId = currentUserId;
                //instruction.GroupId = Int32.Parse(instructionGroupViewModel.SelectedId);
                db.Instructions.Add(instruction);
                db.SaveChanges();

                if (!string.IsNullOrEmpty(instructionGroupViewModel.NumberOfTraining))
                {
                    var tn = new TrainingName();
                    tn.Name = string.Empty;
                    tn.Number = instructionGroupViewModel.NumberOfTraining;
                    db.TrainingNames.Add(tn);
                    db.SaveChanges();

                    var tg = new TrainingGroup();
                    tg.TrainingNameId = tn.ID;
                    tg.InstructionId = instruction.ID;
                    tg.TimeOfCreation = DateTime.Now;
                    tg.TimeOfModification = DateTime.Now;

                    db.TrainingGroups.Add(tg);
                    db.SaveChanges();
                }

                if (instructionGroupViewModel.CheckBoxGroupValue != null && instructionGroupViewModel.CheckBoxGroupValue.Count() > 0)
                    foreach (var item in instructionGroupViewModel.CheckBoxGroupValue)
                    {
                        var groupInstructions = new InstructionGroup
                        {
                            InstructionId = instruction.ID,
                            TimeOfCreation = DateTime.Now,
                            TimeOfModification = DateTime.Now,
                            GroupId = int.Parse(item)
                        };
                        db.InstructionGroups.Add(groupInstructions);
                        db.SaveChanges();
                    }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(instructionGroupViewModel);
        }

        // GET: Instructions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var instruction = db.Instructions.Find(id);
            //var trainingGroups = db.TrainingGroups.Include("TrainingName").Where(x => x.InstructionId.Equals(id));

            if (instruction == null)
            {
                return NotFound();
            }

            var instructionGroupViewModel = new InstructionEditData();
            var groups = workerRepository.GetAllGroups();

            instructionGroupViewModel.Name = instruction.Name;
            instructionGroupViewModel.Version = instruction.Version;
            instructionGroupViewModel.Number = instruction.Number;

            instructionGroupViewModel.SelectedIds =
                (from InstructionGroups in db.InstructionGroups
                 where
                     InstructionGroups.GroupId != null &&
                     InstructionGroups.InstructionId == id
                 select new
                 {
                     val = InstructionGroups.GroupId ?? 0
                 }).Select(x => x.val.ToString()).ToList().ToArray();

            instructionGroupViewModel.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });

            return View(instructionGroupViewModel);
        }

        // POST: Instructions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("ID,Name,Number,Version,SelectedIds")]
            InstructionEditData instructionGroupViewModel)
        {
            if (ModelState.IsValid)
            {
                var instruction = new Instruction();
                instruction.ID = instructionGroupViewModel.ID;
                instruction.TimeOfModification = DateTime.Now;
                instruction.Name = instructionGroupViewModel.Name;
                instruction.Version = instructionGroupViewModel.Version;
                //instruction.GroupId = Int32.Parse(instructionGroupViewModel.SelectedId);

                db.Instructions.Attach(instruction);
                db.Entry(instruction).Property(X => X.Name).IsModified = true;
                db.Entry(instruction).Property(X => X.Version).IsModified = true;
                //db.Entry(instruction).Property(X => X.GroupId).IsModified = true;
                db.Entry(instruction).Property(X => X.Tag).IsModified = true;
                db.Entry(instruction).Property(X => X.TimeOfModification).IsModified = true;
                db.SaveChanges();

                if (instructionGroupViewModel.SelectedIds != null && instructionGroupViewModel.SelectedIds.Count() > 0)
                {
                    var wGroups = db.InstructionGroups.Where(w => w.InstructionId == instructionGroupViewModel.ID)
                        .ToList();

                    foreach (var item in instructionGroupViewModel.SelectedIds)
                        if (wGroups.Where(x =>
                                x.InstructionId.Equals(instructionGroupViewModel.ID) &&
                                x.GroupId.Equals(int.Parse(item))).FirstOrDefault() == null || wGroups.Count() == 0)
                        {
                            var groupInstructions = new InstructionGroup
                            {
                                InstructionId = instruction.ID,
                                TimeOfCreation = DateTime.Now,
                                TimeOfModification = DateTime.Now,
                                GroupId = int.Parse(item)
                            };
                            db.InstructionGroups.Add(groupInstructions);
                            db.SaveChanges();
                        }

                    foreach (var item in wGroups)
                        if (!instructionGroupViewModel.SelectedIds.Contains(item.GroupId.ToString()))
                        {
                            db.InstructionGroups.Remove(item);
                            db.SaveChanges();
                        }
                }
                else
                {
                    //Usuń wszystkie

                    var wGroups = db.InstructionGroups.Where(w => w.InstructionId == instructionGroupViewModel.ID);

                    foreach (var g in wGroups) db.InstructionGroups.Remove(g);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(instructionGroupViewModel);
        }

        // GET: Instructions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) { return BadRequest(); }
            var instruction = db.Instructions.Find(id);

            if (instruction == null)
            {
                return NotFound();
            }

            var instructionGroupViewModel = new InstructionIndexData();
            var groups = workerRepository.GetAllGroups();

            instructionGroupViewModel.Name = instruction.Name;
            instructionGroupViewModel.Version = instruction.Version;
            //instructionGroupViewModel.SelectedId = instruction.GroupId.ToString();

            instructionGroupViewModel.Items = groups.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.GroupName
            });
            if (instructionGroupViewModel == null) { return NotFound(); }
            return View(instructionGroupViewModel);
        }

        // POST: Instructions/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //var instructionGroup = db.InstructionGroups.Where(x => x.InstructionId == id).ToList();
            //db.InstructionGroups.RemoveRange(instructionGroup);
            //var instruction = db.Instructions.Find(id);
            //db.Instructions.Remove(instruction);

            //db.SaveChanges();
            //return RedirectToAction("Index");

            var lstInstructions = db.Instructions.Where(x => x.ID == id).ToList();
            var number = lstInstructions.First().Number;
            var lstInstructions2 = db.Instructions.Where(x => x.Number == number).ToList();

            foreach (var item in lstInstructions2)
            {
                var instructionGroup = db.InstructionGroups.Where(x => x.InstructionId == item.ID).ToList();
                db.InstructionGroups.RemoveRange(instructionGroup);

                var instruction = db.Instructions.Find(item.ID);
                db.Instructions.Remove(instruction);

                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult AddNewVersion(int? id, string version, string training)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var instruction = db.Instructions.Find(id);

            if (instruction == null)
            {
                return NotFound();
            }
            var presetVersion = instruction.Version;
            var newVersion = int.Parse(version);

            if (instruction.Version == int.Parse(version) || newVersion != presetVersion + 1)
            {
                return BadRequest();
            }
            var newInstruction = new Instruction();
            newInstruction.Name = instruction.Name;
            newInstruction.Version = int.Parse(version);
            newInstruction.TimeOfCreation = DateTime.Now;
            newInstruction.TimeOfModification = DateTime.Now;
            newInstruction.Number = instruction.Number;
            var currentUserId = userManager.GetUserId(User); //User.Identity.GetUserId();
            newInstruction.CreatedByUserId = currentUserId;
            db.Instructions.Add(newInstruction);
            db.SaveChanges();

            if (!string.IsNullOrEmpty(training))
            {
                var tn = new TrainingName();
                tn.Name = string.Empty;
                tn.Number = training;
                db.TrainingNames.Add(tn);
                db.SaveChanges();

                var tg = new TrainingGroup();
                tg.TrainingNameId = tn.ID;
                tg.InstructionId = newInstruction.ID;
                tg.TimeOfCreation = DateTime.Now;
                tg.TimeOfModification = DateTime.Now;

                db.TrainingGroups.Add(tg);
                db.SaveChanges();
            }

            var ids =
                (from InstructionGroups in db.InstructionGroups
                 where
                     InstructionGroups.GroupId != null &&
                     InstructionGroups.InstructionId == id
                 select new
                 {
                     val = InstructionGroups.GroupId ?? 0
                 }).Select(x => x.val.ToString()).ToList();

            if (ids != null && ids.Count() > 0)
                foreach (var item in ids)
                {
                    var groupInstructions = new InstructionGroup
                    {
                        InstructionId = newInstruction.ID,
                        TimeOfCreation = DateTime.Now,
                        TimeOfModification = DateTime.Now,
                        GroupId = int.Parse(item)
                    };
                    db.InstructionGroups.Add(groupInstructions);
                    db.SaveChanges();
                }

            return RedirectToAction("Details", new { id = newInstruction.ID });
        }

        public ActionResult Excel(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var instructionVsTrainingList =
                (from w in db.Workers
                 join wg in db.GroupWorkers on w.ID equals wg.WorkerId
                 join gi in db.InstructionGroups on wg.GroupId equals gi.GroupId
                 join i in db.Instructions on gi.InstructionId equals i.ID
                 join t in db.Trainings
                     on new { InstructionId = i.ID, wg.WorkerId }
                     equals new { t.InstructionId, t.WorkerId } into t_join
                 from t in t_join.DefaultIfEmpty()
                 where i.ID == id
                 select new InstructionVsTrainingData
                 {
                     WorkerLastName = w.LastName,
                     WorkerFirstMidName = w.FirstMidName,
                     WorkerFullName = w.LastName + " " + w.FirstMidName,
                     InstructionName = i.Name,
                     GroupId = gi.GroupId,
                     InstructionVersion = i.Version,
                     InstructionNumber = i.Number,
                     DateOfTraining = (DateTime?)t.DateOfTraining,
                     TrainingName = t.TrainingName.Number
                 }).ToList();


            try
            {
                MemoryStream content = new MemoryStream();
                using (ExcelPackage package = new ExcelPackage(content))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets.Add("Trainings");
                    ws.Cells["A1"].LoadFromCollection(instructionVsTrainingList, true);

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

        public ActionResult Search(string srchterminstructions)
        {
            var newInstructions =
                (from i in db.Instructions
                 group i by i.Number
                    into groupedI
                 let maxVersion = groupedI.Max(gt => gt.Version)
                 select new InstructionLatestVersion
                 {
                     Key = groupedI.Key,
                     maxVersion = maxVersion,
                     ID = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).ID
                 }).Where(ni =>
                 ni.Key.ToUpper().Contains(srchterminstructions.ToUpper()) ||
                 string.IsNullOrEmpty(srchterminstructions)).ToList();

            var allInstructions =
                db.Instructions.ToList().Where(x => newInstructions.Any(ni => ni.ID == x.ID)).ToList();
            var groups = workerRepository.GetAllGroups();

            var lstInstructionGroups = new List<InstructionIndexData>();

            foreach (var item in allInstructions)
            {
                var instructioGroup = new InstructionIndexData();

                instructioGroup.ID = item.ID;
                instructioGroup.Name = item.Name;
                instructioGroup.Number = item.Number;
                instructioGroup.Version = item.Version;
                instructioGroup.UserName = item.CreatedByUserId;
                if (item.CreatedByUserId != null)
                    instructioGroup.UserName = db.Users.Find(item.CreatedByUserId).UserName;
                instructioGroup.TimeOfCreation = item.TimeOfCreation.ToShortDateString();

                var lstGroupIds = (from InstructionGroups in db.InstructionGroups
                                   where
                                       InstructionGroups.GroupId != null &&
                                       InstructionGroups.InstructionId == item.ID
                                   select new
                                   {
                                       val = InstructionGroups.GroupId ?? 0
                                   }).Select(x => x.val.ToString()).ToList();
                instructioGroup.SelectedIds = lstGroupIds.ToArray();

                instructioGroup.Items = groups.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.GroupName
                });

                lstInstructionGroups.Add(instructioGroup);
            }

            return View("Index", lstInstructionGroups);
        }
    }

    public class InputGroupItemModel : IInputGroupItem
    {
        public IDictionary<string, object> HtmlAttributes { get; set; }

        public string CssClass { get; set; }

        public bool? Enabled { get; set; }

        public bool? Encoded { get; set; }

        public string Label { get; set; }

        public string Value { get; set; }
    }

    public class DataSourceRequestExt : DataSourceRequest
    {
        public string GroupName { get; set; }
    }
}