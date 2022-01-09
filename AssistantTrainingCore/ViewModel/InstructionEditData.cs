using AssistantTrainingCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AssistantTrainingCore.ViewModel
{
    public class InstructionEditData
    {
        public int ID { get; set; }

        [DisplayName("Nazwa")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string Name { get; set; }

        [DisplayName("Numer")]
        [RegularExpression(@"^\S{2}.\d{2}.\d{2}.\d{2}(.\d{2})?$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string Number { get; set; }

        [DisplayName("Numer szkolenia")]
        [RegularExpression(@"^\S{1,2}\d{1,3}/\d{4}$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]
        public string NumberOfTraining { get; set; }

        [DisplayName("Wersja")]
        [RegularExpression(@"^\d{1,3}$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NumberFormat")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public int Version { get; set; }

        [DisplayName("Grupy")]
        public virtual ICollection<Group> Groups { get; set; }

        public string SelectedId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredFiled")]
        public string[] SelectedIds { get; set; }

        [DisplayName("Grupy")]
        public IEnumerable<SelectListItem> Items { get; set; }
    }
}