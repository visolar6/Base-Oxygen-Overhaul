using System;
using System.IO;
using UnityEngine;

namespace BaseOxygenOverhaul.Utilities
{
    class ResourceHandler
    {
        public static Sprite LoadSpriteFromFile(string filePath)
        {
            try
            {
                // Resolve to absolute path based on assembly location if not already absolute
                if (!Path.IsPathRooted(filePath))
                {
                    string baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
                    filePath = Path.Combine(baseDir, filePath);
                }
                if (!File.Exists(filePath))
                {
                    Plugin.Log.LogWarning($"Sprite file not found: {filePath}");
                    return null;
                }
                byte[] fileData = File.ReadAllBytes(filePath);
                Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                if (!tex.LoadImage(fileData))
                {
                    Plugin.Log.LogError($"Failed to load image data from: {filePath}");
                    return null;
                }
                tex.filterMode = FilterMode.Bilinear;
                tex.wrapMode = TextureWrapMode.Clamp;
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Exception loading sprite from {filePath}: {e.Message}\n{e.StackTrace}");
                return null;
            }
        }
    }
}