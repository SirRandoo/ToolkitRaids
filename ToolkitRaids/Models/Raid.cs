// MIT License
//
// Copyright (c) 2021 SirRandoo
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

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitRaids.Workers;
using UnityEngine;
using Verse;
using Random = UnityEngine.Random;

namespace SirRandoo.ToolkitRaids.Models;

public class Raid : IExposable
{
    private List<string> _army = [];
    private string _leader = null!;
    private float _timer;

    public string ArmyCountLabel { get; private set; } = "0";

    public int TotalTroops => _army.Count + 1;

    public string Leader
    {
        get => _leader;
        set => _leader = value;
    }

    public float Timer
    {
        get => _timer;
        set => _timer = value;
    }

    public List<string> Army
    {
        get => _army;
        set => _army = value;
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref _timer, "timer");
        Scribe_Values.Look(ref _leader!, "leader");
        Scribe_Collections.Look(ref _army, "army", LookMode.Value);
    }

    public void Recruit(string viewer)
    {
        _army.Add(viewer);
        ArmyCountLabel = _army.Count.ToString("N0");
    }

    public void Unrecruit(string viewer)
    {
        _army.RemoveAll(v => v.EqualsIgnoreCase(viewer));
        ArmyCountLabel = _army.Count.ToString("N0");
    }

    internal void Spawn()
    {
        Map? map = Current.Game.Maps.Where(m => m.IsPlayerHome).RandomElementWithFallback();

        if (map == null)
        {
            return;
        }

        TwitchRaidParams defaultParams = TwitchRaidParams.ForRaid(this, map);

        if (!RaidMod.Instance.Settings.UseStoryteller)
        {
            float tellerPoints = defaultParams.points;
            float twitchPoints = RaidMod.Instance.Settings.PointsPerPerson * (_army.Count + 1);
            float diff = tellerPoints - twitchPoints;
            float finalPoints = twitchPoints;

            if (diff > tellerPoints * 0.95f)
            {
                RaidLogger.Debug("Point differential too high!");

                float factor = Mathf.Clamp(Mathf.Round((_army.Count + 1f) / 10f), 10f, 100f) + Random.Range(0.75f, 1.5f);
                finalPoints = Mathf.Clamp(twitchPoints * (diff / tellerPoints * factor), twitchPoints, RaidMod.Instance.Settings.MaximumAllowedPoints);

                RaidLogger.Warn($"Adjusted the raid's points from {twitchPoints:N2} to {finalPoints:N2} (Storyteller points: {tellerPoints:N2})");
            }

            RaidLogger.Debug($"Teller points: {tellerPoints:N4}");
            RaidLogger.Debug($"ToolkitRaid points: {twitchPoints:N4}");
            RaidLogger.Debug($"Differential: {diff:N4}");
            RaidLogger.Debug($"Final points: {finalPoints:N4}");
            defaultParams.points = finalPoints;
        }

        defaultParams.customLetterLabel = "ToolkitRaids.Letters.Title".Translate(_leader.CapitalizeFirst());
        defaultParams.forced = true;
        defaultParams.raidNeverFleeIndividual = true;
        defaultParams.pawnCount = _army.Count + 1;

        var worker = new TwitchRaidWorker { def = IncidentDefOf.RaidEnemy };

        try
        {
            worker.TryExecute(defaultParams);
        }
        catch (Exception e)
        {
            RaidLogger.Error("Could not execute raid worker", e);
        }
    }
}
