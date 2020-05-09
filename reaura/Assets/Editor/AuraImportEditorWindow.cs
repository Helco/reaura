using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;

namespace Aura
{
    public partial class AuraImportEditorWindow : EditorWindow
    {
        [MenuItem("Aura/Importer")]
        static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<AuraImportEditorWindow>();
            window.titleContent.text = "Aura Importer";
            window.ShowTab();
        }

        private struct Progress
        {
            public string text;
            public float percent;

            public Progress(string t, float p)
            {
                text = t;
                percent = p;
            }
        }

        private static readonly string PREFKEY_AURA_PATH = "AuraInstallationPath";
        private static readonly string PREFKEY_FFMPEG_PATH = "AuraFFMpegPath";

        private string auraPath = "";
        private string ffmpegPath;
        private EditorCoroutine importCoroutine;
        private List<Progress> progresses = new List<Progress>();

        private void OnEnable()
        {
            auraPath = EditorPrefs.GetString(PREFKEY_AURA_PATH);
            ffmpegPath = EditorPrefs.GetString(PREFKEY_FFMPEG_PATH,
                Path.Combine(Directory.GetCurrentDirectory(), "Tools", "ffmpeg.exe"));
        }

        private void OnGUI()
        {
            bool prevEnabled;
            BeginHorizontalCentered();
            GUILayout.Label("Aura Importer", EditorStyles.largeLabel);
            EndHorizontalCentered();
            EditorGUILayout.Space();

            prevEnabled = GUI.enabled;
            GUI.enabled = importCoroutine == null;
            auraPath = PathField(auraPath, "Aura installation path", "Aura1.exe", PREFKEY_AURA_PATH);
            ffmpegPath = PathField(ffmpegPath, "FFMPEG path", "ffmpeg.exe", PREFKEY_FFMPEG_PATH);
            EditorGUILayout.Space();
            GUI.enabled = prevEnabled;

            BeginHorizontalCentered();
            prevEnabled = GUI.enabled;
            GUI.enabled = File.Exists(auraPath) && File.Exists(ffmpegPath);
            if (importCoroutine == null && GUILayout.Button("Import"))
            {
                importCoroutine = this.StartCoroutine(ImportGame());
            }
            if (importCoroutine != null && GUILayout.Button("Cancel Import"))
            {
                EditorCoroutineUtility.StopCoroutine(importCoroutine);
                importCoroutine = null;
                Debug.LogWarning("Importing was cancelled");
            }
            GUI.enabled = prevEnabled;
            EndHorizontalCentered();

            if (importCoroutine == null)
                return;
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 2.0f);
            BeginHorizontalCentered();
            GUILayout.Label("Progress", EditorStyles.boldLabel);
            EndHorizontalCentered();
            EditorGUILayout.Space();
            foreach (var progress in progresses)
            {
                var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 1.5f);
                EditorGUI.ProgressBar(rect, progress.percent, progress.text);
            }
        }

        private static void BeginHorizontalCentered()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
        }

        private static void EndHorizontalCentered()
        {
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static void ReadOnlyTextField(string content, params GUILayoutOption[] options)
        {
            bool wasReadOnly = GUI.enabled;
            GUI.enabled = false;
            GUILayout.TextField(content, options);
            GUI.enabled = wasReadOnly;
        }

        private static void AutoSizeLabel(string contentText)
        {
            var content = new GUIContent(contentText);
            EditorStyles.largeLabel.CalcMinMaxWidth(content, out var _, out float width);
            GUILayout.Label(content, GUILayout.Width(width));
        }

        private static string PathField(string path, string name, string filter, string prefKey)
        {
            
            GUILayout.BeginHorizontal();
            AutoSizeLabel(name);
            ReadOnlyTextField(path);
            if (GUILayout.Button("…", GUILayout.Width(24.0f)))
            {
                path = EditorUtility.OpenFilePanelWithFilters("Open " + name, "",
                    new string[] { filter, Path.GetExtension(filter), "All Files", "*" });
                EditorPrefs.SetString(prefKey, path);
            }
            GUILayout.EndHorizontal();
            return path;
        }
    }
}
