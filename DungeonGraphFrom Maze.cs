using CrawfisSoftware.Collections.Graph;
using CrawfisSoftware.Maze;

namespace CrawfisSoftware.Dungeons
{
    public class DungeonGraphFromMaze
    {
        public static DungeonGraph<GridRoom<Direction>, Connection<PassageRasterizerType>> BindingOfIsaac<N, E>(Maze<N, E> maze, int roomWidth, int roomHeight, int openingWidth = 1)
        {
            var builder = new DungeonGraphBuilder<GridRoom<Direction>, Connection<PassageRasterizerType>>();
            int width = maze.Width;
            int height = maze.Height;
            int[,] roomIds = new int[width, height];
            for (int column = 0; column < width; column++)
            {
                for (int row = 0; row < height; row++)
                {
                    roomIds[column, row] = -1;
                    Direction dirs = maze.GetDirection(column, row);
                    if (dirs == Direction.None || dirs == Direction.Undefined) continue;
                    var room = new GridRoom<Direction>(column * roomWidth, row * roomHeight, roomWidth, roomHeight, dirs);
                    int roomId = builder.AddRoom(dirs.NumberOfExits(), 1, room);
                    roomIds[column, row] = roomId;
                }
            }

            var connection = new Connection<PassageRasterizerType>() { ConnectData = PassageRasterizerType.Opening, EdgeWeight = openingWidth };
            for (int column = 0; column < width; column++)
            {
                for (int row = 0; row < height; row++)
                {
                    int exitID = 0;
                    int roomId = roomIds[column, row];
                    if (roomId == -1) continue;
                    Direction dirs = maze.GetDirection(column, row);
                    if ((dirs & Direction.N) == Direction.N)
                    {
                        int northRoomId = roomIds[column, row + 1];
                        connection.Room1 = (roomId, exitID);
                        connection.Room2 = (northRoomId, exitID);
                        exitID++;
                        builder.AddConnection(roomId, northRoomId, connection);
                    }
                    if ((dirs & Direction.E) == Direction.E)
                    {
                        int eastRoomId = roomIds[column + 1, row];
                        connection.Room1 = (roomId, exitID);
                        connection.Room2 = (eastRoomId, exitID);
                        exitID++;
                        builder.AddConnection(roomId, eastRoomId, connection);
                    }
                }
            }

            return builder.Build();
        }
    }
}