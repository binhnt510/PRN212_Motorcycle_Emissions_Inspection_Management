using System;
using System.Collections.Generic;

namespace App_MotoEmissions.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? SentDate { get; set; }

    public bool? IsRead { get; set; }

    public virtual UserAccount? User { get; set; }
}
