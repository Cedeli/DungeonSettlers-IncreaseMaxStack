using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace IncreaseMaxStack;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    private Harmony _harmony;
    
    public static ConfigFile PluginConfig { get; private set; }
    
    private static ConfigEntry<int> _globalMaxStack;
    private static ConfigEntry<bool> _enableGlobalMaxStack;
    
    public static int GlobalMaxStack => _globalMaxStack.Value;
    public static bool EnableGlobalMaxStack => _enableGlobalMaxStack.Value;
    
    public override void Load()
    {
        Log = base.Log;
        PluginConfig = Config;
        
        _enableGlobalMaxStack = Config.Bind("General", "EnableGlobalMaxStack", true, 
            new ConfigDescription("If true, changes ALL items to the GlobalMaxStack value, unless overridden in [Individual Item Stacks].")
        );

        _globalMaxStack = Config.Bind("General", "GlobalMaxStack", 999, 
            new ConfigDescription(
                "The default max stack applied to all items if EnableGlobalMaxStack is true."
            )
        );
        
        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
        
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
    
    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        return true;
    }
}
