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

using FluentAssertions;
using Nancy.Json;
using Nancy.Testing;
using Paramore.Brighter.MessageViewer.Adaptors.API.Modules;
using Paramore.Brighter.MessageViewer.Adaptors.API.Resources;
using Paramore.Brighter.MessageViewer.Ports.ViewModelRetrievers;
using Paramore.Brighter.MessageViewer.Tests.TestDoubles;
using Xunit;

namespace Paramore.Brighter.MessageViewer.Tests.Adaptors.MessagesModuleTests
{
    public class RetreivingNonExistantMessageStoreTests
    {
        private static string storeName = "storeNamenotInStore";
        private string uri = string.Format("/messages/{0}", storeName);
        private Browser _browser;
        protected BrowserResponse _result;

        public RetreivingNonExistantMessageStoreTests()
        {
            _browser = new Browser(new ConfigurableBootstrapper(with =>
            {
                var messageListViewModelRetriever = new FakeMessageListViewModelRetriever(MessageListModelError.StoreNotFound);
                with.Module(new MessagesNancyModule(messageListViewModelRetriever, new FakeHandlerFactory()));
            }));
        }

        [Fact]
        public void When_retrieving_messages_for_non_existent_store()
        {
            _result = _browser.Get(uri, with =>
                {
                    with.Header("accept", "application/json");
                    with.HttpRequest();
                })
                .Result;

            //should_return_404_NotFound
            _result.StatusCode.Should().Be(Nancy.HttpStatusCode.NotFound);
            //should_return_json
            _result.ContentType.Should().Contain("application/json");
            //should_return_error_detail
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<MessageViewerError>(_result.Body.AsString());
            model.Should().NotBeNull();
            model.Message.Should().Contain("Unknown");
        }

   }
}