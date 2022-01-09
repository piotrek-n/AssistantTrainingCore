using AssistantTrainingCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AssistantTrainingCore.ViewModel
{
    public class WorkerGroupViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "required")]
        [DisplayName("Imię")]
        public string FirstMidName { get; set; }

        [Required(ErrorMessage = "required")]
        [DisplayName("Nazwisko")]
        public string LastName { get; set; }

        [DisplayName("Opis")]
        public string? Tag { get; set; }

        [DisplayName("Grupy")]
        public virtual ICollection<Group> WorkerGroups { get; set; }

        public string[] SelectedIds { get; set; }

        [DisplayName("Grupy")]
        public IEnumerable<SelectListItem> Items { get; set; }

        public string FullName { get; set; }

        [DisplayName("Zawieszony")]
        public bool IsSuspend { get; set; }

        public int RowNo { get; set; }

        [DisplayName("Zawieszony")]
        public string IsSuspendDesc { get; set; }
    }
}