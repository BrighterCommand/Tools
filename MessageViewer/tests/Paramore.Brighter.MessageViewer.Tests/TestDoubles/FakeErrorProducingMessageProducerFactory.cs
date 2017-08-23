using System;
using paramore.brighter.commandprocessor;

namespace Paramore.Brighter.MessageViewer.Tests.TestDoubles
{
    internal class FakeErrorProducingMessageProducerFactory : IAmAMessageProducerFactory
    {
        public IAmAMessageProducer Create()
        {
            throw new NotImplementedException();
        }
    }
}