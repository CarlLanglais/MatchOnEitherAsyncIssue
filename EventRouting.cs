using LaYumba.Functional;
using System;
using System.Threading.Tasks;

namespace MatchOnEitherAsyncIssue
{
    public class EventRouting
    {
        public static async Task<Either<CommonMessage, bool>> UpdateUnid(LotusNotesStatusMessage message)
        {
            //
            int count = 0;
            await Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine("Method 1");
                    count += 1;
                }
            });

            if (DateTime.Now.Hour < 6)
                return true;
            else
                return new CommonMessage { Message = "Bad Update", Code = "EXCEPTION" };
        }
    }
}