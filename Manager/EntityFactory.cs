using UnityEngine;

namespace EC.Manager
{
    public class EntityFactory : Singleton<EntityFactory>
    {
        public override void Init()
        {
        }
        
        public override void Dispose()
        {
        }

        public Entity CreateEntity(EntityType type, GameObject parent = null, string resName = "")
        {
            switch (type)
            {
                case EntityType.Player:
                    Player player = new Player(resName, parent);
                    return player;
                default:
                    return null;
            }
        }
    }
}
