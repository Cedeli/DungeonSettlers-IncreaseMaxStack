# IncreaseMaxStack

A BepInEx plugin for Dungeon Settlers that lets you change item stack sizes globally or per item.

## Requirements

- [BepInEx 6 Bleeding Edge, IL2CPP build](https://builds.bepinex.dev/projects/bepinex_be)

## Installation

1. Download `IncreaseMaxStack.zip` from this repo's [Releases](../../releases) page and extract it into your game folder.
2. Launch the game. A config file will be generated at `BepInEx/config/IncreaseMaxStack.cfg`.

## Configuration

Edit `BepInEx/config/IncreaseMaxStack.cfg`:

**`[General] EnableGlobalMaxStack`** - if `true`, every item without a specific override uses `GlobalMaxStack`.

**`[General] GlobalMaxStack`** - the global stack size. Default `999`.

**`[Individual Item Stacks] ItemStackOverrides`** - semicolon-separated `ItemId=Amount` pairs, e.g.:

```
ItemStackOverrides = ITEM_Straw=250;ITEM_LumaLog=50
```

Overrides take priority over the global setting. Item IDs follow the `ITEM_Name` format used on the [wiki](https://dungeonsettlers.wiki/items). Unmatched IDs are logged as a warning on startup.
