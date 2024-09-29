// MIT License
//
// Copyright (c) 2024 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitRaids.Models;
using ToolkitCore;
using TwitchLib.Client.Events;

namespace SirRandoo.ToolkitRaids.Patches;

[HarmonyPatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class TwitchClientPatch
{
    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(TwitchWrapper), nameof(TwitchWrapper.OnRaidNotification));
    }

    [HarmonyFinalizer]
    private static void OnRaidNotification(object sender, OnRaidNotificationArgs e)
    {
        RaidLogger.Debug("Received raid notification from the Twitch client");

        var leader = new RaidLeader { Username = e.RaidNotification.Login };

        if (!int.TryParse(e.RaidNotification.MsgParamViewerCount, out int count))
        {
            RaidLogger.Warn($"Could not parse viewer count of {e.RaidNotification.MsgParamViewerCount}. Defaulted to 1");
            count = 1;
        }

        leader.ViewerCount = count;

        RaidMod.RecentRaids.Enqueue(leader);
    }
}
