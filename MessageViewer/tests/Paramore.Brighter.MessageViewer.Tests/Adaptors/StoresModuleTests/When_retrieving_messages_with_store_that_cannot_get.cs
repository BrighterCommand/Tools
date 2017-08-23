﻿using FluentAssertions;
using Nancy.Json;
using Nancy.Testing;
using Paramore.Brighter.MessageViewer.Adaptors.API.Modules;
using Paramore.Brighter.MessageViewer.Adaptors.API.Resources;
using Paramore.Brighter.MessageViewer.Ports.ViewModelRetrievers;
using Paramore.Brighter.MessageViewer.Tests.TestDoubles;
using Xunit;

namespace Paramore.Brighter.MessageViewer.Tests.Adaptors.StoresModuleTests
{
    public class RetreiveMessageStoreReadFailureTests
    {
        private readonly string _storeUri = "/stores/storeName";
        private Browser _browser;
        private BrowserResponse _result;

        public RetreiveMessageStoreReadFailureTests()
        {
            _browser = new Browser(new ConfigurableBootstrapper(with =>
            {
                ConfigureStoreModuleForStoreError(with, MessageStoreViewerModelError.StoreMessageViewerGetException);
            }));
        }

        [Fact]
        public void When_retrieving_messages_with_store_that_cannot_get()
        {
            _result = _browser.Get(_storeUri, with =>
                {
                    with.Header("accept", "application/json");
                    with.HttpRequest();
                })
                .Result;

            //should_return_500_Server_error
            _result.StatusCode.Should().Be(Nancy.HttpStatusCode.InternalServerError);
            //should_return_json
            _result.ContentType.Should().Contain("application/json");
            //should_return_error
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<MessageViewerError>(_result.Body.AsString());

            model.Should().NotBeNull();
            model.Message.Should().Contain("Unable");
        }

        private static void ConfigureStoreModuleForStoreError(
            ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator with,
            MessageStoreViewerModelError messageStoreViewerModelError)
        {
            var listViewRetriever = FakeActivationListModelRetriever.Empty();
            var storeRetriever = new FakeMessageStoreViewerModelRetriever(messageStoreViewerModelError);
            var messageRetriever = FakeMessageListViewModelRetriever.Empty();

            with.Module(new StoresNancyModule(listViewRetriever, storeRetriever, messageRetriever));
        }
    }
}