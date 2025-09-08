namespace ManagedMod.Core;

internal static class Title
{
    const string _builtinName = "room_goto";
    public static AurieStatus Run(AurieManagedModule Module)
    {
        try
        {
            // GameVariable index = Game.Engine.CallFunction("asset_get_index", ["obj_title_screen_controller"]);
            // Framework.Print($"obj_title_screen_controller: {index.ToInt32()}");
            Game.Events.AddPostBuiltinNotification(Module, _builtinName, WriteKeepModdingTitle);
        }
        catch (InvalidOperationException ex)
        {
            Framework.PrintEx(AurieLogSeverity.Warning, $"Error: {ex.Message}");
        }
        return AurieStatus.Success;

    }

    public static void WriteKeepModdingTitle(BuiltinExecutionContext context)
    {

        Framework.Print($"{context.Name}: {context.Self.Name} ({context.Self.Members.Count}) -> {context.Other.Name} ({context.Other.Members.Count})");
        using GameVariable msg = new("Hello, world!");
        try
        {
        Game.Engine.CallScriptEx("show_message",context.Self,context.Other,["Hello, world!"]);
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

}
