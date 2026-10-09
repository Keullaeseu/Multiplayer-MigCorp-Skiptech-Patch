using HarmonyLib;
using MigCorp.Skiptech;
using MigCorp.Skiptech.SkipNet;
using MigCorp.Skiptech.Utils;
using Multiplayer.API;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerMigCorpSkiptechPatch.Source.Mods;

/// <summary>
///     Syncs <see cref="MigcorpSkiptechSettings" /> and the in-game power override
///     (<see cref="GameComponent_Skiptech.skipdoorPowerOverride" />) via snapshot-compare
///     in the settings window. Uses explicit synced setters with primitive args only,
///     so no dependency on MP being able to serialize ModSettings instances.
///     Power defs are refreshed on all clients via <see cref="SyncedApplyPower" />.
/// </summary>
internal static class SkiptechSettingsSync
{
    private const string LogPrefix = "[Multiplayer MigCorp - Skiptech Patch]";

    public static void Register()
    {
        try
        {
            PatchSettingsWindow();
            RegisterSyncedMethods();
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SkiptechSettingsSync.Register failed: {exception}");
        }
    }

    private static void PatchSettingsWindow()
    {
        try
        {
            var settingsMethod = AccessTools.DeclaredMethod(typeof(MigcorpSkiptechMod),
                nameof(MigcorpSkiptechMod.DoSettingsWindowContents));
            if (settingsMethod == null)
            {
                Log.Error($"{LogPrefix} Could not find MigcorpSkiptechMod.DoSettingsWindowContents for patching.");
                return;
            }

            MpCompat.harmony.Patch(
                settingsMethod,
                new HarmonyMethod(typeof(SkiptechSettingsSync), nameof(PreDoSettingsWindowContents)),
                new HarmonyMethod(typeof(SkiptechSettingsSync), nameof(PostDoSettingsWindowContents)));
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SkiptechSettingsSync.PatchSettingsWindow failed: {exception}");
        }
    }

    private static void RegisterSyncedMethods()
    {
        try
        {
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedSetAccessMode));
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedSetAnimalsCanUse));
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedSetDisableSkipShock));
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedSetEnableSkipShockAvoidance));
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedSetDisableUnpoweredSkipdoors));
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedSetDefaultSkipdoorPower));
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedSetSkipCost));
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedSetWorthItFactor));
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedSetDisableTeleportFlashEffect));
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedSetDebugVerboseLogging));
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedSetPowerOverride));
            MP.RegisterSyncMethod(typeof(SkiptechSettingsSync), nameof(SyncedApplyPower));
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SkiptechSettingsSync.RegisterSyncedMethods failed: {exception}");
        }
    }

    // Snapshot before UI draws. No MP.Watch: ModSettings instances have no MP sync worker,
    // so Watch target would fail to serialize. Snapshot + explicit primitive setters avoid that.
    // Manual SkipLocally FloatMenu needs no delegate sync: TryTakeOrderedJob is synced by MP core.
    private static void PreDoSettingsWindowContents(out SettingsSnapshot __state)
    {
        __state = default;

        if (!MP.IsInMultiplayer) return;

        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings == null) return;

            __state = new SettingsSnapshot
            {
                InMultiplayer = true,
                AccessMode = settings.accessMode,
                AnimalsCanUse = settings.animalsCanUse,
                DisableSkipShock = settings.disableSkipShock,
                EnableSkipShockAvoidance = settings.enableSkipShockAvoidance,
                DisableUnpoweredSkipdoors = settings.disableUnpoweredSkipdoors,
                DefaultSkipdoorPower = settings.defaultSkipdoorPower,
                SkipCost = settings.skipCost,
                WorthItFactor = settings.worthItFactor,
                DisableTeleportFlashEffect = settings.disableTeleportFlashEffect,
                DebugVerboseLogging = settings.debugVerboseLogging,
                HasGame = Current.Game != null,
                PowerOverride = Current.Game != null
                    ? Current.Game.GetComponent<GameComponent_Skiptech>()?.skipdoorPowerOverride ?? -1
                    : -1
            };
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SkiptechSettingsSync.PreDoSettingsWindowContents failed: {exception}");
        }
    }

    private static void PostDoSettingsWindowContents(SettingsSnapshot __state)
    {
        if (!__state.InMultiplayer || !MP.IsInMultiplayer) return;

        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings == null) return;

            var powerChanged = false;

            if (settings.accessMode != __state.AccessMode) SyncedSetAccessMode((int)settings.accessMode);

            if (settings.animalsCanUse != __state.AnimalsCanUse) SyncedSetAnimalsCanUse(settings.animalsCanUse);

            if (settings.disableSkipShock != __state.DisableSkipShock)
                SyncedSetDisableSkipShock(settings.disableSkipShock);

            if (settings.enableSkipShockAvoidance != __state.EnableSkipShockAvoidance)
                SyncedSetEnableSkipShockAvoidance(settings.enableSkipShockAvoidance);

            if (settings.disableUnpoweredSkipdoors != __state.DisableUnpoweredSkipdoors)
                SyncedSetDisableUnpoweredSkipdoors(settings.disableUnpoweredSkipdoors);

            if (settings.defaultSkipdoorPower != __state.DefaultSkipdoorPower)
            {
                SyncedSetDefaultSkipdoorPower(settings.defaultSkipdoorPower);
                powerChanged = true;
            }

            if (!settings.skipCost.Equals(__state.SkipCost)) SyncedSetSkipCost(settings.skipCost);

            if (!settings.worthItFactor.Equals(__state.WorthItFactor)) SyncedSetWorthItFactor(settings.worthItFactor);

            if (settings.disableTeleportFlashEffect != __state.DisableTeleportFlashEffect)
                SyncedSetDisableTeleportFlashEffect(settings.disableTeleportFlashEffect);

            if (settings.debugVerboseLogging != __state.DebugVerboseLogging)
                SyncedSetDebugVerboseLogging(settings.debugVerboseLogging);

            if (__state.HasGame && Current.Game != null)
            {
                var gameComponent = Current.Game.GetComponent<GameComponent_Skiptech>();
                if (gameComponent != null && gameComponent.skipdoorPowerOverride != __state.PowerOverride)
                {
                    SyncedSetPowerOverride(gameComponent.skipdoorPowerOverride);
                    powerChanged = true;
                }
            }

            // Setters are queued first, Apply second, so remotes refresh defs with new values.
            if (powerChanged) SyncedApplyPower();
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SkiptechSettingsSync.PostDoSettingsWindowContents failed: {exception}");
        }
    }

    private static void SyncedSetAccessMode(int value)
    {
        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings != null) settings.accessMode = (AccessMode)value;
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedSetAccessMode failed: {exception}");
        }
    }

    private static void SyncedSetAnimalsCanUse(bool value)
    {
        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings != null) settings.animalsCanUse = value;
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedSetAnimalsCanUse failed: {exception}");
        }
    }

    private static void SyncedSetDisableSkipShock(bool value)
    {
        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings != null) settings.disableSkipShock = value;
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedSetDisableSkipShock failed: {exception}");
        }
    }

    private static void SyncedSetEnableSkipShockAvoidance(bool value)
    {
        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings != null) settings.enableSkipShockAvoidance = value;
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedSetEnableSkipShockAvoidance failed: {exception}");
        }
    }

    private static void SyncedSetDisableUnpoweredSkipdoors(bool value)
    {
        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings != null) settings.disableUnpoweredSkipdoors = value;
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedSetDisableUnpoweredSkipdoors failed: {exception}");
        }
    }

    private static void SyncedSetDefaultSkipdoorPower(int value)
    {
        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings != null) settings.defaultSkipdoorPower = value;
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedSetDefaultSkipdoorPower failed: {exception}");
        }
    }

    private static void SyncedSetSkipCost(float value)
    {
        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings != null) settings.skipCost = value;
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedSetSkipCost failed: {exception}");
        }
    }

    private static void SyncedSetWorthItFactor(float value)
    {
        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings != null) settings.worthItFactor = value;
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedSetWorthItFactor failed: {exception}");
        }
    }

    private static void SyncedSetDisableTeleportFlashEffect(bool value)
    {
        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings != null) settings.disableTeleportFlashEffect = value;
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedSetDisableTeleportFlashEffect failed: {exception}");
        }
    }

    private static void SyncedSetDebugVerboseLogging(bool value)
    {
        try
        {
            var settings = MigcorpSkiptechMod.Settings;
            if (settings != null) settings.debugVerboseLogging = value;
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedSetDebugVerboseLogging failed: {exception}");
        }
    }

    private static void SyncedSetPowerOverride(int value)
    {
        try
        {
            var gameComponent = Current.Game?.GetComponent<GameComponent_Skiptech>();
            if (gameComponent != null) gameComponent.skipdoorPowerOverride = value;
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedSetPowerOverride failed: {exception}");
        }
    }

    private static void SyncedApplyPower()
    {
        try
        {
            SkipdoorCustomPower.ApplyToActiveGame();
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} SyncedApplyPower failed: {exception}");
        }
    }

    private struct SettingsSnapshot
    {
        public bool InMultiplayer;
        public AccessMode AccessMode;
        public bool AnimalsCanUse;
        public bool DisableSkipShock;
        public bool EnableSkipShockAvoidance;
        public bool DisableUnpoweredSkipdoors;
        public int DefaultSkipdoorPower;
        public float SkipCost;
        public float WorthItFactor;
        public bool DisableTeleportFlashEffect;
        public bool DebugVerboseLogging;
        public bool HasGame;
        public int PowerOverride;
    }
}