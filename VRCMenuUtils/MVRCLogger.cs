using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRCMenuUtils
{
    internal static class MVRCLogger
    {
        public static void Log(string text)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[VRCExtended] " + text);
            Console.ForegroundColor = oldColor;
        }
        public static void Log(object obj)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[VRCExtended] " + obj.ToString());
            Console.ForegroundColor = oldColor;
        }

        public static void LogWarning(string text)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[VRCExtended] " + text);
            Console.ForegroundColor = oldColor;
        }
        public static void LogWarning(object obj)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[VRCExtended] " + obj.ToString());
            Console.ForegroundColor = oldColor;
        }

        public static void LogError(string text, Exception exception = null)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[VRCExtended] " + text);
            if (exception != null)
                Console.WriteLine(exception.ToString());
            Console.ForegroundColor = oldColor;
        }
        public static void LogError(object obj, Exception exception = null)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[VRCExtended] " + obj.ToString());
            if (exception != null)
                Console.WriteLine(exception.ToString());
            Console.ForegroundColor = oldColor;
        }
    }
}
