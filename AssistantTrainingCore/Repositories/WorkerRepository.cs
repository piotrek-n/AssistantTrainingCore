//using AssistantTrainingCore.DAL;

using AssistantTrainingCore.Data;
using AssistantTrainingCore.Models;
using AssistantTrainingCore.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace AssistantTrainingCore.Repositories
{
    public class WorkerRepository : IWorkerRepository
    {
        private ApplicationDbContext db;

        public WorkerRepository(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public List<Group> GetAllGroups()
        {
            List<Group> groups = db.Groups.OrderBy(x => x.GroupName).ToList();
            return groups;
        }

        public List<Group> GetGroupsById(List<int> ids)
        {
            List<Group> groups = db.Groups.Where(g => ids.Contains(g.ID)).OrderBy(x => x.GroupName).ToList();
            return groups;
        }

        public List<TrainingGroup> GetTrainings()
        {
            //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            // var newInstructions =
            // (from i in db.Instructions
            //  group i by i.Number into groupedI
            //  let maxVersion = groupedI.Max(gt => gt.Version)
            //  select new InstructionLatestVersion
            //  {
            //      Key = groupedI.Key,
            //      maxVersion = maxVersion,
            //      ID = groupedI.FirstOrDefault(gt2 => gt2.Version == maxVersion).ID
            //  }).ToList();

            var newInstructions = db.Instructions.GroupBy(c => c.Number)
                .Select(g => new
                {
                    Key = g.Key,
                    ID = db.Instructions.FirstOrDefault(x => x.Number == g.Key && x.Version == g.Max(x => x.Version)).ID,
                    maxVersion = g.Max(x => x.Version)
                }).ToList();

            var temp = db.TrainingGroups.Include("Instruction").Include("TrainingName").ToList();

            var trainings = db.TrainingGroups
                .Include("Instruction")
                .Include("TrainingName")
                .Where(x => newInstructions.Any(ni => ni.ID == x.InstructionId) && !x.TrainingName.Number.Equals("undefined"))
                .AsQueryable<TrainingGroup>()
                .OrderByDescending(o => o.TimeOfCreation);

            return temp.Where(x => newInstructions.Any(ni => ni.ID == x.InstructionId) && !x.TrainingName.Number.Equals("undefined")).ToList();
            
            //return trainings.Where(tt => !tt.TrainingName.Number.Equals("undefined"));
            //return trainings;
        }

        public IQueryable<TrainingWorkersGridData> GetWorkersByTraining(string term, string type)
        {
            IQueryable<TrainingWorkersGridData> lst = new List<TrainingWorkersGridData>().AsQueryable();
            int itemID;
            bool res = int.TryParse(term, out itemID);
            DateTime dateForNull = new DateTime(1900, 1, 1);

            if (!string.IsNullOrEmpty(type) && type.Equals("trained") && res)
            {
                lst = (from t in db.Trainings
                    orderby t.Worker.LastName + "  " + t.Worker.FirstMidName
                    where
                        t.TrainingName.ID == itemID && t.DateOfTraining > dateForNull
                    select new TrainingWorkersGridData
                    {
                        WorkerLastName = t.Worker.LastName,
                        WorkerFirstMidName = t.Worker.FirstMidName,
                        DateOfTraining = t.DateOfTraining,
                        WorkerID = t.WorkerId,
                        TrainingNameId = t.TrainingNameId,
                        WorkerFullName = t.Worker.LastName + "  " + t.Worker.FirstMidName
                    }).Distinct().OrderBy(x => x.WorkerLastName).ThenBy(n => n.WorkerFirstMidName);

                return lst;
            }
            else if (!string.IsNullOrEmpty(type) && type.Equals("untrained") && res)
            {
                lst = (from t in db.Trainings
                    orderby t.Worker.LastName + "  " + t.Worker.FirstMidName
                    where
                        t.TrainingName.ID == itemID && t.DateOfTraining == new DateTime(1900, 1, 1)
                    select new TrainingWorkersGridData
                    {
                        WorkerLastName = t.Worker.LastName,
                        WorkerFirstMidName = t.Worker.FirstMidName,
                        DateOfTraining = t.DateOfTraining,
                        WorkerID = t.WorkerId,
                        TrainingNameId = t.TrainingNameId,
                        WorkerFullName = t.Worker.LastName + "  " + t.Worker.FirstMidName
                    }).Distinct().OrderBy(x => x.WorkerLastName).ThenBy(n => n.WorkerFirstMidName);

                return lst;
            }

            return lst;
        }

        public IQueryable<TrainingWorkersGridData> GetWorkersByTraining()
        {
            IQueryable<TrainingWorkersGridData> lst = new List<TrainingWorkersGridData>().AsQueryable();

            lst = (from t in db.Trainings
                //where
                //  t.TrainingName.ID == 1
                select new TrainingWorkersGridData
                {
                    WorkerLastName = t.Worker.LastName,
                    WorkerFirstMidName = t.Worker.FirstMidName,
                    DateOfTraining = t.DateOfTraining,
                    WorkerID = t.WorkerId,
                    TrainingNameId = t.TrainingNameId
                });

            return lst;
        }
    }
}