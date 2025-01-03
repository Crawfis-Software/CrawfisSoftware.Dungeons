namespace CrawfisSoftware.Dungeons
{
    /// <summary>
    /// Defined algorithms for connecting two rooms.
    /// </summary>
    public enum PassageRasterizerType
    {
        /// <summary>
        /// No algorithm selected.
        /// </summary>
        Unspecified,
        /// <summary>
        /// A simple path with one turn.
        /// </summary>
        Elbow,
        /// <summary>
        /// Determine the shortest path between the rooms using edge weights.
        /// </summary>
        ShortestPathBetweenRooms,
        /// <summary>
        /// Shortest path taking advantage of existing paths.
        /// </summary>
        ShortestPathUsingExisting,
        /// <summary>
        /// Use a random walk
        /// </summary>
        RandomWalk
    };
}