using System;
using System.Collections.Generic;

namespace EC
{
    public class TimeEvent
    {
        public int Time;
        public Action<Entity> fn;
        public TimeEvent(int time, Action<Entity> func)
        {
            Time = time;
            fn = func;
        }

        public void Execute(Entity e)
        {
            fn?.Invoke(e);
        }
    }

    public enum EventType
    {
        None,
        JumpUp,
        FallDown,
        FallToGround,
        ControllerDirectionChanged,
        AnimOver,
    }
    
    public delegate void Handler(Entity entity, object data);
    
    public class EventComponent : EComponent
    {
        private Dictionary<EventType, List<Handler>> subscriberList = new Dictionary<EventType, List<Handler>>();
        
        public EventComponent() : base(ComponentType.Event)
        {

        }

        public override void Init()
        {
        }

        public override void Dispose()
        {
            
        }

        public void Subscribe(EventType type, Handler action)
        {
            if (!subscriberList.ContainsKey(type))
            {
                subscriberList.Add(type, new List<Handler>());
            }

            if (!subscriberList[type].Contains(action))
            {
                subscriberList[type].Add(action);
            }
        }

        public void UnSubscribe(EventType type, Handler action)
        {
            if (subscriberList.ContainsKey(type) && subscriberList[type].Contains(action))
            {
                subscriberList[type].Remove(action);
            }
        }

        public void Publish(Entity entity, EventType type, object para)
        {
            if (subscriberList.ContainsKey(type))
            {
                var handlers = subscriberList[type];
                for (int i = 0; i < handlers.Count; i++)
                {
                    handlers[i](entity, para);
                }
            }
        }
    }
}
