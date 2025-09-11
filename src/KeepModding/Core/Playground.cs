namespace ManagedMod.Core;

using Generic;

public class Playground : IPlayground, IDisposable
{
    private AurieManagedModule? _module;
    private double _totalMs;
    private double _delayMs = 1000; // 1 second
    // private static InputManager _inputManager = new();
    private CommandInspector? _inspector;
    private bool _disposedValue;


    public void Initialize(AurieManagedModule IModule)
    {
        this._module = IModule;
        Game.Events.OnFrame += this.DelayInitialization;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                this._inspector?.Dispose();
            }
            this._disposedValue = true;
        }
    }
    public void finish(AurieManagedModule IModule)
    {
        this.Dispose();
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
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

        this._inspector = new() { _module = IModule };

    }
}