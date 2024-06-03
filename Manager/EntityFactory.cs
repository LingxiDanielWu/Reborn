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
                case EntityType.Character:
                    GameObjectComponent resComp = new GameObjectComponent(type, resName, parent);
                    Character player = resComp.BindEntity<Character>();
                    resComp.Attach(player);
                    
                    new ActionComponent().Attach(player);
;                   new StateComponent().Attach(player);
                    new AnimatorComponent().Attach(player);
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
