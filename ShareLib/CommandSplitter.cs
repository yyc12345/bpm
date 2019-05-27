using System;
using System.Collections.Generic;
using System.Text;

//Powered by ShadowPower: https://gist.github.com/ShadowPower/22cceef242fce65217eb7bf912806b99
namespace ShareLib {
    public class CommandSplitter {

        public static List<string> Split(string command) {
            var result = new List<string>();

            SplitterState current = SplitterState.Space, previous = SplitterState.Space;
            string itemCache = "";

            #region processor

            void SpaceProcessor(char chr) {
                switch (chr) {
                    case '\'':
                        current = SplitterState.Single;
                        break;
                    case '"':
                        current = SplitterState.Double;
                        break;
                    case '\\':
                        current = SplitterState.Shift;
                        previous = SplitterState.Normal;
                        break;
                    case ' ':
                        ;//keep
                        break;
                    default:
                        itemCache += chr;
                        current = SplitterState.Normal;
                        break;
                }
            }

            void NormalProcessor(char chr) {
                switch (chr) {
                    case '\'':
                        itemCache += '\'';
                        break;
                    case '"':
                        itemCache += '"';
                        break;
                    case '\\':
                        previous = SplitterState.Normal;
                        current = SplitterState.Shift;
                        break;
                    case ' ':
                        result.Add(itemCache);
                        itemCache = "";
                        current = SplitterState.Space;
                        break;
                    default:
                        itemCache += chr;
                        break;
                }
            }

            void SingleProcessor(char chr) {
                switch (chr) {
                    case '\'':
                        current = SplitterState.Normal;
                        break;
                    case '"':
                        itemCache += '"';
                        break;
                    case '\\':
                        previous = SplitterState.Single;
                        current = SplitterState.Shift;
                        break;
                    case ' ':
                        itemCache += ' ';
                        break;
                    default:
                        itemCache += chr;
                        break;
                }
            }

            void DoubleProcessor(char chr) {
                switch (chr) {
                    case '\'':
                        itemCache += '\'';
                        break;
                    case '"':
                        current = SplitterState.Normal;
                        break;
                    case '\\':
                        previous = SplitterState.Double;
                        current = SplitterState.Shift;
                        break;
                    case ' ':
                        itemCache += ' ';
                        break;
                    default:
                        itemCache += chr;
                        break;
                }
            }

            void ShiftProcessor(char chr) {
                switch (chr) {
                    case '\'':
                        itemCache += '\'';
                        break;
                    case '"':
                        itemCache += '"';
                        break;
                    case '\\':
                        itemCache += '\\';
                        break;
                    case ' ':
                        throw new ArgumentException();
                    default:
                        throw new ArgumentException();
                }
                current = previous;
            }


            #endregion

            try {
                foreach (var item in command) {
                    switch (current) {
                        case SplitterState.Space:
                            SpaceProcessor(item);
                            break;
                        case SplitterState.Normal:
                            NormalProcessor(item);
                            break;
                        case SplitterState.Single:
                            SingleProcessor(item);
                            break;
                        case SplitterState.Double:
                            DoubleProcessor(item);
                            break;
                        case SplitterState.Shift:
                            ShiftProcessor(item);
                            break;
                    }
                }

                switch (current) {
                    case SplitterState.Space:
                        break;
                    case SplitterState.Normal:
                        //add the last one
                        result.Add(itemCache);
                        break;
                    case SplitterState.Single:
                    case SplitterState.Double:
                    case SplitterState.Shift:
                        throw new ArgumentException();
                }
            } catch {
                //when raise a error, clean list
                result.Clear();
            }
            
            return result;
        }

        private enum SplitterState {
            Space,
            Normal,
            Single,
            Double,
            Shift
        }


    }
}