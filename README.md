# Multiplayer MigCorp - Skiptech Patch

A RimWorld Multiplayer compatibility patch for [MigCorp - Skiptech](https://steamcommunity.com/sharedfiles/filedetails/?id=3626074566).

This mod is designed to improve multiplayer synchronization when playing with the [MigCorp - Skiptech](https://steamcommunity.com/sharedfiles/filedetails/?id=3626074566) mod and RimWorld Multiplayer.

## Features

- Syncs Skiptech mod settings (`accessMode`, `animalsCanUse`, skip-shock flags, unpowered doors, default power, `skipCost`, `worthItFactor`, teleport flash, verbose logging).
- Syncs the in-game skipdoor power override and refreshes power defs on all clients via `SkipdoorCustomPower.ApplyToActiveGame`.
- Clears the `MapComponent_SkipNet` static lookup cache on load to avoid stale map references.
- Isolates rename dialog construction from the gameplay RNG seed.
- Relies on Multiplayer core for manual SkipLocally orders (`TryTakeOrderedJob`), skipdoor renaming (`IRenameable`), and deterministic automatic SkipNet pathing.

## Requirements

- RimWorld 1.6
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
- RimWorld Multiplayer
  - [GitHub version](https://github.com/rwmt/Multiplayer) or [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=2606448745) version
- [MigCorp - Skiptech](https://steamcommunity.com/sharedfiles/filedetails/?id=3626074566)

The host and every connected player must use compatible versions of all required mods.

## Installation

### Steam Workshop

Subscribe to the required mods and add them to your RimWorld mod list in the following order:

1. Harmony
2. Core
3. Royalty, Ideology, Biotech, and Anomaly, if applicable
4. RimWorld Multiplayer
5. [MigCorp - Skiptech](https://steamcommunity.com/sharedfiles/filedetails/?id=3626074566)
6. [Multiplayer MigCorp - Skiptech Patch](https://github.com/Keullaeseu/Multiplayer-MigCorp-Skiptech-Patch/releases/latest)

The patch should load after RimWorld Multiplayer and [MigCorp - Skiptech](https://steamcommunity.com/sharedfiles/filedetails/?id=3626074566).

### Manual Installation

1. Download the latest release from the [**Releases**](https://github.com/Keullaeseu/Multiplayer-MigCorp-Skiptech-Patch/releases/latest) section.
2. Extract the mod folder into your RimWorld `Mods` directory.
3. Enable the required mods in RimWorld.
4. Use the recommended load order listed above.
5. Make sure every multiplayer player has the same mod list, configuration, and load order.

## Multiplayer Usage

All players should have the following mods installed and enabled:

- RimWorld Multiplayer
- [MigCorp - Skiptech](https://steamcommunity.com/sharedfiles/filedetails/?id=3626074566)
- [Multiplayer MigCorp - Skiptech Patch](https://github.com/Keullaeseu/Multiplayer-MigCorp-Skiptech-Patch/releases/latest)
- All other required MigCorp - Skiptech dependencies

The host and all connected clients should use the same:

- RimWorld version
- RimWorld Multiplayer version
- MigCorp - Skiptech version
- Multiplayer MigCorp - Skiptech Patch version
- Mod configuration
- Mod load order

Do not add, remove, update, or reorder mods while players are connected to the same multiplayer session.

## Compatibility

This patch is intended to provide multiplayer compatibility for [MigCorp - Skiptech](https://steamcommunity.com/sharedfiles/filedetails/?id=3626074566)
itself (settings window, in-game power override, static cache, rename dialog RNG).

It relies on Multiplayer core for manual SkipLocally job orders, skipdoor renaming, and automatic SkipNet pathfinding, which are deterministic and need no extra sync.

It does not replace:

- [RimWorld Multiplayer](https://steamcommunity.com/sharedfiles/filedetails/?id=2606448745)
- [MigCorp - Skiptech](https://steamcommunity.com/sharedfiles/filedetails/?id=3626074566)

## Known Limitations

- Compatibility may be affected by future RimWorld updates.
- Compatibility may be affected by future updates to RimWorld Multiplayer or MigCorp - Skiptech.

## Credits

- [RimWorld Multiplayer on GitHub](https://github.com/rwmt/Multiplayer)
- [RimWorld Multiplayer on Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=2606448745)
- [MigCorp - Skiptech on Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3626074566)
- [Multiplayer MigCorp - Skiptech Patch](https://github.com/Keullaeseu)
