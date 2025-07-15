using Serilog.Core;

namespace climate;

public class Application
{
    private readonly Logger logger;

    private readonly CliScaffolder scaffolder;

    public Application(Logger logger
        , CliScaffolder scaffolder
    )
    {
        this.logger = logger;
        this.scaffolder = scaffolder;
    }

    public async Task Run()
    {
        await scaffolder.Run();
    }
}