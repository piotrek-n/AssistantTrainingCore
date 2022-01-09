using System.ComponentModel;

namespace AssistantTrainingCore.Models
{
    public class Instruction
    {
        public int ID { get; set; }

        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [DisplayName("Numer")]
        public string Number { get; set; }

        [DisplayName("Wersja")]
        public int Version { get; set; }

        public DateTime TimeOfCreation { get; set; }

        public DateTime TimeOfModification { get; set; }

        public string Tag { get; set; }

        public string CreatedByUserId { get; set; }

        public bool Reminder { get; set; }

        //public int GroupId { get; set; }
        //public Group Group { get; set; }

        //public int GroupInstructionId { get; set; }
        //public GroupInstruction GroupInstruction { get; set; }
    }
}