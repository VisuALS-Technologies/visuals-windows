using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;


namespace VisuALS_WPF_App
{
    public static class InputSimulator
    {
        public enum MouseButtons { LEFT, RIGHT, MIDDLE, PAGE_UP, PAGE_DOWN }

        static InputSimulator()
        {

        }

        public static void SetFocus(DependencyObject dependency, UIElement focusElement)
        {
            Keyboard.Focus(focusElement);
            FocusManager.SetFocusedElement(dependency, focusElement);
        }


        public static uint IndexOfEnum<T>(T enumVal)
        {
            List<string> names = Enum.GetNames(typeof(T)).ToList();
            return (uint)names.IndexOf(Enum.GetName(typeof(T), enumVal));
        }

        //public static implicit operator char(UnicodeControlChars c) => GetUnicodeControlChar(c);

        public static void UnicodeKeyboardSendString(string s)
        {
            foreach (char c in s)
            {
                UnicodeKeyboardKeyClick(c);
            }
        }

        public static void UnicodeKeyboardKeyClickWithModifier(char mod, char c)
        {
            UnicodeKeyboardKeyDown(mod);
            UnicodeKeyboardKeyClick(c);
            UnicodeKeyboardKeyUp(mod);
        }

        public static void UnicodeKeyboardKeyClick(char c)
        {
            UnicodeKeyboardKeyDown(c);
            UnicodeKeyboardKeyUp(c);
        }

        public static void UnicodeKeyboardKeyUp(char c)
        {
            Input input = new Input();
            input.type = 1;

            input.keyboardInput.scanCode = c;
            input.keyboardInput.flags = InputFlags.KEYBOARD_FLAG_KEY_UP | InputFlags.KEYBOARD_FLAG_UNICODE;

            uint result = SendInput(1, new Input[] { input }, Marshal.SizeOf(input));
            if (result != 1)
            {
                Debug.WriteLine("ERROR: Could not send input");
            }
        }

        public static void UnicodeKeyboardKeyDown(char c)
        {
            Input input = new Input();
            input.type = 1;

            byte[] b = Encoding.Default.GetBytes(c.ToString());

            input.keyboardInput.scanCode = c;
            input.keyboardInput.flags = InputFlags.KEYBOARD_FLAG_UNICODE; //not including the key up flag implies key down

            uint result = SendInput(1, new Input[] { input }, Marshal.SizeOf(input));
            if (result != 1)
            {
                Debug.WriteLine("ERROR: Could not send input");
            }
        }

        public static void VirtualKeyboardKeyClickWithModifier(ushort virtualKeyCode_Modif, ushort virtualKeyCode_Key)
        {
            VirtualKeyboardKeyDown(virtualKeyCode_Modif);
            VirtualKeyboardKeyClick(virtualKeyCode_Key);
            VirtualKeyboardKeyUp(virtualKeyCode_Modif);
        }

        public static void VirtualKeyboardKeyClick(ushort virtualKeyCode)
        {
            VirtualKeyboardKeyDown(virtualKeyCode);
            VirtualKeyboardKeyUp(virtualKeyCode);
        }

        public static void VirtualKeyboardKeyUp(ushort virtualKeyCode)
        {
            Input input = new Input();
            input.type = 1;

            input.keyboardInput.keyCode = virtualKeyCode;
            input.keyboardInput.flags = InputFlags.KEYBOARD_FLAG_KEY_UP;

            uint result = SendInput(1, new Input[] { input }, Marshal.SizeOf(input));
            if (result != 1)
            {
                Debug.WriteLine("ERROR: Could not send input");
            }
        }

        public static void VirtualKeyboardKeyDown(ushort virtualKeyCode)
        {
            Input input = new Input();
            input.type = 1;

            input.keyboardInput.keyCode = virtualKeyCode;
            input.keyboardInput.flags = 0; //not including the key up flag implies key down

            uint result = SendInput(1, new Input[] { input }, Marshal.SizeOf(input));
            if (result != 1)
            {
                Debug.WriteLine("ERROR: Could not send input");
            }
        }

        public static void MouseVerticalScroll(int scrollAmount)
        {
            Input input = new Input();
            input.type = 0;

            input.mouseInput.flags = InputFlags.MOUSE_FLAG_VERT_WHEEL;
            input.mouseInput.data = (uint)scrollAmount;

            uint result = SendInput(1, new Input[] { input }, Marshal.SizeOf(input));
            if (result != 1)
            {
                Debug.WriteLine("ERROR: Could not send input");
            }
        }

        public static void MouseHorizontalScroll(int scrollAmount)
        {
            Input input = new Input();
            input.type = 0;

            input.mouseInput.flags = InputFlags.MOUSE_FLAG_HOR_WHEEL;
            input.mouseInput.data = (uint)scrollAmount;

            uint result = SendInput(1, new Input[] { input }, Marshal.SizeOf(input));
            if (result != 1)
            {
                Debug.WriteLine("ERROR: Could not send input");
            }
        }

        public static void MouseButtonClick(MouseButtons button)
        {
            MouseButtonDown(button);
            MouseButtonUp(button);
        }

        public static void MouseButtonUp(MouseButtons button)
        {
            Input input = new Input();
            input.type = 0;

            //Sets up the data for which button you want to press
            //See MouseInput comments for details
            switch (button)
            {
                case MouseButtons.LEFT:
                    input.mouseInput.flags = InputFlags.MOUSE_FLAG_LEFT_BTN_UP;
                    break;
                case MouseButtons.RIGHT:
                    input.mouseInput.flags = InputFlags.MOUSE_FLAG_RIGHT_BTN_UP;
                    break;
                case MouseButtons.MIDDLE:
                    input.mouseInput.flags = InputFlags.MOUSE_FLAG_MIDDLE_BTN_UP;
                    break;
                case MouseButtons.PAGE_DOWN:
                    input.mouseInput.flags = InputFlags.MOUSE_FLAG_PAGE_NAV_BTN_UP; input.mouseInput.data = 1;
                    break;
                case MouseButtons.PAGE_UP:
                    input.mouseInput.flags = InputFlags.MOUSE_FLAG_PAGE_NAV_BTN_UP; input.mouseInput.data = 0;
                    break;
            }

            uint result = SendInput(1, new Input[] { input }, Marshal.SizeOf(input));
            if (result != 1)
            {
                Debug.WriteLine("ERROR: Could not send input");
            }
        }

        public static void MouseButtonDown(MouseButtons button)
        {
            Input input = new Input();
            input.type = 0;

            //Sets up the data for which button you want to press
            //See MouseInput comments for details
            switch (button)
            {
                case MouseButtons.LEFT:
                    input.mouseInput.flags = InputFlags.MOUSE_FLAG_LEFT_BTN_DOWN;
                    break;
                case MouseButtons.RIGHT:
                    input.mouseInput.flags = InputFlags.MOUSE_FLAG_RIGHT_BTN_DOWN;
                    break;
                case MouseButtons.MIDDLE:
                    input.mouseInput.flags = InputFlags.MOUSE_FLAG_MIDDLE_BTN_DOWN;
                    break;
                case MouseButtons.PAGE_DOWN:
                    input.mouseInput.flags = InputFlags.MOUSE_FLAG_PAGE_NAV_BTN_DOWN; input.mouseInput.data = 1;
                    break;
                case MouseButtons.PAGE_UP:
                    input.mouseInput.flags = InputFlags.MOUSE_FLAG_PAGE_NAV_BTN_DOWN; input.mouseInput.data = 0;
                    break;
            }

            uint result = SendInput(1, new Input[] { input }, Marshal.SizeOf(input));
            if (result != 1)
            {
                Debug.WriteLine("ERROR: Could not send input");
            }
        }

        public static void SetCursor(int X, int Y)
        {
            SetCursorPos(X, Y);
        }

        public static void MoveMouse(uint X, uint Y, bool relativeToLastPos = false)
        {
            Input input = new Input();
            input.type = 0; //type is mouse input

            //setup data to send for moving the mouse
            input.mouseInput.xPos = X;
            input.mouseInput.yPos = Y;
            input.mouseInput.flags = (uint)(InputFlags.MOUSE_FLAG_MOVE | (relativeToLastPos ? 0 : InputFlags.MOUSE_FLAG_ABSOLUTE_POS));

            //send the input
            uint result = SendInput(1, new Input[] { input }, Marshal.SizeOf(input));
            if (result != 1)
            {
                Debug.WriteLine("ERROR: Could not send input");
            }
        }



        //public static void SendKeyboardInput()
        //{

        //}

        //public static void SendHardwareInput()
        //{

        //}

        //============ Below here is weird stuff for simulating keyboard inputs ============
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint cInputs, Input[] inputs, int size);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [StructLayout(LayoutKind.Explicit)]
        public struct Input
        {
            [FieldOffset(0)]
            public int type; //0 = mouse, 1 = keyboard, 2 = hardware
            [FieldOffset(4)]
            public MouseInput mouseInput;
            [FieldOffset(4)]
            public KeyboardInput keyboardInput;
            [FieldOffset(4)]
            public HardwareInput hardwareInput;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardInput
        {
            public ushort keyCode; //virtual key code, must be 0 if using Unicode flag
            public ushort scanCode; //hardware scan code, must be the Unicode val if using Unicode flag
            public uint flags; //0x01 = extended key, 0x02 = key up, 0x04 = use scan code, 0x08 = unicode
            public uint timestamp; //if 0 the system will provide a timestamp
            public UIntPtr extraInfo; //additional information
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public uint xPos; //X position of the cursor
            public uint yPos; //Y position of the cursor
            public uint data;  //for Wheel or Horizontal wheel this indicates movement (+ forward/right, - backward/left)
                               //for page nav button flags this indicates which Page button (0x0001 for btn 1, 0x0002 for btn 2)
                               //for all other flags this should be 0
            public uint flags; //0x8000 = AbsolutePos, 0x1000 = hor. Wheel, 0x0001 = Move, 0x2000 = Move no Coalesce, 
                               //0x0002 = Left btn down, 0x0004 = Left btn up, 0x0008 = Right btn down, 0x0010 = Right btn up, 
                               //0x0020 = Middle btn down, 0x0040 = Middle btn up, 0x4000 = Virtual desk, 0x0800 = Wheel, 
                               //0x0080 = page btn down, 0x0100 = page btn up
            public uint timestamp; //if 0 the system will provide a timestamp
            public UIntPtr extraInfo; //additional information
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HardwareInput
        {
            public uint message; //message generated by input hardware
            public ushort lowWord; //low order word parameter for message
            public ushort highWord; //high order word parameter for message
        }

        public static class InputFlags
        {
            public const uint MOUSE_FLAG_ABSOLUTE_POS = 0x8000;
            public const uint MOUSE_FLAG_HOR_WHEEL = 0x1000;
            public const uint MOUSE_FLAG_MOVE = 0x0001;
            public const uint MOUSE_FLAG_MOVE_NO_COALESCE = 0x2000;
            public const uint MOUSE_FLAG_LEFT_BTN_DOWN = 0x0002;
            public const uint MOUSE_FLAG_LEFT_BTN_UP = 0x0004;
            public const uint MOUSE_FLAG_RIGHT_BTN_DOWN = 0x0008;
            public const uint MOUSE_FLAG_RIGHT_BTN_UP = 0x0010;
            public const uint MOUSE_FLAG_MIDDLE_BTN_DOWN = 0x0020;
            public const uint MOUSE_FLAG_MIDDLE_BTN_UP = 0x0040;
            public const uint MOUSE_FLAG_VIRTUAL_DESKTOP = 0x4000;
            public const uint MOUSE_FLAG_VERT_WHEEL = 0x0800;
            public const uint MOUSE_FLAG_PAGE_NAV_BTN_DOWN = 0x0080;
            public const uint MOUSE_FLAG_PAGE_NAV_BTN_UP = 0x0100;

            public const ushort KEYBOARD_FLAG_EXTENDED_KEY = 0x0001;
            public const ushort KEYBOARD_FLAG_KEY_UP = 0x0002;
            public const ushort KEYBOARD_FLAG_USE_SCAN_CODE = 0x0008;
            public const ushort KEYBOARD_FLAG_UNICODE = 0x0004;
        }

        public static class KeyboardKeys
        {
            public const ushort LEFT_MOUSE = 0x01;
            public const ushort RIIGHT_MOUSE = 0x02;
            public const ushort CANCEL = 0x03;
            public const ushort MIDDLE_MOUSE = 0x04;
            public const ushort PAGE_UP_MOUSE = 0x05;
            public const ushort PAGE_DOWN_MOUSE = 0x06;
            public const ushort BACKSPACE = 0x08;
            public const ushort TAB = 0x09;
            public const ushort CLEAR = 0x0C;
            public const ushort RETURN = 0x0D;
            public const ushort SHIFT = 0x10;
            public const ushort CONTROL = 0x11;
            public const ushort ALT = 0x12;
            public const ushort PAUSE = 0x13;
            public const ushort CAPS_LOCK = 0x14;
            public const ushort KANA_MODE = 0x15;
            public const ushort HANGUEL_MODE = 0x15;
            public const ushort HANGUL_MODE = 0x15;
            public const ushort JUNJA_MODE = 0x17;
            public const ushort FINAL_MODE = 0x18;
            public const ushort HANJA_MODE = 0x19;
            public const ushort KANJI_MODE = 0x19;
            public const ushort ESCAPE = 0x1B;
            public const ushort IME_CONVERT = 0x1C;
            public const ushort IME_NONCONVERT = 0x1D;
            public const ushort IME_ACCEPT = 0x1E;
            public const ushort IME_MODE_CHANGE_REQUEST = 0x1F;
            public const ushort SPACEBAR = 0x20;
            public const ushort PAGE_UP = 0x21;
            public const ushort PAGE_DOWN = 0x22;
            public const ushort END = 0x23;
            public const ushort HOME = 0x24;
            public const ushort LEFT_ARROW = 0x25;
            public const ushort UP_ARROW = 0x26;
            public const ushort RIGHT_ARROW = 0x27;
            public const ushort DOWN_ARROW = 0x28;
            public const ushort SELECT = 0x29;
            public const ushort PRINT = 0x2A;
            public const ushort EXECUTE = 0x2B;
            public const ushort PRINT_SCREEN = 0x2C;
            public const ushort INSERT = 0x2D;
            public const ushort DELETE = 0x2E;
            public const ushort HELP = 0x2F;
            public const ushort ZERO = 0x30;
            public const ushort ONE = 0x31;
            public const ushort TWO = 0x32;
            public const ushort THREE = 0x33;
            public const ushort FOUR = 0x34;
            public const ushort FIVE = 0x35;
            public const ushort SIX = 0x36;
            public const ushort SEVEN = 0x37;
            public const ushort EIGHT = 0x38;
            public const ushort NINE = 0x39;
            public const ushort A = 0x41;
            public const ushort B = 0x42;
            public const ushort C = 0x43;
            public const ushort D = 0x44;
            public const ushort E = 0x45;
            public const ushort F = 0x46;
            public const ushort G = 0x47;
            public const ushort H = 0x48;
            public const ushort I = 0x49;
            public const ushort J = 0x4A;
            public const ushort K = 0x4B;
            public const ushort L = 0x4C;
            public const ushort M = 0x4D;
            public const ushort N = 0x4E;
            public const ushort O = 0x4F;
            public const ushort P = 0x50;
            public const ushort Q = 051;
            public const ushort R = 0x52;
            public const ushort S = 0x53;
            public const ushort T = 0x54;
            public const ushort U = 0x55;
            public const ushort V = 0x56;
            public const ushort W = 0x57;
            public const ushort X = 0x58;
            public const ushort Y = 0x59;
            public const ushort Z = 0x5A;
            public const ushort LEFT_WINDOWS = 0x5B;
            public const ushort RIGHT_WINDOWS = 0x5C;
            public const ushort APPLICATIONS = 0x5D;
            public const ushort COMPUTER_SLEEP = 0x5F;
            public const ushort NUMBERPAD_0 = 0x60;
            public const ushort NUMBERPAD_1 = 0x61;
            public const ushort NUMBERPAD_2 = 0x62;
            public const ushort NUMBERPAD_3 = 0x63;
            public const ushort NUMBERPAD_4 = 0x64;
            public const ushort NUMBERPAD_5 = 0x65;
            public const ushort NUMBERPAD_6 = 0x66;
            public const ushort NUMBERPAD_7 = 0x67;
            public const ushort NUMBERPAD_8 = 0x68;
            public const ushort NUMBERPAD_9 = 0x69;
            public const ushort NUMBERPAD_MULTIPLY = 0x6A;
            public const ushort NUMBERPAD_ADD = 0x6B;
            public const ushort NUMBERPAD_SEPERATOR = 0x6C;
            public const ushort NUMBERPAD_SUBTRACT = 0x6D;
            public const ushort NUMBERPAD_DECIMAL = 0x6E;
            public const ushort NUMBERPAD_DIVIDE = 0x6F;
            public const ushort F1 = 0x70;
            public const ushort F2 = 0x71;
            public const ushort F3 = 0x72;
            public const ushort F4 = 0x73;
            public const ushort F5 = 0x74;
            public const ushort F6 = 0x75;
            public const ushort F7 = 0x76;
            public const ushort F8 = 0x77;
            public const ushort F9 = 0x78;
            public const ushort F10 = 0x79;
            public const ushort F11 = 0x7A;
            public const ushort F12 = 0x7B;
            public const ushort F13 = 0x7C;
            public const ushort F14 = 0x7D;
            public const ushort F15 = 0x7E;
            public const ushort F16 = 0x7F;
            public const ushort F17 = 0x80;
            public const ushort F18 = 0x81;
            public const ushort F19 = 0x82;
            public const ushort F20 = 0x83;
            public const ushort F21 = 0x84;
            public const ushort F22 = 0x85;
            public const ushort F23 = 0x86;
            public const ushort F24 = 0x87;
            public const ushort NUM_LOCK = 0x90;
            public const ushort SCROLL_LOCK = 0x91;
            public const ushort LEFT_SHIFT = 0xA0;
            public const ushort RIGHT_SHIFT = 0xA1;
            public const ushort LEFT_CONTROL = 0xA2;
            public const ushort RIGHT_CONTROL = 0xA3;
            public const ushort LEFT_ALT = 0xA4;
            public const ushort RIGHT_ALT = 0xA5;
            public const ushort BROWSER_BACK = 0xA6;
            public const ushort BROWSER_FORWARD = 0xA7;
            public const ushort BROWSER_REFRESH = 0xA8;
            public const ushort BROWSER_STOP = 0xA9;
            public const ushort BROWSER_SEARCH = 0xAA;
            public const ushort BROWSER_FAVORITES = 0xAB;
            public const ushort BROWSER_START_HOME = 0xAC;
            public const ushort VOLUME_MUTE = 0xAD;
            public const ushort VOLUME_DOWN = 0xAE;
            public const ushort VOLUME_UP = 0xAF;
            public const ushort NEXT_TRACK = 0xB0;
            public const ushort PREVIOUS_TRACK = 0xB1;
            public const ushort STOP_MEDIA = 0xB2;
            public const ushort PLAY_PAUSE_MEDIA = 0xB3;
            public const ushort START_MAIL = 0xB4;
            public const ushort SELECT_MEDIA = 0xB5;
            public const ushort START_APPLICATION_1 = 0xB6;
            public const ushort START_APPLICATION_2 = 0xB7;
            public const ushort SEMICOLON_COLON = 0xBA;
            public const ushort EQUALS_ADDITION = 0xBB;
            public const ushort COMMA_LESS_THAN = 0XBC;
            public const ushort DASH_UNDERSCORE = 0xBD;
            public const ushort PERIOD_GREATER_THAN = 0xBE;
            public const ushort FORWARD_SLASH_QUESTION_MARK = 0xBE;
            public const ushort ACCENT_TILDE = 0xC0;
            public const ushort OPEN_BRACKET_OPEN_BRACE = 0xDB;
            public const ushort BACK_SLASH_VERTICAL_BAR = 0xDC;
            public const ushort CLOSE_BRACKET_CLOSE_BRACE = 0xDD;
            public const ushort APOSTROPHE_QUOTATION_MARK = 0xDE;
            public const ushort MISC_1 = 0xDF;
            public const ushort ANGLE_BRACKET_OR_BACKSLASH = 0xE2;
            public const ushort IME_PROCESS = 0xE5;
            public const ushort UNICODE_PACKET = 0xE7;
            public const ushort ATTN = 0xF6;
            public const ushort CR_SEL = 0xF7;
            public const ushort EX_SEL = 0xF8;
            public const ushort ERASE_EOF = 0xF9;
            public const ushort PLAY = 0xFA;
            public const ushort ZOOM = 0xFB;
            public const ushort NO_NAME = 0xFC;
            public const ushort PA1 = 0xFD;
            public const ushort OEM_CLEAR = 0xFE;
        }

        public class UnicodeControlChars
        {
            public const char NULL = (char)0x0000;
            public const char START_OF_HEADING = (char)0x0001;
            public const char START_OF_TEXT = (char)0x0002;
            public const char END_OF_TEXT = (char)0x0003;
            public const char END_OF_TRANSMISSION = (char)0x0004;
            public const char ENQUIRY = (char)0x0005;
            public const char ACKNOWLEDGE = (char)0x0006;
            public const char ALERT = (char)0x0007;
            public const char BACKSPACE = (char)0x0008;
            public const char TAB = (char)0x0009;
            public const char NEW_LINE = (char)0x000A;
            public const char LINE_TAB = (char)0x000B;
            public const char FORM_FEED = (char)0x000C;
            public const char CARRIAGE_RETURN = (char)0x000D;
            public const char LOCKING_SHIFT_1 = (char)0x000E;
            public const char LOCKING_SHIFT_0 = (char)0x000F;
            public const char DATA_LINK_ESCAPE = (char)0x0010;
            public const char DEVICE_CONTROL_1 = (char)0x0011;
            public const char DEVICE_CONTROL_2 = (char)0x0012;
            public const char DEVICE_CONTROL_3 = (char)0x0013;
            public const char DEVICE_CONTROL_4 = (char)0x0014;
            public const char NEGATIVE_ACKNOWLEDGE = (char)0x0015;
            public const char SYNCRONOUS_IDLE = (char)0x0016;
            public const char END_OF_TRANSMISSION_BLOCK = (char)0x0017;
            public const char CANCEL = (char)0x0018;
            public const char END_OF_MEDIUM = (char)0x0019;
            public const char SUBSTITUTE = (char)0x001A;
            public const char ESCAPE = (char)0x001B;
            public const char FILE_SEPARATOR = (char)0x001C;
            public const char GROUP_SEPARATOR = (char)0x001D;
            public const char INFORMATION_SEPARATOR_2 = (char)0x001E;
            public const char INFORMATION_SEPERATOR_1 = (char)0x001F;
            public const char DELETE = (char)0x007F;
            public const char PADDING_CHARACTER = (char)0x0080;
            public const char HIGH_OCTET_PRESET = (char)0x0081;
            public const char BREAK_PERMITTED_HERE = (char)0x0082;
            public const char NO_BREAK_HERE = (char)0x0083;
            public const char INDEX = (char)0x0084;
            public const char NEXT_LINE = (char)0x0085;
            public const char START_OF_SELECTED_AREA = (char)0x0086;
            public const char END_OF_SELECTED_AREA = (char)0x0087;
            public const char CHARACTER_TAB_SET = (char)0x0088;
            public const char CHARACTER_TAB_WITH_JUSTIFICATION = (char)0x0089;
            public const char LINE_TAB_SET = (char)0x008A;
            public const char PARTIAL_LINE_DOWN = (char)0x008B;
            public const char PARTIAL_LINE_BACKWARD = (char)0x008C;
            public const char REVERSE_INDEX = (char)0x008D;
            public const char SINGLE_SHIFT_2 = (char)0x008E;
            public const char SINGLE_SHIFT_3 = (char)0x008F;
            public const char DEVICE_CONTROL_STRING = (char)0x0090;
            public const char PRIVATE_USE_1 = (char)0x0091;
            public const char PRIVATE_USE_2 = (char)0x0092;
            public const char SET_TRANSMIT_STATE = (char)0x0093;
            public const char CANCEL_CHARACTER = (char)0x0094;
            public const char MESSAGE_WAITING = (char)0x0095;
            public const char START_OF_GUARDED_AREA = (char)0x0096;
            public const char END_OF_GUARDED_AREA = (char)0x0097;
            public const char START_OF_STRING = (char)0x0098;
            public const char SINGLE_GRAPHIC_CHARACTER_INTRODUCER = (char)0x0099;
            public const char SINGLE_CHARACTER_INTRODUCER = (char)0x009A;
            public const char CONTROL_SEQUENCE_INTRODUCER = (char)0x009B;
            public const char STRING_TERMINATOR = (char)0x009C;
            public const char OPERATING_SYSTEM_COMMAND = (char)0x009D;
            public const char PRIVACY_MESSAGE = (char)0x009E;
            public const char APPLICATION_PROGRAM_COMMAND = (char)0x009F;
        }
    }
}
