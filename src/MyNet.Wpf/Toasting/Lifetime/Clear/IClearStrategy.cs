// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MyNet.Wpf.Toasting.Lifetime.Clear
{
    public interface IClearStrategy
    {
        IEnumerable<Toast> GetToastsToRemove(IEnumerable<Toast> toasts);
    }
}
