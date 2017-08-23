using paramore.brighter.commandprocessor;

namespace Paramore.Brighter.MessageViewer.Tests.TestDoubles
{
    internal class FakeMessageProducerFactory : IAmAMessageProducerFactory
    {
        private readonly IAmAMessageProducer _producer;

        public FakeMessageProducerFactory(IAmAMessageProducer producer)
        {
            _producer = producer;
        }

        public IAmAMessageProducer Create()
        {
            return _producer;
        }
    }
}