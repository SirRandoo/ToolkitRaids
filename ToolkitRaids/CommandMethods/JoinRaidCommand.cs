using JetBrains.Annotations;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace SirRandoo.ToolkitRaids.CommandMethods;

[UsedImplicitly]
internal class JoinRaidCommand(ToolkitChatCommand command) : CommandMethod(command)
{
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
        RaidMod.ViewerQueue.Enqueue(twitchCommand.Username);
    }
}
