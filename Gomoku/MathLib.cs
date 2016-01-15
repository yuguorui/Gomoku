using System;

namespace Gomoku
{


    [Serializable]
    public struct Range
    {
        private float min, max;

        /// <summary>
        /// Minimum value of the range.
        /// </summary>
        /// 
        /// <remarks><para>The property represents minimum value (left side limit) or the range -
        /// [<b>min</b>, max].</para></remarks>
        /// 
        public float Min
        {
            get { return min; }
            set { min = value; }
        }

        /// <summary>
        /// Maximum value of the range.
        /// </summary>
        /// 
        /// <remarks><para>The property represents maximum value (right side limit) or the range -
        /// [min, <b>max</b>].</para></remarks>
        /// 
        public float Max
        {
            get { return max; }
            set { max = value; }
        }

        /// <summary>
        /// Length of the range (deffirence between maximum and minimum values).
        /// </summary>
        public float Length
        {
            get { return max - min; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> structure.
        /// </summary>
        /// 
        /// <param name="min">Minimum value of the range.</param>
        /// <param name="max">Maximum value of the range.</param>
        /// 
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Check if the specified value is inside of the range.
        /// </summary>
        /// 
        /// <param name="x">Value to check.</param>
        /// 
        /// <returns><b>True</b> if the specified value is inside of the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsInside(float x)
        {
            return ((x >= min) && (x <= max));
        }

        /// <summary>
        /// Check if the specified range is inside of the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check.</param>
        /// 
        /// <returns><b>True</b> if the specified range is inside of the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsInside(Range range)
        {
            return ((IsInside(range.min)) && (IsInside(range.max)));
        }

        /// <summary>
        /// Check if the specified range overlaps with the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check for overlapping.</param>
        /// 
        /// <returns><b>True</b> if the specified range overlaps with the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsOverlapping(Range range)
        {
            return ((IsInside(range.min)) || (IsInside(range.max)) ||
                     (range.IsInside(min)) || (range.IsInside(max)));
        }

        /// <summary>
        /// Convert the signle precision range to integer range.
        /// </summary>
        /// 
        /// <param name="provideInnerRange">Specifies if inner integer range must be returned or outer range.</param>
        /// 
        /// <returns>Returns integer version of the range.</returns>
        /// 
        /// <remarks>If <paramref name="provideInnerRange"/> is set to <see langword="true"/>, then the
        /// returned integer range will always fit inside of the current single precision range.
        /// If it is set to <see langword="false"/>, then current single precision range will always
        /// fit into the returned integer range.</remarks>
        ///
        public IntRange ToIntRange(bool provideInnerRange)
        {
            int iMin, iMax;

            if (provideInnerRange)
            {
                iMin = (int)Math.Ceiling(min);
                iMax = (int)Math.Floor(max);
            }
            else
            {
                iMin = (int)Math.Floor(min);
                iMax = (int)Math.Ceiling(max);
            }

            return new IntRange(iMin, iMax);
        }

        /// <summary>
        /// Equality operator - checks if two ranges have equal min/max values.
        /// </summary>
        /// 
        /// <param name="range1">First range to check.</param>
        /// <param name="range2">Second range to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if min/max values of specified
        /// ranges are equal.</returns>
        ///
        public static bool operator ==(Range range1, Range range2)
        {
            return ((range1.min == range2.min) && (range1.max == range2.max));
        }

        /// <summary>
        /// Inequality operator - checks if two ranges have different min/max values.
        /// </summary>
        /// 
        /// <param name="range1">First range to check.</param>
        /// <param name="range2">Second range to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if min/max values of specified
        /// ranges are not equal.</returns>
        ///
        public static bool operator !=(Range range1, Range range2)
        {
            return ((range1.min != range2.min) || (range1.max != range2.max));

        }

        /// <summary>
        /// Check if this instance of <see cref="Range"/> equal to the specified one.
        /// </summary>
        /// 
        /// <param name="obj">Another range to check equalty to.</param>
        /// 
        /// <returns>Return <see langword="true"/> if objects are equal.</returns>
        /// 
        public override bool Equals(object obj)
        {
            return (obj is Range) ? (this == (Range)obj) : false;
        }

        /// <summary>
        /// Get hash code for this instance.
        /// </summary>
        /// 
        /// <returns>Returns the hash code for this instance.</returns>
        /// 
        public override int GetHashCode()
        {
            return min.GetHashCode() + max.GetHashCode();
        }

        /// <summary>
        /// Get string representation of the class.
        /// </summary>
        /// 
        /// <returns>Returns string, which contains min/max values of the range in readable form.</returns>
        ///
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", min, max);
        }
    }

    public interface IRandomNumberGenerator
    {
        /// <summary>
        /// Mean value of generator.
        /// </summary>
        /// 
        float Mean { get; }

        /// <summary>
        /// Variance value of generator.
        /// </summary>
        /// 
        float Variance { get; }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        float Next();

        /// <summary>
        /// Set seed of the random numbers generator.
        /// </summary>
        /// 
        /// <param name="seed">Seed value.</param>
        /// 
        void SetSeed(int seed);
    }

    public class ExponentialGenerator : IRandomNumberGenerator
    {
        private UniformOneGenerator rand = null;

        private float rate = 0;

        /// <summary>
        /// Rate value (inverse mean).
        /// </summary>
        /// 
        /// <remarks>The rate value should be positive and non zero.</remarks>
        /// 
        public float Rate
        {
            get { return rate; }
        }

        /// <summary>
        /// Mean value of the generator.
        /// </summary>
        /// 
        public float Mean
        {
            get { return 1.0f / rate; }
        }

        /// <summary>
        /// Variance value of the generator.
        /// </summary>
        ///
        public float Variance
        {
            get { return 1f / (rate * rate); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="rate">Rate value.</param>
        /// 
        /// <exception cref="ArgumentException">Rate value should be greater than zero.</exception>
        /// 
        public ExponentialGenerator(float rate) :
            this(rate, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="rate">Rate value (inverse mean).</param>
        /// <param name="seed">Seed value to initialize random numbers generator.</param>
        /// 
        /// <exception cref="ArgumentException">Rate value should be greater than zero.</exception>
        /// 
        public ExponentialGenerator(float rate, int seed)
        {
            // check rate value
            if (rate <= 0)
                throw new ArgumentException("Rate value should be greater than zero.");

            this.rand = new UniformOneGenerator(seed);
            this.rate = rate;
        }

        /// <summary>
        /// Generate next random number
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        public float Next()
        {
            return -(float)Math.Log(rand.Next()) / rate;
        }

        /// <summary>
        /// Set seed of the random numbers generator.
        /// </summary>
        /// 
        /// <param name="seed">Seed value.</param>
        /// 
        /// <remarks>Resets random numbers generator initializing it with
        /// specified seed value.</remarks>
        /// 
        public void SetSeed(int seed)
        {
            rand = new UniformOneGenerator(seed);
        }
    }

    public class GaussianGenerator : IRandomNumberGenerator
    {
        // standard numbers generator
        private StandardGenerator rand = null;
        // mean value
        private float mean;
        // standard deviation value
        private float stdDev;

        /// <summary>
        /// Mean value of the generator.
        /// </summary>
        ///
        public float Mean
        {
            get { return mean; }
        }

        /// <summary>
        /// Variance value of the generator.
        /// </summary>
        ///
        public float Variance
        {
            get { return stdDev * stdDev; }
        }

        /// <summary>
        /// Standard deviation value.
        /// </summary>
        ///
        public float StdDev
        {
            get { return stdDev; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="mean">Mean value.</param>
        /// <param name="stdDev">Standard deviation value.</param>
        /// 
        public GaussianGenerator(float mean, float stdDev) :
            this(mean, stdDev, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="mean">Mean value.</param>
        /// <param name="stdDev">Standard deviation value.</param>
        /// <param name="seed">Seed value to initialize random numbers generator.</param>
        /// 
        public GaussianGenerator(float mean, float stdDev, int seed)
        {
            this.mean = mean;
            this.stdDev = stdDev;

            rand = new StandardGenerator(seed);
        }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        public float Next()
        {
            return (float)rand.Next() * stdDev + mean;
        }

        /// <summary>
        /// Set seed of the random numbers generator.
        /// </summary>
        /// 
        /// <param name="seed">Seed value.</param>
        /// 
        /// <remarks>Resets random numbers generator initializing it with
        /// specified seed value.</remarks>
        /// 
        public void SetSeed(int seed)
        {
            rand = new StandardGenerator(seed);
        }
    }

    public class StandardGenerator : IRandomNumberGenerator
    {
        private UniformOneGenerator rand = null;

        private float secondValue;
        private bool useSecond = false;

        /// <summary>
        /// Mean value of the generator.
        /// </summary>
        /// 
        public float Mean
        {
            get { return 0; }
        }

        /// <summary>
        /// Variance value of the generator.
        /// </summary>
        ///
        public float Variance
        {
            get { return 1; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardGenerator"/> class.
        /// </summary>
        /// 
        public StandardGenerator()
        {
            rand = new UniformOneGenerator();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="seed">Seed value to initialize random numbers generator.</param>
        /// 
        public StandardGenerator(int seed)
        {
            rand = new UniformOneGenerator(seed);
        }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        public float Next()
        {
            // check if we can use second value
            if (useSecond)
            {
                // return the second number
                useSecond = false;
                return secondValue;
            }

            float x1, x2, w, firstValue;

            // generate new numbers
            do
            {
                x1 = (float)rand.Next() * 2.0f - 1.0f;
                x2 = (float)rand.Next() * 2.0f - 1.0f;
                w = x1 * x1 + x2 * x2;
            }
            while (w >= 1.0f);

            w = (float)Math.Sqrt((-2.0f * Math.Log(w)) / w);

            // get two standard random numbers
            firstValue = x1 * w;
            secondValue = x2 * w;

            useSecond = true;

            // return the first number
            return firstValue;
        }

        /// <summary>
        /// Set seed of the random numbers generator.
        /// </summary>
        /// 
        /// <param name="seed">Seed value.</param>
        /// 
        /// <remarks>Resets random numbers generator initializing it with
        /// specified seed value.</remarks>
        /// 
        public void SetSeed(int seed)
        {
            rand = new UniformOneGenerator(seed);
            useSecond = false;
        }
    }

    public class UniformGenerator : IRandomNumberGenerator
    {
        private UniformOneGenerator rand = null;

        // generator's range
        private float min;
        private float length;

        /// <summary>
        /// Mean value of the generator.
        /// </summary>
        ///
        public float Mean
        {
            get { return (min + min + length) / 2; }
        }

        /// <summary>
        /// Variance value of the generator.
        /// </summary>
        ///
        public float Variance
        {
            get { return length * length / 12; }
        }

        /// <summary>
        /// Random numbers range.
        /// </summary>
        /// 
        /// <remarks><para>Range of random numbers to generate. Generated numbers are
        /// greater or equal to minimum range's value and less than maximum range's
        /// value.</para>
        /// </remarks>
        /// 
        public Range Range
        {
            get { return new Range(min, min + length); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="range">Random numbers range.</param>
        /// 
        /// <remarks>Initializes random numbers generator with zero seed.</remarks>
        /// 
        public UniformGenerator(Range range) :
            this(range, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="range">Random numbers range.</param>
        /// <param name="seed">Seed value to initialize random numbers generator.</param>
        /// 
        public UniformGenerator(Range range, int seed)
        {
            rand = new UniformOneGenerator(seed);

            min = range.Min;
            length = range.Length;
        }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        public float Next()
        {
            return (float)rand.Next() * length + min;
        }

        /// <summary>
        /// Set seed of the random numbers generator.
        /// </summary>
        /// 
        /// <param name="seed">Seed value.</param>
        /// 
        /// <remarks>Resets random numbers generator initializing it with
        /// specified seed value.</remarks>
        /// 
        public void SetSeed(int seed)
        {
            rand = new UniformOneGenerator(seed);
        }
    }

    public class UniformOneGenerator : IRandomNumberGenerator
    {
        // .NET random generator as a base
        private ThreadSafeRandom rand = null;

        /// <summary>
        /// Mean value of the generator.
        /// </summary>
        ///
        public float Mean
        {
            get { return 0.5f; }
        }

        /// <summary>
        /// Variance value of the generator.
        /// </summary>
        ///
        public float Variance
        {
            get { return 1f / 12; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformOneGenerator"/> class.
        /// </summary>
        /// 
        /// <remarks>Initializes random numbers generator with zero seed.</remarks>
        /// 
        public UniformOneGenerator()
        {
            rand = new ThreadSafeRandom(0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformOneGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="seed">Seed value to initialize random numbers generator.</param>
        /// 
        public UniformOneGenerator(int seed)
        {
            rand = new ThreadSafeRandom(seed);
        }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        public float Next()
        {
            return (float)rand.NextDouble();
        }

        /// <summary>
        /// Set seed of the random numbers generator.
        /// </summary>
        /// 
        /// <param name="seed">Seed value.</param>
        /// 
        /// <remarks>Resets random numbers generator initializing it with
        /// specified seed value.</remarks>
        /// 
        public void SetSeed(int seed)
        {
            rand = new ThreadSafeRandom(seed);
        }
    }

    public sealed class ThreadSafeRandom : Random
    {
        private object sync = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeRandom"/> class.
        /// </summary>
        /// 
        /// <remarks>See <see cref="Random.Next()"/> for more information.</remarks>
        /// 
        public ThreadSafeRandom()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeRandom"/> class.
        /// </summary>
        /// 
        /// <remarks>A number used to calculate a starting value for the pseudo-random number sequence.
        /// If a negative number is specified, the absolute value of the number is used.</remarks>
        /// 
        /// 
        /// <remarks>See <see cref="Random.Next()"/> for more information.</remarks>
        /// 
        public ThreadSafeRandom(int seed)
            : base(seed)
        {
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// 
        /// <returns>Returns a 32-bit signed integer greater than or equal to zero and less than
        /// <see cref="int.MaxValue"/>.</returns>
        /// 
        /// <remarks>See <see cref="Random.Next()"/> for more information.</remarks>
        /// 
        public override int Next()
        {
            lock (sync) return base.Next();
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// 
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated.
        /// <paramref name="maxValue"/> must be greater than or equal to zero.</param>
        /// 
        /// <returns>Returns a 32-bit signed integer greater than or equal to zero, and less than <paramref name="maxValue"/>;
        /// that is, the range of return values ordinarily includes zero but not <paramref name="maxValue"/>.</returns>
        /// 
        /// <remarks>See <see cref="Random.Next(int)"/> for more information.</remarks>
        /// 
        public override int Next(int maxValue)
        {
            lock (sync) return base.Next(maxValue);
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// 
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.
        /// <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.</param>
        /// 
        /// <returns>Returns a 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less
        /// than <paramref name="maxValue"/>; that is, the range of return values includes
        /// <paramref name="minValue"/> but not <paramref name="maxValue"/>.</returns>
        /// 
        /// <remarks>See <see cref="Random.Next(int,int)"/> for more information.</remarks>
        ///
        public override int Next(int minValue, int maxValue)
        {
            lock (sync) return base.Next(minValue, maxValue);
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// 
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        /// 
        /// <remarks>See <see cref="Random.NextBytes(byte[])"/> for more information.</remarks>
        ///
        public override void NextBytes(byte[] buffer)
        {
            lock (sync) base.NextBytes(buffer);
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// 
        /// <returns>Returns a double-precision floating point number greater than or equal to 0.0, and less than 1.0.</returns>
        /// 
        /// <remarks>See <see cref="Random.NextDouble()"/> for more information.</remarks>
        ///
        public override double NextDouble()
        {
            lock (sync) return base.NextDouble();
        }
    }
}