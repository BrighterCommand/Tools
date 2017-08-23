﻿// ***********************************************************************
// Assembly         : paramore.brighter.commandprocessor
// Author           : ian
// Created          : 25-03-2014
//
// Last Modified By : ian
// Last Modified On : 25-03-2014
// ***********************************************************************
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#region Licence
/* The MIT License (MIT)
Copyright © 2014 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Nancy;
using Nancy.Testing;
using paramore.brighter.commandprocessor;
using Paramore.Brighter.MessageViewer.Adaptors.API.Modules;
using Paramore.Brighter.MessageViewer.Adaptors.API.Resources;
using Paramore.Brighter.MessageViewer.Ports.Handlers;
using Paramore.Brighter.MessageViewer.Tests.TestDoubles;
using Xunit;

namespace Paramore.Brighter.MessageViewer.Tests.Adaptors.MessagesModuleTests
{
    public class RepostEndpointResendMessageTests
    {
        private Browser _browser;
        private BrowserResponse _result;
        private FakeRepostHandler _fakeRepostHandler;
        private List<Message> _messages;
        private string _idList = "";
        private readonly string _storeName = "testStore";

        private class FakeRepostHandler : IHandleCommand<RepostCommand>
        {
            public void Handle(RepostCommand command)
            {
                WasHandled = true;
                InvokedCommand = command;
            }

            public bool WasHandled { get; private set; }
            public RepostCommand InvokedCommand { get; private set; }
        }

        public RepostEndpointResendMessageTests()
        {
            _messages = new List<Message>
            {
                new Message(new MessageHeader(Guid.NewGuid(), "MyTopic1", MessageType.MT_COMMAND), new MessageBody("")),
                new Message(new MessageHeader(Guid.NewGuid(), "MyTopic2", MessageType.MT_COMMAND), new MessageBody(""))
            };
            _idList = string.Join(",", _messages.Select(m => m.Id.ToString()).ToArray());
            var fakeHandlerFactory = new FakeHandlerFactory();
            _fakeRepostHandler = new FakeRepostHandler();
            fakeHandlerFactory.Add(_fakeRepostHandler);
            _browser = new Browser(new ConfigurableBootstrapper(with =>
            {
                var messageListViewModelRetriever = new FakeMessageListViewModelRetriever(new MessageListModel(_messages));
                with.Module(new MessagesNancyModule(messageListViewModelRetriever, fakeHandlerFactory));
            }));
        }


        [Fact]
        public void When_reposting_some_messages()
        {
            _result = _browser.Post(string.Format("/messages/{0}/repost/{1}", _storeName, _idList),
                    with =>
                    {
                        with.Header("content-type", "application/json");
                        with.HttpRequest();
                    })
                .Result;

            //should_return_200_OK
            _result.StatusCode.Should().Be(HttpStatusCode.OK);
            //should_invoke_handler_from_factory
            _fakeRepostHandler.WasHandled.Should().BeTrue();
            //should_invoke_handler_with_store_and_passed_ids
            var command = _fakeRepostHandler.InvokedCommand;
            command.Should().NotBeNull();
            command.StoreName.Should().Be(_storeName);
            command.MessageIds.Contains(_messages[0].Id.ToString()).Should().BeTrue();
            command.MessageIds.Contains(_messages[1].Id.ToString()).Should().BeTrue();
        }
   }
}
