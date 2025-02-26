using System;
using System.Collections.Generic;

namespace CrawfisSoftware.Dungeons
{
    /// <summary>
    /// A delegate to generate a room in a grid.
    /// </summary>
    /// <typeparam name="R">The type used for room data</typeparam>
    /// <typeparam name="Q">The type used for resulting GridRoom data</typeparam>
    /// <param name="roomData">An AbstractRoom{R}.</param>
    /// <param name="room">Outputs a GridRoom{Q}.</param>
    /// <returns>True is the room was able to be generated. False otherwise.</returns>
    public delegate bool SpatialRoomGeneratorDelegate<R, Q>(AbstractRoom<R> roomData, out GridRoom<Q> room);
    /// <summary>
    /// A class to randomly place rooms in a grid.
    /// </summary>
    /// <typeparam name="R">The type used for room data</typeparam>
    /// <typeparam name="C">The type used for connection data</typeparam>
    /// <typeparam name="Q">The type used for resulting GridRoom data</typeparam>
    public class RandomSpatialLayout<R, C, Q>
    {
        private DungeonGraph<R, C> _dungeonGraph;
        private Dictionary<int, GridRoom<Q>> _roomList = new Dictionary<int, GridRoom<Q>>();

        /// <summary>
        /// Get the width of the grid
        /// </summary>
        public int GridWidth { get; private set; }
        /// <summary>
        /// Get the height of the grid
        /// </summary>
        public int GridHeight { get; private set; }

        /// <summary>
        /// Get or set the room generator function. It should return a GridRoom given and AbstractRoom.
        /// </summary>
        public SpatialRoomGeneratorDelegate<R, Q> SpatialRoomGenerator { get; set; }
        /// <summary>
        /// Get or set the outside wall buffer size
        /// </summary>
        public int RoomMoatSize { get; set; } = 1;

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
            SpatialRoomGenerator = (AbstractRoom<R> _, out GridRoom<Q> room) =>
            {
                room = default(GridRoom<Q>);
                return false;
            };
        }
        /// <summary>
        /// Randomly place the rooms in the grid.
        /// </summary>
        /// <returns>A new <c>DungeonGraph></c> with <c>GridRoom</c>'s and <c>GridPassageConnectionData</c> connections.</returns>
        public DungeonGraph<GridRoom<Q>, GridPassageConnectionData<C>> PlaceRooms()
        {
            var newDungeonBuilder = new DungeonGraphBuilder<GridRoom<Q>, GridPassageConnectionData<C>>();
            var roomMapping = new Dictionary<int, int>();
            // Loop over each room in the dungeon graph in some specified order.
            foreach (var roomId in _dungeonGraph.Nodes)
            {
                // Randomly place the room in the grid.
                AbstractRoom<R> roomData = _dungeonGraph.GetNodeLabel(roomId);
                if (TryPlaceRoom(roomData, MaxNumberOfTriesPerRoom, out GridRoom<Q> room))
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

        private bool TryPlaceRoom(AbstractRoom<R> roomData, int numberOfTries, out GridRoom<Q> room)
        {
            int roomTry = 0;
            while (roomTry < numberOfTries)
            {
                if (!this.SpatialRoomGenerator(roomData, out room)) return false;
                roomTry++;
                bool canPlace = CheckForOverlap(room);
                if (canPlace)
                {
                    return true;
                }

            }
            room = default(GridRoom<Q>);
            return false;
        }
        private bool CheckForOverlap(GridRoom<Q> room)
        {
            bool canPlace = true;
            foreach (GridRoom<Q> placedRoom in _roomList.Values)
            {
                int distance = GridRoom<Q>.RoomDistance(placedRoom, room);
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