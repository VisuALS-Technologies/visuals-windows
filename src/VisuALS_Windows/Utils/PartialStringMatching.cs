using System;
using System.Collections.Generic;
using System.Linq;

namespace VisuALS_WPF_App
{
    static class PartialStringMatching
    {
        public enum KeyboardType { QWERTY, ALPHABETICAL };

        /// <summary>
        /// Gets a list of characters that are close to the given character on the specified keyboard
        /// used to aid in word check to guess what word the user may have been trying to type
        /// </summary>
        /// <param name="key"> character to find nearby characters of </param>
        /// <param name="type"> keyboard layout to use </param>
        /// <returns></returns>
        public static List<char> getNearbyKeys(char key, KeyboardType type = KeyboardType.QWERTY)
        {
            List<char> keys = new List<char>(); //the list of nearby keys

            if (type == KeyboardType.QWERTY) //the following switch statement is for the Qwerty layout
            {
                switch (key) //figure out what key the input is, and add the appropriate nearby keys to the list
                {
                    case 'a':
                        keys.Add('s'); keys.Add('q'); keys.Add('z'); keys.Add('w');
                        break;
                    case 'b':
                        keys.Add('n'); keys.Add('v'); keys.Add('g'); keys.Add('h');
                        break;
                    case 'c':
                        keys.Add('v'); keys.Add('x'); keys.Add('d'); keys.Add('f');
                        break;
                    case 'd':
                        keys.Add('s'); keys.Add('f'); keys.Add('e'); keys.Add('c'); keys.Add('x');
                        break;
                    case 'e':
                        keys.Add('r'); keys.Add('w'); keys.Add('d');
                        break;
                    case 'f':
                        keys.Add('d'); keys.Add('g'); keys.Add('e'); keys.Add('c'); keys.Add('x');
                        break;
                    case 'g':
                        keys.Add('f'); keys.Add('h'); keys.Add('t'); keys.Add('b'); keys.Add('v');
                        break;
                    case 'h':
                        keys.Add('g'); keys.Add('j'); keys.Add('n'); keys.Add('b');
                        break;
                    case 'i':
                        keys.Add('o'); keys.Add('u'); keys.Add('k');
                        break;
                    case 'j':
                        keys.Add('h'); keys.Add('k'); keys.Add('n'); keys.Add('u'); keys.Add('m');
                        break;
                    case 'k':
                        keys.Add('l'); keys.Add('j'); keys.Add('i'); keys.Add('m');
                        break;
                    case 'l':
                        keys.Add('k'); keys.Add('o'); keys.Add('p');
                        break;
                    case 'm':
                        keys.Add('n'); keys.Add('k'); keys.Add('j');
                        break;
                    case 'n':
                        keys.Add('b'); keys.Add('m'); keys.Add('h'); keys.Add('j');
                        break;
                    case 'o':
                        keys.Add('i'); keys.Add('p'); keys.Add('l');
                        break;
                    case 'p':
                        keys.Add('o'); keys.Add('l');
                        break;
                    case 'q':
                        keys.Add('w'); keys.Add('a');
                        break;
                    case 'r':
                        keys.Add('e'); keys.Add('t'); keys.Add('f');
                        break;
                    case 's':
                        keys.Add('a'); keys.Add('d'); keys.Add('w'); keys.Add('x');
                        break;
                    case 't':
                        keys.Add('r'); keys.Add('y'); keys.Add('g');
                        break;
                    case 'u':
                        keys.Add('i'); keys.Add('y'); keys.Add('j');
                        break;
                    case 'v':
                        keys.Add('c'); keys.Add('b'); keys.Add('f'); keys.Add('g');
                        break;
                    case 'w':
                        keys.Add('e'); keys.Add('q'); keys.Add('s');
                        break;
                    case 'x':
                        keys.Add('c'); keys.Add('z'); keys.Add('s'); keys.Add('d');
                        break;
                    case 'y':
                        keys.Add('t'); keys.Add('u'); keys.Add('h');
                        break;
                    case 'z':
                        keys.Add('x'); keys.Add('s'); keys.Add('a');
                        break;
                }
            }
            else if (type == KeyboardType.ALPHABETICAL) //the following switch statement is for our ABC keyboard layout
            {
                switch (key) //figure out what key the input is, and add the appropriate nearby keys to the list
                {
                    case 'a':
                        keys.Add('b'); keys.Add('f'); keys.Add('g');
                        break;
                    case 'b':
                        keys.Add('a'); keys.Add('c'); keys.Add('g');
                        break;
                    case 'c':
                        keys.Add('b'); keys.Add('d'); keys.Add('h');
                        break;
                    case 'd':
                        keys.Add('e'); keys.Add('c'); keys.Add('i');
                        break;
                    case 'e':
                        keys.Add('d'); keys.Add('i');
                        break;
                    case 'f':
                        keys.Add('a'); keys.Add('g'); keys.Add('b');
                        break;
                    case 'g':
                        keys.Add('f'); keys.Add('h'); keys.Add('b');
                        break;
                    case 'h':
                        keys.Add('i'); keys.Add('g'); keys.Add('c');
                        break;
                    case 'i':
                        keys.Add('h'); keys.Add('d'); keys.Add('c');
                        break;
                    case 'j':
                        keys.Add('k'); keys.Add('o'); keys.Add('p');
                        break;
                    case 'k':
                        keys.Add('l'); keys.Add('j'); keys.Add('p');
                        break;
                    case 'l':
                        keys.Add('m'); keys.Add('k'); keys.Add('q');
                        break;
                    case 'm':
                        keys.Add('l'); keys.Add('n'); keys.Add('r');
                        break;
                    case 'n':
                        keys.Add('m'); keys.Add('r');
                        break;
                    case 'o':
                        keys.Add('p'); keys.Add('j'); keys.Add('k');
                        break;
                    case 'p':
                        keys.Add('o'); keys.Add('q'); keys.Add('k');
                        break;
                    case 'q':
                        keys.Add('r'); keys.Add('p'); keys.Add('l');
                        break;
                    case 'r':
                        keys.Add('q'); keys.Add('m'); keys.Add('l');
                        break;
                    case 's':
                        keys.Add('t'); keys.Add('x'); keys.Add('z');
                        break;
                    case 't':
                        keys.Add('s'); keys.Add('u'); keys.Add('y');
                        break;
                    case 'u':
                        keys.Add('t'); keys.Add('v'); keys.Add('z');
                        break;
                    case 'v':
                        keys.Add('u'); keys.Add('w');
                        break;
                    case 'w':
                        keys.Add('v');
                        break;
                    case 'x':
                        keys.Add('y'); keys.Add('s'); keys.Add('t');
                        break;
                    case 'y':
                        keys.Add('z'); keys.Add('x'); keys.Add('t');
                        break;
                    case 'z':
                        keys.Add('y'); keys.Add('u'); keys.Add('t'); keys.Add('v');
                        break;
                }
            }

            return keys;
        }


        //TODO: Fix nearby key replacements, they no longer add functionality

        /// <summary>
        /// Selects partial matching strings from the possibleMatches list
        /// based on the target string and the given parameters
        /// </summary>
        /// <param name="target"> target string for matching </param>
        /// <param name="possibleMatches"> list of possible matches to select partial matches from </param>
        /// <param name="tolerance"> number of false characters permitted in a partial match </param>
        /// <param name="cutPossibleMatchesToTargetSizeForComparison"> if true only characters up to the target length will be checked when looking for partial matches </param>
        /// <param name="tryForMissingCharacters"> allow for character skipping </param>
        /// <param name="type"> type of keyboard to use for getting nearby key replacements </param>
        /// <returns></returns>
        public static List<string> GetNearMatches(string target_, List<string> possibleMatches, int tolerance = 2, bool cutPossibleMatchesToTargetSizeForComparison = true, bool tryForMissingCharacters = true)
        {
            string target = target_.ToLower();
            List<Tuple<int, string>> nearMatches = new List<Tuple<int, string>>();

            foreach (string match in possibleMatches.Where(x => DiceCoeff(target, x, 0.7))) //go through each possible match
            {
                string m;
                m = match.ToLower();

                int missAccounts = 0; //keeps track of how many characters were "skipped" if the tryForMissingCharacters is true
                int falseCharAccounts = 0; //keeps track of how many times a char comparison was wrong AND a skip or nearby key replacement could not be made for that char

                for (int i = 0; i < target.Length; i++) //go through every character in the target and check against the possible match
                {
                    //if the match is smaller than the current index, or the current character for the match is not equal to that of the target
                    if (m.Length <= i || m[i] != target[i - missAccounts])
                    {
                        bool skipChar = false;

                        //if try for missing characters is enabled and a nearby key replacement wasn't made
                        if (tryForMissingCharacters && m.Length > (i + 1) && target[i - missAccounts] == m[i + 1])
                        {
                            skipChar = true; //skip this character
                            falseCharAccounts++; //increment the missed key counter
                        }

                        //if the character has not been skipped and a nearby key replacement has not been fount
                        if (!skipChar)
                        {
                            falseCharAccounts++; //increment false character counter
                        }

                        if (falseCharAccounts > tolerance)
                        {
                            break;
                        }
                    }
                }

                if (falseCharAccounts <= tolerance) //if the words miss counter and false character counter together are less than the tolerance
                {
                    nearMatches.Add(new Tuple<int, string>(falseCharAccounts, match)); //add the near match to the list along with it's false character count, it passes the bar
                }
            }
            return (nearMatches.OrderBy(x => x.Item1).Select(x => x.Item2)).ToList(); //return the list, sorted in descending order of false character counts
        }


        public static bool DiceCoeff(string target, string possibleMatch, double tolerance)
        {
            if (target.Distinct().Intersect(possibleMatch.Distinct()).Count() / target.Distinct().Count() >= tolerance)
            {
                return true;
            }
            return false;
        }
    }
}
