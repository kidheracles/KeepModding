using System.Runtime.InteropServices;
using System.Globalization;

namespace ManagedMod.Generic;

/// <summary>
/// An implementation for inspecting various aspects of the game state.
/// </summary>
public class GameInspector : IGameInspector
{
    private Dictionary<string, int> _eventCounts = [];
    private double _totalMs;
    private double _printInterval = 10000; // 10 seconds
    private double _nextPrintTime;


    public void Initialize(AurieManagedModule IModule)
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
        this.PrintEventCounter();
    }

    private void ReportEventsSometimes(int FrameNumber, double DeltaTime)
    {
        this._totalMs += DeltaTime;
        if (this._totalMs > this._nextPrintTime)
        {
            // Framework.Print($"Frame: {FrameNumber}, Delta Time: {DeltaTime.ToString(CultureInfo.InvariantCulture)}");
            this._nextPrintTime += this._printInterval;
            this.PrintEventCounter();
        }
    }

    private void CountEvent(CodeExecutionContext context)
    {
        this._eventCounts[context.Name] = this._eventCounts.GetValueOrDefault(context.Name, 0) + 1;

    }
    private void PrintEventCounter()
    {
        Framework.Print($"--- Event Counter --- [{this._totalMs}]");
        IOrderedEnumerable<KeyValuePair<string, int>> sortedEvents = this._eventCounts.OrderBy(entry => entry.Key);
        foreach (KeyValuePair<string, int> entry in sortedEvents)
        {
            Framework.PrintEx(AurieLogSeverity.Trace, $"Event: {entry.Key}, Count: {entry.Value}");
        }

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

    public static void SnapshotAfterInput(ScriptExecutionContext context)
    {
        if (context != null)
        {
            Framework.Print($"Snapshot requested for {context.Self.Name} ({context.Self.Members.Count})");
        }
    }

    
    public static void SnapshotAfterBuiltin(BuiltinExecutionContext context)
    {
        if (context != null)
        {
            Framework.Print($"Snapshot requested for {context.Self.Name} ({context.Self.Members.Count})");
        }
    }
}