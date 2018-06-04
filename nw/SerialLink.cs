using System;

namespace nodewire
{
    public class SerialLink: Link
    {
        // https://github.com/genielabs/serialport-lib-dotnet/

        public SerialLink()
        {
        }

        public override System.Threading.Tasks.Task<PlainMessage> Receive()
        {
            return base.Receive();
        }

        public override void send(PlainMessage message)
        {
            base.send(message);
        }
    }
}
