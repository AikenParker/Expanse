using System;
using UnityEngine;

namespace Expanse
{
    [Serializable]
    public struct IntVector2
    {
        public int x;
        public int y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public IntVector2(IntVector2 source)
        {
            this = source;
        }

        public IntVector2(Vector2 source, FloatToIntConversionMethod conversionMethod = FloatToIntConversionMethod.FLOOR)
        {
            switch (conversionMethod)
            {
                case FloatToIntConversionMethod.CEIL:
                    this.x = Mathf.CeilToInt(source.x);
                    this.y = Mathf.CeilToInt(source.y);
                    break;

                case FloatToIntConversionMethod.FLOOR:
                    this.x = Mathf.FloorToInt(source.x);
                    this.y = Mathf.FloorToInt(source.y);
                    break;

                case FloatToIntConversionMethod.ROUND:
                    this.x = Mathf.RoundToInt(source.x);
                    this.y = Mathf.RoundToInt(source.y);
                    break;

                default:
                    throw new ArgumentException("conversionMethod");
            }
        }

        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Normalize()
        {
            float magnitude = this.Magnitude;

            if (magnitude > 0)
            {
                this = new IntVector2(((Vector2)this / magnitude), FloatToIntConversionMethod.ROUND);
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
                return (this.x * this.x) + (this.y * this.y);
            }
        }

        public static float Distance(IntVector2 a, IntVector2 b)
        {
            return (a - b).Magnitude;
        }

        public static float Dot(IntVector2 a, IntVector2 b)
        {
            return (a.x * b.x) + (a.y * b.y);
        }

        public static float Angle(IntVector2 a, IntVector2 b)
        {
            return Mathf.Acos(Mathf.Clamp(Vector2.Dot((Vector2)a, (Vector2)b), -1f, 1f)) * Mathf.Rad2Deg;
        }

        public static IntVector2 Scale(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x * b.x, a.y * b.y);
        }

        public static IntVector2 Reflect(IntVector2 inDirection, IntVector2 inNormal)
        {
            return -2 * Mathf.RoundToInt(IntVector2.Dot(inNormal, inDirection)) * inNormal + inDirection;
        }

        public static IntVector2 Min(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
        }

        public static IntVector2 Max(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
        }

        public static IntVector2 Lerp(IntVector2 a, IntVector2 b, float t)
        {
            t = Mathf.Clamp01(t);

            Vector2 vec = new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);

            return new IntVector2(vec, FloatToIntConversionMethod.ROUND);
        }

        public static IntVector2 LerpUnclamped(IntVector2 a, IntVector2 b, float t)
        {
            Vector2 vec = new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);

            return new IntVector2(vec, FloatToIntConversionMethod.ROUND);
        }

        public static IntVector2 Zero
        {
            get { return new IntVector2(0, 0); }
        }

        public static IntVector2 One
        {
            get { return new IntVector2(1, 1); }
        }

        public static IntVector2 Right
        {
            get { return new IntVector2(1, 0); }
        }

        public static IntVector2 Left
        {
            get { return new IntVector2(-1, 0); }
        }

        public static IntVector2 Up
        {
            get { return new IntVector2(0, 1); }
        }

        public static IntVector2 Down
        {
            get { return new IntVector2(0, -1); }
        }

        public IntVector2 Normalized
        {
            get
            {
                IntVector2 vec = new IntVector2(this.x, this.y);
                vec.Normalize();
                return vec;
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", this.x, this.y);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.Equals((IntVector2)obj);
        }

        public bool Equals(IntVector2 other)
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

                return hash;
            }
        }

        public static bool operator ==(IntVector2 a, IntVector2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(IntVector2 a, IntVector2 b)
        {
            return !a.Equals(b);
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x + b.x, a.y + b.y);
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x - b.x, a.y - b.y);
        }

        public static IntVector2 operator -(IntVector2 vec)
        {
            return new IntVector2(-vec.x, -vec.y);
        }

        public static IntVector2 operator *(IntVector2 vec, int scale)
        {
            return new IntVector2(vec.x * scale, vec.y * scale);
        }

        public static IntVector2 operator *(int scale, IntVector2 vec)
        {
            return new IntVector2(vec.x * scale, vec.y * scale);
        }

        public static IntVector2 operator /(IntVector2 vec, int scale)
        {
            return new IntVector2(vec.x / scale, vec.y / scale);
        }

        public static explicit operator Vector2(IntVector2 vec)
        {
            return new Vector2(vec.x, vec.y);
        }
    }
}
