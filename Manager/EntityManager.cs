using System.Collections.Generic;
using UnityEngine;

namespace EC.Manager
{
    public class EntityManager : Singleton<EntityManager>
    {
        private Dictionary<EntityType, List<Entity>> entityGroups = new Dictionary<EntityType, List<Entity>>();

        public override void Init()
        {
        }
        
        public override void Dispose()
        {
        }
        
        public override void Tick(float deltaTime)
        {
            foreach (var group in entityGroups)
            {
                for (int i = 0; i < group.Value.Count; i++)
                {
                    var e = group.Value[i];
                    if (e != null)
                    {
                        e.Tick(deltaTime);
                    }
                }
            }
        }

        public override void LateTick(float deltaTime)
        {
            foreach (var group in entityGroups)
            {
                for (int i = 0; i < group.Value.Count; i++)
                {
                    var e = group.Value[i];
                    if (e != null)
                    {
                        e.LateTick(deltaTime);
                    }
                }
            }
        }

        public void AddEntity(Entity e)
        {
            var type = e.EType;
            if (!entityGroups.ContainsKey(type))
            {
                entityGroups.Add(type, new List<Entity>());
            }
            
            var entityList = entityGroups[type];
            if (entityList.IndexOf(e) == -1)
            {
                e.Init();
                entityList.Add(e);
            }
            else
            {
                Debug.Log($"已添加实体  {e.EType.ToString()}");
            }
        }

        public void RemoveEntity(Entity e)
        {
            var type = e.EType;
            if (entityGroups.ContainsKey(type))
            {
                var entityList = entityGroups[type];
                if (entityList.IndexOf(e) == -1)
                {
                    Debug.Log($"删除实体失败 实体不存在  {e.EType.ToString()}");
                }
                else
                {
                    e.Dispose();
                    entityList.Remove(e);
                }
            }
        }
    }
}
