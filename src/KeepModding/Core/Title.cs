using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace ManagedMod.Core;

internal sealed class Title : IDisposable
{
    const string _builtinName = "draw_text";
    const string _objectName = "struct obj_title_screen_controller";
    const string _startEvent = "gml_Object_obj_title_screen_controller_Create";
    const string _endEvent = "gml_Object_obj_title_screen_controller_Alarm";
    private AurieManagedModule? _module;
    private const string _titleString = "& MODDING";
#pragma warning disable CA2213
    internal GameVariable? _titleVar;
#pragma warning restore CA2213
    internal int? _titleID;
    const string _titleFont = "fnt_music_unlock_cd";
    const int _titleScale = 1;
    const int _titleAngle = -10;

    const int _titleX = 256;
    const int _titleY = 100;
    public void Initialize(AurieManagedModule Module)
    {
        this._module = Module;
        try
        {
            Game.Events.OnGameEvent += this.OnGameEvent;
        }
        catch (InvalidOperationException ex)
        {
            Framework.PrintEx(AurieLogSeverity.Warning, $"Error: {ex.Message}");
        }

    }

    private void CreateTitle()
    {
        
        try
        {
            GameVariable font = Game.Engine.CallFunction("asset_get_index", [_titleFont]);

            Game.Engine.CallFunction("draw_set_font", [font]);
            int w = Game.Engine.CallFunction("string_width", [_titleString]).ToInt32() * _titleScale;
            int h = Game.Engine.CallFunction("string_height", [_titleString]).ToInt32() * _titleScale;
            this._titleVar = Game.Engine.CallFunction("surface_create", [w+10, h]);
            Framework.PrintEx(AurieLogSeverity.Trace, $"Created title surface: {this._titleVar} w: {w} h: {h}");

            this._titleID = int.Parse(this._titleVar.ToString().Split(" ").Last(), System.Globalization.CultureInfo.InvariantCulture);
            Game.Engine.CallFunction("surface_set_target", [this._titleID]);
            Game.Engine.CallFunction("draw_rectangle", [1, 1, w+8, h-2, true]);
            Game.Engine.CallFunction("draw_set_halign", [0]);
            Game.Engine.CallFunction("draw_set_valign", [0]);
            Game.Engine.CallFunction("draw_text", [5, 0, _titleString]);
            Game.Engine.CallFunction("surface_reset_target");

        }
        catch (InvalidOperationException ex)
        {
            Framework.PrintEx(AurieLogSeverity.Warning, $"Error: {ex.Message}");
        }
    }

    public void WriteKeepModdingTitle(BuiltinExecutionContext context)
    {
        
        if (context.Self.Name == _objectName && context.Arguments[2].TryGetString(out string text) && text == "PRESS ANY KEY")
        {
            try
            {
                if (this._titleID == null || !Game.Engine.CallFunction("surface_exists", [this._titleID]))
                {
                    this.CreateTitle();
                }
                Game.Engine.CallFunction("draw_surface_ext", [this._titleID, _titleX, _titleY, 1, 1, _titleAngle, 255, 1]);
            }
            catch (System.Runtime.InteropServices.SEHException ex)
            {
                Framework.PrintEx(AurieLogSeverity.Warning, $"Error: {ex.Message}");
            }
        }
    }

    private bool _disposed;

    public void Dispose()
    {
        // Dispose of resources held by this instance.
        if (this._titleVar != null)
        {
            Game.Engine.CallFunction("surface_free", [this._titleVar]);
            this._titleVar.Dispose();
        }
        Framework.PrintEx(AurieLogSeverity.Trace, "Title Screen controller gone; disposing of hook..");
        if (!this._disposed)
        {
            Game.Events.RemovePostBuiltinNotification(this._module, _builtinName, this.WriteKeepModdingTitle);
            Game.Events.OnGameEvent -= this.OnGameEvent;
            this._disposed = true;
            // Suppress finalization of this disposed instance.
            GC.SuppressFinalize(this);
        }

    }

    public void finish(AurieManagedModule IModule)
    {
        this.Dispose();
    }

    public void OnGameEvent(CodeExecutionContext context)
    {
        if (context.Name.Contains(_startEvent, System.StringComparison.Ordinal))
        {
            Framework.PrintEx(AurieLogSeverity.Trace, "Hooking into draw_text functions to draw title..");
            Game.Events.AddPostBuiltinNotification(this._module, _builtinName, this.WriteKeepModdingTitle);
        }
        else if (context.Name.Contains(_endEvent, System.StringComparison.Ordinal))
        {
            this.Dispose();
        }
    }
}
