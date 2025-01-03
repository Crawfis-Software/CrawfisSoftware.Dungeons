using System;
using System.Collections.Generic;

namespace CrawfisSoftware.Dungeons
{
    /// <summary>
    /// A class to randomly place rooms in a grid.
    /// </summary>
    /// <typeparam name="R">The type used for room data</typeparam>
    /// <typeparam name="C">The type used for connection data</typeparam>
    public class RandomSpatialLayout<R, C>
    {
        private DungeonGraph<R, C> _dungeonGraph;
        private Dictionary<int, GridRoom<R>> _roomList = new Dictionary<int, GridRoom<R>>();

        /// <summary>
        /// Get the width of the grid
        /// </summary>
        public int GridWidth { get; private set; }
        /// <summary>
        /// Get the height of the grid
        /// </summary>
        public int GridHeight { get; private set; }
        /// <summary>
        /// Get or set the outside wall buffer size
        /// </summary>
        public int RoomMoatSize { get; set; } = 1;
        /// <summary>
        /// Get or set the maximum room size to create
        /// </summary>
        public int MaxRoomSize { get; set; } = 9;
        /// <summary>
        /// Get or set the minimum room size to create
        /// </summary>
        public int MinRoomSize { get; set; } = 4;
        /// <summary>
        /// Get or set the type of rasterizer to use for the passage.
        /// </summary>
        public PassageRasterizerType RasterType { get; set; }
        /// <summary>
        /// Get or set the maximum number of tries to place a room.
        /// </summary>
        public int MaxNumberOfTriesPerRoom { get; set; } = 1000;
        /// <summary>
        /// Get or set the random number generator.
        /// </summary>
        public Random RandomGenerator { get; set; }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gridWidth">The width of the desired grid.</param>
        /// <param name="gridHeight">The height of the desired grid.</param>
        /// <param name="dungeonGraph">The initial (abstract) <c>DungeonGrid</c>.</param>
        public RandomSpatialLayout(int gridWidth, int gridHeight, DungeonGraph<R, C> dungeonGraph)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            _dungeonGraph = dungeonGraph;
            RandomGenerator = new Random();
        }
        /// <summary>
        /// Randomly place the rooms in the grid.
        /// </summary>
        /// <returns>A new <c>DungeonGraph></c> with <c>GridRoom</c>'s and <c>GridPassageConnectionData</c> connections.</returns>
        public DungeonGraph<GridRoom<R>, GridPassageConnectionData<C>> RandomRoomPlacement()
        {
            var newDungeonBuilder = new DungeonGraphBuilder<GridRoom<R>, GridPassageConnectionData<C>>();
            var roomMapping = new Dictionary<int, int>();
            // Loop over each room in the dungeon graph in some specified order.
            foreach (var roomId in _dungeonGraph.Nodes)
            {
                // Randomly place the room in the grid.
                var roomData = _dungeonGraph.GetNodeLabel(roomId);
                if (TryPlaceRoom(roomData, MaxNumberOfTriesPerRoom, out GridRoom<R> room))
                {
                    _roomList[roomId] = room;
                    // Create a GridRoom with the specified size and location and add it to the new DungeonGraph.
                    // Exits and room weight are ignored for now.
                    int newRoomId = newDungeonBuilder.AddRoom(1, 1, room);
                    roomMapping[roomId] = newRoomId;
                }
                else
                {
                    // Remove the room from the list of rooms, and any connections to it.
                    continue;
                }
                // Create a GridRoom with the specified size and location and add it to the new DungeonGraph.
            }
            // Loop over every connection in the dungeon graph and add it to the newDungeonBuilder.
            foreach (var edge in _dungeonGraph.Edges)
            {
                Connection<C> connection = edge.Value;
                if (_roomList.ContainsKey(connection.Room1.roomID) && _roomList.ContainsKey(connection.Room2.roomID))
                {
                    var passageData = new GridPassageConnectionData<C>(connection.ConnectData, this.RasterType);
                    int room1 = roomMapping[connection.Room1.roomID];
                    int room2 = roomMapping[connection.Room2.roomID];
                    newDungeonBuilder.AddConnection(room1, room2, passageData, connection.EdgeWeight);
                }
            }
            // Get the resulting dungeon graph from the newDungeonBuilder.
            return newDungeonBuilder.Build();
        }

        private bool TryPlaceRoom(AbstractRoom<R> roomData, int numberOfTries, out GridRoom<R> room)
        {
            int roomTry = 0;
            while (roomTry < numberOfTries)
            {
                room = GenerateRoom(roomData);
                roomTry++;
                bool canPlace = CheckForOverlap(room);
                if (canPlace)
                {
                    return true;
                }

            }
            room = default(GridRoom<R>);
            return false;
        }

        private GridRoom<R> GenerateRoom(AbstractRoom<R> abstractRoom)
        {
            int deltaWidth = MaxRoomSize - MinRoomSize + 1;
            int roomWidth = MinRoomSize + RandomGenerator.Next(deltaWidth);
            int roomHeight = MinRoomSize + RandomGenerator.Next(deltaWidth);
            int minimumXCoord = GridWidth - roomWidth;
            int minimumYCoord = GridHeight - roomHeight;
            int minX = RandomGenerator.Next(minimumXCoord);
            int minY = RandomGenerator.Next(minimumYCoord);
            GridRoom<R> room = new GridRoom<R>(minX, minY, roomWidth, roomHeight, abstractRoom.RoomData);
            return room;
        }
        private bool CheckForOverlap(GridRoom<R> room)
        {
            bool canPlace = true;
            foreach (GridRoom<R> placedRoom in _roomList.Values)
            {
                int distance = GridRoom<R>.RoomDistance(placedRoom, room);
                // Ensure they are RoomMoatSize apart.
                if (distance - RoomMoatSize < 0)
                {
                    canPlace = false;
                    break;
                }
            }
            return canPlace;
        }
    }
}