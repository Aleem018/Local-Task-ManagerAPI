using System;

namespace LocalTaskManager.Models;

public class SystemTask
{
    public int ProcessId { get; set; }

    public string ProcessName { get; set; } = string.Empty;
}