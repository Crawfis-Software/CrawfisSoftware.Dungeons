using CrawfisSoftware.Collections.Graph;

using System;
using System.Collections.Generic;
using System.Text;

namespace CrawfisSoftware.Dungeons
{
    internal class BindingOfIsaacRoomFactory
    {
        public int RoomWidth { get; private set; }
        public int RoomHeight { get; private set; }
        public int OpeningWidth { get; private set; }
        public Direction Direction { get; private set; }
    }
}
