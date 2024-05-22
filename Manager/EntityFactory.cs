using EC.Component;
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
                    GameObjectComponent resComp = new GameObjectComponent(resName, parent);
                    Player player = resComp.EGameObject.AddComponent<Player>();
                    
                    resComp.Attach(player);
                    new ActionComponent(player).Attach(player);
;                   new StateComponent().Attach(player);
                    new AnimatorComponent(resComp.EGameObject.GetComponent<Animator>()).Attach(player);
                    new ControllerComponent().Attach(player);
                    new CharacterMoveComponent().Attach(player);
                    new CameraComponent().Attach(player);
                    new EventComponent().Attach(player);
                    return player;
                default:
                    return null;
            }
        }
    }
}
