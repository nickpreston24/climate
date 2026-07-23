using System.Collections.Concurrent;
using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using Microsoft.Extensions.Hosting;
using Serilog.Core;

namespace climate;

public class BgWorker : BackgroundService
{
    private readonly Logger logger;
    private readonly BgQueue bg_queue;
    private readonly Listener listener;

    private readonly ConcurrentDictionary<string, DateTime> file_map;

    private readonly TodoService todos;

    public BgWorker(
        Logger logger
        , BgQueue queue
        , Listener listener
        , ConcurrentDictionary<string, DateTime> map
        , TodoService TodoService
    )
    {
        this.logger = logger;

        // setup the file watchers
        this.listener = listener;
        this.file_map = map;

        // Set up specialized services which depend on file watchers.

        // this.cleaner = cleanup;
        this.todos = TodoService;

        // set up normal services that depend on file watchers.
        bg_queue = queue;
        logger.Information($"{nameof(BgWorker)} setup complete.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.Information($"{nameof(ExecuteAsync)} :>> {ExecuteAsync}");

        await listener.Setup();

        listener.Start();
        while (!stoppingToken.IsCancellationRequested)
        {
            var fs_event = await bg_queue.Consume(stoppingToken);
            var now = DateTime.UtcNow;

            string filepath = fs_event.FullPath.AsUnixPath();
            string filename = Path.GetFileName(filepath);

            logger.Information("[{date}] {filepath} arrived at BgWorker.",
                $"{now}", filepath);

            // do stuff:
            bool can_access = filepath.AsFileInfo().IsFileAccessible();
            bool in_queue = file_map.TryGetValue(filepath, out var date);

            Console.WriteLine($"Total Queued files: {file_map}");

            if (!can_access)
            {
                logger.Information(
                    $"[yellow] Cannot access {filename} right now... [/]\n");

                await Task.Delay(250);
                await bg_queue.Produce(fs_event);
            }

            if (in_queue && can_access)
            {
                logger.Information(
                    $"File '{filepath}' will now be processed...");

                await todos.ProcessFileAsync(filepath);

                file_map.Dump($"{file_map.Count} files remaining in the queue", printFn: logger.Information);
            }
        }
    }
}