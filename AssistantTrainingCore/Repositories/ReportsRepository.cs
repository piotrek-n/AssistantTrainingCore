using AssistantTrainingCore.Data;
using Newtonsoft.Json;

namespace AssistantTrainingCore.Repositories
{
    public class ReportsRepository
    {
        private readonly ApplicationDbContext db;

        public ReportsRepository(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// „Instrukcje” (lista instrukcji, do których nie zostały utworzone szkolenia)
        /// </summary>
        public string InstructionsWithoutTraining()
        {
            string json2 = InstructionsWithoutTrainingJSON();

            return json2.Insert(1, @"columns: [
                                    {
                                                    title: ""Number"", data: ""Number""
                                                        },
                                    {
                                                    title: ""Version"", data: ""Version""
                                    },
                                    {
                                                    data: null,
                                                    className: ""center"",
                                                    defaultContent: '<a href=""javascript:instructionsWithoutTraining();"" class=""editor_edit"">Details</a>'
                                    }],"
                                );
        }

        public string InstructionsWithoutTrainingJSON()
        {
            //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            var result =
                (from max_ins in (
                (from i in db.Instructions
                 join ig in (
                     (from Instructions in db.Instructions
                      group Instructions by new
                      {
                          Instructions.Number
                      } into g
                      select new
                      {
                          Ver = g.Max(p => p.Version),
                          g.Key.Number
                      }))
                       on new { i.Version, i.Number }
                   equals new { Version = ig.Ver, ig.Number }
                 select new
                 {
                     i.ID,
                     i.Version,
                     i.Number
                 }))
                 join tg in db.TrainingGroups on new { InstructionId = max_ins.ID } equals new { InstructionId = tg.InstructionId } into tg_join
                 from tg in tg_join.DefaultIfEmpty()
                 where
                   tg.InstructionId == null
                 select new
                 {
                     max_ins.Number,
                     max_ins.Version
                 }
             ).ToList().Select((currRow, index) => new { Number = currRow.Number, Version = currRow.Version, DT_RowId = index + 1 });

            var json2 = JsonConvert.SerializeObject(new
            {
                data = result
            });
            return json2;
        }

        public string WorkersWithoutTraining()
        {
            string json2 = WorkersWithoutTrainingJSON();

            return json2.Insert(1, @"columns: [
                                    {
                                                    title: ""Name"", data: ""Name""
                                    },
                                    {
                                                    title: ""Number"", data: ""Number""
                                    },
                                    {
                                                    title: ""Version"", data: ""Version""
                                    },
                                    {
                                                    data: null,
                                                    className: ""center"",
                                                    defaultContent: '<a href=""javascript:workersWithoutTraining();"" class=""editor_edit"">Details</a>'
                                    }],"
                    );
        }

        public string WorkersWithoutTrainingJSON()
        {
            //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            //db.Database.CommandTimeout = 180;
            var result = (from w in db.Workers
                          join gw in db.GroupWorkers on w.ID equals gw.WorkerId
                          join g in db.Groups on gw.GroupId equals g.ID
                          join ig in db.InstructionGroups on g.ID equals ig.GroupId
                          join ei in (
                              (from max_ins in (
                                  (from ii in db.Instructions
                                   join ig in (
                                     (from Instructions in db.Instructions
                                      group Instructions by new
                                      {
                                          Instructions.Number
                                      } into g
                                      select new
                                      {
                                          Ver = g.Max(p => p.Version),
                                          g.Key.Number
                                      }))
                                           on new { ii.Version, ii.Number }
                                       equals new { Version = ig.Ver, ig.Number }
                                   select new
                                   {
                                       ii.ID,
                                       ii.Version,
                                       ii.Number
                                   }))
                               select new
                               {
                                   InstructionId = max_ins.ID,
                                   max_ins.Number,
                                   max_ins.Version
                               })
                          )
                          on ig.InstructionId equals ei.InstructionId
                          join t in db.Trainings
                                on new { WorkerId = w.ID, ID = ei.InstructionId }
                            equals new { t.WorkerId, ID = t.InstructionId } into t_join
                          from t in t_join.DefaultIfEmpty()
                          where
                            w.IsSuspend == false &&
                            (t.DateOfTraining == new System.DateTime(1900, 1, 1) || t.DateOfTraining == null)
                          orderby
                            ei.InstructionId
                          select new
                          {
                              ei.Number,
                              ei.Version,
                              w.ID,
                              w.LastName,
                              w.FirstMidName
                          }
                            ).Distinct()
                            .ToList()
                            .Select((currRow, index) => new { Name = currRow.LastName + " " + currRow.FirstMidName, Number = currRow.Number, Version = currRow.Version, DT_RowId = index + 1 });
            //.Select((currRow, index) => new { FirstName = currRow.FirstMidName, LastName = currRow.LastName, Number = currRow.Number, Version = currRow.Version, DT_RowId = index + 1 });

            var json2 = JsonConvert.SerializeObject(new
            {
                data = result
            });
            return json2;
        }

        /// <summary>
        /// Wsród szkolen powiazanych z instrukcjami sa jacyś pracownic, których jeszcze należy przeszkolić.
        /// </summary>
        /// <returns></returns>
        public string IncompleteTraining()
        {
            string json2 = IncompleteTrainingJSON();

            return json2.Insert(1, @"columns: [
                                    {
                                                    title: ""TrainingNumber"", data: ""TrainingNumber""
                                    },
                                    {
                                                    title: ""InstructionNumber"", data: ""InstructionNumber""
                                    },
                                    {
                                                    data: null,
                                                    className: ""center"",
                                                    defaultContent: '<a href=""javascript:incompleteTraining();"" class=""editor_edit"">Details</a>'
                                    }],"
                    );
        }

        /// <summary>
        /// Szkolenia
        /// </summary>
        /// <returns></returns>
        public string IncompleteTrainingJSON()
        {
            //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            var result = (from w in db.Workers
                          join gw in db.GroupWorkers on w.ID equals gw.WorkerId
                          join g in db.Groups on gw.GroupId equals g.ID
                          join ig in db.InstructionGroups on g.ID equals ig.GroupId
                          join ei in (
                              (from max_ins in (
                                  (from ii in db.Instructions
                                   join ig in (
                                     (from Instructions in db.Instructions
                                      group Instructions by new
                                      {
                                          Instructions.Number
                                      } into g
                                      select new
                                      {
                                          Ver = g.Max(p => p.Version),
                                          g.Key.Number
                                      }))
                                           on new { ii.Version, ii.Number }
                                       equals new { Version = ig.Ver, ig.Number }
                                   select new
                                   {
                                       ii.ID,
                                       ii.Version,
                                       ii.Number
                                   }))
                               select new
                               {
                                   InstructionId = max_ins.ID,
                                   max_ins.Number,
                                   max_ins.Version
                               })
                          )
                          on ig.InstructionId equals ei.InstructionId
                          join t in db.Trainings
                                on new { WorkerId = w.ID, ID = ei.InstructionId }
                            equals new { t.WorkerId, ID = t.InstructionId } into t_join
                          from t in t_join.DefaultIfEmpty()
                          join tn in db.TrainingNames on t.TrainingNameId equals tn.ID
                          where
                            w.IsSuspend == false &&
                              (from Trainings in db.Trainings
                               select new
                               {
                                   Trainings.InstructionId
                               }).Contains(new { InstructionId = ei.InstructionId }) &&
                            t.DateOfTraining == (new System.DateTime(1900, 1, 1))
                          select new
                          {
                              InstructionNumber = ei.Number,
                              TrainingName = tn.Number
                              //((from TrainingGroups in db.TrainingGroups
                              //  where
                              //        TrainingGroups.InstructionId == i.ID
                              //  orderby
                              //        TrainingGroups.TimeOfCreation descending
                              //  select new
                              //  {
                              //      TrainingGroups.TrainingName.Number
                              //  }).Select(x => x.Number).FirstOrDefault())
                          }).Distinct()
              .ToList()
              .Select((currRow, index) => new { TrainingNumber = currRow.TrainingName, InstructionNumber = currRow.InstructionNumber, DT_RowId = index + 1 });

            var json2 = JsonConvert.SerializeObject(new
            {
                data = result
            });
            return json2;
        }

        public static string EmptyReport()
        {
            string json = @"{
                    columns: [{
                        title: ""INFO""
                    }, {
                        title: ""VALUE""
                    }],
                    data: [
                      [""No data"", ""No data""]
                    ]
                }";

            return json;
        }
    }
}