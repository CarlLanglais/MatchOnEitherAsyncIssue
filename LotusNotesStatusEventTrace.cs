using System;
using System.Collections.Generic;

namespace MatchOnEitherAsyncIssue
{
    public class LotusNotesStatusEventTrace
    {
        internal static LotusNotesStatusEventTrace CreateErroredTraceFromEvent(LotusNotesStatusMessage message, ConsumeResult<string, LotusNotesStatusMessage> consumedMessage, List<CommonMessage> list)
        {
            throw new NotImplementedException();
        }

        internal static LotusNotesStatusEventTrace CreateSuccessTraceFromEvent(LotusNotesStatusMessage message, ConsumeResult<string, LotusNotesStatusMessage> consumedMessage, List<CommonMessage> list)
        {
            throw new NotImplementedException();
        }
    }
}