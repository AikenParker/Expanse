using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Expanse
{
    /// <summary>
    /// Determines weather a chance operation occurs or not.
    /// </summary>
    [System.Serializable]
    public struct Chance : IEnumerable<Chance>
    {
        [Range(0, 1)]
        public float chance;

        public Chance(float val)
        {
            chance = val;
        }

        /// <summary>
        /// Based on the chance value determines if chance operation should occur or not.
        /// </summary>
        public bool Check()
        {
            return UnityEngine.Random.Range(0f, 1f) < this.chance;
        }

        public static implicit operator bool(Chance val)
        {
            return !val.Equals(null) && val.Check();
        }

        public static implicit operator Chance(float val)
        {
            return new Chance() { chance = val };
        }

        public IEnumerator<Chance> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }}