using GameLib.Alg;
using uconsole;

namespace Core.Dbg
{
    public class GamepadVisualizerSingleton : Singleton<GamepadVisualizerSingleton>
    {
        public bool EnabledStatusOnAwake;

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(EnabledStatusOnAwake);
        }

        [ConsoleMethod("input.gamepad.visualize", "gamepadvis", "Visualize gamepad",
             "Enable or disable gamepad debug visualization as overlay"), UnityEngine.Scripting.Preserve]
        public static void SetEnabled(bool flag)
        {
            GamepadVisualizerSingleton.Instance.gameObject.SetActive(flag);
        }
    }
}
