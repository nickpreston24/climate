using System.Collections.Concurrent;
using System.Diagnostics;
using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using Serilog.Core;

namespace climate;

public class Listener
{
    private readonly List<FileSystemWatcher> watchers = new();
    private readonly Logger logger;
    private readonly BgQueue BgQueue;
    private readonly ConcurrentDictionary<string, DateTime> file_map;
    private readonly ArgsMap arguments;
    private string cwd = Directory.GetCurrentDirectory();
    private readonly TodoService todos;
    private bool debug;
    private readonly ToolSettings tool;

    private static string Name => nameof(Listener);

    public Listener(
        Logger logger
        , ArgsMap arguments
        , ToolSettings toolSettings
        , BgQueue BgQueue
        , TodoService todos
        , ConcurrentDictionary<string, DateTime> map)
    {
        this.tool = toolSettings;
        this.debug = arguments.HasFlag("--debug");
        this.logger = logger;

        this.BgQueue = BgQueue;
        this.file_map = map;
        this.arguments = arguments;
        this.todos = todos;

        logger.Information("Setting up Listeners ...");
    }

    public async Task Setup()
    {
        logger.Information($"{nameof(Setup)} :>> {Setup}");

        // default, sample setup to watch .txt and .md files for changes.  todo: replace with your custom implementations
        string mask = "*.txt,*.md";
        var grepper = new Grepper()
        {
            Recursive = true, DirectoryPattern = DirectoryBlackLists.DeepFolders.CompiledRegex,
            FileSearchMask = mask
        };

        var watcher = CreateWatcher(grepper);
        watchers.Add(watcher);

        logger.Information($"{nameof(Listener)} setup complete.");
    }


    private FileSystemWatcher CreateWatcher(Grepper grepper)
    {
        var watch = Stopwatch.StartNew();

        string dir = grepper.RootPath;
        string[] filters = grepper.FileSearchMask.Split(',');

        string watch_path = dir.NotEmpty() && dir.IsDirectory() ? dir : cwd;

        logger.Information($"{nameof(watch_path)} :>> {watch_path}");

        var watcher = new FileSystemWatcher();
        watcher.IncludeSubdirectories = true;
        watcher.Path = watch_path;
        watcher.NotifyFilter =
            NotifyFilters.FileName |
            NotifyFilters.CreationTime |
            NotifyFilters.LastWrite |
            NotifyFilters.Size;

        if (debug) watcher.Dump(nameof(watcher));

        foreach (var filter in filters)
        {
            watcher.Filters.Add(filter);
        }

        if (debug) watcher.Filters.Dump("All filters added to watcher");

        watcher.Created += OnCreate;
        watcher.Changed += OnChange;
        this.logger.Debug(
            $"[{DateTime.Now:O}] {Name}: Listener Instance created.");

        watch.LogTime();

        return watcher;
    }

    /// <summary>
    /// Todo: On file change, get only the lines that HAVE changed.  this will require caching the previous Read.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnChange(object sender, FileSystemEventArgs e)
    {
        // 1. Only when the file gets updated do we want to make refactors.
        if (e.ChangeType != WatcherChangeTypes.Changed)
            return;

        var now = DateTime.Now;
        string filepath = e.FullPath.AsUnixPath();

        logger.Information(
            $"[{now}] {Name}: The following file has been changed. File : {Path.GetFileName(filepath)}");

        bool successful_add = this.file_map.TryAdd(filepath, now);
        if (debug) logger.Information($"{nameof(file_map)} :>> {file_map}");
        if (debug) logger.Information($"add success? :>> {successful_add}");

        BgQueue.Produce(e).Wait();
    }


    private void OnCreate(object sender, FileSystemEventArgs e)
    {
        logger.Information(
            $"[{DateTime.Now:O}] {Name}: The file has been created. File : {e.FullPath.AsUnixPath()}");
        BgQueue.Produce(e).Wait();
    }


    public void Start()
    {
        Parallel.ForEach(watchers,
            watcher => watcher.EnableRaisingEvents = true);
        // watchers.EnableRaisingEvents = true;
    }
}