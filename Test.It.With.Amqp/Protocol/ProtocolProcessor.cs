using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Test.It.With.Amqp.NetworkClient;

namespace Test.It.With.Amqp.Protocol
{
    public class ProtocolProcessor
    {
        private readonly INetworkClient _networkClient;
        private readonly IProtocol _protocol;
        private bool _startupPhase = true;

        public ProtocolProcessor(INetworkClient networkClient, IProtocol protocol)
        {
            _networkClient = networkClient;
            _protocol = protocol;
        }

        public void Process(AmqpReader reader)
        {
            if (_startupPhase)
            {
                var header = ProtocolHeader.ReadFrom(reader);

                if (header.Version.Major == _protocol.Version.Major &&
                    header.Version.Minor == _protocol.Version.Minor &&
                    header.Version.Revision == _protocol.Version.Revision &&
                    header.IsValid)
                {
                    // todo: Default, need to check if callback is registered in the test framework
                    var start = new Connection.Start
                    {
                        VersionMajor = new Octet((byte)header.Version.Major),
                        VersionMinor = new Octet((byte)header.Version.Minor),
                        ServerProperties = new PeerProperties(new Dictionary<string, object>
                        {
                            { "host", "localhost"},
                            { "product", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>()?.Product },
                            { "version", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyVersionAttribute>()?.Version },
                            { "platform", Enum.GetName(typeof(PlatformID), Environment.OSVersion.Platform) },
                            { "copyright", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright },
                            { "information", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description }
                        }),
                        Locales = new Longstr(Encoding.UTF8.GetBytes("en_US")),
                        Mechanisms = new Longstr(Encoding.UTF8.GetBytes("PLAIN"))
                    };

                     // todo: need to define a statemachine keeping track on expected responses

                    using (var stream = new MemoryStream())
                    {
                        using (var writer = new AmqpWriter(stream))
                        {
                            var frame = new Frame(Constants.FrameMethod, 0, start);
                            frame.WriteTo(writer);
                        }

                        var bytes = stream.ToArray();
                        _networkClient.Send(bytes, 0, bytes.Length);
                    }
                }
                else
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var writer = new AmqpWriter(stream))
                        {
                            var protocolHeader = new ProtocolHeader(_protocol.Version);
                            protocolHeader.WriteTo(writer);
                        }

                        var bytes = stream.ToArray();
                        _networkClient.Send(bytes, 0, bytes.Length);
                    }

                    Close();
                }

            }
        }

        private void Close()
        {
            _networkClient.Dispose();
        }

    }
}