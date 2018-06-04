using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace nodewire
{
    public class PlainMessage
    {
        public String address;
        public String command;
        public List<String> parameters = new List<string>();
        private String port;
        private dynamic _value;
        public String sender;

        public PlainMessage()
        {

        }

        public PlainMessage(string msg)
        {
            Parse(msg);
        }

        public string Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
                if (parameters.Count >= 1)
                    parameters[0] = value;
                else
                    parameters.Add(value);
            }
        }

        public dynamic Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                if (parameters.Count >= 2)
                    parameters[1] = value.ToString();
                else
                {
                    if (parameters.Count == 0) parameters.Add(""); //add dummy command
                    parameters.Add(value.ToString());
                }
            }
        }


        public override string ToString()
        {
            string p = "";
            foreach (var el in parameters)
                p += el + " ";
            return address + " " + command + " " + p + sender;
        }

        private char opposite(char c)
        {
            if (c == '{') return '}';
            if (c == '[') return ']';
            if (c == '(') return ')';
            return c;
        }


        public void Parse(String msg)
        {
            List<string> tokens = new List<string>();
            String token = "";
            int tokcount = 0;
            var sep = '\0';
            Boolean isparam = false;
            for (int i = 0; i < msg.Length; i++)
            {
                var c = msg[i];
                if (@" '[({""".Contains(c.ToString()) && sep == '\0')
                {
                    if (token.Length != 0 && c == ' ')
                    {
                        if (token == "=" || isparam)
                        {
                            isparam = !isparam;
                            tokens[tokens.Count - 1] += token;
                        }
                        else
                            tokens.Add(token);
                        token = "";
                        sep = '\0';
                        tokcount = 0;
                    }
                    else if (c != ' ')
                    {
                        sep = c;
                        tokcount += 1;
                    }
                }
                else if (@" '[({""".Contains(c.ToString()) && c == sep)
                {
                    tokcount += 1;
                    token += c;
                }
                else if (@" '])}""".Contains(c.ToString()) && c == opposite(sep))
                {
                    tokcount -= 1;
                    if (tokcount == 0)
                    {
                        if (token == "=" || isparam)
                        {
                            isparam = !isparam;
                            tokens[tokens.Count - 1] += token;
                        }
                        else
                            tokens.Add(sep + token + c);
                        token = "";
                        sep = '\0';
                    }
                    else
                        token += c;
                }
                else
                    token += c;
            }
            if (token != "") tokens.Add(token);

            address = tokens[0];
            command = tokens[1];
            sender = tokens[tokens.Count - 1];
            if(tokens.Count>3) parameters = tokens.GetRange(2, tokens.Count - 3);

            if (tokens.Count == 5 && (command=="portvalue" || command=="set"))
            {
                int iv; float fv;
                port = parameters[0];
                if (parameters[1].StartsWith("{"))
                    _value = JObject.Parse(parameters[1]);
                else if (parameters[1].StartsWith("["))
                    _value = JArray.Parse(parameters[1]);
                else if (int.TryParse(parameters[1], out iv))
                {
                    _value = iv;
                }
                else if (float.TryParse(parameters[1], out fv))
                {
                    _value = fv;
                }
                else
                    _value = parameters[1];
            }
            else if(tokens.Count==4 && command =="get")
            {
                port = parameters[0];
            }

        }
    }
}
