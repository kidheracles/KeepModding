namespace ManagedMod.Core;
using Generic;

public class Playground : IPlayground
{
    private AurieManagedModule? _module;
    private double _totalMs;
    private double _delayMs = 1000; // 1 second
    private static InputManager _inputManager = new();


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
        _inputManager.AddPostInputHook(IModule, "honk", GameInspector.SnapshotAfterInput);
        // Game.Events.AddPostBuiltinNotification(IModule, "keyboard_check_released", GameInspector.SnapshotAfterBuiltin);
        using GameVariable profile = InputManager.GetInputProfile(IModule);
        Framework.Print($"Profile: {profile}");
        using GameVariable config_verbs = Game.Engine.CallScript("gml_Script___input_config_verbs");
        Framework.Print($"Config Verbs: {config_verbs}");

    }

}