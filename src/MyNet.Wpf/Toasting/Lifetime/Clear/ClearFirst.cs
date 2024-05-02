// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace MyNet.Wpf.Toasting.Lifetime.Clear
{
    public class ClearFirst : IClearStrategy
    {
        public IEnumerable<Toast> GetToastsToRemove(IEnumerable<Toast> toasts)
        {
            if (!toasts.Any())
            {
                return [];
            }

            var lastMessage = toasts.First();

            return [lastMessage];
        }
    }
}
