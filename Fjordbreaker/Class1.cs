using BepInEx;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("com.yourname.fjordbreaker", "Fjordbreaker Mod", "1.0.0")]
public class FjordbreakerMod : BaseUnityPlugin
{
    private static BepInEx.Logging.ManualLogSource Log;

    private void Awake()
    {
        Log = Logger;
        new Harmony("com.yourname.fjordbreaker").PatchAll();
        Log.LogInfo("Fjordbreaker Mod loaded!");
    }

    private static void ModifyKrom()
    {
        var db = ObjectDB.instance;
        if (db == null)
        {
            Log.LogWarning("ObjectDB.instance is null");
            return;
        }

        foreach (var prefab in db.m_items)
        {
            Log.LogInfo($"Prefab loaded: {prefab.name}");
        }

        var krom = db.m_items.Find(i => i.name == "THSwordKrom");
        var mistwalker = db.m_items.Find(i => i.name == "SwordMistwalker");

        foreach (var recipe in db.m_recipes)
        {
            if (recipe?.m_item != null)
                Log.LogInfo($"Recipe item: {recipe.m_item.name}");
        }
        var kromRecipe = db.m_recipes.Find(r => r?.m_item != null && r.m_item.name == "THSwordKrom");

        if (kromRecipe != null)
        {
            Log.LogInfo($"Krom recipe crafting station: {kromRecipe.m_craftingStation?.name ?? "None"}");
            kromRecipe.m_enabled = true;
        }
        else
        {
            Log.LogWarning("Krom recipe not found.");
        }

        if (krom && mistwalker)
        {
            var kromData = krom.GetComponent<ItemDrop>().m_itemData;
            var mistData = mistwalker.GetComponent<ItemDrop>().m_itemData;

            kromData.m_shared.m_name = "Fjordbreaker";
            kromData.m_shared.m_damages.m_frost = mistData.m_shared.m_damages.m_frost;
            kromData.m_shared.m_attackStatusEffect = mistData.m_shared.m_attackStatusEffect;
            kromData.m_shared.m_damages.m_chop = kromData.m_shared.m_damages.m_slash;
            kromData.m_shared.m_toolTier = 3;

            Log.LogInfo("Krom modified to Fjordbreaker with frost/chill and woodcutting ability.");
        }
        else
        {
            Log.LogWarning("Could not find Krom or Mistwalker.");
        }
    }

    [HarmonyPatch(typeof(ObjectDB), "Awake")]
    public static class ObjectDB_Awake_Patch
    {
        static void Postfix() => ModifyKrom();
    }

    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.CopyOtherDB))]
    public static class ObjectDB_CopyOtherDB_Patch
    {
        static void Postfix() => ModifyKrom();
    }

    [HarmonyPatch(typeof(Player), "OnSpawned")]
    public static class Player_OnSpawned_Patch
    {
        static void Postfix() => ModifyKrom();
    }
}
