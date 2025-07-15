namespace climate;

/// <summary>
/// If the dev wishes to add messages before and after each step, he can override these funcs.
/// </summary>
public class QueuedStepMessages
{
    public bool run_on_debug { get; set; }
    public Func<string> before_step { get; set; }
    public Func<string> after_step { get; set; }
}