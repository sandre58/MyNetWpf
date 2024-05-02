// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using MyNet.Utilities;

namespace MyNet.Wpf.TestApp.TacticEditor
{
    public class Position : Enumeration<Position>, ISimilar<Position>
    {
        private static int _value = 1;
        public static readonly int RowsCount = 7;
        public static readonly int ColumnsCount = 5;

        public static IEnumerable<Position> GetPlayerPositions()
            => [.. Enumeration.GetAll<Position>().Where(x => x.Side is PositionSide.Left or PositionSide.Right or PositionSide.Center).OrderBy(x => x.Value)];

        // GoalKeeper
        public static readonly Position GoalKeeper = new(nameof(GoalKeeper), PositionType.GoalKeeper, PositionLine.GoalKeeper, PositionSide.Center);

        // Defenders
        public static readonly Position Sweeper = new(nameof(Sweeper), PositionType.Defender, PositionLine.Sweeper, PositionSide.Center);
        public static readonly Position LeftFullBack = new(nameof(LeftFullBack), PositionType.Defender, PositionLine.Defender, PositionSide.Left);
        public static readonly Position CenterLeftBack = new(nameof(CenterLeftBack), PositionType.Defender, PositionLine.Defender, PositionSide.CenterLeft);
        public static readonly Position CenterBack = new(nameof(CenterBack), PositionType.Defender, PositionLine.Defender, PositionSide.Center);
        public static readonly Position CenterRightBack = new(nameof(CenterRightBack), PositionType.Defender, PositionLine.Defender, PositionSide.CenterRight);
        public static readonly Position RightFullBack = new(nameof(RightFullBack), PositionType.Defender, PositionLine.Defender, PositionSide.Right);
        public static readonly Position LeftWingBack = new(nameof(LeftWingBack), PositionType.Defender, PositionLine.DefensiveMidfielder, PositionSide.Left);
        public static readonly Position RightWingBack = new(nameof(RightWingBack), PositionType.Defender, PositionLine.DefensiveMidfielder, PositionSide.Right);

        // Midfielders
        public static readonly Position DefensiveLeftMidfielder = new(nameof(DefensiveLeftMidfielder), PositionType.Midfielder, PositionLine.DefensiveMidfielder, PositionSide.CenterLeft);
        public static readonly Position DefensiveMidfielder = new(nameof(DefensiveMidfielder), PositionType.Midfielder, PositionLine.DefensiveMidfielder, PositionSide.Center);
        public static readonly Position DefensiveRightMidfielder = new(nameof(DefensiveRightMidfielder), PositionType.Midfielder, PositionLine.DefensiveMidfielder, PositionSide.CenterRight);
        public static readonly Position LeftMidfielder = new(nameof(LeftMidfielder), PositionType.Midfielder, PositionLine.Midfielder, PositionSide.Left);
        public static readonly Position CenterLeftMidfielder = new(nameof(CenterLeftMidfielder), PositionType.Midfielder, PositionLine.Midfielder, PositionSide.CenterLeft);
        public static readonly Position CenterMidfielder = new(nameof(CenterMidfielder), PositionType.Midfielder, PositionLine.Midfielder, PositionSide.Center);
        public static readonly Position CenterRightMidfielder = new(nameof(CenterRightMidfielder), PositionType.Midfielder, PositionLine.Midfielder, PositionSide.CenterRight);
        public static readonly Position RightMidfielder = new(nameof(RightMidfielder), PositionType.Midfielder, PositionLine.Midfielder, PositionSide.Right);
        public static readonly Position LeftAttackingMidfielder = new(nameof(LeftAttackingMidfielder), PositionType.Midfielder, PositionLine.AttackingMidfielder, PositionSide.Left);
        public static readonly Position CenterLeftAttackingMidfielder = new(nameof(CenterLeftAttackingMidfielder), PositionType.Midfielder, PositionLine.AttackingMidfielder, PositionSide.CenterLeft);
        public static readonly Position CenterAttackingMidfielder = new(nameof(CenterAttackingMidfielder), PositionType.Midfielder, PositionLine.AttackingMidfielder, PositionSide.Center);
        public static readonly Position CenterRightAttackingMidfielder = new(nameof(CenterRightAttackingMidfielder), PositionType.Midfielder, PositionLine.AttackingMidfielder, PositionSide.CenterRight);
        public static readonly Position RightAttackingMidfielder = new(nameof(RightAttackingMidfielder), PositionType.Midfielder, PositionLine.AttackingMidfielder, PositionSide.Right);

        // Forwards
        public static readonly Position LeftWinger = new(nameof(LeftWinger), PositionType.Forward, PositionLine.Striker, PositionSide.Left);
        public static readonly Position LeftForward = new(nameof(LeftForward), PositionType.Forward, PositionLine.Striker, PositionSide.CenterLeft);
        public static readonly Position Forward = new(nameof(Forward), PositionType.Forward, PositionLine.Striker, PositionSide.Center);
        public static readonly Position RightForward = new(nameof(RightForward), PositionType.Forward, PositionLine.Striker, PositionSide.CenterRight);
        public static readonly Position RightWinger = new(nameof(RightWinger), PositionType.Forward, PositionLine.Striker, PositionSide.Right);

        public PositionType Type { get; }

        public PositionLine Line { get; }

        public PositionSide Side { get; }

        private Position(string name, PositionType type, PositionLine line, PositionSide side)
            : base(name, _value, $"{nameof(Position)}{name}")
        {
            Type = type;
            Line = line;
            Side = side;

            IncrementValue();
        }

        private static void IncrementValue() => _value++;

        public bool IsSimilar(Position? obj)
            => obj is not null
                && obj.Line == Line
                && (obj.Side == Side
                || Side == PositionSide.Center && obj.Side is PositionSide.CenterLeft or PositionSide.CenterRight
                || obj.Side == PositionSide.Center && Side is PositionSide.CenterLeft or PositionSide.CenterRight);
    }
}
