using KeepModding.Data;

internal static class TheMod
{
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
        Framework.Print("Hello, world!");
        Framework.Print($"Game Directory: {Framework.GetGameDirectory()}");
        Framework.Print($"Game Process: {Framework.GetGameProcessPath()}");

        var inspector = new GameInspector();
        Task.Delay(2000).ContinueWith(_ =>
        {
            Framework.Print("This message appears after 2 seconds!");
            inspector.InspectGlobalObject();
        }, TaskScheduler.Default);

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
    }

}
