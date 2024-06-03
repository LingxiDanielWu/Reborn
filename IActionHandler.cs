using EC.Component;

namespace EC
{
    public interface IActionHandler
    {
        public void HandleAction(BufferedAction data);
    }
}