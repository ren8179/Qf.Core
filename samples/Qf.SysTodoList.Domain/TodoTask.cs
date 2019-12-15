using Qf.Core.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qf.SysTodoList.Domain
{
    [Table("TodoTask")]
    public class TodoTask : FullAuditedEntity<Guid>
    {
        public string Title { get; set; }

        public TodoType Type { get; set; }

        public TodoStatus Status { get; set; }
    }
    public enum TodoType
    {
        NearTerm,
        MediumTerm,
        LongTerm,
    }
    public enum TodoStatus
    {
        Default,
        Doing,
        Completed,
        Delayed,
    }
}
