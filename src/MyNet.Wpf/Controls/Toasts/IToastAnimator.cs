// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

namespace MyNet.Wpf.Controls.Toasts
{
    public interface INotificationAnimator
    {
        void Setup();
        void PlayShowAnimation();
        void PlayHideAnimation();
    }
}
