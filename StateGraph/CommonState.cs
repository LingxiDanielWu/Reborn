using System.Collections.Generic;
using EC.Component;
using UnityEngine;

namespace EC
{
    public class CommonState
    {
        public static void AddAttackState(Entity entt, Dictionary<StateEnum, State> states)
        {
            var comp = entt.GetEComponent<StateComponent>(ComponentType.State);
            if (comp != null)
            {
                comp.RegisterActionHandler(ActionType.Attack, () =>
                {
                    comp.GotoState(StateEnum.Attack);
                });
            }

            var attackState = new State(StateEnum.Attack, entt, OnEnterAttack, null, OnExitAttack, IsAttackBusy);
            attackState.RegisterEvent(EventType.AnimOver, (entity, data) =>
            {
                var stateComp = entity.GetEComponent<StateComponent>(ComponentType.State);
                stateComp?.GotoState(StateEnum.Idle);
            });
            states.Add(StateEnum.Attack, attackState);
        }

        public static void OnEnterAttack(Entity e, object param = null)
        {
            var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
            if (animComp != null)
            {
                animComp.Play("Attack", 1);   
            }
            
            var controllerComp = e.GetEComponent<ControllerComponent>(ComponentType.Controller);
            var moveComp = e.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
            if (moveComp != null)
            {
                moveComp.DoAttackMove(controllerComp.JoyStickDir);
            }
        }

        public static void OnExitAttack(Entity e, object param = null)
        {
            var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
            if (animComp != null)
            {
                animComp.Play("Attack", 0);   
            }
            
            var moveComp = e.GetEComponent<CharacterMoveComponent>(ComponentType.CharacterMove);
            if (moveComp != null)
            {
                moveComp.Stop();
            }
        }

        public static bool IsAttackBusy(Entity e)
        {
            return true;
        }
    }
}