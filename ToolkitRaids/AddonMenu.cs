using System.Collections.Generic;
using ToolkitCore.Interfaces;
using ToolkitCore.Windows;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    public class AddonMenu : IAddonMenu
    {
        public List<FloatMenuOption> MenuOptions()
        {
            return new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "ToolkitRaids.AddonMenu.Settings".Translate(),
                    () =>
                    {
                        var window = new Window_ModSettings(LoadedModManager.GetMod<ToolkitRaids>());

                        Find.WindowStack.TryRemove(window.GetType());
                        Find.WindowStack.Add(window);
                    }
                ),
                new FloatMenuOption(
                    "ToolkitRaids.AddonMenu.ForceNoRegister".Translate(),
                    () =>
                    {
                        if (!UnityData.IsInMainThread)
                        {
                            return;
                        }

                        Log.Message("ToolkitRaids :: Forcibly closing registration for all pending raids...");
                        var component = Current.Game?.GetComponent<GameComponentTwitchRaid>();

                        component?.ForceCloseRegistry();
                    }
                )
            };
        }
    }
}
