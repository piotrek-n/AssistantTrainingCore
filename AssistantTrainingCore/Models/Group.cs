using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AssistantTrainingCore.Models
{
    public class Group
    {
        public int ID { get; set; }

        [DisplayName("Nazwa Grupy")]
        public string GroupName { get; set; }

        [ScaffoldColumn(true)]
        [DisplayName("Czas utworzenia")]
        public DateTime TimeOfCreation { get; set; }

        [ScaffoldColumn(true)]
        [DisplayName("Czas modyfikacji")]
        public DateTime TimeOfModification { get; set; }

        public string? Tag { get; set; }

        //public virtual ICollection<Instruction> Instructions { get; set; }
    }
}