namespace CrawfisSoftware.Dungeons
{
    /// <summary>
    /// A struct to represent a room in a grid.
    /// </summary>
    /// <typeparam name="C">The type of the data associated with the connection.</typeparam>
    public struct GridPassageConnectionData<C>
    {
        /// <summary>
        /// The data associated with the connection.
        /// </summary>
        public C connectionData;
        /// <summary>
        /// The type of rasterizer to use for the passage.
        /// </summary>
        public PassageRasterizerType PassageRasterizer { get; private set; }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionData">The data associated with the connection.</param>
        /// <param name="passageRasterizer">The type of rasterizer to use for the passage.</param>
        public GridPassageConnectionData(C connectionData, PassageRasterizerType passageRasterizer)
        {
            this.connectionData = connectionData;
            this.PassageRasterizer = passageRasterizer;
        }
    }
}