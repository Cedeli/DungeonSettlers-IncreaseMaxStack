using System.Collections.Generic;
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

        var matchedKeys = new HashSet<string>();

        foreach (var itemData in allItems.ToArray())
        {
            if (string.IsNullOrEmpty(itemData.Key)) continue;

            if (Plugin.Overrides.TryGetValue(itemData.Key, out var overrideValue))
            {
                itemData.MaxStack = overrideValue;
                matchedKeys.Add(itemData.Key);
            }
            else if (Plugin.EnableGlobalMaxStack)
            {
                itemData.MaxStack = Plugin.GlobalMaxStack;
            }
        }

        foreach (var key in Plugin.Overrides.Keys)
        {
            if (!matchedKeys.Contains(key))
            {
                Plugin.Log.LogWarning(
                    $"ItemStackOverrides contains '{key}', but no item with that ID was found. Check spelling/casing against the wiki.");
            }
        }
    }
}