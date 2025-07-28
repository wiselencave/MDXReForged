# MDX ReForged

A cleaned-up, extended parser for Warcraft 3: Reforged `.mdx` model files.

This project is a fork of [barncastle/MDXReForged](https://github.com/barncastle/MDXReForged), originally started in 2019 as an experimental reader.  
Support has been expanded to cover MDX1000–MDX1200 formats, with architectural refactoring, improved null safety, and moderate performance improvements.

## Usage

```csharp
// Load from a file path
var model = new MDXReForged.MDX.Model("path/to/model.mdx");

// Load from an existing stream
using var stream = File.OpenRead("path/to/model.mdx");
var fromStream = new MDXReForged.MDX.Model(stream);

// Or pass a BinaryReader directly
using var reader = new BinaryReader(File.OpenRead("path/to/model.mdx"));
var fromReader = new MDXReForged.MDX.Model(reader);

// Print model summary (debug info)
Console.WriteLine(model.GetDetailedInfo());
```

## 2025 – Major Changes

### Format updates:
- Added support for `MDX1000`. Updated `LAYS`.
- Added support for `MDX1100`. Updated `MTLS`. Updated `LAYS`:
    - Added support for multi-texturing.
    - Introduced new `TextureEntry` class that holds the texture ID, semantic, and optional flip track.
    - Layers now can expose a `Textures` list instead of a single `TextureId`.
- Added support for `MDX1200`. Updated `LITE`.
- Reworked `GEOS`:
  - Added enums for level of detail and primitive types.
  - Parser no longer reorders indices into triangles automatically. Raw index buffer is now preserved.
  - Vertex influences and weights merged into new `CSkinData` structure for skinning.
- Updated property names for `CORN`.
- Removed unused placeholder fields from `PRE2`.
- Refactored the `C34Matrix` structure.
- Removed unused structures and fields. `SNDS` was removed.

### Performance improvements:
- Core math and geometry types rewritten as structs instead of classes.
- Replaced properties exposing `List<T>` with arrays where collection size is known in advance. For all chunk readers with a known object count, `List<T>` collections now preallocate capacity.
- Optimized `Track<T>` loading. `Model` refactored.

### Null safety:
- Added `GetItems<T>` for safe access to list-based chunks. If the chunk is missing, it returns an empty list.
- Animation tracks and their node arrays are now always initialized. Absent animations result in empty tracks instead of `null`.
- Certain newer-format properties are now `nullable`.
- Reworked `CLID`:
  - Introduced `ICollisionGeometry` interface.
  - Removed individual fields for `Box`, `Sphere`, `Plane`, and `Cylinder`.

### Architectural changes:
- Public fields converted to `get`-only properties.
- Tag comparisons now use centralized uint constants (`Tags` class) instead of string literals.
- Project updated to `.NET 8.0`.

### API usage improvements:
- Added `ToString()` overrides for all major data classes to improve debugging.
- Added new constructors for `Model` to support initialization from file path, `Stream`, or `BinaryReader`.
- Added strongly-typed accessors for common chunks (e.g., `GetBones()`, `GetTextures()`), as convenience wrappers over `GetItems<T>()`.
- Added helper methods to `Geoset` for accessing geometry indices: `EnumeratePrimitiveGroups`, `EnumerateTriangles` and `GetTriangleIndexBuffer`.
- Added `GetTextureId(semantic)` method to `Layer` for easy texture lookup by purpose (e.g., diffuse, normal, emissive).