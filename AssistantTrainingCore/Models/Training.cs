using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AssistantTrainingCore.Models
{
    public class Training
    {
        public int ID { get; set; }

        public int InstructionId { get; set; }
        public virtual Instruction Instruction { get; set; }

        public int TrainingNameId { get; set; }
        public virtual TrainingName TrainingName { get; set; }

        public int WorkerId { get; set; }
        public virtual Worker Worker { get; set; }

        [ScaffoldColumn(true)]
        [DisplayName("Czas utworzenia")]
        public DateTime TimeOfCreation { get; set; }

        [ScaffoldColumn(true)]
        [DisplayName("Czas modyfikacji")]
        public DateTime TimeOfModification { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Training")]
        public DateTime DateOfTraining { get; set; }
    }
}