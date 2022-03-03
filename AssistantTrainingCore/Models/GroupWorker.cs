using System.ComponentModel.DataAnnotations.Schema;

namespace AssistantTrainingCore.Models
{
    public class GroupWorker
    {
        public int ID { get; set; }

        public DateTime TimeOfCreation { get; set; }

        public DateTime TimeOfModification { get; set; }

        public int GroupId { get; set; }

        public Group Group { get; set; }

        public int WorkerId { get; set; }

        [ForeignKey("WorkerId")]
        public Worker Worker { get; set; }
    }
}