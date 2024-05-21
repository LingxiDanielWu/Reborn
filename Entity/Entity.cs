using System.Collections.Generic;

namespace EC
{
    public enum EntityType
    {
        None,
        Player,
        NPC,
        Item,
        Plant
    }

    public class Entity
    {
        public EntityType EType
        {
            get;
            protected set;
        }

        private Dictionary<ComponentType, EComponent> eComponents = new Dictionary<ComponentType, EComponent>();

        public Entity(EntityType type)
        {
            EType = type;
        }

        public virtual void Init()
        {
            foreach (var pair in eComponents)
            {
                if (pair.Value != null)
                    pair.Value.Init();
            }
        }

        public virtual void Dispose()
        {
            foreach (var pair in eComponents)
            {
                if (pair.Value != null)
                    pair.Value.Dispose();
            }
            eComponents.Clear();
        }

        public T AddEComponent<T>(EComponent c) where T : EComponent
        {
            eComponents.Add(c.CType, c);
            return c as T;
        }

        public void RemoveEComponent(EComponent c)
        {
            eComponents.Remove(c.CType);
        }

        public T GetEComponent<T>(ComponentType type) where T:EComponent
        {
            if (eComponents.TryGetValue(type, out EComponent comp))
                return comp as T;
            
            return null;
        }

        public void Tick(float deltaTime)
        {
            foreach (var pair in eComponents)
            {
                if (pair.Value != null)
                {
                    pair.Value.Tick(deltaTime);
                }
            }
        }

        public void LateTick(float deltaTime)
        {
            foreach (var pair in eComponents)
            {
                if (pair.Value != null)
                {
                    pair.Value.LateTick(deltaTime);
                }
            }
        }
        
        public void Subscribe(EventType type, Handler action)
        {
            var eventComp = GetEComponent<EventComponent>(ComponentType.Event);
            if (eventComp == null)
            {
                eventComp = AddEComponent<EventComponent>(new EventComponent(this));
            }
            
            eventComp.Subscribe(type, action);
        }

        public void UnSubscribe(EventType type, Handler action)
        {
            var eventComp = GetEComponent<EventComponent>(ComponentType.Event);
            if (eventComp != null)
            {
                eventComp.UnSubscribe(type, action);
            }
        }

        public void Publish(Entity entity, EventType type, object para = null)
        {
            var eventComp = GetEComponent<EventComponent>(ComponentType.Event);
            if (eventComp != null)
            {
                eventComp.Publish(entity, type, para);
            }

            foreach (var pair in eComponents)
            {
                if (pair.Value != null)
                {
                    pair.Value.HandleEvent(type, pair);
                }
            }
        }
    }
}
