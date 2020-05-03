using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

[ScriptedImporter(1, "psc")]
public class AuraSceneScriptImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {/*}
        catch(System.InvalidProgramException e)
        {
            Debug.LogException(e);
            ctx.LogImportError(e.Message);
        }
        catch(Aura.Script.TokenizerException e)
        {
            Debug.LogException(e);
            ctx.LogImportError(e.Message);
        }
        catch(System.IO.InvalidDataException e)
        {
            Debug.LogException(e);
            ctx.LogImportError(e.Message);
        }*/
            var files = ReadFiles(ctx);
            var sceneName = Path.GetFileNameWithoutExtension(ctx.assetPath);

            if (!files.TryGetValue(sceneName + ".scc", out var sceneScriptText))
                throw new InvalidDataException("Aura script pack does not contain a scene script");
            var sceneScriptTokenizer = new Aura.Script.Tokenizer(sceneName + ".scc", sceneScriptText);
            foreach (var t in sceneScriptTokenizer)
                Debug.Log($"{t.Type} \"{t.Value}\"");
        
    }

    private Dictionary<string, string> ReadFiles(AssetImportContext ctx)
    {
        var files = new Dictionary<string, string>();
        var reader = new AuraPackFileReader(new FileStream(ctx.assetPath, FileMode.Open, FileAccess.Read));
        var fileNames = reader.ReadFileList();
        foreach (var fileName in fileNames)
        {
            var fileContent = new StringWriter();
            uint lineCount = reader.ReadU32();
            for (uint i = 0; i < lineCount; i++)
                fileContent.WriteLine(reader.ReadString((int)reader.ReadU32()));
            files[fileName] = fileContent.ToString();
        }
        return files;
    }
}
