using HarmonyLib;
using MigCorp.Skiptech.SkipNet;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerMigCorpSkiptechPatch.Source.Mods;

/// <summary>
///     Clears <see cref="MapComponent_SkipNet" /> static lookup cache on load,
///     avoiding stale map references after save/load or desync recovery.
///     Same pattern as Hospitality cache clear in Multiplayer-Compatibility.
/// </summary>
internal static class SkipNetCacheMaintenance
{
    private const string LogPrefix = "[Multiplayer MigCorp - Skiptech Patch]";

    public static void Register()
    {
        try
        {
            MpCompat.harmony.Patch(
                AccessTools.DeclaredMethod(typeof(GameComponentUtility), nameof(GameComponentUtility.FinalizeInit)),
                postfix: new HarmonyMethod(typeof(SkipNetCacheMaintenance), nameof(PostFinalizeInitClearCache)));
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SkipNetCacheMaintenance.Register failed: {exception}");
        }
    }

    private static void PostFinalizeInitClearCache()
    {
        try
        {
            var cachedMapField = AccessTools.Field(typeof(MapComponent_SkipNet), "cachedMap");
            var cachedCompField = AccessTools.Field(typeof(MapComponent_SkipNet), "cachedComp");

            cachedMapField?.SetValue(null, null);
            cachedCompField?.SetValue(null, null);
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SkipNetCacheMaintenance.PostFinalizeInitClearCache failed: {exception}");
        }
    }
}