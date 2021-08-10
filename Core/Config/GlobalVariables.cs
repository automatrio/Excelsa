using System.Collections.Generic;
using System.Text;

namespace Excelsa.Core
{
    public static class GlobalVariables
    {
        public static string Url { get; set; }
        public static List<string> FallbackUrls { get; set; }
        public static bool EmbeddedLogin { get; set; }
        public static string Login { get; set; }
        public static string Password { get; set; }
        public static bool TakeScreenshot { get; set; }
        
        public static string ScreenshotFolderPath { get; set; }
        public static string LogFolderPath { get; set; }
        public static float WaitTimePadding { get; set; } = 1.00f;

        public static StringBuilder MainLog { get; set; } = new StringBuilder();
    }
}