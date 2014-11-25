using System;
using System.Reflection;

namespace PEG.Proxies
{
    public class Invocation
    {
        public MethodInfo Method { get; private set; }
        public object ReturnValue { get; set; }
        public object[] Arguments { get; set; }

        private Func<object[], object> implementation;

        public Invocation(MethodInfo method, object[] arguments, Func<object[], object> implementation)
        {
            Method = method;
            Arguments = arguments;
            this.implementation = implementation;
        }

        public void Proceed()
        {
            ReturnValue = implementation(Arguments);
        }
    }
}