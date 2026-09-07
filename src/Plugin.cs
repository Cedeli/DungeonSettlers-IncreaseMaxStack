using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace IncreaseMaxStack;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("DungeonSettlers.exe")]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    private Harmony _harmony;

    public static ConfigFile PluginConfig { get; private set; }

    private static ConfigEntry<int> _globalMaxStack;
    private static ConfigEntry<bool> _enableGlobalMaxStack;
    private static ConfigEntry<string> _itemStackOverridesRaw;

    private static Dictionary<string, int> _overrides = new();

    public static int GlobalMaxStack => _globalMaxStack.Value;
    public static bool EnableGlobalMaxStack => _enableGlobalMaxStack.Value;

    public static IReadOnlyDictionary<string, int> Overrides => _overrides;

    public override void Load()
    {
        Log = base.Log;
        PluginConfig = Config;

        _enableGlobalMaxStack = Config.Bind("General", "EnableGlobalMaxStack", true,
            new ConfigDescription(
                "If true, changes ALL items to GlobalMaxStack, unless listed in ItemStackOverrides."
            )
        );

        _globalMaxStack = Config.Bind("General", "GlobalMaxStack", 999,
            new ConfigDescription(
                "The default max stack applied to all items if EnableGlobalMaxStack is true.",
                new AcceptableValueRange<int>(1, 9999)
            )
        );

        _itemStackOverridesRaw = Config.Bind("Individual Item Stacks", "ItemStackOverrides", "",
            new ConfigDescription(
                "Semicolon-separated list of ItemId=MaxStack pairs, e.g. ITEM_WildLongGrass=100;ITEM_LumaLog=50. " +
                "Find item IDs on the wiki. Unknown IDs are logged as a warning at startup."
            )
        );

        ParseOverrides(_itemStackOverridesRaw.Value);
        _itemStackOverridesRaw.SettingChanged += (_, _) => ParseOverrides(_itemStackOverridesRaw.Value);

        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private static void ParseOverrides(string raw)
    {
        var result = new Dictionary<string, int>(StringComparer.Ordinal);

        if (string.IsNullOrWhiteSpace(raw))
        {
            _overrides = result;
            return;
        }

        var entries = raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var entry in entries)
        {
            var parts = entry.Split('=', 2);
            if (parts.Length != 2)
            {
                Log.LogWarning($"Malformed ItemStackOverrides entry (expected Key=Value): '{entry}'");
                continue;
            }

            var key = parts[0].Trim();
            var valueStr = parts[1].Trim();

            if (key.Length == 0)
            {
                Log.LogWarning($"ItemStackOverrides entry has an empty key: '{entry}'");
                continue;
            }

            if (!int.TryParse(valueStr, out var value) || value <= 0)
            {
                Log.LogWarning(
                    $"ItemStackOverrides entry for '{key}' has an invalid value: '{valueStr}' (must be a positive integer)");
                continue;
            }

            if (!result.TryAdd(key, value))
            {
                Log.LogWarning(
                    $"Duplicate ItemStackOverrides entry for '{key}' - keeping first value ({result[key]}), ignoring {value}.");
            }
        }

        _overrides = result;
    }

    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        return true;
    }
}