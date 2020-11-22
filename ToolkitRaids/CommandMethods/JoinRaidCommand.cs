using JetBrains.Annotations;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace SirRandoo.ToolkitRaids.CommandMethods
{
    [UsedImplicitly]
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

            return Current.Game != null;
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
            ToolkitRaids.ViewerQueue.Enqueue(twitchCommand.Username);
        }
    }
}
