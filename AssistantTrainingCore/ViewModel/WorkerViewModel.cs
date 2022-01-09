using AssistantTrainingCore.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AssistantTrainingCore.ViewModel
{
    //https://www.codeproject.com/articles/292050/checkboxlist-for-a-missing-mvc-extension
    public class WorkerViewModel
    {
        //Groups
        [DisplayName("Dostępne grupy")]
        public IList<Group> AvailableGroups { get; set; }

        public IList<Group> SelectedGroups { get; set; }
        public PostingGroup PostingGroups { get; set; }

        //Other properties

        public int ID { get; set; }

        [Required(ErrorMessage = "required")]
        [DisplayName("Imię")]
        public string FirstMidName { get; set; }

        [Required(ErrorMessage = "required")]
        [DisplayName("Nazwisko")]
        public string LastName { get; set; }

        [DisplayName("Opis")]
        public string Tag { get; set; }

        public string FullName { get; set; }

        [DisplayName("Zawieszony")]
        public bool IsSuspend { get; set; }
    }

    // Helper class to make posting back selected values easier
    public class PostingGroup
    {
        public string[] GroupIDs { get; set; }
    }
}