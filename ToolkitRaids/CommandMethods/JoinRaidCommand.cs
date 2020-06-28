using ToolkitCore.Controllers;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace SirRandoo.ToolkitRaids.CommandMethods
{
    public class JoinRaidCommand : CommandMethod
    {
        public JoinRaidCommand(ToolkitChatCommand command) : base(command)
        {
        }

        public override bool CanExecute(ITwitchCommand twitchCommand)
        {
            if (!base.CanExecute(twitchCommand))
            {
                return false;
            }

            return Current.Game?.GetComponent<GameComponentTwitchRaid>()?.CanJoinRaid() ?? false;
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
            var component = Current.Game?.GetComponent<GameComponentTwitchRaid>();

            if (component == null)
            {
                return;
            }

            if (!component.TryJoinRaid(ViewerController.GetViewer(twitchCommand.Username)))
            {
                RaidLogger.Warn($@"Could not add ""{twitchCommand.Username}"" to any raid.");
            }
        }
    }
}
