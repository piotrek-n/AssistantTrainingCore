using AssistantTrainingCore.Data;
using AssistantTrainingCore.Models;
using AssistantTrainingCore.Repositories;
using AssistantTrainingCore.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace AssistantTrainingCore.Controllers
{
    [Authorize]
    public class TrainingsController : Controller
    {
        /* Staff Training*/
        /*z jakich instrukcji pracownik powinien być przeszkolony i czy te szkolenia się odbyły*/
        /*
         * W1
         *  I1 Przeszkolony: Tak/Nie T.Date
         *  I2 Przeszkolony: Tak/Nie T.Date
         * W2
         *  I1 Przeszkolony: Tak/Nie T.Date
         *  I2 Przeszkolony: Tak/Nie T.Date
         *
                Select * from [dbo].[Workers] w
                LEFT JOIN [dbo].[GroupInstructions] gi ON w.ID = gi.WorkerID
                LEFT JOIN [dbo].[Instructions] i ON gi.GroupID = i.GroupID
                LEFT JOIN [dbo].[Trainings] t ON t.WorkerId = w.ID AND t.[InstructionId] = i.ID
                order BY w.LastName, w.FirstMidName
         */

        /* Training */
        /*śledzenie instrukcji – jacy pracownicy są podpięci do tych instrukcji i którzy wymagają przeszkolenia*/
        /*
         * I1
         *  W1 Przeszkolony: Tak/Nie
         *  W2 Przeszkolony: Tak/Nie
         * I12
         *  W1 Przeszkolony: Tak/Nie
         *  W2 Przeszkolony: Tak/Nie
         */

        //public ActionResult Index(int? id)
        //{
        //    var viewModel = new TrainingIndexData();

        //    //var lst = (from w in db.Workers
        //    //           join gw in db.GroupWorkers on w.ID equals gw.WorkerId
        //    //           join ig in db.InstructionGroups on new { GroupId = gw.GroupId } equals new { GroupId = Convert.ToInt32(ig.GroupId) }
        //    //           join i in db.Instructions on ig.InstructionId equals i.ID
        //    //           join t in db.Trainings on new { wID = w.ID , iID = i.ID } equals new { wID = t.WorkerId, iID = t.InstructionId }
        //    //           into gj
        //    //           from e in gj.DefaultIfEmpty()
        //    //           select new TrainingItemIndexData{ WorkId=w.ID, Worker =w, Instruction =i, Training =e}
        //    //           //select new {w, i= (new InstructionExt(w.ID,i)),e = (new TrainingExt{ WorkerID = w.ID }) }
        //    //           ).OrderBy(x=>x.Worker.LastName).ToList();
        //    viewModel.items = null;

        //    return View(viewModel);
        //}

        private readonly ApplicationDbContext db;
        private readonly IWorkerRepository workerRepository;

        public TrainingsController(ApplicationDbContext dbContext, IWorkerRepository workerRepository)
        {
            db = dbContext;
            this.workerRepository = workerRepository;
        }

        public ActionResult Index()
        {
            return View(workerRepository.GetTrainings());
        }

        //public ActionResult MainGridReadBound([DataSourceRequest] DataSourceRequest request)
        //{
        //    var items = workerRepository.GetTrainings();
        //    return Json(items.ToDataSourceResult(request)); //zmienic na ViewModel
        //}

        public ActionResult Trainings_Read_Bound([DataSourceRequest] DataSourceRequest request, string trainingNameId, string term)
        {
            var items = workerRepository.GetWorkersByTraining(trainingNameId, term).OrderBy(p => 0).OrderBy(x => x.WorkerFullName).ToList();
            return Json(items.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public JsonResult GetInstructionsByQuery(string query)
        {
            return Json((from i in db.Instructions where i.Number.Contains(query) select i.Number).Distinct().ToList());
        }

        public JsonResult GetTrainingNamesByQuery(string query)
        {
            return Json((from i in db.TrainingNames where i.Number.Contains(query) select i.Number).Distinct()
                .ToList());
        }

        public ActionResult Search(string instruction, string training)
        {
            var viewModel = new TrainingIndexData();
            return View(viewModel);
        }

        public ActionResult RemoveTrainings([FromBody] TrainingUpdateData model)
        {
            if (model.Workers != null)
            {
                foreach (var w in model.Workers)
                {
                    //Czy zaznaczeniej do pojedynczej instrukcji traktujemu jako zaznaczenie szkolenia do tej instrukcji, czy do wszystkich instrukcji
                    //z tego szkolenia.
                    var tr = db.Trainings.Where(x => x.TrainingNameId.Equals(w.TrainingNameId) && x.WorkerId.Equals(w.WorkerID)).ToList();

                    if (tr != null && w.Checked.Equals(true) && tr.Count > 0)
                    {
                        foreach (var item in tr)
                        {
                            db.Trainings.Attach(item);
                            db.Trainings.Remove(item);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var b = 0;
                    }
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult UpdateTrainings([FromBody] TrainingUpdateData model)
        {
            if (model != null)
            {
                if (model.Workers != null)
                {
                    foreach (var w in model.Workers)
                    {
                        //Czy zaznaczeniej do pojedynczej instrukcji traktujemu jako zaznaczenie szkolenia do tej instrukcji, czy do wszystkich instrukcji
                        //z tego szkolenia.
                        var tr = db.Trainings.Where(x => x.TrainingNameId.Equals(w.TrainingNameId) && x.WorkerId.Equals(w.WorkerID)).ToList();

                        if (tr != null && w.Checked.Equals(true) && tr.Count > 0)
                        {
                            var dt = DateTime.ParseExact(model.TrainingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                            foreach (var item in tr)
                            {
                                item.DateOfTraining = dt;
                                db.Entry(item).Property(X => X.DateOfTraining).IsModified = true;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var b = 0;
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteTraining(int id)
        {
            try
            {
                var tr = db.Trainings.Where(x => x.TrainingNameId.Equals(id)).ToList();

                if (tr != null && tr.Count > 0)
                {
                    foreach (var item in tr)
                    {
                        db.Trainings.Attach(item);
                        db.Trainings.Remove(item);
                        //db.SaveChanges();
                    }
                }

                var tg = db.TrainingGroups.Where(x => x.TrainingNameId.Equals(id)).ToList();
                if (tg != null && tg.Count > 0)
                {
                    foreach (var item in tg)
                    {
                        db.TrainingGroups.Attach(item);
                        db.TrainingGroups.Remove(item);
                        //db.SaveChanges();
                    }
                }

                var tn = db.TrainingNames.Where(x => x.ID.Equals(id)).ToList();

                if (tn != null && tn.Count > 0)
                {
                    foreach (var item in tn)
                    {
                        db.TrainingNames.Attach(item);
                        db.TrainingNames.Remove(item);
                        //db.SaveChanges();
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //Handle Exception;
                return View("Error");
            }
        }

        public ActionResult DeleteWorkerTraining(int TrainingNameID, int WorkerID)
        {
            var tr = db.Trainings.Where(x => x.TrainingNameId.Equals(TrainingNameID) && x.WorkerId.Equals(WorkerID))
                .ToList();

            if (tr != null && tr.Count > 0)
            {
                foreach (var item in tr)
                {
                    db.Trainings.Attach(item);
                    db.Trainings.Remove(item);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult InstructionsJsonAction(string q, string t)
        {
            //Session["term"] = t;
            var lstInstructions = new List<InstructionsJson>();
            if (t.Equals("true"))
            {
                //lstInstructions = (
                //        from i in db.Instructions
                //        group i by i.Number
                //        into groupedI
                //        let maxVersion = groupedI.Max(gt => gt.Version)
                //        select new
                //        {
                //            Key = groupedI.Key,
                //            ID = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).ID,
                //            Number = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).Number,
                //            Name = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).Name,
                //            Version = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).Version,
                //            Reminder = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).Reminder
                //        }
                //    ).Select(x => new InstructionsJson
                //    { id = x.ID.ToString(), text = x.Number, name = x.Name, version = x.Version })
                //    .Where(x => x.text.ToUpper().Contains(q.ToUpper()))
                //    .ToList();

                lstInstructions = db.Instructions.GroupBy(c => c.Number)
                    .Select(g => new
                    {
                        Key = g.Key,
                        ID = db.Instructions.FirstOrDefault(x => x.Number == g.Key && x.Version == g.Max(x => x.Version)).ID,
                        Number = g.FirstOrDefault(gt2 => gt2.Version == g.Max(x => x.Version)).Number,
                        Name = g.FirstOrDefault(gt2 => gt2.Version == g.Max(x => x.Version)).Name,
                        Version = g.Max(x => x.Version),
                        Reminder = g.FirstOrDefault(gt2 => gt2.Version == g.Max(x => x.Version)).Reminder
                    }).Select(x => new InstructionsJson
                    { id = x.ID.ToString(), text = x.Number, name = x.Name, version = x.Version, reminder = x.Reminder })
                    .Where(x => x.text.ToUpper().Contains(q.ToUpper()))
                    .ToList();
            }
            else
            {
                // var items = (
                //     from i in db.Instructions
                //     group i by i.Number
                //     into groupedI
                //     let maxVersion = groupedI.Max(gt => gt.Version)
                //     select new
                //     {
                //         Key = groupedI.Key,
                //         ID = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).ID
                //     }
                // ).Select(x => x.ID).ToList();

                var items = db.Instructions.GroupBy(c => c.Number)
                    .Select(g => new
                    {
                        Key = g.Key,
                        ID = db.Instructions.FirstOrDefault(x => x.Number == g.Key && x.Version == g.Max(x => x.Version)).ID,
                        maxVersion = g.Max(x => x.Version)
                    }).Select(x => x.ID).ToList();

                lstInstructions = (from i in db.Instructions
                                   join tg in db.TrainingGroups on new { ID = i.ID } equals new { ID = tg.InstructionId } into tg_join
                                   from tg in tg_join.DefaultIfEmpty()
                                   where
                                       i.Number.ToUpper().Contains(q.ToUpper())
                                       && items.Contains(i.ID)
                                       && tg.InstructionId == null
                                   select new InstructionsJson
                                   {
                                       id = i.ID.ToString(),
                                       text = i.Number,
                                       name = i.Name,
                                       version = i.Version,
                                       reminder = i.Reminder
                                   }).ToList();
                //**FIX
                //If a new worker was added.
                if (lstInstructions.Count() == 0)
                {
                    lstInstructions =
                        (from w in db.Workers
                         join wg in db.GroupWorkers on w.ID equals wg.WorkerId
                         join gi in db.InstructionGroups on wg.GroupId equals gi.GroupId
                         join i in db.Instructions on gi.InstructionId equals i.ID
                         join tt in db.Trainings
                             on new { InstructionId = i.ID, WorkerId = wg.WorkerId }
                             equals new { tt.InstructionId, tt.WorkerId } into t_join
                         from tt in t_join.DefaultIfEmpty()
                         where
                             w.IsSuspend == false
                             && items.Contains(i.ID)
                         select new InstructionsJson
                         {
                             id = i.ID.ToString(),
                             text = i.Number,
                             name = i.Name,
                             version = i.Version,
                             reminder = i.Reminder
                         }).Distinct().ToList();
                }
            }

            var countInstructions = lstInstructions.Count();

            InstructionsJsonDTO result = new InstructionsJsonDTO();
            if (countInstructions > 0)
            {
                result.total_count = countInstructions.ToString();
                result.items = lstInstructions;
            }

            //var json = JsonConvert.SerializeObject(result);
            // JavaScriptSerializer j = new JavaScriptSerializer();
            // object a = j.Deserialize(json, typeof(object));

            return Json(result);
        }

        [HttpPost]
        public ActionResult AddNewTrainings(string selectedValues, string trainingNumber)
        {
            if (selectedValues.Length > 0)
            {
                string[] selectedValuesInstruction = selectedValues.Split(',');
                var intInstructionIDs = selectedValuesInstruction.Select(int.Parse).ToList();

                #region Add new training

                TrainingName tn = new TrainingName();
                tn.Name = String.Empty;
                tn.Number = trainingNumber;
                db.TrainingNames.Add(tn);
                db.SaveChanges();

                #endregion Add new training

                #region Add TrainingGroup

                foreach (var val in selectedValuesInstruction)
                {
                    TrainingGroup tg = new TrainingGroup();
                    tg.TrainingNameId = tn.ID;
                    tg.InstructionId = Int32.Parse(val);
                    tg.TimeOfCreation = DateTime.Now;
                    tg.TimeOfModification = DateTime.Now;
                    db.TrainingGroups.Add(tg);
                    db.SaveChanges();
                }

                #endregion Add TrainingGroup

                #region Add all workers and assigned instruction per TrainingGroup

                //select* from[dbo].[Workers] w
                //inner join[dbo].[GroupWorkers] gw on gw.[WorkerId] = w.ID
                //inner join dbo.InstructionGroups ig on ig.[GroupId] = gw.[GroupId]

                var instructionWorkerList =
                    (from w in db.Workers
                     join gw in db.GroupWorkers on w.ID equals gw.WorkerId
                     join ig in db.InstructionGroups on gw.GroupId equals ig.GroupId
                     join t in db.Trainings
                         on new { WorkerId = w.ID, ID = ig.Instruction.ID }
                         equals new { t.WorkerId, ID = t.InstructionId } into t_join
                     from t in t_join.DefaultIfEmpty()
                     where
                         w.IsSuspend == false && intInstructionIDs.Contains(ig.Instruction.ID) && (int?)t.ID == null
                     select new
                     {
                         WorkerID = w.ID,
                         InstructionID = ig.Instruction.ID
                     }
                    ).ToList();

                if (instructionWorkerList != null)
                {
                    foreach (var instruction in instructionWorkerList)
                    {
                        if (instruction != null)
                        {
                            Training newTraining = new Training();
                            newTraining.WorkerId = instruction.WorkerID;
                            newTraining.TrainingNameId = tn.ID;
                            newTraining.TimeOfCreation = DateTime.Now;
                            newTraining.TimeOfModification = DateTime.Now;
                            newTraining.DateOfTraining = new DateTime(1900, 1, 1);
                            newTraining.InstructionId = instruction.InstructionID;
                            db.Trainings.Add(newTraining);
                            db.SaveChanges();
                        }
                    }
                }

                #endregion Add all workers and assigned instruction per TrainingGroup
            }

            return Json("success");
        }
    }

    public class InstructionsJsonDTO
    {
        public string total_count;
        public List<InstructionsJson> items;
    }

    public class InstructionsJson
    {
        public string id;
        public string text;
        public string name;
        public int version;
        public bool reminder;
    }
}