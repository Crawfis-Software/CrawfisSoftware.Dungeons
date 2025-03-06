using System;

namespace CrawfisSoftware.Dungeons
{
    /// <summary>
    /// A struct to represent a room in a grid.
    /// </summary>
    /// <typeparam name="R">The type of the room data.</typeparam>
    public struct GridRoom<R>
    {
        /// <summary>
        /// The minimum x coordinate of the room.
        /// </summary>
        public int minX;
        /// <summary>
        /// The minimum y coordinate of the room.
        /// </summary>
        public int minY;
        /// <summary>
        /// The width of the room.
        /// </summary>
        public int width;
        /// <summary>
        /// The height of the room.
        /// </summary>
        public int height;
        /// <summary>
        /// The data associated with the room.
        /// </summary>
        public R roomData;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="minX">The minimum x coordinate of the room.</param>
        /// <param name="minY">The minimum y coordinate of the room.</param>
        /// <param name="width">The width of the room.</param>
        /// <param name="height">The height of the room.</param>
        /// <param name="roomData">The data associated with the room.</param>
        public GridRoom(int minX, int minY, int width, int height, R roomData)
        {
            this.minX = minX;
            this.minY = minY;
            this.width = width;
            this.height = height;
            this.roomData = roomData;
        }

        /// <summary>
        /// The Manhattan distance between two rooms.
        /// </summary>
        /// <param name="room1">A <c>GridRoom</c>.</param>
        /// <param name="room2">A <c>GridRoom</c>.</param>
        /// <returns>The Manhattan distance between the rooms.</returns>
        public static int RoomDistance(GridRoom<R> room1, GridRoom<R> room2)
        {
            int xDistance = 0;
            int yDistance = 0;
            int x1 = room1.minX;
            int x2 = x1 + room1.width;
            int y1 = room1.minY;
            int y2 = y1 + room1.height;
            int u1 = room2.minX;
            int u2 = u1 + room2.width;
            int v1 = room2.minY;
            int v2 = v1 + room2.height;
            if (x2 < u1)
            {
                xDistance = u1 - x2;
            }
            else if (u2 < x1)
            {
                xDistance = x1 - u2;
            }
            if (y2 < v1)
            {
                yDistance = v1 - y2;
            }
            else if (v2 < y1)
            {
                yDistance = y1 - v2;
            }
            xDistance = Math.Max(0, xDistance);
            yDistance = Math.Max(0, yDistance);
            int distance = xDistance + yDistance;
            //if (distance == 0)
            //{
            //    // Not entirely accurate if one is completely within the other
            //    distance = Math.Min(Math.Max(x1, u1) - Math.Max(x2, u2), Math.Max(y1, v1) - Math.Max(y2, v2));
            //}
            return distance;
        }
    }
}