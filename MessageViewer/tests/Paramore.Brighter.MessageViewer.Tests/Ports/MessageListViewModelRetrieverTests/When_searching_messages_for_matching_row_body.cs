﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using paramore.brighter.commandprocessor;
using Paramore.Brighter.MessageViewer.Adaptors.API.Resources;
using Paramore.Brighter.MessageViewer.Ports.Domain;
using Paramore.Brighter.MessageViewer.Ports.ViewModelRetrievers;
using Paramore.Brighter.MessageViewer.Tests.TestDoubles;
using Xunit;

namespace Paramore.Brighter.MessageViewer.Tests.Ports.MessageListViewModelRetrieverTests
{
    public class MessageListModelRetrieverFilterOnBodyTests
    {
        private MessageListViewModelRetriever _messageListViewModelRetriever;
        private ViewModelRetrieverResult<MessageListModel, MessageListModelError> _result;
        private List<Message> _messages;
        private readonly string _storeName = "testStore";

        public MessageListModelRetrieverFilterOnBodyTests()
        {
            _messages = new List<Message>{
                new Message(new MessageHeader(Guid.NewGuid(), "MyTopic1", MessageType.MT_COMMAND), new MessageBody("topic3")),
                new Message(new MessageHeader(Guid.NewGuid(), "MyTopic2", MessageType.MT_COMMAND), new MessageBody(""))};

            var fakeStore = new FakeMessageStoreWithViewer();
            _messages.ForEach(m => fakeStore.Add(m));
            var modelFactory = new FakeMessageStoreViewerFactory(fakeStore, _storeName);
            _messageListViewModelRetriever = new MessageListViewModelRetriever(modelFactory);
        }

        [Fact]
        public void When_searching_messages_for_matching_row_body()
        {
            _result = _messageListViewModelRetriever.Filter(_storeName, "topic3");

            //should_return_expected_model
             var model = _result.Result;

            model.Should().NotBeNull();
            model.MessageCount.Should().Be(1);
        }
   }
}