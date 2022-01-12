using System.Threading.Tasks;

namespace MatchOnEitherAsyncIssue
{
    public interface IEventTraceDal
    {
        Task CreateTraceRecordAsync(LotusNotesStatusEventTrace eventTrace);
    }
}