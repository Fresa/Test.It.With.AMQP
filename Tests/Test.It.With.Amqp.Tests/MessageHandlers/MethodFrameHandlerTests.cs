using FakeItEasy;
using Should.Fluent;
using Test.It.With.Amqp.MessageClient;
using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Tests.Messages;
using Test.It.With.Amqp.Tests.TestFramework;
using Xunit;
using Xunit.Abstractions;

namespace Given_a_automatic_replying_method_frame_handler
{
    public class When_receiving_a_method_frame_with_a_message_that_has_not_been_subscribed_on : XUnit2SpecificationWithNLog
    {
        public When_receiving_a_method_frame_with_a_message_that_has_not_been_subscribed_on(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        private MethodFrameHandler _handler;
        private MethodFrame _methodFrameSent;
        private MethodFrame _methodFrame;

        protected override void Given()
        {
            var method = A.Fake<IMethod>();
            A.CallTo(() => method.Responses()).Returns(new[] { typeof(RespondingMethod) });
            _methodFrame = new MethodFrame(0, method);
            var sender = A.Fake<ISender<MethodFrame>>();
            A.CallTo(() => sender.Send(A<MethodFrame>.Ignored)).Invokes((MethodFrame methodFrame) => _methodFrameSent = methodFrame);
            _handler = new MethodFrameHandler(true, sender);
        }

        protected override void When()
        {
            _handler.Handle(_methodFrame);
        }

        [Fact]
        public void It_should_send_a_method_frame_on_same_channel_as_the_receiving_frame()
        {
            _methodFrameSent.Channel.Should().Equal((short)0);
        }

        [Fact]
        public void It_should_send_a_default_instance_of_the_first_response_method()
        {
            _methodFrameSent.Message.Should().Be.OfType<RespondingMethod>();
        }
    }
}
