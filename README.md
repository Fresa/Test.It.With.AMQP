# Test.It.With.AMQP
An application and integration independent test framework for the AMQP protocol.

[![Continuous Delivery](https://github.com/Fresa/Test.It.With.AMQP/actions/workflows/ci.yml/badge.svg)](https://github.com/Fresa/Test.It.With.AMQP/actions/workflows/ci.yml)

## Download
https://www.nuget.org/packages/Test.It.With.Amqp/

## Example
The following is an example how an integration test might look like. Be aware that the test is pretty raw specifying communcation on a low level. Most often you'll write the handshaking routines once and abstract them away focusing instead on your high level protocol (consuming and producing messages).

Test an application that publishes messages:
[Given_a_client_application_sending_messages_over_rabbitmq.When_publishing_a_message](https://github.com/Fresa/Test.It.With.RabbitMQ.091/blob/master/Tests/Test.It.With.RabbitMQ.091.Integration.Tests/When_publishing_messages.cs)

Test an application that consumes messages:
[Given_a_client_application_receiving_messages_over_rabbitmq.When_consuming_messages](https://github.com/Fresa/Test.It.With.RabbitMQ.091/blob/master/Tests/Test.It.With.RabbitMQ.091.Integration.Tests/When_consuming_messages.cs)

## Getting Started
[AmqpTestFramework.cs] is the test framework entry point where you set up your test scenario by subscribing on and sending protocol methods. The in-memory version exposes a `ConnectionFactory` property  which you can jack into your application during integration testing in order to divert any AMQP communication to the test framework. This makes the AMQP integration completly isolated in memory during the test session.

It's also possible to configure the test framework to listen on a TCP socket by instantiate it with `AmqpTestFramework.WithSocket(...)`.

### Upgrading from 1.x -> 2.x
Previously the test framework was initialized by instantiating [AmqpTestFramework.cs]:
```c#
var testServer = new AmqpTestFramework(...);
``` 
This has been moved to a static method:
```c#
await using var testFramework = AmqpTestFramework.InMemory(...);
``` 

### Protocol Definitions
You can find the complete protocol definitions here:
- [AMQP 0.9.1](https://github.com/Fresa/Test.It.With.AMQP.091.Protocol)

### Logging
The test framework uses structured logging as logging strategy with the [Log.It](https://github.com/Fresa/Log.It) log abstraction.
