using System.Threading.Tasks;

namespace StartWithHangFire.Service
{
    public interface IJobToProcess
    {
        Task InsertUser(string message);

        void JobException(string message);
    }
}
