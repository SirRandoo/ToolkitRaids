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
                    "ToolkitRaids.AddonMenu.Settings".TranslateSimple(),
                    () => Find.WindowStack.Add(new Window_ModSettings(LoadedModManager.GetMod<ToolkitRaids>()))
                ),
                new FloatMenuOption(
                    "ToolkitRaids.AddonMenu.ForceNoRegister".TranslateSimple(),
                    () =>
                    {
                        RaidLogger.Warn("Forcibly closing registration for all pending raids...");
                        Current.Game?.GetComponent<GameComponentTwitchRaid>()?.ForceCloseRegistry();
                    }
                ),
                new FloatMenuOption(
                    "ToolkitRaids.AddonMenu.ForceNewRaid".TranslateSimple(),
                    () =>
                    {
                        string result = ToolkitRaids.GenerateNameForRaid();

                        ToolkitRaids.RecentRaids.Enqueue(result);
                        RaidLogger.Info($@"Scheduled a new raid with leader ""{result}"".");
                    }
                ),
                new FloatMenuOption(
                    "ToolkitRaids.AddonMenu.ForceNewRaidLarge".TranslateSimple(),
                    () =>
                    {
                        RaidLogger.Info("Forcibly starting a new large raid...");

                        var component = Current.Game.GetComponent<GameComponentTwitchRaid>();

                        if (component == null)
                        {
                            return;
                        }

                        var raid = new Raid(ToolkitRaids.GenerateNameForRaid());

                        for (var index = 0; index < Rand.Range(20, 50); index++)
                        {
                            raid.Recruit(ToolkitRaids.GenerateNameForRaid());
                        }

                        component.RegisterRaid(raid);
                    }
                )
            };
        }
    }
}
