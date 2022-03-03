using System.ComponentModel;

namespace AssistantTrainingCore.ViewModel
{
    public class GroupDetails
    {
        public int ID { get; set; }

        [DisplayName("Nazwa Grupy")]
        public string GroupName { get; set; }

        public List<InstructionInGroup> Instructions { get; set; }
    }

    public class InstructionInGroup
    {
        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [DisplayName("Numer")]
        public string Number { get; set; }

        [DisplayName("Wersja")]
        public int Version { get; set; }

        [DisplayName("ID")]
        public int ID { get; set; }
    }
}