using UnityEngine;
using EC.Component;
namespace EC
{
    public class Player : Entity
    {
        public Player(string resName, GameObject parent = null) : base(EntityType.Player)
        {
            var resComp = AddEComponent<GameObjectComponent>(new GameObjectComponent(this, resName, parent));
            var actionComp = AddEComponent<ActionComponent>(new ActionComponent(this));
            AddEComponent<AnimatorComponent>(new AnimatorComponent(this, resComp.EGameObject.GetComponent<Animator>()));
            AddEComponent<ControllerComponent>(new ControllerComponent(this));
            var stateComp = AddEComponent<StateComponent>(new StateComponent(this));
            AddEComponent<CharacterMoveComponent>(new CharacterMoveComponent(this));
            AddEComponent<CameraComponent>(new CameraComponent(this));
            AddEComponent<EventComponent>(new EventComponent(this));
            
            actionComp.AddDoActionListener(stateComp);
        }
    }
}