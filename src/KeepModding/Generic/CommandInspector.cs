namespace ManagedMod.Generic;

/// <summary>
/// An implementation for inspecting various aspects of the game state.
/// </summary>

internal sealed class CommandInspector : IDisposable
{
    internal required AurieManagedModule _module;
    internal readonly HashSet<string> _builtins = [];
    internal readonly HashSet<string> _scripts = [];
    internal readonly HashSet<Tuple<string, string?>> _toDo = [];
    internal readonly HashSet<Tuple<string, string?>> _done = [];
    internal readonly HashSet<string> _objectNames = [];

    public void Watch(string Command, bool IsBuiltin, string? objectName = null)
    {
        var cmd = Tuple.Create(Command, objectName);
        if (this._toDo.Contains(cmd))
        {
            return;
        }
        this._toDo.Add(cmd);
        try
        {
            if (IsBuiltin)
            {
                if (this._builtins.Contains(Command))
                {
                    return;
                }
                this._builtins.Add(Command);
                Game.Events.AddPostBuiltinNotification(this._module, Command, this.SnapshotAfterBuiltin);
            }
            else
            {
                if (this._scripts.Contains(Command))
                {
                    return;
                }
                this._scripts.Add(Command);
                Game.Events.AddPostScriptNotification(this._module, Command, this.SnapshotAfterScript);
            }
            Framework.Print($"Added hook for {(IsBuiltin ? "builtin" : "script")} {Command}");
        }
        catch (InvalidOperationException ex)
        {
            Framework.PrintEx(AurieLogSeverity.Warning, $"Could not add hook for {(IsBuiltin ? "builtin" : "script")} {Command}: {ex.Message}");
        }
    }

    public void Dispose()
    {
        foreach (string builtin in this._builtins)
        {
            try
            {
                Game.Events.RemovePostBuiltinNotification(this._module, builtin, this.SnapshotAfterBuiltin);
            }
            catch (InvalidOperationException ex)
            {
                Framework.PrintEx(AurieLogSeverity.Warning, $"Could not remove hooks for builtin {builtin}: {ex.Message}");
            }
        }
        this._builtins.Clear();
        foreach (string script in this._scripts)
        {
            try
            {
                Game.Events.RemovePostScriptNotification(this._module, script, this.SnapshotAfterScript);
            }
            catch (InvalidOperationException ex)
            {
                Framework.PrintEx(AurieLogSeverity.Warning, $"Could not remove hooks for script {script}: {ex.Message}");
            }
        }
        this._scripts.Clear();
    }

    private void printObject(GameObject obj)
    {
        if (!this._objectNames.Contains(obj.Name))
        {
            this._objectNames.Add(obj.Name);
            Framework.Print($"{obj.Name} ({obj.Members.Count})");
            foreach (KeyValuePair<string, GameVariable> member in obj.Members)
            {
                try {
                    Framework.Print($"    {member.Key} = {member.Value}");
                }
                catch (InvalidCastException ex)
                {
                    Framework.Print($"    {member.Key} = Error! {ex.Message}");
                }
            }
        }
    }

    private void Snapshot(IReadOnlyList<GameVariable> argv, GameObject self, GameObject other)
    {
        foreach (GameVariable arg in argv)
        {
            Framework.Print($"    Argument: {(arg.IsAccessible() && arg.TryGetString(out string argStr) ? argStr : $"({arg.Type})")}");
        }
        this.printObject(self);
        this.printObject(other);
    }

    private bool isDone(string command, string? selfName)
    {
        var cmd = Tuple.Create(command, selfName?.Split(" ").Last());
        if (this._toDo.Contains(cmd))
        {
            return this._done.Contains(cmd);
        }
        else
        {
            return this._done.Contains(Tuple.Create<string, string?>(command, null));
        }
    }

    private void markDone(string command, string? selfName)
    {
        var cmd = Tuple.Create(command, selfName?.Split(" ").Last());
        if (this._toDo.Contains(cmd))
        {

            this._done.Add(cmd);
        }
        else
        {
            this._done.Add(Tuple.Create<string, string?>(command, null));
        }

    }

    private void SnapshotAfterScript(ScriptExecutionContext context)
    {
        if (context == null)
        {
            Framework.PrintEx(AurieLogSeverity.Warning, $"Null context for watched script post-hook!");
            return;
        }
        if (!this.isDone(context.Name, context.Self.Name))
        {
            Framework.Print($"Script snapshot: {context.Name}");
            this.Snapshot(context.Arguments, context.Self, context.Other);
            this.markDone(context.Name, context.Self.Name);
        }
    }

    private void SnapshotAfterBuiltin(BuiltinExecutionContext context)
    {
        if (context == null)
        {
            Framework.PrintEx(AurieLogSeverity.Warning, $"Null context for watched builtin post-hook!");
            return;
        }
        if (!this.isDone(context.Name, context.Self.Name))
        {
            Framework.Print($"Builtin snapshot: {context.Name}");
            this.Snapshot(context.Arguments, context.Self, context.Other);
            this.markDone(context.Name, context.Self.Name);
        }
    }
}