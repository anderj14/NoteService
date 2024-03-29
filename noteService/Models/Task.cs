using System;
using System.Collections.Generic;

namespace NoteService.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public int UserId { get; set; }

    public string TaskTitle { get; set; } = null!;

    public string TaskDescription { get; set; } = null!;

    public DateTime ScheduledDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
