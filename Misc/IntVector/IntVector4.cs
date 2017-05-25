using System;
using Expanse.Utilities;
using UnityEngine;

namespace Expanse.Misc
{
    [Serializable]
    public struct IntVector4
    {
        public int x;
        public int y;
        public int z;
        public int w;

        public IntVector4(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public IntVector4(Vector4 source, FloatConversionMethod conversionMethod = FloatConversionMethod.Floor)
        {
            this.x = FloatConversionUtil.ConvertToInt(source.x, conversionMethod, false);
            this.y = FloatConversionUtil.ConvertToInt(source.y, conversionMethod, false);
            this.z = FloatConversionUtil.ConvertToInt(source.z, conversionMethod, false);
            this.w = FloatConversionUtil.ConvertToInt(source.w, conversionMethod, false);
        }

        /// <summary>
        /// Sets the X, Y, Z and W values.
        /// </summary>
        public void Set(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// Normalizes the magnitude of this vector to a length of 1.
        /// </summary>
        public void Normalize()
        {
            float magnitude = this.Magnitude;

            if (magnitude > 0)
            {
                this = new IntVector4(((Vector4)this / magnitude), FloatConversionMethod.Round);
            }
        }

        /// <summary>
        /// Returns the length of this vector.
        /// </summary>
        public float Magnitude
        {
            get
            {
                return Mathf.Sqrt(this.SqrMagnitude);
            }
        }

        /// <summary>
        /// Returns the squared length of this vector.
        /// </summary>
        public float SqrMagnitude
        {
            get
            {
                return (this.x * this.x) + (this.y * this.y) + (this.z * this.z) + (this.w * this.w);
            }
        }

        /// <summary>
        /// Calculates the direct distance between 2 vectors.
        /// </summary>
        public static float Distance(IntVector4 a, IntVector4 b)
        {
            return (a - b).Magnitude;
        }

        /// <summary>
        /// Calculates the grid-based distance between 2 vectors.
        /// </summary>
        public static int GridDistance(IntVector4 a, IntVector4 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)
                + Mathf.Abs(a.z - b.z) + Mathf.Abs(a.w - b.w);
        }

        /// <summary>
        /// Calculates the dot product of 2 vectors.
        /// </summary>
        public static float Dot(IntVector4 a, IntVector4 b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z) + (a.w * b.w);
        }

        /// <summary>
        /// Calculates the angle between 2 vectors.
        /// </summary>
        public static float Angle(IntVector4 a, IntVector4 b)
        {
            return Mathf.Acos(Mathf.Clamp(Vector4.Dot((Vector4)a, (Vector4)b), -1f, 1f)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Scales vector A by vector B.
        /// </summary>
        public static IntVector4 Scale(IntVector4 a, IntVector4 b)
        {
            return new IntVector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        /// <summary>
        /// Calculates a reflection vector.
        /// </summary>
        public static IntVector4 Reflect(IntVector4 inDirection, IntVector4 inNormal)
        {
            return -2 * Mathf.RoundToInt(IntVector4.Dot(inNormal, inDirection)) * inNormal + inDirection;
        }

        /// <summary>
        /// Creates a new vector using the smallest component values.
        /// </summary>
        public static IntVector4 Min(IntVector4 a, IntVector4 b)
        {
            return new IntVector4(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z), Mathf.Min(a.w, b.w));
        }

        /// <summary>
        /// Creates a new vector using the largest component values.
        /// </summary>
        public static IntVector4 Max(IntVector4 a, IntVector4 b)
        {
            return new IntVector4(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z), Mathf.Max(a.w, b.w));
        }

        /// <summary>
        /// Linearly interpolates the component values between 2 vectors.
        /// </summary>
        public static IntVector4 Lerp(IntVector4 a, IntVector4 b, float t)
        {
            t = Mathf.Clamp01(t);

            Vector4 vec = new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);

            return new IntVector4(vec, FloatConversionMethod.Round);
        }

        /// <summary>
        /// Linearly interpolates the component values between 2 vectors without clamping T.
        /// </summary>
        public static IntVector4 LerpUnclamped(IntVector4 a, IntVector4 b, float t)
        {
            Vector4 vec = new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);

            return new IntVector4(vec, FloatConversionMethod.Round);
        }

        /// <summary>
        /// Short-hand for new IntVector4(0, 0, 0, 0).
        /// </summary>
        public static IntVector4 Zero
        {
            get { return new IntVector4(0, 0, 0, 0); }
        }

        /// <summary>
        /// Short-hand for new IntVector4(1, 1, 1, 1).
        /// </summary>
        public static IntVector4 One
        {
            get { return new IntVector4(1, 1, 1, 1); }
        }

        /// <summary>
        /// Short-hand for new IntVector4(1, 0, 0, 0).
        /// </summary>
        public static IntVector4 Right
        {
            get { return new IntVector4(1, 0, 0, 0); }
        }

        /// <summary>
        /// Short-hand for new IntVector4(-1, 0, 0, 0).
        /// </summary>
        public static IntVector4 Left
        {
            get { return new IntVector4(-1, 0, 0, 0); }
        }

        /// <summary>
        /// Short-hand for new IntVector4(0, 1, 0, 0).
        /// </summary>
        public static IntVector4 Up
        {
            get { return new IntVector4(0, 1, 0, 0); }
        }

        /// <summary>
        /// Short-hand for new IntVector4(0, -1, 0, 0).
        /// </summary>
        public static IntVector4 Down
        {
            get { return new IntVector4(0, -1, 0, 0); }
        }

        /// <summary>
        /// Short-hand for new IntVector4(0, 0, 1, 0).
        /// </summary>
        public static IntVector4 Forward
        {
            get { return new IntVector4(0, 0, 1, 0); }
        }

        /// <summary>
        /// Short-hand for new IntVector4(0, 0, -1, 0).
        /// </summary>
        public static IntVector4 Backward
        {
            get { return new IntVector4(0, 0, -1, 0); }
        }

        /// <summary>
        /// Short-hand for new IntVector4(0, 0, 0, 1).
        /// </summary>
        public static IntVector4 Ana
        {
            get { return new IntVector4(0, 0, 0, 1); }
        }

        /// <summary>
        /// Short-hand for new IntVector4(0, 0, 0, -1).
        /// </summary>
        public static IntVector4 Kata
        {
            get { return new IntVector4(0, 0, 0, -1); }
        }

        /// <summary>
        /// Returns a new normalized vector.
        /// </summary>
        public IntVector4 Normalized
        {
            get
            {
                IntVector4 vec = new IntVector4(this.x, this.y, this.z, this.w);
                vec.Normalize();
                return vec;
            }
        }

        /// <summary>
        /// Returns a formatted string containing component values of this vector.
        /// </summary>
        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", this.x, this.y, this.z, this.w);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.Equals((IntVector4)obj);
        }

        public bool Equals(IntVector4 other)
        {
            return this.GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int primeA = 17;
                int primeB = 23;

                int hash = primeA;

                hash = hash * primeB + this.x.GetHashCode();
                hash = hash * primeB + this.y.GetHashCode();
                hash = hash * primeB + this.z.GetHashCode();
                hash = hash * primeB + this.w.GetHashCode();

                return hash;
            }
        }

        public static bool operator ==(IntVector4 a, IntVector4 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(IntVector4 a, IntVector4 b)
        {
            return !a.Equals(b);
        }

        public static IntVector4 operator +(IntVector4 a, IntVector4 b)
        {
            return new IntVector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static IntVector4 operator -(IntVector4 a, IntVector4 b)
        {
            return new IntVector4(a.x - b.x, a.y - b.y, a.z + b.z, a.w - b.w);
        }

        public static IntVector4 operator -(IntVector4 vec)
        {
            return new IntVector4(-vec.x, -vec.y, -vec.z, -vec.w);
        }

        public static IntVector4 operator *(IntVector4 vec, int scale)
        {
            return new IntVector4(vec.x * scale, vec.y * scale, vec.z * scale, vec.w * scale);
        }

        public static IntVector4 operator *(int scale, IntVector4 vec)
        {
            return new IntVector4(vec.x * scale, vec.y * scale, vec.z * scale, vec.w * scale);
        }

        public static IntVector4 operator /(IntVector4 vec, int scale)
        {
            return new IntVector4(vec.x / scale, vec.y / scale, vec.z / scale, vec.w / scale);
        }

        public static explicit operator Vector4(IntVector4 vec)
        {
            return new Vector4(vec.x, vec.y, vec.z, vec.w);
        }
    }
}
