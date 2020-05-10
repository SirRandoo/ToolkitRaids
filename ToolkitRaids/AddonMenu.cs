using System.Collections.Generic;
using System.IO;
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
                ),
                new FloatMenuOption(
                    "ToolkitRaids.AddonMenu.ForceNewRaid".Translate(),
                    () =>
                    {
                        if (!UnityData.IsInMainThread)
                        {
                            return;
                        }

                        Log.Message("ToolkitRaids :: Forcibly starting a new raid...");

                        var result = Path.GetRandomFileName()
                            .Replace(".", "")
                            .Substring(0, 8);

                        ToolkitRaids.RecentRaids.Enqueue(result);

                        Log.Message($@"ToolkitRaids :: Scheduled a new raid with leader ""{result}"".");
                    }
                ),
                new FloatMenuOption(
                    "ToolkitRaids.AddonMenu.ForceNewRaidLarge".Translate(),
                    () =>
                    {
                        if (!UnityData.IsInMainThread)
                        {
                            return;
                        }

                        Log.Message("ToolkitRaids :: Forcible starting a new large raid...");

                        var component = Current.Game.GetComponent<GameComponentTwitchRaid>();

                        if (component == null)
                        {
                            return;
                        }

                        var leader = Path.GetRandomFileName()
                            .Replace(".", "")
                            .Substring(0, 8);
                        var raid = new Raid(leader);

                        for (var index = 0; index < 20; index++)
                        {
                            raid.Army.Add(
                                Path.GetRandomFileName()
                                    .Replace(".", "")
                                    .Substring(0, 8)
                            );
                        }

                        component.RegisterRaid(raid);
                    }
                )
            };
        }
    }
}
