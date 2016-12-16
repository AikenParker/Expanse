namespace Expanse
{
    public interface IRandomNumberGenerator
    {
        /// <summary>
        /// Returns a random double between 0 and 1.
        /// </summary>
        double NextDouble();

        /// <summary>
        /// Returns a random int between min [inclusive] and max (exclusive unless min == max)
        /// </summary>
        int NextInt(int min, int max);

        /// <summary>
        /// Randomly sets the bits in a given byte array.
        /// </summary>
        /// <param name="data"></param>
        void NextBytes(byte[] data);
    }
}
