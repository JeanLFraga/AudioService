using System;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace JeanLF.AudioService.Editor
{
    public class AudioServicePreBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            AudioServiceSettings settings = AudioServiceSettingsEditor.GetOrCreateSettings();
            Debug.Log($"Audio Service - Pre-build step\nGenerating code for {settings.Database}");

            if (settings.Database != null)
            {
                AudioServiceSettingsEditor.GenerateCode(settings.Database);
            }
        }
    }
}
