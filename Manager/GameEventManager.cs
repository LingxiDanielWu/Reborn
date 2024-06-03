using System.Collections.Generic;

namespace EC.Manager
{
    //全局事件监听
    public class GameEventManager : Singleton<GameEventManager>
    {
        private Dictionary<EventType, List<Entity>> registerEntts = new Dictionary<EventType, List<Entity>>();

        public override void Init()
        {
        }

        public override void Dispose()
        {

        }

        public void RegisterListener(EventType type, Entity e)
        {
            if (!registerEntts.ContainsKey(type))
            {
                registerEntts.Add(type, new List<Entity>());
            }

            if (!registerEntts[type].Contains(e))
            {
                registerEntts[type].Add(e);
            }
        }

        public void UnRegisterListener(EventType type, Entity e)
        {
            if (registerEntts.TryGetValue(type, out List<Entity> etts))
            {
                if (etts.Contains(e))
                {
                    etts.Remove(e);
                }
            }
        }

        public void PublishToEntity(EventType type, Entity e, object data)
        {
            if (registerEntts.TryGetValue(type, out List<Entity> etts))
            {
                for (int i = 0; i < etts.Count; i++)
                {
                    etts[i].Publish(e, type, data);
                }
            }
        }
    }
}