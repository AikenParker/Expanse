namespace Expanse.Random
{
    public interface IRNG
    {
        /// <summary>
        /// Returns a random double between 0 and 1.
        /// </summary>
        double NextDouble();

        /// <summary>
        /// Returns a random int between 0 [inclusive] and Int32.MAX.
        /// </summary>
        int NextInt();

        /// <summary>
        /// Returns a random int between 0 [inclusive] and max [exclusive unless min == max].
        /// </summary>
        int NextInt(int max);

        /// <summary>
        /// Returns a random int between min [inclusive] and max [exclusive unless min == max].
        /// </summary>
        int NextInt(int min, int max);

        /// <summary>
        /// Randomly sets the bits in a given byte array.
        /// </summary>
        void NextBytes(byte[] data);
    }
}
