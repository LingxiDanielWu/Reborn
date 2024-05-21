using System.Collections.Generic;
using EC.Component;
namespace EC
{
    public class CommonState
    {
        public static void AddAttackState(Entity entt, Dictionary<StateEnum, State> states)
        {
            states.Add(StateEnum.Attack, new State(StateEnum.Attack, entt, OnEnterAttack, null, OnExitAttack));
        }

        public static void OnEnterAttack(Entity e, object param = null)
        {
            var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
            if (animComp != null)
            {
                animComp.SetInt("Attack", 2);   
            }
        }

        public static void OnExitAttack(Entity e, object param = null)
        {
            var animComp = e.GetEComponent<AnimatorComponent>(ComponentType.Animator);
            if (animComp != null)
            {
                animComp.SetInt("Attack", 0);   
            }
        }

        public static bool IsAttackBreakable(StateEnum curStateEnum, Entity e = null)
        {
            return false;
        }
    }
}