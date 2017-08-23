using System.Threading.Tasks;
using paramore.brighter.commandprocessor;

namespace Paramore.Brighter.MessageViewer.Tests.TestDoubles
{
    public class FakeMessageProducer : IAmAMessageProducer, IAmAMessageProducerAsync
    {
        public bool MessageWasSent { get; set; }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { }

        public Task SendAsync(Message message)
        {
            var tcs = new TaskCompletionSource<Message>();
            Send(message);
            tcs.SetResult(message);
            return tcs.Task;
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public void Send(Message message)
        {
            MessageWasSent = true;
        }
    }
}
