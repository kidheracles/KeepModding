using System.Runtime.InteropServices;
using System.Collections.Immutable;

namespace ManagedMod.Generic;

/// <summary>
/// An implementation for inspecting various aspects of the game state.
/// </summary>
public class GameInspector : IGameInspector
{
    private Dictionary<Tuple<string,string>, int> _eventCounts = [];
    private double _totalMs;
    private double _printInterval = 5000; // 5 seconds
    private double _nextPrintTime;
    private HashSet<Tuple<string,string>> _previousEvents = [];
    private HashSet<Tuple<string,string>> _intervalEvents = [];

    public void Initialize(AurieManagedModule Module)
    {
        this._nextPrintTime = this._printInterval;
        Game.Events.OnGameEvent += this.CountEvent;
        Game.Events.OnFrame += this.ReportEventsSometimes;
        Framework.Print($"Loaded Event Counter");
    }

    public void finish(AurieManagedModule IModule)
    {
        Game.Events.OnGameEvent -= this.CountEvent;
        Game.Events.OnFrame -= this.ReportEventsSometimes;
        Framework.Print($"Unloaded Event Counter");
        Framework.Print($"--- Event Counter --- [{this._totalMs}]");
        PrintEventCounter(this._eventCounts);
    }

    private void ReportEventsSometimes(int FrameNumber, double DeltaTime)
    {
        this._totalMs += DeltaTime;
        if (this._totalMs > this._nextPrintTime)
        {
            // Framework.Print($"Frame: {FrameNumber}, Delta Time: {DeltaTime.ToString(CultureInfo.InvariantCulture)}");
            this._nextPrintTime += this._printInterval;
            this.PrintNewEvents();
        }
    }

    private void CountEvent(CodeExecutionContext context)
    {
        var eventKey = Tuple.Create(context.Name, context.Self.Name);
        this._eventCounts[eventKey] = this._eventCounts.GetValueOrDefault(eventKey, 0) + 1;
        this._intervalEvents.Add(eventKey);
    }
    private static void PrintEventCounter(Dictionary<Tuple<string,string>, int> eventCounts)
    {
        IOrderedEnumerable<KeyValuePair<Tuple<string,string>, int>> sortedEvents = eventCounts.OrderBy(entry => entry.Key);
        foreach (KeyValuePair<Tuple<string,string>, int> entry in sortedEvents)
        {
            Framework.PrintEx(AurieLogSeverity.Trace, $"Event: {entry.Key}, Count: {entry.Value}");
        }

    }
    private void PrintNewEvents()
    {
        var newEventKeys = this._intervalEvents.Except(this._previousEvents).ToImmutableList();

        if (!newEventKeys.IsEmpty)
        {
            Framework.Print($"--- New Events {newEventKeys.Count}/{this._eventCounts.Count}---");
            PrintEventCounter(this._eventCounts.Where(entry => newEventKeys.Contains(entry.Key)).ToDictionary(entry => entry.Key, entry => entry.Value));
            this._previousEvents.UnionWith(this._intervalEvents);
        }
        this._intervalEvents.Clear();
    }

    public static void InspectGlobalObject()
    {

        GameObject globalObject = Game.Engine.GetGlobalObject();
        Framework.Print($"Does Global Object exist? {globalObject.IsInstance()} How many members? {globalObject.Members.Count}");

        foreach (KeyValuePair<string, GameVariable> member in globalObject.Members)
        {
            try
            {
                Framework.Print($"Key: {member.Key}, Value: {member.Value}");
            }
            catch (SEHException sehex)
            {
                Framework.PrintEx(AurieLogSeverity.Warning, $"Key: {member.Key}, Error: {sehex.Message}");
            }
        }
    }

    public static void InspectRunningRoom()
    {

        GameRoom runningRoom = Game.Engine.GetRunningRoom();
        Framework.Print($"How many instances? Active: {runningRoom.ActiveInstances.Count} Inactive: {runningRoom.InactiveInstances.Count}");

        foreach (GameInstance gameInstance in runningRoom.ActiveInstances)
        {
            Framework.Print($"--- Instance: ${gameInstance.Name} (${gameInstance.ID}) ---");
            foreach (KeyValuePair<string, GameVariable> member in gameInstance.Members)
            {
                try
                {
                    Framework.Print($"Key: {member.Key}, Value: {member.Value}");
                }
                catch (SEHException sehex)
                {
                    Framework.PrintEx(AurieLogSeverity.Warning, $"Key: {member.Key}, Error: {sehex.Message}");
                }
            }
        }
    }

}