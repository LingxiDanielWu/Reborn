using EC.Component;

namespace EC
{
    public interface IActionHandler
    {
        public void HandleAction(ActionType type);
    }
}