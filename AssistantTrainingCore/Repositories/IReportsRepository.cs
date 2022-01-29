namespace AssistantTrainingCore.Repositories
{
    public interface IReportsRepository
    {
        string IncompleteTraining();
        string IncompleteTrainingJSON();
        IEnumerable<IncompleteTrainingDataReport> IncompleteTrainingResult();
        string InstructionsWithoutTraining();
        string InstructionsWithoutTrainingJSON();
        IEnumerable<InstructionsWithoutTrainingResult> InstructionsWithoutTrainingResult();
        string WorkersWithoutTraining();
        string WorkersWithoutTrainingJSON();
        IEnumerable<WorkersWithoutTrainingResult> WorkersWithoutTrainingResult();
        
    }
}