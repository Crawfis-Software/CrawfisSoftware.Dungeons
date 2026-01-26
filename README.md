# CrawfisSoftware.Dungeons

[![NuGet](https://img.shields.io/nuget/v/CrawfisSoftware.Dungeons.svg)](https://www.nuget.org/packages/CrawfisSoftware.Dungeons)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET library for procedural dungeon generation, providing a complete framework for creating dungeons through graph-based room structures, spatial layouts, and maze rasterization. Ideal for roguelikes, dungeon crawlers, and other games requiring procedural level generation.

## Features

- **Graph-Based Dungeon Structure** - Define dungeons as connected room graphs with weighted edges
- **Spatial Layout Algorithm** - Automatically place rooms in a 2D grid while avoiding overlaps
- **Multiple Passage Types** - Various corridor generation algorithms (elbow, shortest path, random walk, etc.)
- **Maze Integration** - Rasterize dungeon layouts into navigable maze representations
- **Highly Generic** - Associate custom data with rooms and connections via type parameters
- **Builder Pattern** - Fluent API for constructing dungeon graphs

## Installation

Install via NuGet:

```bash
dotnet add package CrawfisSoftware.Dungeons
```

Or via the Package Manager Console:

```powershell
Install-Package CrawfisSoftware.Dungeons
```

## Architecture

The library follows a three-stage dungeon generation pipeline:

```
┌─────────────────────┐    ┌─────────────────────┐    ┌─────────────────────┐
│  Abstract Graph     │ -> │   Spatial Layout    │ -> │    Rasterization    │
│  (Rooms + Edges)    │    │   (Grid Placement)  │    │   (Maze Output)     │
└─────────────────────┘    └─────────────────────┘    └─────────────────────┘
```

1. **Abstract Graph Generation** - Create a logical graph of rooms and connections
2. **Spatial Layout** - Place rooms randomly in a 2D grid while avoiding overlaps
3. **Rasterization** - Convert the spatial layout into a navigable maze

## Core Components

### DungeonGraph&lt;R, C&gt;

The main data structure representing a dungeon as a graph with rooms (nodes) and connections (edges).

```csharp
// R = room data type, C = connection data type
var dungeon = new DungeonGraph<RoomType, ConnectionType>();
```

### DungeonGraphBuilder&lt;R, C&gt;

Builder class for constructing dungeon graphs with a fluent API.

```csharp
var builder = new DungeonGraphBuilder<MyRoomData, MyConnectionData>();
builder.AddRoom(roomId: 0, data: myRoomData, traversalCost: 1.0f);
builder.AddRoom(roomId: 1, data: otherRoomData, traversalCost: 1.0f);
builder.AddConnection(fromRoom: 0, toRoom: 1, connectionData, exitNumber: 0);

var dungeon = builder.Build();
```

### RandomSpatialLayout&lt;R, C, Q&gt;

Places abstract rooms randomly in a 2D grid space.

```csharp
var layout = new RandomSpatialLayout<R, C, Q>(
    abstractDungeon,
    roomGenerator,
    roomSizeConfig
);

var spatialDungeon = layout.PlaceRooms();
```

### Room Size Configuration

Predefined room size templates are available:

| Configuration | Width | Height | Moat |
|---------------|-------|--------|------|
| `DefaultRoomSizeData` | 4-8 | 4-8 | 1 |
| `SmallRoomSizeData` | 3-5 | 3-5 | 1 |
| `LargeRoomSizeData` | 6-10 | 6-10 | 3 |
| `ExtraLargeRoomSizeData` | 13-17 | 6-10 | 3 |

### Passage Rasterization Types

Different algorithms for connecting rooms:

| Type | Description |
|------|-------------|
| `None` | No connection (rooms are directly adjacent) |
| `Opening` | Simple door/opening between adjacent rooms |
| `Elbow` | L-shaped two-segment corridor |
| `ShortestPathBetweenRooms` | A* pathfinding between room centers |
| `ShortestPathUsingExisting` | Optimal path reusing existing corridors |
| `RandomWalk` | Randomized meandering path |
| `Unspecified` | Random selection from available types |

### DungeonMazeBuilder

Extension methods for rasterizing dungeon graphs into mazes:

```csharp
// Rasterize the entire dungeon
mazeBuilder.RasterizeDungeon(spatialDungeon);

// Or rasterize individual elements
mazeBuilder.CarveGridRoom(room);
mazeBuilder.CarveCorridor(fromRoom, toRoom, rasterizerType);
```

### DungeonGraphFromMaze

Convert existing mazes back into dungeon graphs:

```csharp
var dungeonGraph = DungeonGraphFromMaze.BindingOfIsaac(existingMaze);
```

## Dependencies

This library depends on other CrawfisSoftware packages:

- [CrawfisSoftware.IGraph](https://www.nuget.org/packages/CrawfisSoftware.IGraph) - Graph interfaces and implementations
- [CrawfisSoftware.Maze](https://www.nuget.org/packages/CrawfisSoftware.Maze) - Maze building and pathfinding

## Target Frameworks

- .NET Standard 2.1
- .NET 8.0

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! This project uses:

- **Conventional Commits** - Commit messages are validated via commitlint
- **Release Please** - Automated versioning and changelog generation

## Author

**Crawfis Software**

- GitHub: [@Crawfis-Software](https://github.com/Crawfis-Software)
