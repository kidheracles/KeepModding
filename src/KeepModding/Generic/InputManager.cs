using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ManagedMod.Generic;


public class InputManager : IInputManager
{
    private readonly Dictionary<string, IEnumerable<BeforeScriptCallbackHandler>> _preHooks = [];
    private readonly Dictionary<string, IEnumerable<AfterScriptCallbackHandler>> _postHooks = [];
    private readonly HashSet<string> _inputs = [];
    
    // private readonly string _inputEventName = "gml_Object_input_controller_object_Step_1";gml_Object_obj_input_controller_Step_1
    private readonly string _inputEventName = "gml_Object_obj_input_controller_Other_75";
    public InputManager()
    {
        Framework.PrintEx(AurieLogSeverity.Trace, "Listening for input events...");
        Game.Events.OnGameEvent += this.InputEvent;
        // Game.Events.OnFrame += this.InputFrame;
    }

    // private void InputFrame(int FrameNumber, double DeltaTime)
    // {
    //     Game.Engine.GetBuiltinVariable("keyboard_key");
    // }

    // Config Verbs: { keyboard_and_mouse : { car_left : [ arrow left,A ], up : [ arrow up,W ], car_right : [ arrow right,D ], accept : space, pause : escape, right : [ arrow right,D ], car_menu : C, cancel : backspace, action : enter, shoot : mouse button left, honk : space, down : [ arrow down,S ], special : shift, left : [ arrow left,A ] }, touch : { up : virtual button, accept : virtual button, pause : virtual button, special : virtual button, down : virtual button, right : virtual button, left : virtual button, cancel : virtual button, action : virtual button }, gamepad : { car_left : [ gamepad thumbstick l left,gamepad trigger l,gamepad dpad left ], left_down : gamepad thumbstick l down, left_left : gamepad thumbstick l left, left_right : gamepad thumbstick l right, up : [ gamepad thumbstick l up,gamepad dpad up ], accept : gamepad face south, rt : gamepad trigger r, pause : gamepad start, car_menu : gamepad face north, left_up : gamepad thumbstick l up, action : gamepad face west, shoot : [ gamepad trigger l,gamepad trigger r ], honk : gamepad thumbstick l click, down : [ gamepad thumbstick l down,gamepad dpad down ], special : gamepad face north, lb : gamepad shoulder l, select : gamepad select, dpad_down : gamepad dpad down, dpad_left : gamepad dpad left, dpad_right : gamepad dpad right, only_A : gamepad face south, only_B : gamepad face east, dpad_up : gamepad dpad up, rb : gamepad shoulder r, car_right : [ gamepad thumbstick l right,gamepad trigger r,gamepad dpad right ], lt : gamepad trigger l, right : [ gamepad thumbstick l right,gamepad dpad right ], cancel : gamepad face east, right_down : gamepad thumbstick r down, right_left : gamepad thumbstick r left, right_right : gamepad thumbstick r right, right_stick_press : gamepad thumbstick r click, left : [ gamepad thumbstick l left,gamepad dpad left ], right_up : gamepad thumbstick r up } }

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
            Framework.PrintEx(AurieLogSeverity.Trace, $"Event: {context.Name} ({context.Self.Name} ({context.Self.Members.Count}) -> {context.Other.Name} ({context.Other.Members.Count}))");
            try
            {
                foreach (GameVariable arg in context.Arguments)
                {
                    Framework.PrintEx(AurieLogSeverity.Trace, $"Argument: {arg}");
                }
            }
            catch (Exception ex)
            {
                Framework.PrintEx(AurieLogSeverity.Error, $"{ex.Message} Source: {ex.Source} {ex.Data}");
                Framework.PrintEx(AurieLogSeverity.Error, ex.StackTrace);
                throw;
            }
            foreach (KeyValuePair<string, GameVariable> member in context.Self.Members)
            {
                Framework.PrintEx(AurieLogSeverity.Trace, $"Key: {member.Key}, Value: {member.Value}");
            }
        }
    }

    public static GameVariable GetInputProfile(AurieManagedModule IModule, int playerIndex = 0)
    {
        return Game.Engine.CallScript("gml_Script_input_profile_get", [playerIndex]);
    }
}
