using AssistantTrainingCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace AssistantTrainingCore.ViewModel
{
    //lista grup do których należy dana instrukcja
    //lista pracowników przeszkolonych i do przeszkolenia (można umieścić na jednej liście z możliwością filtrowania)
    public class InstructionDetailsData
    {
        public int ID { get; set; }

        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [DisplayName("Numer")]
        public string Number { get; set; }

        [DisplayName("Wersja")]
        public int Version { get; set; }

        [DisplayName("Grupy")]
        public virtual ICollection<Group> Groups { get; set; }

        [DisplayName("Grupy")]
        public virtual IEnumerable<GroupViewModel> GroupWithNumbers { get; set; }

        public virtual ICollection<Training> Trainings { get; set; }

        public List<InstructionVsTrainingData> instructionVsTrainingList { get; set; }

        [DisplayName("Grupy")]
        public IEnumerable<SelectListItem> Items { get; set; }
    }
}