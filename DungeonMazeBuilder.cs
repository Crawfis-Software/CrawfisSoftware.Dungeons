using CrawfisSoftware.Collections.Graph;
using CrawfisSoftware.Maze;

using System;

namespace CrawfisSoftware.Dungeons
{
    /// <summary>
    /// Maze builder extensions that rasterize a <c>DungeonGraph</c>'s, <c>GridRoom</c>'s and <c>PassageRasterizerType</c> connections.
    /// </summary>
    public static class DungeonMazeBuilder
    {
        /// <summary>
        /// Take a dungeon graph with <c>GridRoom<typeparamref name="R"/></c> node data and <c>GridPassageConnectionData<typeparamref name="C"/></c> connection data and rasterize it into a maze.
        /// </summary>
        /// <typeparam name="N">The type used for node labels</typeparam>
        /// <typeparam name="E">The type used for edge weights</typeparam>
        /// <typeparam name="R">The type used for room data</typeparam>
        /// <typeparam name="C">The type used for connection data</typeparam>
        /// <param name="mazeBuilder">The underlying MazeBuilder</param>
        /// <param name="dungeonGraph">A dungeon graph or <c>GridRoom</c>'s and <c>GridPassageConnectionData</c>.</param>
        /// <param name="preserveExistingCells">Boolean indicating whether to only replace maze cells that are undefined.</param>
        public static void RasterizeDungeon<N, E, R, C>(this IMazeBuilder<N, E> mazeBuilder, DungeonGraph<GridRoom<R>, GridPassageConnectionData<C>> dungeonGraph, bool preserveExistingCells = false)
        {
            foreach (var roomId in dungeonGraph.Nodes)
            {
                var room = dungeonGraph.GetNodeLabel(roomId);
                mazeBuilder.CarveGridRoom(room.RoomData);
            }
            //bool _shortestPathCalculated = false;
            MazePathCost<N, E> mazePathCost = null;
            foreach (var connection in dungeonGraph.Edges)
            {
                var passageData = connection.Value;
                var room1 = dungeonGraph.GetNodeLabel(connection.From).RoomData;
                var room2 = dungeonGraph.GetNodeLabel(connection.To).RoomData;
                PassageRasterizerType rasterizerType = GetRasterizationType(mazeBuilder.RandomGenerator, passageData.ConnectData.PassageRasterizer);
                if (rasterizerType == PassageRasterizerType.ShortestPathUsingExisting && mazePathCost == null)
                {
                    mazePathCost = new MazePathCost<N, E>(mazeBuilder);
                }
                CarveCorridor(mazeBuilder, rasterizerType, room1, room2, mazePathCost);
            }
        }

        /// <summary>
        /// Carve out a rectangular room.
        /// </summary>
        /// <typeparam name="N">The type of the node labels in the graph.</typeparam>
        /// <typeparam name="E">The type of the edge labels in the graph.</typeparam>
        /// <typeparam name="R">The type of the room data.</typeparam>
        /// <param name="mazeBuilder">The underlying MazeBuilder</param>
        /// <param name="rasterizer">How to carve the path.</param>
        /// <param name="room1">A <c>GridRoom</c> to carve from.</param>
        /// <param name="room2">A <c>GridRoom</c> to carve to.</param>
        /// <param name="mazePathCost">An <c>MazePathCost</c> used to determine the cost of an edge (if needed - can be null).</param>
        public static void CarveCorridor<N, E, R>(this IMazeBuilder<N, E> mazeBuilder, PassageRasterizerType rasterizer, GridRoom<R> room1, GridRoom<R> room2, MazePathCost<N, E> mazePathCost = null)
        {
            switch (rasterizer)
            {
                case PassageRasterizerType.Elbow:
                    mazeBuilder.CarveElbowPassage(room1, room2);
                    break;
                //case PassageRasterizerType.ShortestPathBetweenRooms:
                //    CarvePathFromSource(gridData.Item2);
                //    break;
                case PassageRasterizerType.ShortestPathUsingExisting:
                    mazeBuilder.CarveShortestPath(room1, room2, mazePathCost.EdgeCostDelegate);
                    break;
            }
        }

        /// <summary>
        /// Carve out a rectangular room.
        /// </summary>
        /// <typeparam name="N">The type of the node labels in the graph.</typeparam>
        /// <typeparam name="E">The type of the edge labels in the graph.</typeparam>
        /// <typeparam name="R">The type of the room data.</typeparam>
        /// <param name="mazeBuilder">The underlying MazeBuilder</param>
        /// <param name="room">A <c>GridRoom</c> to carve from.</param>
        public static void CarveGridRoom<N, E, R>(this IMazeBuilder<N, E> mazeBuilder, GridRoom<R> room)
        {
            int width = mazeBuilder.Width;
            int left = room.minX;
            int right = left + room.width - 1;
            int bottom = room.minY;
            int top = bottom + room.height - 1;
            // Set corners
            mazeBuilder.AddDirectionExplicitly(left, bottom, Direction.N | Direction.E);
            mazeBuilder.AddDirectionExplicitly(left, top, Direction.S | Direction.E);
            mazeBuilder.AddDirectionExplicitly(right, bottom, Direction.N | Direction.W);
            mazeBuilder.AddDirectionExplicitly(right, top, Direction.S | Direction.W);
            // Top and bottom rows
            for (int i = left + 1; i < right; i++)
            {
                mazeBuilder.AddDirectionExplicitly(i, bottom, Direction.W | Direction.N | Direction.E);
                mazeBuilder.AddDirectionExplicitly(i, top, Direction.W | Direction.E | Direction.S);
            }
            // Left and right columns
            for (int j = bottom + 1; j < top; j++)
            {
                mazeBuilder.AddDirectionExplicitly(left, j, Direction.N | Direction.E | Direction.S);
                mazeBuilder.AddDirectionExplicitly(right, j, Direction.W | Direction.N | Direction.S);
            }
            // Middle
            for (int i = left + 1; i < right; i++)
            {
                for (int j = bottom + 1; j < top; j++)
                {
                    mazeBuilder.AddDirectionExplicitly(i, j, Direction.W | Direction.N | Direction.E | Direction.S);
                }
            }
        }

        ///// <summary>
        ///// Using existing path distances, find the furthest paths and carve them.
        ///// </summary>
        ///// <param name="numberOfPathsToCarve">The number of passageways to create.</param>
        //public static void CarveExtraPassagesFurthestAway<N, E>(this MazeBuilderAbstract<N, E> mazeBuilder, int numberOfPathsToCarve = 1, bool preserveExistingCells = false)
        //{
        //    int pathsToCarve = numberOfPathsToCarve;
        //    pathsToCarve = (pathsToCarve >= _pathLengthsFromSource.Count) ? _pathLengthsFromSource.Count - 1 : pathsToCarve;
        //    _pathLengthsFromSource.Sort(TupleComparer);
        //    for (int i = 1; i <= pathsToCarve; i += 2)
        //    {
        //          int room1 = _pathLengthsFromSource[_pathLengthsFromSource.Count - i].Item1;
        //          int room2 = _pathLengthsFromSource[_pathLengthsFromSource.Count - i - 1].Item1;
        //          CarveElbowPassage(mazeBuilder, _roomList[room1], _roomList[room2], preserveExistingCells)
        //    }
        //}

        //private static void CarvePathFromSource(int targetRoom, bool preserveExistingCells = false)
        //{
        //    if (_pathGenerator == null || targetRoom < 0 || targetRoom >= _roomList.Count)
        //        throw new InvalidOperationException("Target room index is out of bounds or ComputePathLengthsFromSource has not been called");
        //    int room1CenterX = _roomList[targetRoom].minX + _roomList[targetRoom].width / 2;
        //    int room1CenterY = _roomList[targetRoom].minY + _roomList[targetRoom].height / 2;
        //    int targetIndex = room1CenterX + room1CenterY * Width;
        //    foreach (var cell in _pathGenerator.GetPath(targetIndex))
        //    {
        //        CarvePassage(cell.From, cell.To);
        //    }
        //}

        /// <summary>
        /// Create a grid path between rooms using a shortest path algorithm.
        /// </summary>
        /// <typeparam name="N">The type of the node labels in the graph.</typeparam>
        /// <typeparam name="E">The type of the edge labels in the graph.</typeparam>
        /// <typeparam name="R">The type of the room data.</typeparam>
        /// <param name="mazeBuilder">The underlying MazeBuilder</param>
        /// <param name="room1">A <c>GridRoom</c> to carve from.</param>
        /// <param name="room2">A <c>GridRoom</c> to carve to.</param>
        /// <param name="edgeCostFunction">An <c>EdgeCostDelegate</c> used to determine the cost of an edge.</param>
        /// <param name="preserveExistingCells">Boolean indicating whether to only replace maze cells that are undefined.</param>
        public static void CarveShortestPath<N, E, R>(this IMazeBuilder<N, E> mazeBuilder, GridRoom<R> room1, GridRoom<R> room2, EdgeCostDelegate<E> edgeCostFunction, bool preserveExistingCells = false)
        {
            int room1CenterX = room1.minX + room1.width / 2;
            int room1CenterY = room1.minY + room1.height / 2;
            int room2CenterX = room2.minX + room2.width / 2;
            int room2CenterY = room2.minY + room2.height / 2;
            int sourceIndex = room1CenterX + room1CenterY * mazeBuilder.Width;
            int targetIndex = room2CenterX + room2CenterY * mazeBuilder.Width;
            foreach (var cell in PathQuery<N, E>.FindPath(mazeBuilder.Grid, sourceIndex, targetIndex, edgeCostFunction as EdgeCostDelegate<E>))
            {
                mazeBuilder.CarvePassage(cell.From, cell.To, preserveExistingCells);
            }
        }

        /// <summary>
        /// Create a grid path with two straight line segments.
        /// </summary>
        /// <typeparam name="N">The type of the node labels in the graph.</typeparam>
        /// <typeparam name="E">The type of the edge labels in the graph.</typeparam>
        /// <typeparam name="R">The type of the room data.</typeparam>
        /// <param name="mazeBuilder">The underlying MazeBuilder</param>
        /// <param name="room1">A <c>GridRoom</c> to carve from.</param>
        /// <param name="room2">A <c>GridRoom</c> to carve to.</param>
        /// <param name="preserveExistingCells">Boolean indicating whether to only replace maze cells that are undefined.</param>
        public static void CarveElbowPassage<N, E, R>(this IMazeBuilder<N, E> mazeBuilder, GridRoom<R> room1, GridRoom<R> room2, bool preserveExistingCells = false)
        {
            int room1CenterX = room1.minX + room1.width / 2;
            int room1CenterY = room1.minY + room1.height / 2;
            int room2CenterX = room2.minX + room2.width / 2;
            int room2CenterY = room2.minY + room2.height / 2;
            mazeBuilder.CarveHorizontalSpan(room1CenterY, room1CenterX, room2CenterX, preserveExistingCells);
            mazeBuilder.CarveVerticalSpan(room2CenterX, room1CenterY, room2CenterY, preserveExistingCells);
        }

        private static PassageRasterizerType GetRasterizationType(System.Random randomGenerator, PassageRasterizerType rasterizerType)
        {
            if (rasterizerType == PassageRasterizerType.Unspecified)
            {
                int randomRasterizer = randomGenerator.Next(Enum.GetValues(typeof(PassageRasterizerType)).Length) + 1;
                rasterizerType = (PassageRasterizerType)randomRasterizer;
            }

            return rasterizerType;
        }

        private static int TupleComparer(Tuple<int, int, float> x, Tuple<int, int, float> y)
        {
            return x.Item3.CompareTo(y.Item3);
        }
    }
}