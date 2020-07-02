using System;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitRaids
{
    public static class RaidLogger
    {
        public static void Debug(string message)
        {
            if (Prefs.DevMode)
            {
            #if DEBUG
                Log("DEBUG", message, $"#{ColorUtility.ToHtmlStringRGB(ColorLibrary.HotPink)}");
            #endif
            }
        }

        public static void Error(string message, Exception exception)
        {
            Verse.Log.Error($"{message}: {exception.GetType().Name}({exception.Message})\n{exception.StackTrace}");
        }

        public static void Info(string message)
        {
            Log("INFO", message);
        }

        private static void Log(string level, string message, string color = null)
        {
            Verse.Log.Message(
                color.NullOrEmpty()
                    ? $"{level.ToUpper()} ToolkitRaids :: {message}"
                    : $"<color=\"{color}\">{level.ToUpper()} ToolkitRaids :: {message}</color>"
            );
        }

        public static void Warn(string message)
        {
            Log("WARN", message, "#ff8080");
        }
    }
}
