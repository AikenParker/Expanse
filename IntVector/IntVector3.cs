using System;
using UnityEngine;

namespace Expanse
{
    [Serializable]
    public struct IntVector3
    {
        public int x;
        public int y;
        public int z;

        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public IntVector3(IntVector3 source)
        {
            this = source;
        }

        public IntVector3(Vector3 source, FloatToIntConversionMethod conversionMethod = FloatToIntConversionMethod.FLOOR)
        {
            switch (conversionMethod)
            {
                case FloatToIntConversionMethod.CEIL:
                    this.x = Mathf.CeilToInt(source.x);
                    this.y = Mathf.CeilToInt(source.y);
                    this.z = Mathf.CeilToInt(source.z);
                    break;

                case FloatToIntConversionMethod.FLOOR:
                    this.x = Mathf.FloorToInt(source.x);
                    this.y = Mathf.FloorToInt(source.y);
                    this.z = Mathf.FloorToInt(source.z);
                    break;

                case FloatToIntConversionMethod.ROUND:
                    this.x = Mathf.RoundToInt(source.x);
                    this.y = Mathf.RoundToInt(source.y);
                    this.z = Mathf.RoundToInt(source.z);
                    break;

                default:
                    throw new ArgumentException("conversionMethod");
            }
        }

        public void Set(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Normalize()
        {
            float magnitude = this.Magnitude;

            if (magnitude > 0)
            {
                this = new IntVector3(((Vector3)this / magnitude), FloatToIntConversionMethod.ROUND);
            }
        }

        public float Magnitude
        {
            get
            {
                return Mathf.Sqrt(this.SqrMagnitude);
            }
        }

        public float SqrMagnitude
        {
            get
            {
                return (this.x * this.x) + (this.y * this.y) + (this.z * this.z);
            }
        }

        public static float Distance(IntVector3 a, IntVector3 b)
        {
            return (a - b).Magnitude;
        }

        public static float Dot(IntVector3 a, IntVector3 b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        }

        public static float Angle(IntVector3 a, IntVector3 b)
        {
            return Mathf.Acos(Mathf.Clamp(Vector3.Dot((Vector3)a, (Vector3)b), -1f, 1f)) * Mathf.Rad2Deg;
        }

        public static IntVector3 Scale(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static IntVector3 Reflect(IntVector3 inDirection, IntVector3 inNormal)
        {
            return -2 * Mathf.RoundToInt(IntVector3.Dot(inNormal, inDirection)) * inNormal + inDirection;
        }

        public static IntVector3 Min(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        }

        public static IntVector3 Max(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }

        public static IntVector3 Lerp(IntVector3 a, IntVector3 b, float t)
        {
            t = Mathf.Clamp01(t);

            Vector3 vec = new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);

            return new IntVector3(vec, FloatToIntConversionMethod.ROUND);
        }

        public static IntVector3 LerpUnclamped(IntVector3 a, IntVector3 b, float t)
        {
            Vector3 vec = new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);

            return new IntVector3(vec, FloatToIntConversionMethod.ROUND);
        }

        public static IntVector3 Zero
        {
            get { return new IntVector3(0, 0, 0); }
        }

        public static IntVector3 One
        {
            get { return new IntVector3(1, 1, 1); }
        }

        public static IntVector3 Right
        {
            get { return new IntVector3(1, 0, 0); }
        }

        public static IntVector3 Left
        {
            get { return new IntVector3(-1, 0, 0); }
        }

        public static IntVector3 Up
        {
            get { return new IntVector3(0, 1, 0); }
        }

        public static IntVector3 Down
        {
            get { return new IntVector3(0, -1, 0); }
        }

        public static IntVector3 Forward
        {
            get { return new IntVector3(0, 0, 1); }
        }

        public static IntVector3 Backward
        {
            get { return new IntVector3(0, 0, -1); }
        }

        public IntVector3 Normalized
        {
            get
            {
                IntVector3 vec = new IntVector3(this.x, this.y, this.z);
                vec.Normalize();
                return vec;
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", this.x, this.y, this.z);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.Equals((IntVector3)obj);
        }

        public bool Equals(IntVector3 other)
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

                return hash;
            }
        }

        public static bool operator ==(IntVector3 a, IntVector3 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(IntVector3 a, IntVector3 b)
        {
            return !a.Equals(b);
        }

        public static IntVector3 operator +(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static IntVector3 operator -(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x - b.x, a.y - b.y, a.z + b.z);
        }

        public static IntVector3 operator -(IntVector3 vec)
        {
            return new IntVector3(-vec.x, -vec.y, -vec.z);
        }

        public static IntVector3 operator *(IntVector3 vec, int scale)
        {
            return new IntVector3(vec.x * scale, vec.y * scale, vec.z * scale);
        }

        public static IntVector3 operator *(int scale, IntVector3 vec)
        {
            return new IntVector3(vec.x * scale, vec.y * scale, vec.z * scale);
        }

        public static IntVector3 operator /(IntVector3 vec, int scale)
        {
            return new IntVector3(vec.x / scale, vec.y / scale, vec.z / scale);
        }

        public static explicit operator Vector3(IntVector3 vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }
    }
}
