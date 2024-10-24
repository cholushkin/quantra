using Gamelib;

namespace uconsole
{
    public class ConsoleCommandsDebug
    {
        [ConsoleMethod("dbg.destroy", "dbgdestroy", "Destroy all debug objects")]
        public static void DestroyDebug()
        {
            DebugWidgetsManager.Instance.DestroyDebug();
        }
    }
}