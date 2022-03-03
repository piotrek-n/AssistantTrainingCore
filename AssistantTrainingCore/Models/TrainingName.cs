using System.ComponentModel;

namespace AssistantTrainingCore.Models
{
    public class TrainingName
    {
        public int ID { get; set; }

        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [DisplayName("Numer")]
        public string Number { get; set; }
    }
}