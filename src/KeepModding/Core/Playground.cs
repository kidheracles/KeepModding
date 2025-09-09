namespace ManagedMod.Core;
using Generic;
using UndertaleModLib;
using UndertaleModLib.Models;


public class Playground : IPlayground
{
    private AurieManagedModule? _module;
    private double _totalMs;
    private double _delayMs = 1000; // 1 second
    // private static InputManager _inputManager = new();


    public void Initialize(AurieManagedModule IModule)
    {
        this._module = IModule;
        Game.Events.OnFrame += this.DelayInitialization;
    }

    private void DelayInitialization(int FrameNumber, double DeltaTime)
    {
        this._totalMs += DeltaTime;
        if (this._totalMs > this._delayMs)
        {
            Game.Events.OnFrame -= this.DelayInitialization;
            this.DelayedInitialize(this._module!);
        }
    }

    private void DelayedInitialize(AurieManagedModule IModule)
    {
        Framework.Print($"Delayed commencing initialization until after {this._totalMs}");
        // _inputManager.AddPostInputHook(IModule, "space", GameInspector.SnapshotAfterInput);
        // Game.Events.AddPostBuiltinNotification(IModule, "keyboard_check_released", GameInspector.SnapshotAfterBuiltin);
        // using GameVariable profile = InputManager.GetInputProfile(IModule);
        // Framework.Print($"Profile: {profile}");
        // using GameVariable config_verbs = Game.Engine.CallScript("gml_Script___input_config_verbs");
        // Framework.Print($"Config Verbs: {config_verbs}");
        // using GameVariable scribble_preflush_state = Game.Engine.CallScript("gml_Script___scribble_get_generator_state");
        // Framework.Print($"Scribble Pre-flush: {scribble_preflush_state}");
        // using GameVariable scribble_flush = Game.Engine.CallScript("gml_Script_scribble_flush_everything");
        // Framework.Print($"Scribble Pre-flush: {scribble_flush}");
        // using GameVariable scribble_refresh = Game.Engine.CallScript("gml_Script_scribble_refresh_everything");
        // Framework.Print($"Scribble Refresh: {scribble_refresh}");
        // using GameVariable scribble = Game.Engine.CallScript("gml_Script_draw_text_scribble", [10, 10, "[scaleStack,5][c_white][pin_left]Hello world!"]);
        // Framework.Print($"Scribble: {scribble}");
        // using GameVariable scribble_state = Game.Engine.CallScript("gml_Script___scribble_get_state");
        // Framework.Print($"Scribble State: {scribble_state}");
        // using GameVariable scribble_generator_state = Game.Engine.CallScript("gml_Script___scribble_get_generator_state");
        // Framework.Print($"Scribble Generator: {scribble_generator_state}");

        GameInspector gameInspector = new();
        gameInspector.Initialize(IModule);


        UndertaleData data = UMT.GetData(IModule);
        int title_screen_index = data.GameObjects.ToList().FindIndex(obj => obj.Name.Content == "obj_title_screen_controller");
        if (title_screen_index != -1)
        {
            UndertaleGameObject title_screen = data.GameObjects[title_screen_index];
            Framework.Print($"Title Screen: {title_screen.Name.Content} at index {title_screen_index}");
        }
        else
        {
            Framework.Print($"Title Screen object 'obj_title_screen_controller' not found.");
        }
        Title title = new(IModule);


        // using GameVariable macros_map = Game.Engine.CallScript("gml_Script___scribble_get_macros_map");
        // Framework.Print($"Macros Map: {macros_map}");
        // GameInspector.InspectGlobalObject();

    }


}