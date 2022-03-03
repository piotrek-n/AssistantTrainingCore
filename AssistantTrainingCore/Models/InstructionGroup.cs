using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AssistantTrainingCore.Models
{
    public class InstructionGroup
    {
        public int ID { get; set; }
        public DateTime TimeOfCreation { get; set; }
        public DateTime TimeOfModification { get; set; }

        public int? GroupId { get; set; }
        public virtual Group Group { get; set; }

        public int? InstructionId { get; set; }

        [ForeignKey("InstructionId")]
        public virtual Instruction Instruction { get; set; }
    }
}