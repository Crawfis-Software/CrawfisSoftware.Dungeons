namespace CrawfisSoftware.Dungeons
{
    /// <summary>
    /// An "edge" from one room to another room.
    /// </summary>
    /// <typeparam name="C">The type of the data associated with the connection.</typeparam>
    public struct Connection<C>
    {
        /// <summary>
        /// The two rooms that are connected.
        /// </summary>
        public (int roomID, int exitNumber) Room1 { get; set; }
        /// <summary>
        /// The two rooms that are connected.
        /// </summary>
        public (int roomID, int exitNumber) Room2 { get; set; }
        /// <summary>
        /// The weight of the edge between the two rooms.
        /// </summary>
        public float EdgeWeight { get; set; }
        /// <summary>
        /// The data associated with this connection.
        /// </summary>
        public C ConnectData { get; set; }
    }
}