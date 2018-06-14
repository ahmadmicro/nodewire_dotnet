using System;
using System.Dynamic;
using System.Collections.Generic;

namespace nodewire
{
    public class Remote: DynamicObject
    {
        Dictionary<string, object> ports = new Dictionary<string, object>();
        String myaddress;
        string username;
        Link _link;

        public Remote(String name, String user, Link link)
        {
            myaddress = name;
            username = user;
            _link = link;

            _link.send(new PlainMessage($"cp subscribe {name} portvalue {user}"));
            _link.send(new PlainMessage($"cp getnode {name} {user}"));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (ports.ContainsKey(binder.Name))
            {
                result = ports[binder.Name];
                return true;
            }
            else if(binder.Name=="address")
            {
                result = myaddress;
                return true;
            }
            else
            {
                result = "Invalid Property!";
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (ports.ContainsKey(binder.Name))
            {
                _link.send(new PlainMessage($"{myaddress} set {binder.Name} {value} {username}"));
                return true;
            }
            else
            {
                return false;
            }
        }

        public dynamic this[string key]
        {
            get
            {
                if (ports.ContainsKey(key))
                {
                    return ports[key];
                }
                else
                    return null;
            }
            set
            {
                if (ports.ContainsKey(key))
                {
                    _link.send(new PlainMessage($"{myaddress} set {key} {value} {username}"));
                }
            }
        }

        public void set(String port, dynamic value)
        {
            ports[port] = value;
        }

    }
}
