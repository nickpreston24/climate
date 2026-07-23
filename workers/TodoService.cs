using CodeMechanic.Async;

namespace climate;

public class TodoService : QueuedService
{
    public async Task<string> ProcessFileAsync(string filepath)
    {
        // dummy
        Thread.Sleep(500);
        return filepath;
    }
}