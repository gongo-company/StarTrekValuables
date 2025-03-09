using HarmonyLib;
using StarTrekValuables.Scripts;
using UnityEngine;

namespace StarTrekValuables
{
    [HarmonyPatch(typeof(ValuableObject), nameof(ValuableObject.Discover))]
    static class ValuableObjectDiscoverPatch
    {
        static bool Prefix(ValuableObject __instance, ValuableDiscoverGraphic.State _state)
        {
            if (_state == ValuableDiscoverGraphic.State.Bad)
            {
                // The original was already overridden, just skip.
                return true;
            }
            DaxMug daxMugComp;
            if (__instance.TryGetComponent<DaxMug>(out daxMugComp))
            {
                Plugin.Logger.LogInfo("DaxMug discovered. Running patch.");

                daxMugComp.DiscoverScare();

                // Call the original method with the overridden state and prevent the original from running
                __instance.Discover(ValuableDiscoverGraphic.State.Bad);
                return false;
            }
            return true;
        }
    }
}
