using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Rpg_Dungeon.Systems
{
    /// <summary>
    /// Represents a rectangular region on the console that can trigger a tooltip on mouse hover.
    /// </summary>
    internal class HoverRegion
    {
        public int Left { get; }
        public int Top { get; }
        public int Width { get; }
        public int Height { get; }
        public string TooltipText { get; }
        public Rarity Rarity { get; }

        public HoverRegion(int left, int top, int width, int height, string tooltipText, Rarity rarity = Rarity.Common)
        {
            Left = left;
            Top = top;
            Width = Math.Max(1, width);
            Height = Math.Max(1, height);
            TooltipText = tooltipText;
            Rarity = rarity;
        }

        public bool Contains(int x, int y)
        {
            return x >= Left && x < Left + Width && y >= Top && y < Top + Height;
        }
    }

    /// <summary>
    /// Provides mouse input support for the Windows console, enabling hover-based tooltips
    /// for items and other UI elements. Saves and restores the screen buffer under the tooltip
    /// so existing content is not destroyed.
    /// </summary>
    internal static class MouseInput
    {
        #region Native Interop

        private const int STD_INPUT_HANDLE = -10;
        private const int STD_OUTPUT_HANDLE = -11;

        private const uint ENABLE_MOUSE_INPUT = 0x0010;
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        private const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        private const uint ENABLE_WINDOW_INPUT = 0x0008;

        private const ushort MOUSE_EVENT = 0x0002;
        private const ushort KEY_EVENT = 0x0001;

        private const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001;
        private const uint RIGHTMOST_BUTTON_PRESSED = 0x0002;
        private const uint MOUSE_MOVED = 0x0001;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadConsoleInput(
            IntPtr hConsoleInput,
            [Out] INPUT_RECORD[] lpBuffer,
            uint nLength,
            out uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetNumberOfConsoleInputEvents(
            IntPtr hConsoleInput,
            out uint lpcNumberOfEvents);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadConsoleOutput(
            IntPtr hConsoleOutput,
            [In, Out] CHAR_INFO[] lpBuffer,
            COORD dwBufferSize,
            COORD dwBufferCoord,
            ref SMALL_RECT lpReadRegion);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteConsoleOutput(
            IntPtr hConsoleOutput,
            [In] CHAR_INFO[] lpBuffer,
            COORD dwBufferSize,
            COORD dwBufferCoord,
            ref SMALL_RECT lpWriteRegion);

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT_RECORD
        {
            [FieldOffset(0)] public ushort EventType;
            [FieldOffset(4)] public MOUSE_EVENT_RECORD MouseEvent;
            [FieldOffset(4)] public KEY_EVENT_RECORD KeyEvent;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public uint dwButtonState;
            public uint dwControlKeyState;
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEY_EVENT_RECORD
        {
            public int bKeyDown;
            public short wRepeatCount;
            public short wVirtualKeyCode;
            public short wVirtualScanCode;
            public char UnicodeChar;
            public uint dwControlKeyState;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct CHAR_INFO
        {
            [FieldOffset(0)] public char UnicodeChar;
            [FieldOffset(2)] public ushort Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        #endregion

        #region State

        private static readonly List<HoverRegion> _regions = new();
        private static IntPtr _inputHandle = IntPtr.Zero;
        private static IntPtr _outputHandle = IntPtr.Zero;
        private static uint _originalMode;
        private static bool _initialized;
        private static bool _supported;

        // Tooltip rendering state
        private static int _tooltipX = -1;
        private static int _tooltipY = -1;
        private static int _tooltipWidth;
        private static int _tooltipHeight;
        private static bool _tooltipVisible;
        private static CHAR_INFO[]? _savedBuffer;
        private static HoverRegion? _activeRegion;

        // Current mouse position
        public static int MouseX { get; private set; } = -1;
        public static int MouseY { get; private set; } = -1;
        public static bool IsLeftButtonDown { get; private set; }
        public static bool IsRightButtonDown { get; private set; }

        /// <summary>
        /// True if mouse input is supported on this platform (Windows only).
        /// </summary>
        public static bool IsSupported => _supported;

        #endregion

        #region Initialization

        /// <summary>
        /// Enables mouse input on the console. Call once at startup.
        /// Returns true if mouse input was successfully enabled.
        /// </summary>
        public static bool Enable()
        {
            if (_initialized) return _supported;
            _initialized = true;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _supported = false;
                return false;
            }

            try
            {
                _inputHandle = GetStdHandle(STD_INPUT_HANDLE);
                _outputHandle = GetStdHandle(STD_OUTPUT_HANDLE);

                if (_inputHandle == IntPtr.Zero || _inputHandle == new IntPtr(-1) ||
                    _outputHandle == IntPtr.Zero || _outputHandle == new IntPtr(-1))
                {
                    _supported = false;
                    return false;
                }

                if (!GetConsoleMode(_inputHandle, out _originalMode))
                {
                    _supported = false;
                    return false;
                }

                uint newMode = (_originalMode | ENABLE_MOUSE_INPUT | ENABLE_EXTENDED_FLAGS | ENABLE_WINDOW_INPUT)
                               & ~ENABLE_QUICK_EDIT_MODE;

                if (!SetConsoleMode(_inputHandle, newMode))
                {
                    _supported = false;
                    return false;
                }

                _supported = true;
                return true;
            }
            catch
            {
                _supported = false;
                return false;
            }
        }

        /// <summary>
        /// Restores the original console mode. Call on shutdown.
        /// </summary>
        public static void Disable()
        {
            if (!_initialized || !_supported) return;

            HideTooltip();

            try
            {
                SetConsoleMode(_inputHandle, _originalMode);
            }
            catch { }

            _initialized = false;
            _supported = false;
            _regions.Clear();
        }

        #endregion

        #region Region Management

        /// <summary>
        /// Registers a hover region on the console that will display a tooltip when the mouse hovers over it.
        /// </summary>
        public static void RegisterRegion(int left, int top, int width, int height, string tooltipText, Rarity rarity = Rarity.Common)
        {
            _regions.Add(new HoverRegion(left, top, width, height, tooltipText, rarity));
        }

        /// <summary>
        /// Registers a hover region for an Item, using its GetTooltip() text and Rarity.
        /// </summary>
        public static void RegisterItemRegion(int left, int top, int width, int height, Item item)
        {
            _regions.Add(new HoverRegion(left, top, width, height, item.GetTooltip(), item.Rarity));
        }

        /// <summary>
        /// Removes all registered hover regions. Call when the screen content changes.
        /// </summary>
        public static void ClearRegions()
        {
            HideTooltip();
            _regions.Clear();
            _activeRegion = null;
        }

        #endregion

        #region Polling

        /// <summary>
        /// Polls for mouse events without blocking. Updates MouseX, MouseY and button state.
        /// Returns the HoverRegion currently under the mouse, or null if none.
        /// </summary>
        public static HoverRegion? Poll()
        {
            if (!_supported) return null;

            try
            {
                if (!GetNumberOfConsoleInputEvents(_inputHandle, out uint eventCount) || eventCount == 0)
                    return FindHoveredRegion();

                var buffer = new INPUT_RECORD[eventCount];
                if (!ReadConsoleInput(_inputHandle, buffer, eventCount, out uint eventsRead))
                    return FindHoveredRegion();

                for (int i = 0; i < eventsRead; i++)
                {
                    if (buffer[i].EventType != MOUSE_EVENT) continue;

                    var me = buffer[i].MouseEvent;
                    MouseX = me.dwMousePosition.X;
                    MouseY = me.dwMousePosition.Y;
                    IsLeftButtonDown = (me.dwButtonState & FROM_LEFT_1ST_BUTTON_PRESSED) != 0;
                    IsRightButtonDown = (me.dwButtonState & RIGHTMOST_BUTTON_PRESSED) != 0;
                }

                return FindHoveredRegion();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Polls for mouse events and automatically shows/hides tooltips for registered regions.
        /// Tracks which region is active to avoid flickering redraws.
        /// </summary>
        public static void PollAndShowTooltips()
        {
            var region = Poll();

            if (region != null)
            {
                if (!ReferenceEquals(region, _activeRegion))
                {
                    HideTooltip();
                    _activeRegion = region;
                    ShowTooltip(region.TooltipText, MouseX, MouseY, region.Rarity);
                }
            }
            else if (_tooltipVisible)
            {
                HideTooltip();
                _activeRegion = null;
            }
        }

        /// <summary>
        /// Blocking wait that shows tooltips on hover and returns the region on left-click.
        /// Returns null if the user presses a key instead.
        /// </summary>
        public static HoverRegion? WaitForClick(CancellationToken cancel = default)
        {
            if (!_supported) return null;

            while (!cancel.IsCancellationRequested)
            {
                if (Console.KeyAvailable) return null;

                PollAndShowTooltips();

                if (_activeRegion != null && IsLeftButtonDown)
                    return _activeRegion;

                Thread.Sleep(16);
            }

            return null;
        }

        private static HoverRegion? FindHoveredRegion()
        {
            if (MouseX < 0 || MouseY < 0) return null;

            for (int i = _regions.Count - 1; i >= 0; i--)
            {
                if (_regions[i].Contains(MouseX, MouseY))
                    return _regions[i];
            }

            return null;
        }

        #endregion

        #region Tooltip Rendering

        /// <summary>
        /// Gets the console color for a rarity tier.
        /// </summary>
        private static ConsoleColor GetRarityColor(Rarity rarity) => rarity switch
        {
            Rarity.Uncommon => ConsoleColor.Green,
            Rarity.Rare => ConsoleColor.Cyan,
            Rarity.Epic => ConsoleColor.Magenta,
            Rarity.Legendary => ConsoleColor.Yellow,
            _ => ConsoleColor.White
        };

        /// <summary>
        /// Computes the tooltip position and size, clamped to the console window.
        /// </summary>
        private static (int drawX, int drawY, int boxWidth, int boxHeight, string[] lines) ComputeTooltipLayout(string text, int nearX, int nearY)
        {
            var rawLines = text.Split('\n');
            var lines = new string[rawLines.Length];
            int maxLineLen = 0;
            for (int i = 0; i < rawLines.Length; i++)
            {
                lines[i] = rawLines[i].TrimEnd('\r');
                if (lines[i].Length > maxLineLen) maxLineLen = lines[i].Length;
            }

            int boxWidth = maxLineLen + 4;   // │ + space + content + space + │
            int boxHeight = lines.Length + 2; // top border + content + bottom border

            int consoleWidth, consoleHeight;
            try
            {
                consoleWidth = Console.WindowWidth;
                consoleHeight = Console.WindowHeight;
            }
            catch
            {
                consoleWidth = 120;
                consoleHeight = 30;
            }

            // Clamp box size to console bounds
            if (boxWidth > consoleWidth) boxWidth = consoleWidth;
            if (boxHeight > consoleHeight) boxHeight = consoleHeight;

            int drawX = nearX + 2;
            int drawY = nearY + 1;

            // Flip left if it would overflow right
            if (drawX + boxWidth > consoleWidth)
                drawX = Math.Max(0, nearX - boxWidth - 1);

            // Flip up if it would overflow bottom
            if (drawY + boxHeight > consoleHeight)
                drawY = Math.Max(0, nearY - boxHeight);

            // Final clamp
            if (drawX < 0) drawX = 0;
            if (drawY < 0) drawY = 0;

            return (drawX, drawY, boxWidth, boxHeight, lines);
        }

        /// <summary>
        /// Saves the console buffer region that the tooltip will cover.
        /// </summary>
        private static CHAR_INFO[]? SaveBufferRegion(int x, int y, int width, int height)
        {
            try
            {
                var bufSize = new COORD { X = (short)width, Y = (short)height };
                var bufCoord = new COORD { X = 0, Y = 0 };
                var readRect = new SMALL_RECT
                {
                    Left = (short)x,
                    Top = (short)y,
                    Right = (short)(x + width - 1),
                    Bottom = (short)(y + height - 1)
                };

                var buffer = new CHAR_INFO[width * height];
                if (ReadConsoleOutput(_outputHandle, buffer, bufSize, bufCoord, ref readRect))
                    return buffer;
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Restores a previously saved console buffer region.
        /// </summary>
        private static void RestoreBufferRegion(CHAR_INFO[] buffer, int x, int y, int width, int height)
        {
            try
            {
                var bufSize = new COORD { X = (short)width, Y = (short)height };
                var bufCoord = new COORD { X = 0, Y = 0 };
                var writeRect = new SMALL_RECT
                {
                    Left = (short)x,
                    Top = (short)y,
                    Right = (short)(x + width - 1),
                    Bottom = (short)(y + height - 1)
                };

                WriteConsoleOutput(_outputHandle, buffer, bufSize, bufCoord, ref writeRect);
            }
            catch { }
        }

        /// <summary>
        /// Draws a tooltip box near the given console coordinates.
        /// The tooltip sizes itself to fit the content and preserves the screen content underneath.
        /// </summary>
        public static void ShowTooltip(string text, int nearX, int nearY, Rarity rarity = Rarity.Common)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            if (_tooltipVisible) HideTooltip();

            var (drawX, drawY, boxWidth, boxHeight, lines) = ComputeTooltipLayout(text, nearX, nearY);
            int contentWidth = boxWidth - 4; // inner content width

            // Save the region we're about to overwrite
            _savedBuffer = SaveBufferRegion(drawX, drawY, boxWidth, boxHeight);

            var savedLeft = Console.CursorLeft;
            var savedTop = Console.CursorTop;
            var savedFg = Console.ForegroundColor;
            var savedBg = Console.BackgroundColor;
            bool savedVisible = true;
            try { savedVisible = Console.CursorVisible; } catch { }

            try
            {
                Console.CursorVisible = false;

                ConsoleColor borderColor = GetRarityColor(rarity);
                ConsoleColor bodyBg = ConsoleColor.DarkGray;
                ConsoleColor bodyFg = ConsoleColor.White;

                // Top border
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = borderColor;
                Console.SetCursorPosition(drawX, drawY);
                Console.Write("┌" + new string('─', boxWidth - 2) + "┐");

                // Content lines
                for (int i = 0; i < lines.Length && i < boxHeight - 2; i++)
                {
                    Console.SetCursorPosition(drawX, drawY + 1 + i);

                    // Border chars use rarity color on black
                    Console.ForegroundColor = borderColor;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write("│");

                    // Inner content: first line (name) uses rarity color, rest white on dark gray
                    if (i == 0)
                    {
                        Console.ForegroundColor = borderColor;
                        Console.BackgroundColor = bodyBg;
                    }
                    else
                    {
                        Console.ForegroundColor = bodyFg;
                        Console.BackgroundColor = bodyBg;
                    }

                    string padded = lines[i].Length > contentWidth
                        ? lines[i][..contentWidth]
                        : lines[i].PadRight(contentWidth);
                    Console.Write(" " + padded + " ");

                    Console.ForegroundColor = borderColor;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write("│");
                }

                // Bottom border
                Console.ForegroundColor = borderColor;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(drawX, drawY + boxHeight - 1);
                Console.Write("└" + new string('─', boxWidth - 2) + "┘");

                _tooltipX = drawX;
                _tooltipY = drawY;
                _tooltipWidth = boxWidth;
                _tooltipHeight = boxHeight;
                _tooltipVisible = true;
            }
            catch { }
            finally
            {
                Console.ForegroundColor = savedFg;
                Console.BackgroundColor = savedBg;
                Console.SetCursorPosition(savedLeft, savedTop);
                try { Console.CursorVisible = savedVisible; } catch { }
            }
        }

        /// <summary>
        /// Hides the tooltip and restores the original screen content underneath.
        /// </summary>
        public static void HideTooltip()
        {
            if (!_tooltipVisible) return;

            if (_savedBuffer != null)
            {
                RestoreBufferRegion(_savedBuffer, _tooltipX, _tooltipY, _tooltipWidth, _tooltipHeight);
                _savedBuffer = null;
            }
            else
            {
                // Fallback: blank the area if we couldn't save the buffer
                var savedLeft = Console.CursorLeft;
                var savedTop = Console.CursorTop;
                var savedFg = Console.ForegroundColor;
                var savedBg = Console.BackgroundColor;

                try
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.Black;
                    string blank = new string(' ', _tooltipWidth);
                    for (int row = 0; row < _tooltipHeight; row++)
                    {
                        int y = _tooltipY + row;
                        if (y < 0 || y >= Console.WindowHeight) continue;
                        Console.SetCursorPosition(_tooltipX, y);
                        Console.Write(blank);
                    }
                }
                catch { }
                finally
                {
                    Console.ForegroundColor = savedFg;
                    Console.BackgroundColor = savedBg;
                    Console.SetCursorPosition(savedLeft, savedTop);
                }
            }

            _tooltipVisible = false;
        }

        #endregion
    }
}
