using Microsoft.AspNetCore.Mvc.Rendering;

namespace AssistantTrainingCore.ViewModel
{
    public class ReportsIndexData
    {
        public string SelectedId { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
    }
}