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


            var newInstructions = db.Instructions.GroupBy(c => c.Number)
                .Select(g => new
                {
                    Key = g.Key,
                    ID = db.Instructions.FirstOrDefault(x => x.Number == g.Key && x.Version == g.Max(x => x.Version)).ID,
                    maxVersion = g.Max(x => x.Version)
                }).ToList();
            
            // TrainingName is null after delete training
            //var test1 = db.TrainingGroups.Include("Instruction").ToList();

            var temp = db.TrainingGroups.Include("Instruction").Include("TrainingName").ToList();

            return temp.Where(x => newInstructions.Any(ni => ni.ID == x.InstructionId) && !x.TrainingName.Number.Equals("undefined")).OrderByDescending(t=>t.TimeOfCreation).ToList();
            
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
                        t.TrainingName.ID == itemID && t.DateOfTraining.Date > DateTime.Parse("1900-01-01 00:00:00.000").Date //> dateForNull.Date
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
                        t.TrainingName.ID == itemID && t.DateOfTraining.Date == DateTime.Parse("1900-01-01 00:00:00.000").Date // t.DateOfTraining.ToString().Equals("1900-01-01 00:00:00.000")
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