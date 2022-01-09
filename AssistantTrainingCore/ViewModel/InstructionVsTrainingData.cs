namespace AssistantTrainingCore.ViewModel
{
    public class InstructionVsTrainingData
    {
        public string WorkerLastName { get; set; }
        public string WorkerFirstMidName { get; set; }
        public string WorkerFullName { get; set; }
        public string WorkerIsSuspendedDesc { get; set; }
        public string InstructionName { get; set; }
        public int? GroupId { get; set; }
        public int InstructionVersion { get; set; }
        public string InstructionNumber { get; set; }
        public DateTime? DateOfTraining { get; set; }

        public string TrainingName { get; set; }

        public string DateOfTran
        { get { return DateOfTraining != null ? DateOfTraining.Value.ToShortDateString() : String.Empty; } }

        public int RowNo { get; set; }
    }
}