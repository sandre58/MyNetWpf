// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.UI.Busy;

namespace MyNet.Wpf.Busy
{
    public class BusyServiceFactory : IBusyServiceFactory
    {
        public IBusyService Create() => new BusyService();
    }
}
