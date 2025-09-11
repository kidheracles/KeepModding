using ManagedMod.Generic;
using ManagedMod.Core;

namespace ManagedMod;

internal static class ManagedMod
{
    private static GameInspector _gameInspector = new();

    private static Playground _playground = new();
    private static Title _title = new();


    /// <summary>
    /// The mod entrypoint. Called once when the mod is being loaded.
    /// </summary>
    /// <param name="Module">
    /// A unique opaque structure describing the loaded mod.
    /// </param>
    /// <returns>
    /// A status value representing if the method succeeded or not.<br/>
    /// If a mod fails loading, it is promptly unloaded.
    /// </returns>
    public static AurieStatus InitializeMod(AurieManagedModule Module)
    {
        Framework.Print($"Game Process: {Framework.GetGameProcessPath()}");

        _playground.Initialize(Module);
        _title.Initialize(Module);



        return AurieStatus.Success;
    }

    /// <summary>
    /// The mod unload routine. Called when a mod is unloaded or hot-reloaded.
    /// </summary>
    /// <param name="Module">
    /// A unique opaque structure describing the loaded mod.
    /// Is the same as the one passed to InitializeMod.
    /// </param>
    public static void UnloadMod(AurieManagedModule Module)
    {
        Framework.Print("Goodbye, world!");
        _gameInspector.finish(Module);
        _playground.finish(Module);
        _title.finish(Module);
    }

}
