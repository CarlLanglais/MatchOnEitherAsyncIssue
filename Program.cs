using System;
using System.Threading.Tasks;
using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace MatchOnEitherAsyncIssue
{
    // This example is setup for getting signatures corrected and async calls corrected
    // it will compile, but some methods were left w/ NotImplementedExcetions being thrown instead of full logic felshed out

    class Program
    {
        private readonly IEventTraceDal _eventTraceDal;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        protected async Task HandleMEssage(LotusNotesStatusMessage message)
        {
            var errTrace = ErrorStringToErrorTrace.Apply(message).Apply(new ConsumeResult<string, LotusNotesStatusMessage>());
            var successTrace = SuccessStringToSuccessTrace.Apply(message).Apply(new ConsumeResult<string, LotusNotesStatusMessage>());

            // eventTracer is LotusNotesStatusEventTrace, get a warning that invocation could benefit from the use of Task async
            var eventTracer = ValidateStatusEventMessage(message)
                .Match(
                (left) => errTrace(ValidationFailMessage(left.Message)),
                (right) => RouteUpdateMessageToEventTrace(UpdateUnidAsync(message).Result, successTrace, errTrace)
                );

            // evenTraceAsync is valuetuple, need as LotusNotesStatusEventTrace back
            // also get a 'Avoid using async lambda for a void returning delegate type warning'
            var eventTraceAsync = ValidateStatusEventMessage(message)
                .Match(
                (left) => errTrace(ValidationFailMessage(left.Message)),
                async (right) => RouteUpdateMessageToEventTrace(await UpdateUnidAsync(message), successTrace, errTrace)
                );

            // eventTraceMultiLine is LotusNotesStatusEventTrace, but get a 'Avoid using async lambda for a void returning delegate type' warning
            // also adding return in front of RouteUpdateMessageToEventTrace generates a compile error
            var eventTraceMultiLine = ValidateStatusEventMessage(message)
                .Match(
                (left) => errTrace(ValidationFailMessage(left.Message)),
                async (right) =>
                {
                    Either<CommonMessage, bool> update = await UpdateUnidAsync(message);
                    RouteUpdateMessageToEventTrace(update, successTrace, errTrace);
                    }
                );

            // resultTrace becomes a Option<Task<Either<CommonMessage, bool>>>, bit of a code smell
            // also await result gives warning 'Avoid awaiting or returning a Task representing work that was not started within your context ...'
            var maptest = Some(message).Map(async (key) => await UpdateUnidAsync(key));
            var resultTrace = maptest.Map(async (result) => await result);

            await PerisistEventTrace(_eventTraceDal, eventTracer);

            // need this for the whole HandleMEssage to not light up with an async warning
            Console.WriteLine("Hello World");
        }

        public static Func<Either<CommonMessage, bool>, Func<string, LotusNotesStatusEventTrace>, Func<string, LotusNotesStatusEventTrace>, LotusNotesStatusEventTrace> RouteUpdateMessageToEventTrace
            = (message, successTrace, errTrace)
            => message.Match(
                Left: (left) => errTrace(left.Message.ToString()),
                Right: (right) => successTrace(right ? "UNID Updated" : "UNID not Updated"));

        public static async Task<Either<CommonMessage, bool>> UpdateUnidAsync(LotusNotesStatusMessage message)
        {
            var updateUnidResult = await EventRouting.UpdateUnid(message);
            return updateUnidResult;
        }

        public static Func<string, string> ValidationFailMessage
            = (message)
            => $"Validation of event message failed for -> {message}";

        public Func<LotusNotesStatusMessage, ConsumeResult<string, LotusNotesStatusMessage>, string, LotusNotesStatusEventTrace> ErrorStringToErrorTrace
            = (message, consumedMessage, exceptionMessage)
            => LotusNotesStatusEventTrace.CreateErroredTraceFromEvent(message, consumedMessage,
                new System.Collections.Generic.List<CommonMessage> { new CommonMessage { Message = exceptionMessage, Code = "EXCEPTION" } });

        public Func<LotusNotesStatusMessage, ConsumeResult<string, LotusNotesStatusMessage>, string, LotusNotesStatusEventTrace> SuccessStringToSuccessTrace
            = (message, consumedMessage, successMessage)
            => LotusNotesStatusEventTrace.CreateSuccessTraceFromEvent(message, consumedMessage,
                new System.Collections.Generic.List<CommonMessage> { new CommonMessage { Message = successMessage, Code = "SUCCESS" } });

        public Task PerisistEventTrace(IEventTraceDal eventTraceDal, LotusNotesStatusEventTrace eventTrace)
            => eventTraceDal.CreateTraceRecordAsync(eventTrace);

        public Either<CommonMessage, LotusNotesStatusMessage> ValidateStatusEventMessage(LotusNotesStatusMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
