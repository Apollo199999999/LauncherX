using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LauncherXWinUI.Classes
{
    /// <summary>
    /// Contains helper functions for dealing with keys
    /// Note that KeycodeToChar and CharToKeycode are perfect inverses,
    /// and composing the 2 functions can be helpful in mapping both LShift and RShift to just Shift, for example
    /// </summary>
    public static class KeyClass
    {

        /// <summary>
        /// Converts a key to a string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string KeycodeToChar(Keys key)
        {

            switch (key)
            {
                case Keys.Add:
                    return "(Numpad) +";
                case Keys.Decimal:
                    return "(Numpad) .";
                case Keys.Divide:
                    return "(Numpad) /";
                case Keys.Multiply:
                    return "(Numpad) *";
                case Keys.OemBackslash:
                    return "\\";
                case Keys.OemCloseBrackets:
                    return "]";
                case Keys.OemMinus:
                    return "-";
                case Keys.OemOpenBrackets:
                    return "[";
                case Keys.OemPeriod:
                    return ".";
                case Keys.OemPipe:
                    return "|";
                case Keys.OemQuestion:
                    return "/";
                case Keys.OemQuotes:
                    return "\"";
                case Keys.OemSemicolon:
                    return ";";
                case Keys.Oemcomma:
                    return ",";
                case Keys.Oemplus:
                    return "+";
                case Keys.Oemtilde:
                    return "`";
                case Keys.Subtract:
                    return "(Numpad) -";
                case Keys.D0:
                    return "0";
                case Keys.D1:
                    return "1";
                case Keys.D2:
                    return "2";
                case Keys.D3:
                    return "3";
                case Keys.D4:
                    return "4";
                case Keys.D5:
                    return "5";
                case Keys.D6:
                    return "6";
                case Keys.D7:
                    return "7";
                case Keys.D8:
                    return "8";
                case Keys.D9:
                    return "9";
                case Keys.Space:
                    return "Space";
                case Keys.Menu:
                    return "Alt";
                case Keys.LMenu:
                    return "Alt";
                case Keys.RMenu:
                    return "Alt";
                case Keys.ShiftKey:
                    return "Shift";
                case Keys.LShiftKey:
                    return "Shift";
                case Keys.RShiftKey:
                    return "Shift";
                case Keys.ControlKey:
                    return "Ctrl";
                case Keys.LControlKey:
                    return "Ctrl";
                case Keys.RControlKey:
                    return "Ctrl";
                case Keys.LWin:
                    return "Win";
                case Keys.RWin:
                    return "Win";
                default:
                    return key.ToString();
            }
        }

        /// <summary>
        /// Converts a string back to a key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Keys CharToKeycode(string key)
        {

            switch (key)
            {
                case "(Numpad) +":
                    return Keys.Add;
                case "(Numpad) .":
                    return Keys.Decimal;
                case "(Numpad) /":
                    return Keys.Divide;
                case "(Numpad) *":
                    return Keys.Multiply;
                case "\\":
                    return Keys.OemBackslash;
                case "]":
                    return Keys.OemCloseBrackets;
                case "-":
                    return Keys.OemMinus;
                case "[":
                    return Keys.OemOpenBrackets;
                case ".":
                    return Keys.OemPeriod;
                case "|":
                    return Keys.OemPipe;
                case "/":
                    return Keys.OemQuestion;
                case "\"":
                    return Keys.OemQuotes;
                case ";":
                    return Keys.OemSemicolon;
                case ",":
                    return Keys.Oemcomma;
                case "+":
                    return Keys.Oemplus;
                case "`":
                    return Keys.Oemtilde;
                case "(Numpad) -":
                    return Keys.Subtract;
                case "0":
                    return Keys.D0;
                case "1":
                    return Keys.D1;
                case "2":
                    return Keys.D2;
                case "3":
                    return Keys.D3;
                case "4":
                    return Keys.D4;
                case "5":
                    return Keys.D5;
                case "6":
                    return Keys.D6;
                case "7":
                    return Keys.D7;
                case "8":
                    return Keys.D8;
                case "9":
                    return Keys.D9;
                case "Space":
                    return Keys.Space;
                case "Alt":
                    return Keys.Menu;
                case "Shift":
                    return Keys.ShiftKey;
                case "Ctrl":
                    return Keys.ControlKey;
                case "Win":
                    return Keys.LWin;

                default:
                    return (Keys)Enum.Parse(typeof(Keys), key);
            }
        }
    }
}
