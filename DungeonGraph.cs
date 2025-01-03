using CrawfisSoftware.Collections.Graph;

using System.Collections.Generic;
using System.Linq;

namespace CrawfisSoftware.Dungeons
{
    /// <summary>
    /// Class to represent a dungeon graph. It is an <c>IIndexedGraph</c> with <c>AbstractRoom</c> data and <c>Connection</c> data
    /// </summary>
    /// <typeparam name="R">A type for any associated room data.</typeparam>
    /// <typeparam name="C">A type for any associated connection data.</typeparam>
    public class DungeonGraph<R, C> : IIndexedGraph<AbstractRoom<R>, Connection<C>>
    {
        private Dictionary<int, AbstractRoom<R>> rooms = new Dictionary<int, AbstractRoom<R>>();
        private List<Connection<C>> connections = new List<Connection<C>>();

        /// <inheritdoc />
        public int NumberOfEdges => connections.Count;
        /// <inheritdoc />
        public int NumberOfNodes => rooms.Count;
        /// <inheritdoc />
        public IEnumerable<int> Nodes => rooms.Keys;
        /// <inheritdoc />
        public IEnumerable<IIndexedEdge<Connection<C>>> Edges => connections.Select((connection) => new IndexedEdge<Connection<C>>(connection.Room1.roomID, connection.Room2.roomID, connection) as IIndexedEdge<Connection<C>>);

        /// <inheritdoc />
        public AbstractRoom<R> GetNodeLabel(int nodeIndex)
        {
            return rooms[nodeIndex];
        }

        /// <inheritdoc />
        public IEnumerable<int> Neighbors(int nodeIndex)
        {
            return connections.Where(c => c.Room1.roomID == nodeIndex || c.Room2.roomID == nodeIndex)
                              .Select(c => c.Room1.roomID == nodeIndex ? c.Room2.roomID : c.Room1.roomID);
        }

        /// <inheritdoc />
        public IEnumerable<IIndexedEdge<Connection<C>>> OutEdges(int nodeIndex)
        {
            return connections.Where(c => c.Room1.roomID == nodeIndex)
                              .Select((connection) => new IndexedEdge<Connection<C>>(connection.Room1.roomID, connection.Room2.roomID, connection) as IIndexedEdge<Connection<C>>);
        }

        /// <inheritdoc />
        public IEnumerable<int> Parents(int nodeIndex)
        {
            return connections.Where(c => c.Room2.roomID == nodeIndex)
                              .Select(c => c.Room1.roomID);
        }

        /// <inheritdoc />
        public IEnumerable<IIndexedEdge<Connection<C>>> InEdges(int nodeIndex)
        {
            return connections.Where(c => c.Room2.roomID == nodeIndex)
                              .Select((connection) => new IndexedEdge<Connection<C>>(connection.Room1.roomID, connection.Room2.roomID, connection) as IIndexedEdge<Connection<C>>);
        }

        /// <inheritdoc />
        public bool ContainsEdge(int fromNode, int toNode)
        {
            return connections.Any(c => (c.Room1.roomID == fromNode && c.Room2.roomID == toNode) ||
                                        (c.Room1.roomID == toNode && c.Room2.roomID == fromNode));
        }

        /// <inheritdoc />
        public Connection<C> GetEdgeLabel(int fromNode, int toNode)
        {
            return connections.First(c => (c.Room1.roomID == fromNode && c.Room2.roomID == toNode) ||
                                          (c.Room1.roomID == toNode && c.Room2.roomID == fromNode));
        }

        /// <inheritdoc />
        public bool TryGetEdgeLabel(int fromNode, int toNode, out Connection<C> edge)
        {
            var edges = connections.Where(c => (c.Room1.roomID == fromNode && c.Room2.roomID == toNode) ||
                                                   (c.Room1.roomID == toNode && c.Room2.roomID == fromNode));
            bool found = edges.Any();
            edge = found ? edges.First() : default;
            return found;
        }

        /// <summary>
        /// Construct a new dungeon graph. Called by a builder
        /// </summary>
        /// <param name="rooms">A List of Abstract Rooms.</param>
        /// <param name="connections">A List of Connections.</param>
        public DungeonGraph(Dictionary<int, AbstractRoom<R>> rooms, List<Connection<C>> connections)
        {
            this.rooms = rooms;
            this.connections = connections;
        }
    }
}