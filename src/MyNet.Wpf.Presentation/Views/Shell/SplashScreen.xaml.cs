// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.UI.ViewModels.Shell;

namespace MyNet.Wpf.Presentation.Views.Shell
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        public SplashScreen(SplashScreenViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
