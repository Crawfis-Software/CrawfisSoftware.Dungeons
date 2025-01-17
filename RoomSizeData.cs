using System;
using System.Collections.Generic;
using System.Text;

namespace CrawfisSoftware.Dungeons
{
    /// <summary>
    /// A class to hold the room size data.
    /// </summary>
    public class RoomSizeData
    {
        /// <summary>
        /// Get or set the minimum room width to create
        /// </summary>
        public int MinRoomWidth { get; set; } = 4;
        /// <summary>
        /// Get or set the maximum room width to create
        /// </summary>
        public int MaxRoomWidth { get; set; } = 8;
        /// <summary>
        /// Get or set the minimum room height to create
        /// </summary>
        public int MinRoomHeight { get; set; } = 4;
        /// <summary>
        /// Get or set the maximum room height to create
        /// </summary>
        public int MaxRoomHeight { get; set; } = 8;
        /// <summary>
        /// Get or set the outside wall buffer size
        /// </summary>
        public int RoomMoatSize { get; set; } = 1;

        public static readonly RoomSizeData DefaultRoomSizeData = new RoomSizeData();
        public static readonly RoomSizeData SmallRoomSizeData = new RoomSizeData() { MinRoomWidth = 3, MaxRoomWidth = 5, MinRoomHeight = 3, MaxRoomHeight = 5, RoomMoatSize = 1 };
        public static readonly RoomSizeData LargeRoomSizeData = new RoomSizeData() { MinRoomWidth = 6, MaxRoomWidth = 10, MinRoomHeight = 6, MaxRoomHeight = 10, RoomMoatSize = 3 };
        public static readonly RoomSizeData ExtraLargeRoomSizeData = new RoomSizeData() { MinRoomWidth = 13, MaxRoomWidth = 17, MinRoomHeight = 6, MaxRoomHeight = 10, RoomMoatSize = 3 };
    }
}