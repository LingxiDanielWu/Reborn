using System.Collections.Generic;

namespace EC
{
    public interface IStateGraph
    {
        public Dictionary<StateEnum, State> CreateStates(Entity entt);
    }
}