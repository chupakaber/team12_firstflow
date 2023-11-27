using System;
using System.Collections.Generic;
using System.Reflection;

namespace Scripts
{
    public class EventBus
    {
        public List<ISystem> Systems = new List<ISystem>();
        private Dictionary<Type, List<Reciever>> Recievers = new Dictionary<Type, List<Reciever>>();
        public void CallEvent(IEvent newEvent)
        {
            if (Recievers.TryGetValue(newEvent.GetType(), out var recievers))
            {
                foreach (var reciever in recievers) 
                { 
                    reciever.MethodInfo.Invoke(reciever.System, new object[] { newEvent });
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
                            Recievers.TryAdd(key, new List<Reciever>());
                            Recievers[key].Add(reciever); 
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
