using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace ManagedMod.Core;

internal sealed class Title
{
    const string _builtinName = "room_goto";
    const string _eventName = "gml_Object_obj_title_screen_controller_Step_0";
    public Title(AurieManagedModule Module)
    {
        try
        {
            // GameVariable index = Game.Engine.CallFunction("asset_get_index", ["obj_title_screen_controller"]);
            // Framework.Print($"obj_title_screen_controller: {index.ToInt32()}");
            Game.Events.AddPostBuiltinNotification(Module, _builtinName, WriteKeepModdingTitle);
            Game.Events.OnGameEvent += this.OnGameEvent;
        }
        catch (InvalidOperationException ex)
        {
            Framework.PrintEx(AurieLogSeverity.Warning, $"Error: {ex.Message}");
        }

    }

    public static void WriteKeepModdingTitle(BuiltinExecutionContext context)
    {

        Framework.Print($"{context.Name}: {context.Self.Name} ({context.Self.Members.Count}) -> {context.Other.Name} ({context.Other.Members.Count})");
        using GameVariable msg = new("Hello, world!");
        try
        {
            Game.Engine.CallScriptEx("gml_Script_draw_text_scribble", context.Self, context.Other, [10, 10, "[scaleStack,5][c_white][pin_left]Hello world!"]);
        }
        catch (InvalidOperationException ex)
        {
            Framework.PrintEx(AurieLogSeverity.Warning, $"Error: {ex.Message}");
        }
        // foreach (KeyValuePair<string, GameVariable> member in context.Self.Members)
        // {
        //     Framework.Print($"Key: {member.Key}, Value: {member.Value}");
        // }
    }

    public void OnGameEvent(CodeExecutionContext context)
    {
        if (context.Name == _eventName)
        {
            Game.Events.OnGameEvent -= this.OnGameEvent;
            Framework.Print($"Event: {context.Name} ({context.Self.Name} -> {context.Other.Name})");
            foreach (KeyValuePair<string, GameVariable> member in context.Self.Members)
            {
                Framework.Print($"{member.Key} = {member.Value}");
            }
            GameVariable options_button_ref = context.Self.Members["options_button"];
            GameInstance? options_button = null;
            if (options_button_ref.IsAccessible())
            {
                if (options_button_ref.TryGetGameInstance(out options_button))
                {

                    Framework.Print($"{options_button} is {options_button.ID} ({options_button.Name}). Visible? {options_button.Visible}");

                    options_button.Visible = !options_button.Visible;
                    Framework.Print($"{options_button} is {options_button.ID} ({options_button.Name}). Visible? {options_button.Visible}");

                }
                else if (options_button_ref.TryGetInt32(out int options_button_id))
                {

                    options_button = GameInstance.FromInstanceID(options_button_id);
                    Framework.Print($"{options_button_ref.ToGameInstance()} is not an instance");
                }
                else
                {
                    options_button = GameInstance.FromInstanceID(int.Parse(options_button_ref.ToString().Split(" ").Last(), System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            if (options_button != null)
            {
                Framework.Print($"{options_button.Name} ({options_button.ID})");
            }
            else
            {
                Framework.Print($"Could not dereference options_button {options_button_ref}");
            }
            Game.Engine.CallScriptEx("gml_Script_draw_text_scribble", context.Self, context.Other, [10, 10, "[scaleStack,5][c_white][pin_left]Hello world!"]);
        }
        


    }

}
