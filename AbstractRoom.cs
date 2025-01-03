namespace CrawfisSoftware.Dungeons
{
    /// <summary>
    /// A simple class to represent a room in a dungeon.
    /// </summary>
    public struct AbstractRoom<R>
    {
        /// <summary>
        /// The next id to assign to a room.
        /// </summary>
        public static int _nextId = 0;
        /// <summary>
        /// The unique id for this room.
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// The number of exits from this room. In a more general case, A room could be a subgraph with different costs from one exit to another exit.
        /// </summary>
        public float RoomWeight { get; private set; }
        /// <summary>
        /// The data associated with this room.
        /// </summary>
        public R RoomData { get; set; }
        /// <summary>
        /// Factory to create a new Room
        /// </summary>
        /// <param name="numberOfExits">The number of entrances and exits this room has.</param>
        /// <param name="roomWeight">The cost to move through this room.</param>
        /// <param name="roomData">The data associated with this room.</param>
        /// <returns>An <c>AbstractRoom</c></returns>
        public static AbstractRoom<R> CreateAbstractRoom(int numberOfExits, float roomWeight, R roomData)
        {
            var room = new AbstractRoom<R>();
            room.ID = _nextId++;
            room.RoomWeight = roomWeight;
            room.RoomData = roomData;
            return room;
        }
    }
}
