// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyNet.Observable;
using MyNet.Observable.Attributes;
using MyNet.Observable.Translatables;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Utilities;
using MyNet.Utilities.Generator;
using MyNet.Utilities.Sequences;
using MyNet.Wpf.TestApp.TacticEditor;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class TacticEditorViewModel : NavigableWorkspaceViewModel
    {
        public IReadOnlyCollection<TacticPosition> TacticPositions { get; } = Enumeration.GetAll<Position>().Select(x => new TacticPosition(x)).OrderBy(x => x.Position).ToList().AsReadOnly();

        public IEnumerable? TacticPositionsReadOnly { get; set; }

        public IReadOnlyCollection<PlayerPosition> PlayerPositions { get; } = Position.GetPlayerPositions().Select(x => new PlayerPosition(x)).ToList().AsReadOnly();

        public IEnumerable? PlayerPositionsReadOnly { get; set; }
    }

    internal class PlayerPosition : IPositionWrapper
    {
        public Position Position { get; }

        public double OffsetX { get; set; }

        public double OffsetY { get; set; }

        public PlayerPosition(Position position) => Position = position;
    }

    internal class TacticPosition : EditableObject, IPositionWrapper
    {
        public static readonly AcceptableValueRange<int> AcceptableRangeNumber = new(1, 99);
        public static readonly AcceptableValueRange<int> AcceptableRangeOffset = new(-100, 100);

        [IsRequired]
        public Position Position { get; }

        public AcceptableValue<int> Number { get; } = new AcceptableValue<int>(AcceptableRangeNumber);

        public double OffsetX { get; set; }

        public double OffsetY { get; set; }

        public TacticPosition(Position position)
        {
            Position = position;
            Number.Value = RandomGenerator.Int(AcceptableRangeNumber.MinOrDefault(), AcceptableRangeNumber.MaxOrDefault());
        }
    }
}
