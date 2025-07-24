# MDX ReForged

Experiments with reading the new Warcraft 3: Reforged MDX files. 

The project *should be* compliant with all of the models used in the current client (as of time of writing).

## 2025 – Major Changes

### Format updates:
- Added support for `MDX1000`. Updated `LAYS`.
- Added support for `MDX1100`. Updated `MTLS`. Updated `LAYS`:
    - Added support for multi-texturing.
    - Introduced new `TextureEntry` class that holds the texture ID, semantic, and optional flip track.
    - Layers now can expose a `Textures` list instead of a single `TextureId`.
    - Added `GetTextureId(semantic)` method for easy texture lookup by purpose (e.g., diffuse, normal, emissive).

- Added support for `MDX1200`. Updated `LITE`.
- Reworked `GEOS`:
  - Added enums for level of detail and primitive types (polygon section).
  - Parser no longer reorders indices into triangles automatically. Raw index buffer is now preserved, with new helper methods `EnumeratePrimitiveGroups`, `EnumerateTriangles` and `GetTriangleIndexBuffer`.
  - Vertex influences and weights merged into new `CSkinData` structure for skinning.
- Updated property names for `CORN`.
- Removed non-existent fields from `PRE2`.
- Refactored the `C34Matrix` structure.
- Removed unused structures and fields. `SNDS` was removed.

### Performance improvements:
- Core math and geometry types rewritten as structs instead of classes.

### Null safety:
- Added `GetItems<T>` for safe access to list-based chunks. If the chunk is missing, it returns an empty list. Added strongly-typed overloads (e.g., `GetBones()`, `GetTextures()`).
- Animation tracks are now always initialized. Absent animations result in empty tracks instead of `null`.
- Certain newer-format properties are now `nullable`.
- Reworked `CLID`:
  - Introduced `ICollisionGeometry` interface.
  - Removed individual fields for `Box`, `Sphere`, `Plane`, and `Cylinder`.

### Architectural changes:
- Public fields converted to `get`-only properties.
- Project updated to `.NET 8.0`.

## Legacy

#### Notable changes:

Changes appear to have been made as per below. Most of which are wrapped in a version check by the client as to preserve backwards compatibility.

**New BPOS Chunk** - Bind Positions

**New FAFX Chunk** - FaceFX, used for the new facial animations, see vendor docs

**New CORN Chunk** - PopcornFX particle emitter, see vendor docs

**Changed GEOS Chunk** - Now contains a `LevelOfDetail` and a `FilePath/GeosetName` field

- **New TANG Sub-Chunk** - Tangents. These are C4Vectors with `w` storing the handedness (see Unity's docs)

- **New SKIN Sub-Chunk** - Contains bone indices and weights

**Changed MTLS Chunk** - Now contains a `Shader` file path field

- **Changed LAYS Sub-Chunk** - Contains a new float and float track, presumed to be `EmissiveGain`
