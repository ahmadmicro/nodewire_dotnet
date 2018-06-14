using System;
using System.Threading.Tasks;
using System.Dynamic;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace nodewire
{
    public class Node : DynamicObject
    {
        String myaddress;
        bool announcing = true;

        List<Remote> nodes = new List<Remote>();
        Dictionary<string, when_delegate> when_list = new Dictionary<string, when_delegate>();

        Link _link;
        Dictionary<string, object> inputs = new Dictionary<string, object>();
        Dictionary<string, object> outputs = new Dictionary<string, object>();
        Dictionary<string, object> getfns = new Dictionary<string, object>();
        Dictionary<string, object> setfns = new Dictionary<string, object>();

        delegate dynamic get_delegate();
        delegate void set_delegate(dynamic p);

        public delegate void when_delegate(PlainMessage msg);

        delegate void dlg_connected();
        dlg_connected fn_connected = null;
        set_delegate fn_got_node = null;

        public Node(String address, Link link, dynamic controller = null)
        {
            _link = link;
            var config = new IniFile("nw.cfg");
            myaddress = config.GetValue("node", "name");
            if (myaddress == null)
            {
                myaddress = address;
                config.SetValue("node", "name", myaddress);
                config.SetValue("node", "id", "none");
                config.Save("nw.cfg");
            }

            if (controller != null)
            {
                MethodInfo[] methodInfos = controller.GetType()
                           .GetMethods(BindingFlags.Public | BindingFlags.Instance);
                var getmethods = from method in methodInfos where method.Name.StartsWith("get_") select method;
                var setmethods = from method in methodInfos where method.Name.StartsWith("on_") select method;
                foreach (var g in getmethods)
                {
                    getfns.Add(g.Name.Substring(4), g.CreateDelegate(typeof(get_delegate), controller));
                }
                foreach (var s in setmethods)
                {
                    setfns.Add(s.Name.Substring(3), s.CreateDelegate(typeof(set_delegate), controller));
                }

                var fncon = from method in methodInfos where method.Name == "connected" select method;
                if(fncon.Count()!=0)
                    fn_connected = fncon.First().CreateDelegate(typeof(dlg_connected), controller);
                var fnnode = from method in methodInfos where method.Name == "got_node" select method;
                if (fnnode.Count() != 0)
                    fn_got_node = fnnode.First().CreateDelegate(typeof(set_delegate), controller);
            }
        }

        public string Inputs
        {
            set
            {
                foreach (var port in value.Split())
                    inputs[port] = null;
            }
        }

        public string Outputs
        {
            set
            {
                foreach (var port in value.Split())
                    outputs[port] = null;
            }
        }

        public Remote ConnectNode(String name)
        {
            Remote r = new Remote(name, myaddress, _link);
            nodes.Add(r);
            return r;
        }

        public void When(String condition, when_delegate func)
        {
            when_list.Add(condition, func);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (inputs.ContainsKey(binder.Name))
            {
                result = inputs[binder.Name];
                return true;
            }
            else if (getfns.ContainsKey(binder.Name))
            {
                dynamic fn = getfns[binder.Name];
                result = fn();
                return true;
            }
            else if (outputs.ContainsKey(binder.Name))
            {
                result = outputs[binder.Name];
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
            if (binder.Name.StartsWith("on_") && inputs.ContainsKey(binder.Name.Substring(3)))
            {
                string port = binder.Name.Substring(3);
                setfns[port] = value;
                return true;
            }
            else if (binder.Name.StartsWith("get_") && outputs.ContainsKey(binder.Name.Substring(4)))
            {
                string port = binder.Name.Substring(4);
                setfns[port] = value;
                return true;
            }
            else if (setfns.ContainsKey(binder.Name))
            {
                dynamic fn = setfns[binder.Name];
                fn(value);
                _link.send(new PlainMessage($"re portvalue {binder.Name} {value} {myaddress}"));
                return true;
            }
            else if (inputs.ContainsKey(binder.Name))
            {
                inputs[binder.Name] = value;
                _link.send(new PlainMessage($"re portvalue {binder.Name} {value} {myaddress}"));
                return true;
            }
            else if (outputs.ContainsKey(binder.Name))
            {
                outputs[binder.Name] = value;
                _link.send(new PlainMessage($"re portvalue {binder.Name} {value} {myaddress}"));
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task process()
        {
            while (true)
            {
                try
                {
                    var result = await _link.Receive();
                    switch (result.command)
                    {
                        case "get":
                            if (result.Port == "name")
                            {
                                var config = new IniFile("nw.cfg");
                                var nodname = config.GetValue("node", "name");
                                _link.send(new PlainMessage { address = result.sender, command = "ThisIs", sender = myaddress });
                            }
                            else if (result.Port == "id")
                            {
                                var config = new IniFile("nw.cfg");
                                var id = config.GetValue("node", "id");
                                _link.send(new PlainMessage($"{result.sender} id {id} {myaddress}"));
                                if (fn_connected != null) fn_connected();
                            }
                            else if (result.Port == "ports")
                            {
                                string p = "";
                                foreach (var el in inputs)
                                    p += el.Key + " ";
                                foreach (var el in outputs)
                                    p += el.Key + " ";

                                _link.send(new PlainMessage($"{result.sender} ports {p}{myaddress}"));
                            }
                            else
                            {
                                if (inputs.ContainsKey(result.Port))
                                {
                                    _link.send(new PlainMessage($"{result.sender} portvalue {result.Port} {inputs[result.Port]} {myaddress}"));
                                }
                                else if (getfns.ContainsKey(result.Port))
                                {
                                    dynamic fn = getfns[result.Port];
                                    _link.send(new PlainMessage($"{result.sender} portvalue {result.Port} {fn()} {myaddress}"));
                                }
                                else if (outputs.ContainsKey(result.Port))
                                {
                                    _link.send(new PlainMessage($"{result.sender} portvalue {result.Port} {outputs[result.Port]} {myaddress}"));
                                }
                            }
                            break;
                        case "set":
                            if (result.Port == "name")
                            {
                                var config = new IniFile("nw.cfg");
                                config.SetValue("node", "name", result.Value);
                                myaddress = result.Value;
                                config.Save("nw.cfg");
                                _link.send(new PlainMessage { address = result.sender, command = "ThisIs", sender = myaddress });
                            }
                            else if (result.Port == "id")
                            {
                                var config = new IniFile("nw.cfg");
                                config.SetValue("node", "id", result.Value);
                                var id = result.Value;
                                config.Save("nw.cfg");
                                _link.send(new PlainMessage($"{result.address} id {id} {myaddress}"));
                            }
                            else
                            {
                                if (setfns.ContainsKey(result.Port))
                                {
                                    dynamic fn = setfns[result.Port];
                                    fn(result.Value);
                                    _link.send(new PlainMessage($"{result.sender} portvalue {result.Port} {result.Value} {myaddress}"));
                                }
                                else if (inputs.ContainsKey(result.Port))
                                {
                                    inputs[result.Port] = result.Value;
                                    _link.send(new PlainMessage($"{result.sender} portvalue {result.Port} {result.Value} {myaddress}"));
                                }
                            }
                            break;
                        case "ping":
                            announcing = true;
                            break;
                        case "ack":
                            announcing = false;
                            break;
                        case "node":
                            {
                                var nodename = result.parameters[1];
                                var node = ((IEnumerable<Remote>)nodes).Cast<dynamic>().Where(p => p.address == nodename);
                                if (node.Count() != 0)
                                {
                                    foreach (var port in result.Value)
                                    {
                                        node.First().set(port.Name, port.Value);
                                    }
                                    if (fn_got_node!=null) fn_got_node(node.First());
                                }
                            }
                            break;
                        case "portvalue":
                            {
                                var nodename = result.sender;
                                var node = ((IEnumerable<Remote>)nodes).Cast<dynamic>().Where(p => p.address == nodename);
                                if (node.Count() != 0)
                                {
                                    node.First().set(result.Port, result.Value);
                                }
                                var cond = result.sender + "." + result.Port;
                                if(when_list.ContainsKey(cond)){
                                    when_list[cond](result);
                                }
                            }
                            break;
                    }
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        public async Task announce()
        {
            while (true)
            {
                await Task.Delay(5000);
                if (announcing) _link.send(new PlainMessage($"cp ThisIs {myaddress}"));
            }
        }

        public async Task Run()
        {
            await Task.WhenAll(process(), announce());
        }
    }
}