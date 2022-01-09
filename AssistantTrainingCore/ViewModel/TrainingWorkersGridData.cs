namespace AssistantTrainingCore.ViewModel
{
    public class TrainingWorkersGridData
    {
        public int WorkerID { get; set; }
        public string WorkerLastName { get; set; }
        public string WorkerFirstMidName { get; set; }
        public string WorkerFullName { get; set; }
        public string InstructionName { get; set; }
        public int? GroupId { get; set; }
        public string InstructionVersion { get; set; }
        public string InstructionNumber { get; set; }
        public DateTime? DateOfTraining { get; set; }
        public int TrainingNameId { get; set; }
        public string TrainingNumber { get; set; }
    }
}