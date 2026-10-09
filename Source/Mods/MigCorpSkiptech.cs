using Multiplayer.Compat;
using Verse;

namespace MultiplayerMigCorpSkiptechPatch.Source.Mods;

/// <summary>
///     Multiplayer Patch for MigCorp - Skiptech by TheMiggiestMig,
///     Last Update: 9 Oct @ 1:33pm 2026
///     <see href="https://steamcommunity.com/sharedfiles/filedetails/?id=3626074566" />
///     Entry point only, logic lives in SkiptechSettingsSync, SkipNetCacheMaintenance, SkipdoorDialogSync.
///     Automatic SkipNet pathing (SkipNetManager, SkipNetSeam, Pawn_PathFollower patches) is
///     deterministic simulation and needs no sync.
/// </summary>
[MpCompatFor("migcorp.skiptech")]
public class MigCorpSkiptech
{
    private const string LogPrefix = "[Multiplayer MigCorp - Skiptech Patch]";

    public MigCorpSkiptech(ModContentPack content)
    {
        LongEventHandler.ExecuteWhenFinished(LatePatch);
    }

    private static void LatePatch()
    {
        Log.Message($"{LogPrefix} Initializing...");

        try
        {
            SkiptechSettingsSync.Register();
            SkipNetCacheMaintenance.Register();
            SkipdoorDialogSync.Register();
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} LatePatch failed: {exception}");
        }

        Log.Message($"{LogPrefix} Initialized.");
    }
}