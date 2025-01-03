using System.Collections.Generic;

namespace CrawfisSoftware.Dungeons
{
    /// <summary>
    /// A class to build an abstract dungeon graph.
    /// </summary>
    /// <typeparam name="R">The type used for room data</typeparam>
    /// <typeparam name="C">The type used for connection data</typeparam>
    public class DungeonGraphBuilder<R, C>
    {
        private Dictionary<int, AbstractRoom<R>> _roomList = new Dictionary<int, AbstractRoom<R>>();
        HashSet<Connection<C>> _roomConnections = new HashSet<Connection<C>>();

        /// <summary>
        /// Get or set the minimum room size to create
        /// </summary>
        public int MinRoomSize { get; set; } = 4;

        /// <summary>
        /// Get or set the maximum room size to create
        /// </summary>
        public int MaxRoomSize { get; set; } = 8;

        /// <summary>
        /// Get or set the outside wall buffer size
        /// </summary>
        public int RoomMoatSize { get; set; } = 1;

        /// <summary>
        /// Create a room explicitly at the specified location with the specified size.
        /// </summary>
        /// <returns>An id for the room.</returns>
        public int AddRoom(int numberOfExits, float roomTraversalCost, R roomData)
        {
            AbstractRoom<R> room = AbstractRoom<R>.CreateAbstractRoom(numberOfExits, roomTraversalCost, roomData);
            _roomList[room.ID] = room;
            return room.ID;
        }

        /// <summary>
        /// Associate a passage between tow rooms
        /// </summary>
        /// <param name="room1">The id of the first room.</param>
        /// <param name="room2">The id of the second room.</param>
        /// <param name="edgeData">Any data to associate with this connection.</param>
        /// <param name="edgeWeight">The weight of the edge between the two rooms. The default is 1.</param>
        public void AddConnection(int room1, int room2, C edgeData, float edgeWeight = 1f)
        {
            _roomConnections.Add(new Connection<C>() { Room1 = (room1, -1), Room2 = (room2, -1), ConnectData = edgeData, EdgeWeight = edgeWeight });
        }

        /// <summary>
        /// Add passageways between rooms in the order they were generated.
        /// </summary>
        /// <param name="skip">The number of rooms to skip between connections. The default is 0.</param>
        /// <param name="edgeData">Any data to associate with this connection.</param>
        /// <param name="edgeWeight">The weight of the edge between the two rooms. The default is 1.</param>
        public void MakeSequentialRoomConnections(int skip = 0, C edgeData = default(C), float edgeWeight = 1f)
        {
            var roomIds = _roomList.Keys.GetEnumerator();
            int lastId = roomIds.Current;
            while (roomIds.MoveNext())
            {
                AddConnection(lastId, roomIds.Current, edgeData);
                lastId = roomIds.Current;
                for (int i = 0; i < skip; i++)
                {
                    roomIds.MoveNext();
                    lastId = roomIds.Current;
                }
            }
        }

        /// <summary>
        /// Build the dungeon graph.
        /// </summary>
        /// <returns>A <c>DungeonGraph</c></returns>
        public DungeonGraph<R, C> Build()
        {
            return new DungeonGraph<R, C>(new Dictionary<int, AbstractRoom<R>>(_roomList), new List<Connection<C>>(_roomConnections));
        }
    }
}