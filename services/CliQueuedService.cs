using CodeMechanic.Async;
using CodeMechanic.Diagnostics;
using CodeMechanic.Shargs;
using QueuedService = CodeMechanic.Async.QueuedService;

namespace climate;

public class CliQueuedService : QueuedService
{
    private readonly bool debug;

    private readonly bool run_as_cli;
    protected readonly ArgsMap arguments;
    protected QueuedStepMessages messages { get; set; } = new();

    // This is to reduce assignment boilerplate.
    protected CliQueuedService(ArgsMap arguments)
    {
        this.arguments = arguments;
        this.debug = this.arguments.HasFlag("--debug");
        if (debug) arguments.Dump(nameof(arguments));
    }

    public override async Task Run()
    {
        var Q = new SerialQueue();
        var tasks = steps.Select(step =>
        {
            if (messages.before_step != null)
                Console.WriteLine(messages.before_step);

            var task = Q.Enqueue(step);

            if (messages.after_step != null)
                Console.WriteLine(messages.after_step);

            return task;
        });
        await Task.WhenAll(tasks);
        steps.Clear();
    }
}