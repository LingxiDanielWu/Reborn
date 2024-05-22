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
