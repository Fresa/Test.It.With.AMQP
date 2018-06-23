# Test.It.With.AMQP
An application and integration independent test framework for the AMQP protocol.

[![Build status](https://ci.appveyor.com/api/projects/status/an6ehc4093jii5s9?svg=true)](https://ci.appveyor.com/project/Fresa/test-it-with-amqp/)

[![Build history](https://buildstats.info/appveyor/chart/Fresa/test-it-with-amqp)](https://ci.appveyor.com/project/Fresa/test-it-with-amqp/history)

## Download
https://www.nuget.org/packages/Test.It.With.Amqp/

## Example
The following is an example how an integration test might look like. Be aware that the test is pretty raw specifying communcation on a low level. Most often you'll write the handshaking routines once and abstract them away focusing instead on your high level protocol (consuming and producing messages).
[Given_a_client_application_sending_messages_over_rabbitmq When_publishing_a_message][TestExample]

## Getting Started
[AmqpTestFramework.cs] is the test framework entry point where you set up your test scenario by subscribing on and sending protocol methods. It exposes a `ConnectionFactory` property  which you can jack into your application during integration testing in order to divert any AMQP communication to the test framework. This makes the AMQP integration completly isolated in memory during the test session.

### Protocol Definitions
You can find the complete protocol definitions here:
- [AMQP 0.9.1][Amqp091ProtocolDefinitionRepository]

### Logging
The test framework uses structured logging as logging strategy with the [Log.It][LogItRepository] log abstraction.

[AmqpTestFramework.cs]: <https://github.com/Fresa/Test.It.With.AMQP/blob/master/Test.It.With.Amqp/AmqpTestFramework.cs>
[Amqp091ProtocolDefinitionRepository]:
<https://github.com/Fresa/Test.It.With.AMQP.091.Protocol>
[LogItRepository]:
<https://github.com/Fresa/Log.It>
[TestExample]:
https://github.com/Fresa/Test.It.With.RabbitMQ.091/blob/master/Tests/Test.It.With.RabbitMQ.091.Integration.Tests/When_publishing_messages.cs
