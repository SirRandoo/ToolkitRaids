using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitRaids;

[PublicAPI]
public sealed class Settings : ModSettings
{
    private int _duration;
    private float _maximumAllowedPoints;
    private bool _mergeRaids;
    private string _messageToSend = null!;
    private int _minimumRaiders;
    private float _pointsPerPerson;
    private bool _sendMessage;
    private bool _useStoryteller;

    public float PointsPerPerson
    {
        get => _pointsPerPerson;
        set => _pointsPerPerson = value;
    }

    public float MaximumAllowedPoints
    {
        get => _maximumAllowedPoints;
        set => _maximumAllowedPoints = value;
    }

    public bool MergeRaids
    {
        get => _mergeRaids;
        set => _mergeRaids = value;
    }

    public bool UseStoryteller
    {
        get => _useStoryteller;
        set => _useStoryteller = value;
    }

    public int Duration
    {
        get => _duration;
        set => _duration = value;
    }

    public bool SendMessage
    {
        get => _sendMessage;
        set => _sendMessage = value;
    }

    public string MessageToSend
    {
        get => _messageToSend;
        set => _messageToSend = value;
    }

    public int MinimumRaiders
    {
        get => _minimumRaiders;
        set => _minimumRaiders = value;
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref _mergeRaids, "mergeRaids");
        Scribe_Values.Look(ref _sendMessage, "sendMessage");
        Scribe_Values.Look(ref _duration, "duration", 60);
        Scribe_Values.Look(ref _minimumRaiders, "minimumRaiders", 2);
        Scribe_Values.Look(ref _useStoryteller, "storyteller", true);
        Scribe_Values.Look(ref _pointsPerPerson, "pointsPerPerson", 50f);
        Scribe_Values.Look(ref _maximumAllowedPoints, "maxPoints", 20000f);
        Scribe_Values.Look(ref _messageToSend, "messageToSend", "!so %raider%");
    }
}
