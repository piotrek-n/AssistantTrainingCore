namespace AssistantTrainingCore.Repositories
{
    public interface IReportsRepository
    {
        string IncompleteTraining();
        string IncompleteTrainingJSON();
        string InstructionsWithoutTraining();
        string InstructionsWithoutTrainingJSON();
        string WorkersWithoutTraining();
        string WorkersWithoutTrainingJSON();
    }
}