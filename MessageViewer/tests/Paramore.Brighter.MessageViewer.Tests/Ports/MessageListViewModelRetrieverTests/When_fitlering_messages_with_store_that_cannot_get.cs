﻿using System;
using FluentAssertions;
using Paramore.Brighter.MessageViewer.Adaptors.API.Resources;
using Paramore.Brighter.MessageViewer.Ports.Domain;
using Paramore.Brighter.MessageViewer.Ports.ViewModelRetrievers;
using Paramore.Brighter.MessageViewer.Tests.TestDoubles;
using Xunit;

namespace Paramore.Brighter.MessageViewer.Tests.Ports.MessageListViewModelRetrieverTests
{
    public class MessageListViewModelRetrieverFilterStoreErrorTests
    {
        private ViewModelRetrieverResult<MessageListModel, MessageListModelError> _result;
        private string storeName = "storeThatCannotGet";
        private MessageListViewModelRetriever _messageListViewModelRetriever;

        public MessageListViewModelRetrieverFilterStoreErrorTests()
        {
            var fakeStoreNotViewer = new FakeMessageStoreViewerWithGetException();
            var modelFactory = new FakeMessageStoreViewerFactory(fakeStoreNotViewer, storeName);
            _messageListViewModelRetriever = new MessageListViewModelRetriever(modelFactory);
        }

        [Fact]
        public void When_fitlering_messages_with_store_that_cannot_get()
        {
            _result = _messageListViewModelRetriever.Filter(storeName, "term");

            //should_return_error
            var model = _result.Result;
            model.Should().BeNull();
            _result.IsError.Should().BeTrue();
            _result.Error.Should().Be(MessageListModelError.StoreMessageViewerGetException);
            _result.Exception.Should().NotBeNull();
            _result.Exception.Should().BeOfType<AggregateException>();
        }
    }
}