using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class DIContainer
    {
        private List<ISystem> Systems = new List<ISystem>();
        private List<Link> Links = new List<Link>();

        public void AddSystem(ISystem system)
        {
            Systems.Add(system);
        }

        public void AddLink<T>(T value, string name) where T : class
        {
            var link = new Link() 
            {
                Name = name,
                Type = typeof(T),
                Value = value   
            };
            Links.Add(link);
        }

        public void Init()
        {
            foreach (var system in Systems)
            {
                var systemType = system.GetType();
                var fields = systemType.GetFields();
                foreach (var link in Links)
                {
                    foreach (var field in fields)
                    {
                        if (link.Type.Equals(field.FieldType))
                        {
                            if (field.Name.CompareTo(link.Name) == 0)
                            {
                                field.SetValue(system, link.Value);
                            }

                        }
                    }

                }
            }
        }

        private class Link
        {
            public string Name;
            public Type Type;
            public object Value;
        }
    }
}
