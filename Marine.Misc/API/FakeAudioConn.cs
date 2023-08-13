using Mirror;
using System;

namespace Marine.Misc.API
{
    public class FakeAudioConn : NetworkConnectionToClient
    {
        public FakeAudioConn(int connectionId) : base(connectionId) { }

        public override string address { get; } = "localhost";

        public override void Send(ArraySegment<byte> segment, int channelId = 0) { }

        public override void Disconnect() { }
    }
}
