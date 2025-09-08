using System.Linq;
using System.Text.Json;

EnsureDataLoaded();

string stringsPath = PromptSaveFile(".txt", "TXT files (*.txt)|*.txt|All files (*.*)|*.*");
if (string.IsNullOrWhiteSpace(stringsPath))
{
    return;
}


using (StreamWriter writer = new StreamWriter(stringsPath))
{
    foreach (var scr in Data.Scripts)
    {
        writer.WriteLine(scr.Name.Content);
    }
}

ScriptMessage($"Successfully exported to\n{stringsPath}");
