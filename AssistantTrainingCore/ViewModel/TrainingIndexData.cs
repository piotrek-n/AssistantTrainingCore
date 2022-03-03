using AssistantTrainingCore.Models;

namespace AssistantTrainingCore.ViewModel
{
    public class TrainingIndexData
    {
        public IEnumerable<TrainingItemIndexData> items = null;
    }

    public class TrainingItemIndexData
    {
        public int WorkId { get; set; }
        public Training Training { get; set; }
        public Instruction Instruction { get; set; }
        public Worker Worker { get; set; }

        //public IEnumerable<Training> Trainings { get; set; }
        //public IEnumerable<Instruction> Instructions { get; set; }
        //public IEnumerable<Worker> Workers { get; set; }
    }
}