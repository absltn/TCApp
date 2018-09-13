using System;

namespace TCApp
{
    public static class ControlExtensions
    {
        public static void Invoke(this System.Windows.Forms.Control control, Action action)
        {
            if (control.InvokeRequired) control.Invoke(new System.Windows.Forms.MethodInvoker(action), null);
            else action.Invoke();
        }
    }
}
