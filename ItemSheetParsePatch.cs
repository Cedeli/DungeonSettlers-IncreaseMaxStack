using HarmonyLib;
using Il2CppSystem.IO;
using Refactor.Util;

namespace IncreaseMaxStack;

[HarmonyPatch(typeof(ItemSheet))]
public class ItemSheetParsePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ItemSheet.Parse))]
    public static void PostfixItemSheetParse(ItemSheet __instance)
    {
        var allItems = __instance?.GetAll();
        if (allItems == null) return;
        
        Plugin.PluginConfig.SaveOnConfigSet = false;
        
        foreach (var itemData in allItems.ToArray()) 
        {
            if (string.IsNullOrEmpty(itemData.Key)) continue;
            
            var itemStackConfig = Plugin.PluginConfig.Bind(
                "Individual Item Stacks", 
                itemData.Key, 
                -1, 
                $"Vanilla default is {itemData.MaxStack}."
            );
            
            if (itemStackConfig.Value != -1)
            {
                itemData.MaxStack = itemStackConfig.Value;
            }
            else if (Plugin.EnableGlobalMaxStack)
            {
                itemData.MaxStack = Plugin.GlobalMaxStack;
            }
        }
        
        Plugin.PluginConfig.Save();
        Plugin.PluginConfig.SaveOnConfigSet = true;
    }
}