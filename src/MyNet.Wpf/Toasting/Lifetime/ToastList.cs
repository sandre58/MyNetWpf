// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyNet.Wpf.Toasting.Lifetime
{
    internal class ToastList : ConcurrentDictionary<int, ToastMetadata>
    {
        private int _id = 0;

        internal ToastMetadata Add(Toast toast)
        {
            var id = Interlocked.Increment(ref _id);
            var metaData = new ToastMetadata(id, toast);
            this[id] = metaData;
            return metaData;
        }
    }

    internal class ToastMetadata
    {
        public int Id { get; }

        public TimeSpan CreatedDate { get; }

        public Toast Toast { get; }

        internal ToastMetadata(int id, Toast toast)
        {
            (Id, Toast, CreatedDate) = (id, toast, DateTime.Now.TimeOfDay);
            Toast.Id = id;
        }
    }
}
