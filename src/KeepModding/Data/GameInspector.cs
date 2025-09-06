using System.Runtime.InteropServices;

namespace KeepModding.Data;

/// <summary>
/// An implementation for inspecting various aspects of the game state.
/// </summary>
public class GameInspector : IGameInspector
{
    public void InspectGlobalObject()
    {

        GameObject globalObject = Game.Engine.GetGlobalObject();
        Framework.Print($"Does Global Object exist? {globalObject.IsInstance()} How many members? {globalObject.Members.Count}");

        foreach (KeyValuePair<string, GameVariable> member in globalObject.Members)
        {
            try
            {
                Framework.Print($"Key: {member.Key}, Value: {member.Value}");
            }
            catch (SEHException sehex)
            {
                Framework.PrintEx(AurieLogSeverity.Warning, $"Key: {member.Key}, Error: {sehex.Message}");
            }
        }

    }
}