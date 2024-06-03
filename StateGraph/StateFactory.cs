using System.Collections.Generic;
using EC.StateGraph;

namespace EC
{
    public class StateFactory : Singleton<StateFactory>
    {
        public override void Init()
        {
            base.Init();
        }

        public override void Dispose()
        {
            
        }

        public Dictionary<StateEnum, State> GetStates(Entity entt, EntityType ettType)
        {
            IStateGraph sg;
            Dictionary<StateEnum, State> states = new Dictionary<StateEnum, State>();
            switch (ettType)
            {
                case EntityType.Character:
                    sg = new SGPlayer();
                    states = sg.CreateStates(entt);
                    CommonState.AddAttackState(entt, states);
                    break;
            }

            return states;
        }
    }
}