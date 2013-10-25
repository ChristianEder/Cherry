using System;
using Cherry.MessageBus.Contracts.Portable;
using Cherry.MessageBus.Tests.Handlers;
using Cherry.MessageBus.Tests.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cherry.MessageBus.Tests
{
    [TestClass]
    public partial class MessageBusTests
    {
        private const string TestName1 = "Test1";

        private IMessageBus _bus;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _bus = CreateMessageBus();
        }

        [TestMethod]
        public void BasicSubscribeUnsubscribe()
        {
            var handlerMock = new Mock<IMessageHandler<HelloMessage>>();
            var message = new HelloMessage(TestName1);

            _bus.Publish(message);
            var subscription = _bus.Subscribe(handlerMock.Object);
            _bus.Publish(message);
            subscription.Dispose();
            _bus.Publish(message);

            handlerMock.Verify(h => h.Handle(message), Times.Once);
        }

        [TestMethod]
        public void MessageBusOnlyHoldsWeakReferences()
        {
            var handlerMock = new Mock<IMessageHandler<HelloMessage>>();

            // The bus is now the only one holding a (hopefully weak)
            // reference to the proxy handler
            _bus.Subscribe(new HelloMessageProxy(handlerMock.Object));

            // If this is the case, garbage collection should collect the proxy handler
            GC.Collect(10, GCCollectionMode.Forced);

            _bus.Publish(new HelloMessage("xyz"));
            _bus.Publish(new HelloMessage("abc"));
            _bus.Publish(new HelloMessage("123"));

            // If the proxy handler has been collected successfully,
            // the mock handler will never have been called
            handlerMock.Verify(h => h.Handle(It.IsAny<HelloMessage>()), Times.Never);
        }

        [TestMethod]
        public void MessageBaseTypeSubscription()
        {
            var handlerMock = new Mock<IMessageHandler<HelloMessage>>();
            var message = new HelloWorldMessage(TestName1);

            _bus.Publish(message);
            var subscription = _bus.Subscribe(handlerMock.Object);
            _bus.Publish(message);
            subscription.Dispose();
            _bus.Publish(message);

            // TODO: this will fail for now. This test is a feature request.
            handlerMock.Verify(h => h.Handle(message), Times.Once);

        }
    }
}
