using System.Threading.Channels;

namespace climate;

public sealed class BgQueue
{
    private readonly Channel<FileSystemEventArgs> channel;

    public BgQueue()
    {
        channel = Channel.CreateBounded<FileSystemEventArgs>(
            new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true
            });
    }

    public async Task Produce(FileSystemEventArgs @event)
    {
        await channel.Writer.WriteAsync(@event);
    }

    public async ValueTask<FileSystemEventArgs> Consume(
        CancellationToken cancellationToken)
    {
        return await channel.Reader.ReadAsync(cancellationToken);
    }
}