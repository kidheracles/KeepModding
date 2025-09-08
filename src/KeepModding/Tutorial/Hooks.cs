
namespace ManagedMod.Tutorial;

static class Hooks
{

    public static void DoTutorial(AurieManagedModule Module)
    {
        Game.Events.OnFrame += OnFrame;
        Game.Events.AddPostScriptNotification(Module, "gml_GlobalScript_input_any_pressed", TestHook);
    }

    public static void OnFrame(int FrameNumber, double DeltaTime)
    {
        if (DeltaTime < 10)
        {
            Framework.Print($"Frame: {FrameNumber}, Delta Time: {DeltaTime}");
        }
    }

    public static void TestHook(ScriptExecutionContext context)
    {
        if (!context.Executed)
        {
            Framework.Print("TestHook not yet executed");
        }


        Framework.Print("TestHook is executing");

        if (context.Executed)
        {
            Framework.Print("TestHook executed");
        }
    }
}