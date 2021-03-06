﻿using System;
using JetBrains.Annotations;
using Selkie.Geometry.Shapes;
using SelkieConstants = Selkie.Geometry.Constants;

namespace Selkie.Geometry.Calculators
{
    public class LineDirectionCalculator : ILineDirectionCalculator
    {
        public LineDirectionCalculator()
        {
            Line = Shapes.Line.Unknown;
            Point = Point.Unknown;
            Direction = Constants.TurnDirection.Unknown;
        }

        [NotNull]
        public ILine Line { get; set; }

        [NotNull]
        public Point Point { get; set; }

        public Constants.TurnDirection Direction { get; private set; }

        public void Calculate()
        {
            Direction = Calculate(Line,
                                  Point);
        }

        // ReSharper disable once MethodTooLong
        internal Constants.TurnDirection Calculate([NotNull] ILine line,
                                                   [NotNull] Point point)
        {
            double ax = line.StartPoint.X;
            double ay = line.StartPoint.Y;
            double bx = line.EndPoint.X;
            double by = line.EndPoint.Y;
            double cx = point.X;
            double cy = point.Y;

            Side side = FindSide(ax,
                                 ay,
                                 bx,
                                 by,
                                 cx,
                                 cy);

            switch ( side )
            {
                case Side.Right:
                    return Constants.TurnDirection.Clockwise;

                case Side.Left:
                    return Constants.TurnDirection.Counterclockwise;

                case Side.Unknown:
                    return Constants.TurnDirection.Unknown;

                default:
                    return Constants.TurnDirection.Unknown;
            }
        }

        /*
       * returns:
       * 1 for left turn 
       * -1 means right turn
       * 0 if all three are on a line
       */
        // ReSharper disable once MethodTooLong
        // ReSharper disable once TooManyArguments
        internal Side FindSide(double ax,
                               double ay,
                               double bx,
                               double by,
                               double cx,
                               double cy)
        {
            if ( Math.Abs(bx - ax) < SelkieConstants.EpsilonDistance )
            {
                return FindSideForVerticalLine(ay,
                                               bx,
                                               by,
                                               cx);
            }

            if ( Math.Abs(by - ay) < SelkieConstants.EpsilonDistance )
            {
                return FindSideForHorizontalLine(ax,
                                                 bx,
                                                 by,
                                                 cy);
            }

            double slope = ( by - ay ) / ( bx - ax );
            double yIntercept = ay - ax * slope;
            double cSolution = slope * cx + yIntercept;

            if ( cy > cSolution )
            {
                return bx > ax
                           ? Side.Left
                           : Side.Right;
            }
            if ( cy < cSolution )
            {
                return bx > ax
                           ? Side.Right
                           : Side.Left;
            }
            return Side.Unknown;
        }

        // ReSharper disable once TooManyArguments
        private Side FindSideForHorizontalLine(double ax,
                                               double bx,
                                               double by,
                                               double cy)
        {
            if ( cy < by )
            {
                return bx > ax
                           ? Side.Right
                           : Side.Left;
            }
            if ( cy > by )
            {
                return bx > ax
                           ? Side.Left
                           : Side.Right;
            }
            return Side.Unknown;
        }

        // ReSharper disable once TooManyArguments
        private Side FindSideForVerticalLine(double ay,
                                             double bx,
                                             double by,
                                             double cx)

        {
            if ( cx < bx )
            {
                return by > ay
                           ? Side.Left
                           : Side.Right;
            }
            if ( cx > bx )
            {
                return by > ay
                           ? Side.Right
                           : Side.Left;
            }
            return Side.Unknown;
        }

        #region Nested type: Side

        internal enum Side
        {
            Right = -1,
            Left = 1,
            Unknown = 0
        }

        #endregion
    }
}