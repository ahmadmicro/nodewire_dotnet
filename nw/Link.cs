using System;
using System.Threading.Tasks;

namespace nodewire
{
    public class Link
    {
        public Link()
        {

        }

        public virtual void send(PlainMessage message)
        {

        }

        public virtual async Task<PlainMessage> Receive() {
            await Task.Delay(1);
            return new PlainMessage();
        } 
    }
}
