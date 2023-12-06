using System;
using System.Collections.Generic;
using System.Reflection;

namespace Scripts
{
    public class EventBus
    {
        public List<ISystem> Systems = new List<ISystem>();
        
        private Dictionary<Type, List<Reciever>> _recievers = new Dictionary<Type, List<Reciever>>();
        private object[] _invokingParameters = new object[1];

        public void CallEvent(IEvent newEvent)
        {
            if (_recievers.TryGetValue(newEvent.GetType(), out var recievers))
            {
                foreach (var reciever in recievers)
                {
                    _invokingParameters[0] = newEvent;
                    reciever.MethodInfo.Invoke(reciever.System, _invokingParameters);
                }
            } 
        }

        public void Init()
        {
            foreach (var system in Systems)
            {
                var systemType = system.GetType();
                var methods = systemType.GetMethods();
                foreach (var method in methods) 
                {
                    if (method.Name.CompareTo("EventCatch") == 0)
                    {
                        var methodParams = method.GetParameters();
                        if (methodParams.Length == 1)
                        {
                            var reciever = new Reciever() { MethodInfo = method, System = system };
                            var key = methodParams[0].ParameterType;
                            _recievers.TryAdd(key, new List<Reciever>());
                            _recievers[key].Add(reciever); 
                        }
                    }
                }
            }
        }

        private class Reciever
        {
            public MethodInfo MethodInfo;
            public ISystem System;
        }
    }
}
