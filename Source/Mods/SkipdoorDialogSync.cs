using HarmonyLib;
using MigCorp.Skiptech.SkipNet;
using MigCorp.Skiptech.SkipNet.Comps;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerMigCorpSkiptechPatch.Source.Mods;

/// <summary>
///     Isolates rename dialog construction (UI) from the gameplay RNG seed.
///     Same pattern as VEF DoorTeleporter rename dialog in Multiplayer-Compatibility.
///     The rename itself needs no explicit sync: MP core auto-syncs all
///     IRenameable.RenamableLabel setters it CanHandle, and CompSkipdoor (ThingComp) is syncable.
///     The rename gizmo (open dialog) and SkipLocally FloatMenu (TryTakeOrderedJob) are
///     already handled by MP core and need no delegate sync.
/// </summary>
internal static class SkipdoorDialogSync
{
    private const string LogPrefix = "[Multiplayer MigCorp - Skiptech Patch]";

    public static void Register()
    {
        try
        {
            var renameConstructor =
                AccessTools.DeclaredConstructor(typeof(Dialog_RenameSkipdoor), new[] { typeof(CompSkipdoor) });
            if (renameConstructor != null)
                PatchingUtilities.PatchPushPopRand(renameConstructor);
            else
                Log.Warning($"{LogPrefix} Could not find Dialog_RenameSkipdoor ctor for Rand patch, skipping.");
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SkipdoorDialogSync.Register failed: {exception}");
        }
    }
}