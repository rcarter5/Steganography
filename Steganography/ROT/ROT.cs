using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.ROT
{
    /// <summary>
    /// Encrpts the Message
    /// </summary>
    public static class Rot
    {

        /// <summary>
        /// Rotates the specified number.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="num">The number.</param>
        /// <returns>The Rotated message</returns>
        public static string ForwardRot(this string message, int num)
        {
            string result = String.Empty;
            foreach (var character in message)
            {
                char nextCharacter = character;
                if ((character >= 'A' && character <= 'Z') || (character >= 'a' && character <= 'z'))
                {
                    nextCharacter = (char)(nextCharacter + num);
                    if (character <= 'Z' && nextCharacter > 'Z')
                    {
                        nextCharacter = (char)(nextCharacter - 'Z' + 'A' - 1);
                    }
                    else if (character <= 'z' && nextCharacter > 'z')
                    {
                        nextCharacter = (char)(nextCharacter - 'Z' + 'A' - 1);
                    }
                }

                result += nextCharacter;
            }
            return result;
        }

        /// <summary>
        /// Reverses the rot.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="num">The number.</param>
        /// <returns>The original message</returns>
        public static string ReverseRot(this string message, int num)
        {
            var roatationAmount = 26 - num;
            string result = String.Empty;
            foreach (var character in message)
            {
                char nextCharacter = character;
                if ((character >= 'A' && character <= 'Z') || (character >= 'a' && character <= 'z'))
                {
                    nextCharacter = (char)(nextCharacter + roatationAmount);
                    if (character <= 'Z' && nextCharacter > 'Z')
                    {
                        nextCharacter = (char)(nextCharacter - 'Z' + 'A' - 1);
                    }
                    else if (character <= 'z' && nextCharacter > 'z')
                    {
                        nextCharacter = (char)(nextCharacter - 'Z' + 'A' - 1);
                    }
                }

                result += nextCharacter;
            }
            return result;
        }
    }
}
