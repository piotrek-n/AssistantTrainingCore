using AssistantTrainingCore.Models;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AssistantTrainingCore.ViewModel
{
    public class InstructionIndexData
    {
        public int ID { get; set; }

        [DisplayName("Nazwa")]
        //[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string Name { get; set; }


        //[RegularExpression(@"^[A-Z]{2}.\d{2}.\d{2}(.\d{2,3})?(.\d{2,3})?$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]
        //[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        [DisplayName("Numer")]
        [RegularExpression(@"^[A-Z]{2}.\d{2}.\d{2}(.\d{2,3})?(.\d{2,3})?$", ErrorMessage = "Niepoprawny format numeru")]
        [Required(ErrorMessage = "To pole jest wymagane")]
        public string Number { get; set; }

        [DisplayName("Numer szkolenia")]
        //[RegularExpression(@"^\S{1,2}\d{1,3}/\d{4}$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]
        public string NumberOfTraining { get; set; }

        [DisplayName("Wersja")]
        //[RegularExpression(@"^\d{1,3}$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]
        //[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public int Version { get; set; }

        [DisplayName("Grupy")]
        public virtual ICollection<Group> Groups { get; set; }

        public string SelectedId { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string[] SelectedIds { get; set; }

        //We want to replace it by ItemsList
        [DisplayName("Grupy")]
        public IEnumerable<SelectListItem> Items { get; set; }

        public string UserName { get; set; }

        public string TimeOfCreation { get; set; }

        public int RowNo { get; set; }

        [DisplayName("Lista Grup")]
        public List<IInputGroupItem> ItemsList { get; set; }

        //[Required]
        public string[] CheckBoxGroupValue { get; set; }

        public string GrupsInString { get; set; }

        public string HiddenWorkersString { get; set; }

        public bool Reminder { get; set; }
    }
}