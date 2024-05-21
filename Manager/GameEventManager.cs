using System.Collections.Generic;

namespace EC.Manager
{
    public enum GameEventType
    {
        
    }
    
    public class GameEventManager : Singleton<GameEventManager>
    {
        public delegate void Handler(object data);

        private Dictionary<EventType, List<Handler>> subscriberList = new Dictionary<EventType, List<Handler>>();

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

        public void Publish(EventType type, object para)
        {
            if (subscriberList.ContainsKey(type))
            {
                var handlers = subscriberList[type];
                for (int i = 0; i < handlers.Count; i++)
                {
                    handlers[i](para);
                }
            }
        }
    }
}