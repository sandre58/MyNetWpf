// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;

namespace MyNet.Wpf.Helpers
{
    public static class KeyboardHelper
    {
        public static (bool ctrl, bool shift) GetMetaKeyState()
            => ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control, (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
    }
}
