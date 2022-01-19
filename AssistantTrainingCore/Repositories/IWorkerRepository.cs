//using AssistantTrainingCore.DAL;
using AssistantTrainingCore.Models;
using AssistantTrainingCore.ViewModel;

namespace AssistantTrainingCore.Repositories
{
    public interface IWorkerRepository
    {
        List<Group> GetAllGroups();
        List<Group> GetGroupsById(List<int> ids);
        List<TrainingGroup> GetTrainings();
        IQueryable<TrainingWorkersGridData> GetWorkersByTraining();
        IQueryable<TrainingWorkersGridData> GetWorkersByTraining(string term, string type);
    }
}