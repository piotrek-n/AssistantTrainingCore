using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AssistantTrainingCore.Models
{
    //http://webdesignwith.net/object-mappers/
    public class Worker
    {
        public int ID { get; set; }

        [DisplayName("Nazwisko")]
        public string LastName { get; set; }

        [DisplayName("Imię")]
        public string FirstMidName { get; set; }

        [ScaffoldColumn(true)]
        [DisplayName("Czas utworzenia")]
        public DateTime TimeOfCreation { get; set; }

        [ScaffoldColumn(true)]
        [DisplayName("Czas modyfikacji")]
        public DateTime TimeOfModification { get; set; }

        public string? Tag { get; set; }

        public virtual ICollection<GroupWorker> GroupInstructions { get; set; }

        [DisplayName("Zawieszony")]
        public bool IsSuspend { get; set; }
    }
}