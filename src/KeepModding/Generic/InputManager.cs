using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ManagedMod.Generic;


public class InputManager : IInputManager
{
    private readonly Dictionary<string, IEnumerable<BeforeScriptCallbackHandler>> _preHooks = [];
    private readonly Dictionary<string, IEnumerable<AfterScriptCallbackHandler>> _postHooks = [];
    private readonly HashSet<string> _inputs = [];
    private readonly string _inputEventName = "gml_Object_input_controller_object_Step_1";
    public InputManager()
    {
        Framework.PrintEx(AurieLogSeverity.Trace, "Listening for input events...");
        Game.Events.OnGameEvent += this.InputEvent;
    }

    private AurieStatus _BindInput(AurieManagedModule Module, String Input)
    {
        if (!this._inputs.Contains(Input))
        {
            try
            {
                Framework.PrintEx(AurieLogSeverity.Trace, $"Attempting to tell Input library to listen for {Input}");
                // GameVariable result = Game.Engine.CallScript("gml_Script_input_binding_key", Input);
                // Framework.PrintEx(AurieLogSeverity.Trace, $"Bind {Input} result: {result}");
                this._preHooks[Input] = [];
                this._postHooks[Input] = [];
                this._inputs.Add(Input);
            }
            catch (InvalidOperationException ex)
            {
                Framework.PrintEx(AurieLogSeverity.Warning, $"Error: {ex.Message} (Valid input? Does the game use the Input library?)");
                return AurieStatus.ModuleInitializationFailed;
            }
        }
        return AurieStatus.Success;
    }

    public AurieStatus AddPostInputHook(AurieManagedModule IModule, string Input, AfterScriptCallbackHandler Callback)
    {        // Simple ASCII-safe hash for the callback method name
        // This is a basic example and might not be suitable for all hashing needs.
        // For more robust hashing, consider using cryptographic hash functions.

        if (Callback == null)
        {
            Framework.PrintEx(AurieLogSeverity.Warning, $"Can't register null callback for input {Input}");
            return AurieStatus.InvalidParameter;
        }
        try
        {
            AurieStatus result = this._BindInput(IModule, Input);
            if (result != AurieStatus.Success)
            {
                return AurieStatus.ModuleInitializationFailed;
            }
            Game.Events.AddPostScriptNotification(IModule, "gml_Script_input_check_released", this._PostInputEvent);
            this._postHooks[Input] = this._postHooks[Input].Append(Callback);
        }
        catch (InvalidOperationException ex)
        {
            Framework.PrintEx(AurieLogSeverity.Warning, $"Error: {ex.Message} (Valid input? Is Input library loaded?)");
            return AurieStatus.ModuleInitializationFailed;
        }
        return AurieStatus.Success;
    }

    private void _PostInputEvent(ScriptExecutionContext context)
    {
        Framework.PrintEx(AurieLogSeverity.Trace, $"Event: input_check_released, Name: {context.Name}, Self: {context.Self.Name}, Other: {context.Other.Name}");
        foreach (KeyValuePair<string, GameVariable> member in context.Self.Members)
        {
            try
            {
                Framework.PrintEx(AurieLogSeverity.Trace, $"Key: {member.Key}, Value: {member.Value}");
                // foreach(Callback in _postHooks[])
            }
            catch (SEHException sehex)
            {
                Framework.PrintEx(AurieLogSeverity.Warning, $"Key: {member.Key}, Error: {sehex.Message}");
            }
        }
    }

    public void InputEvent(CodeExecutionContext context)
    {
        if (context != null && context.Name == this._inputEventName)
        {
            Framework.PrintEx(AurieLogSeverity.Trace, $"Event: {context.Name} ({context.Self.Name} -> {context.Other.Name})");
            foreach (GameVariable arg in context.Arguments)
            {
                Framework.PrintEx(AurieLogSeverity.Trace, $"Argument: {arg}");
            }
            foreach (KeyValuePair<string, GameVariable> member in context.Self.Members)
            {
                try
                {
                    Framework.PrintEx(AurieLogSeverity.Trace, $"Key: {member.Key}, Value: {member.Value}");
                }
                catch (SEHException sehex)
                {
                    Framework.PrintEx(AurieLogSeverity.Warning, $"Key: {member.Key}, Error: {sehex.Message}");
                }
            }
        }
    }

    public static GameVariable GetInputProfile(AurieManagedModule IModule, int playerIndex = 0)
    {
        return Game.Engine.CallScript("gml_Script_input_profile_get", [playerIndex]);
    }
}
