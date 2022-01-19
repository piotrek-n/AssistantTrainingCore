using System.ComponentModel.DataAnnotations.Schema;

namespace AssistantTrainingCore.Models
{
    public class TrainingGroup
    {
        public int ID { get; set; }
        public DateTime TimeOfCreation { get; set; }
        public DateTime TimeOfModification { get; set; }

        public int InstructionId { get; set; }

        [ForeignKey("InstructionId")]
        public virtual Instruction Instruction { get; set; }

        public int TrainingNameId { get; set; }

        [ForeignKey("TrainingNameId")]
        public virtual TrainingName TrainingName { get; set; }

        //public virtual ICollection<TrainingName> TrainingNames { get; set; }

        //public virtual ICollection<Instruction> Instructions { get; set; }
    }
}