using System;
using System.Collections;
using System.Collections.Generic;
using EC.Component;
using UnityEngine;

namespace EC
{
    public enum EntityType
    {
        None,
        Character,
        NPC,
        Item,
        Plant
    }

    public class Entity : MonoBehaviour
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

        private void OnDestroy()
        {
            Dispose();
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

        public void AddEComponent(EComponent c)
        {
            if (!eComponents.TryAdd(c.CType, c))
            {
                Debug.LogError($"Component {c.CType} already exist in {EType}");
            }
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

        public void Update()
        {
            foreach (var pair in eComponents)
            {
                if (pair.Value != null)
                {
                    pair.Value.Tick(Time.deltaTime);
                }
            }
        }

        public void LateUpdate()
        {
            foreach (var pair in eComponents)
            {
                if (pair.Value != null)
                {
                    pair.Value.LateTick(Time.deltaTime);
                }
            }
        }
        
        public void Subscribe(EventType type, Handler action)
        {
            var eventComp = GetEComponent<EventComponent>(ComponentType.Event);
            if (eventComp == null)
            {
                eventComp = new EventComponent();
                eventComp.Attach(this);
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
                    pair.Value.HandleEvent(type, para);
                }
            }
        }

        public void DoTask(Action func, float delayTime = 0)
        {
            if (delayTime == 0)
            {
                func();
                return;
            }

            StartCoroutine(DoTaskCoroutine(func, delayTime));
        }

        private IEnumerator DoTaskCoroutine(Action func, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            func();
        }

        public string GetWeaponTypeName(bool isLower = true)
        {
            WeaponType weaponType = WeaponType.None;
            var weaponComp = GetEComponent<WeaponComponent>(ComponentType.Weapon);
            if (weaponComp == null)
            {
                weaponType = WeaponType.Unarmed;
            }
            else
            {
                weaponType = weaponComp.HoldingWeapon;
            }
            string weaponTypeName = Utils.GetEnumName(typeof(WeaponType), weaponType, isLower);
            return weaponTypeName;
        }
    }
}
