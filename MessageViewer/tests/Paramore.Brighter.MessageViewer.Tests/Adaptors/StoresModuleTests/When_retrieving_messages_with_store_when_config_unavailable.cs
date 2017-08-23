﻿// ***********************************************************************
// Assembly         : paramore.brighter.commandprocessor
// Author           : ianp
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
using Nancy;
using Nancy.Json;
using Nancy.Testing;
using Paramore.Brighter.MessageViewer.Adaptors.API.Modules;
using Paramore.Brighter.MessageViewer.Adaptors.API.Resources;
using Paramore.Brighter.MessageViewer.Ports.ViewModelRetrievers;
using Paramore.Brighter.MessageViewer.Tests.TestDoubles;
using Xunit;

namespace Paramore.Brighter.MessageViewer.Tests.Adaptors.StoresModuleTests
{
    public class RetrieveMessageStoreNoConfigurationTests
    {
        private readonly string _storesUri = "/stores";
        private Browser _browser;
        private static BrowserResponse _result;

        public RetrieveMessageStoreNoConfigurationTests()
        {
            _browser = new Browser(new ConfigurableBootstrapper(with =>
            {
                var listViewRetriever = new FakeActivationListModelRetriever(MessageStoreActivationStateListModelError.GetActivationStateFromConfigError);
                var storeRetriever = FakeMessageStoreViewerModelRetriever.Empty();
                var messageRetriever = FakeMessageListViewModelRetriever.Empty();

                with.Module(new StoresNancyModule(listViewRetriever, storeRetriever, messageRetriever));
            }));
        }

        [Fact]
        public void When_retrieving_messages_with_store_when_config_unavailable()
        {
            _result = _browser.Get(_storesUri, with =>
                {
                    with.Header("accept", "application/json");
                    with.HttpRequest();
                })
                .Result;

            //should_return_500_Server_error
            _result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            //should_return_json
            _result.ContentType.Should().Contain("application/json");
            //should_return_error
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<MessageViewerError>(_result.Body.AsString());

            model.Should().NotBeNull();
            model.Message.Should().Contain("Mis-configured");
        }
   }
}