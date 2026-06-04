using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
namespace Editor.Application
{
    public static class AudioUtilityOnlyEditor
    {
        public static void PlayClip(AudioClip audioClip, int index = 0)
        {
            if (audioClip == null)
            {
                return;
            }
            
            var audioUtil = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AudioUtil");
            var playClipMethod = audioUtil.GetMethod(
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] {typeof(AudioClip), typeof(int), typeof(bool)},
                null
            );
        
            playClipMethod.Invoke(null, new object[] {audioClip, 0, false});
        }
    }
}
#endif
