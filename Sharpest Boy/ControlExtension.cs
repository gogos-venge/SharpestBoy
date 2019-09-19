using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Windows.Forms {
    public static class ControlExtensions {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        public static void Suspend(this Control control) {
            LockWindowUpdate(control.Handle);
        }

        public static void Resume(this Control control) {
            LockWindowUpdate(IntPtr.Zero);
        }

        public static void DoubleBuffered(this Control control, bool enable) {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }
    }
}