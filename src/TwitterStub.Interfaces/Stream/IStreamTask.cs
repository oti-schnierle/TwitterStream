using System.IO;
using System.Threading.Tasks;

namespace TwitterStub.Interfaces.Stream
{
    public interface IStreamTask
    {
        Task<StreamReader> StartAsync(System.Threading.CancellationToken stoppingToken);

    }
}
