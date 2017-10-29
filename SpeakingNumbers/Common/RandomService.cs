using System;
using System.Collections.Generic;

namespace NumberSpeaker.Common
{
    /// <summary>
    /// A service to provide individual or sets of random values. 
    /// </summary>
    public static class RandomService
    {
        private static readonly Random Random = new Random();
        /// <summary>
        /// Serves a single random integer, between the requested range, inclusive.
        /// </summary>
        /// <param name="rangeStart">optional range start</param>
        /// <param name="rangeEnd">optional range end</param>
        /// <returns></returns>
        public static int GetRandomInt(int rangeStart = 1, int rangeEnd = 100)
        {
            return Random.Next(rangeStart, rangeEnd + 1); 
        }

        /// <summary>
        /// Return a set of integers, based on the requested size, range start and end.
        /// One of this is the spoken one, ie, the correct one. So that this is not always
        /// in the same position, randomize that position.
        /// </summary>
        /// <param name="rangeStart"></param>
        /// <param name="rangeEnd"></param>
        /// <returns>A dictionary of numbers in 1 column (eg 278,344, 877), with 1 occurrence of "right", and all else "wrong"</returns>
        public static Dictionary<string, int> GetRandomIntSet(int count = 6, int rangeStart = 1, int rangeEnd = 100){
            var correctIndex = GetRandomInt(1, count) - 1;
            var numbers = new Dictionary<string, int>();
            for (int i = 0; i < count; i++) {
                numbers.Add(i == correctIndex ? "correct" : Guid.NewGuid().ToString(), GetRandomInt(rangeStart, rangeEnd));
            }
            return numbers;
        }
    }
}
