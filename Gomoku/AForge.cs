namespace Gomoku
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	public interface IChromosome : IComparable
	{
		/// <summary>
		/// Chromosome's fintess value.
		/// </summary>
		/// 
		/// <remarks><para>The fitness value represents chromosome's usefulness - the greater the
		/// value, the more useful it.</para></remarks>
		/// 
		double Fitness { get; }

		/// <summary>
		/// Generate random chromosome value.
		/// </summary>
		/// 
		/// <remarks><para>Regenerates chromosome's value using random number generator.</para>
		/// </remarks>
		/// 
		void Generate();

		/// <summary>
		/// Create new random chromosome with same parameters (factory method).
		/// </summary>
		/// 
		/// <remarks><para>The method creates new chromosome of the same type, but randomly
		/// initialized. The method is useful as factory method for those classes, which work
		/// with chromosome's interface, but not with particular chromosome class.</para></remarks>
		/// 
		IChromosome CreateNew();

		/// <summary>
		/// Clone the chromosome.
		/// </summary>
		/// 
		/// <remarks><para>The method clones the chromosome returning the exact copy of it.</para>
		/// </remarks>
		/// 
		IChromosome Clone();

		/// <summary>
		/// Mutation operator.
		/// </summary>
		/// 
		/// <remarks><para>The method performs chromosome's mutation, changing its part randomly.</para></remarks>
		/// 
		void Mutate();

		/// <summary>
		/// Crossover operator.
		/// </summary>
		/// 
		/// <param name="pair">Pair chromosome to crossover with.</param>
		/// 
		/// <remarks><para>The method performs crossover between two chromosomes – interchanging some parts of chromosomes.</para></remarks>
		/// 
		void Crossover(IChromosome pair);

		/// <summary>
		/// Evaluate chromosome with specified fitness function.
		/// </summary>
		/// 
		/// <param name="function">Fitness function to use for evaluation of the chromosome.</param>
		/// 
		/// <remarks><para>Calculates chromosome's fitness using the specifed fitness function.</para></remarks>
		/// 
	}

	public abstract class ChromosomeBase : IChromosome
	{
		/// <summary>
		/// Chromosome's fintess value.
		/// </summary>
		protected double fitness = 0;

		/// <summary>
		/// Chromosome's fintess value.
		/// </summary>
		/// 
		/// <remarks><para>Fitness value (usefulness) of the chromosome calculate by calling
		/// <see cref="Evaluate"/> method. The greater the value, the more useful the chromosome.
		/// </para></remarks>
		/// 
		public double Fitness
		{
			get { return fitness; }
		}

		/// <summary>
		/// Generate random chromosome value.
		/// </summary>
		/// 
		/// <remarks><para>Regenerates chromosome's value using random number generator.</para>
		/// </remarks>
		/// 
		public abstract void Generate();

		/// <summary>
		/// Create new random chromosome with same parameters (factory method).
		/// </summary>
		/// 
		/// <remarks><para>The method creates new chromosome of the same type, but randomly
		/// initialized. The method is useful as factory method for those classes, which work
		/// with chromosome's interface, but not with particular chromosome class.</para></remarks>
		/// 
		public abstract IChromosome CreateNew();

		/// <summary>
		/// Clone the chromosome.
		/// </summary>
		/// 
		/// <remarks><para>The method clones the chromosome returning the exact copy of it.</para>
		/// </remarks>
		/// 
		public abstract IChromosome Clone();

		/// <summary>
		/// Mutation operator.
		/// </summary>
		/// 
		/// <remarks><para>The method performs chromosome's mutation, changing its part randomly.</para></remarks>
		/// 
		public abstract void Mutate();

		/// <summary>
		/// Crossover operator.
		/// </summary>
		/// 
		/// <param name="pair">Pair chromosome to crossover with.</param>
		/// 
		/// <remarks><para>The method performs crossover between two chromosomes – interchanging some parts of chromosomes.</para></remarks>
		/// 
		public abstract void Crossover(IChromosome pair);

		/// <summary>
		/// Evaluate chromosome with specified fitness function.
		/// </summary>
		/// 
		/// <param name="function">Fitness function to use for evaluation of the chromosome.</param>
		/// 
		/// <remarks><para>Calculates chromosome's fitness using the specifed fitness function.</para></remarks>
		///

		/// <summary>
		/// Compare two chromosomes.
		/// </summary>
		/// 
		/// <param name="o">Binary chromosome to compare to.</param>
		/// 
		/// <returns>Returns comparison result, which equals to 0 if fitness values
		/// of both chromosomes are equal, 1 if fitness value of this chromosome
		/// is less than fitness value of the specified chromosome, -1 otherwise.</returns>
		/// 
		public int CompareTo(object o)
		{
			double f = ((ChromosomeBase)o).fitness;

			return (fitness == f) ? 0 : (fitness < f) ? 1 : -1;
		}
	}

	public class DoubleArrayChromosome : ChromosomeBase
	{
		/// <summary>
		/// Chromosome generator.
		/// </summary>
		/// 
		/// <remarks><para>This random number generator is used to initialize chromosome's genes,
		/// which is done by calling <see cref="Generate"/> method.</para></remarks>
		/// 
		protected IRandomNumberGenerator chromosomeGenerator;

		/// <summary>
		/// Mutation multiplier generator.
		/// </summary>
		/// 
		/// <remarks><para>This random number generator is used to generate random multiplier values,
		/// which are used to multiply chromosome's genes during mutation.</para></remarks>
		/// 
		protected IRandomNumberGenerator mutationMultiplierGenerator;

		/// <summary>
		/// Mutation addition generator.
		/// </summary>
		/// 
		/// <remarks><para>This random number generator is used to generate random addition values,
		/// which are used to add to chromosome's genes during mutation.</para></remarks>
		/// 
		protected IRandomNumberGenerator mutationAdditionGenerator;

		/// <summary>
		/// Random number generator for crossover and mutation points selection.
		/// </summary>
		/// 
		/// <remarks><para>This random number generator is used to select crossover
		/// and mutation points.</para></remarks>
		/// 
		protected static ThreadSafeRandom rand = new ThreadSafeRandom();

		/// <summary>
		/// Chromosome's maximum length.
		/// </summary>
		/// 
		/// <remarks><para>Maxim chromosome's length in array elements.</para></remarks>
		///
		/* -------------------------MODIFIED BY YUGUORUI------------------------------ */ 
		public const int MaxLength = 65536*2;
		/* -----------------------------MODIFTED END---------------------------------- */

		/// <summary>
		/// Chromosome's length in number of elements.
		/// </summary>
		private int length;

		/// <summary>
		/// Chromosome's value.
		/// </summary>
		protected double[] val = null;

		// balancers to control type of mutation and crossover
		private double mutationBalancer = 0.5;
		private double crossoverBalancer = 0.5;

		/// <summary>
		/// Chromosome's length.
		/// </summary>
		/// 
		/// <remarks><para>Length of the chromosome in array elements.</para></remarks>
		///
		public int Length
		{
			get { return length; }
		}

		/// <summary>
		/// Chromosome's value.
		/// </summary>
		/// 
		/// <remarks><para>Current value of the chromosome.</para></remarks>
		///
		public double[] Value
		{
			get { return val; }
			/* ------------------MODIFIED BY YUGUORUI------------------------------ */
			set { val = value; }
			/* ---------------------MODIFIED END----------------------------------- */
		}

		/// <summary>
		/// Mutation balancer to control mutation type, [0, 1].
		/// </summary>
		/// 
		/// <remarks><para>The property controls type of mutation, which is used more
		/// frequently. A radnom number is generated each time before doing mutation -
		/// if the random number is smaller than the specified balance value, then one
		/// mutation type is used, otherwse another. See <see cref="Mutate"/> method
		/// for more information.</para>
		/// 
		/// <para>Default value is set to <b>0.5</b>.</para>
		/// </remarks>
		/// 
		public double MutationBalancer
		{
			get { return mutationBalancer; }
			set { mutationBalancer = Math.Max(0.0, Math.Min(1.0, value)); }
		}

		/// <summary>
		/// Crossover balancer to control crossover type, [0, 1].
		/// </summary>
		/// 
		/// <remarks><para>The property controls type of crossover, which is used more
		/// frequently. A radnom number is generated each time before doing crossover -
		/// if the random number is smaller than the specified balance value, then one
		/// crossover type is used, otherwse another. See <see cref="Crossover"/> method
		/// for more information.</para>
		/// 
		/// <para>Default value is set to <b>0.5</b>.</para>
		/// </remarks>
		/// 
		public double CrossoverBalancer
		{
			get { return crossoverBalancer; }
			set { crossoverBalancer = Math.Max(0.0, Math.Min(1.0, value)); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DoubleArrayChromosome"/> class.
		/// </summary>
		/// 
		/// <param name="chromosomeGenerator">Chromosome generator - random number generator, which is 
		/// used to initialize chromosome's genes, which is done by calling <see cref="Generate"/> method
		/// or in class constructor.</param>
		/// <param name="mutationMultiplierGenerator">Mutation multiplier generator - random number
		/// generator, which is used to generate random multiplier values, which are used to
		/// multiply chromosome's genes during mutation.</param>
		/// <param name="mutationAdditionGenerator">Mutation addition generator - random number
		/// generator, which is used to generate random addition values, which are used to
		/// add to chromosome's genes during mutation.</param>
		/// <param name="length">Chromosome's length in array elements, [2, <see cref="MaxLength"/>].</param>
		/// 
		/// <remarks><para>The constructor initializes the new chromosome randomly by calling
		/// <see cref="Generate"/> method.</para></remarks>
		/// 
		public DoubleArrayChromosome(
			IRandomNumberGenerator chromosomeGenerator,
			IRandomNumberGenerator mutationMultiplierGenerator,
			IRandomNumberGenerator mutationAdditionGenerator,
			int length)
		{

			// save parameters
			this.chromosomeGenerator = chromosomeGenerator;
			this.mutationMultiplierGenerator = mutationMultiplierGenerator;
			this.mutationAdditionGenerator = mutationAdditionGenerator;
			this.length = Math.Max(2, Math.Min(MaxLength, length)); ;

			// allocate array
			val = new double[length];

			// generate random chromosome
			Generate();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DoubleArrayChromosome"/> class.
		/// </summary>
		/// 
		/// <param name="chromosomeGenerator">Chromosome generator - random number generator, which is 
		/// used to initialize chromosome's genes, which is done by calling <see cref="Generate"/> method
		/// or in class constructor.</param>
		/// <param name="mutationMultiplierGenerator">Mutation multiplier generator - random number
		/// generator, which is used to generate random multiplier values, which are used to
		/// multiply chromosome's genes during mutation.</param>
		/// <param name="mutationAdditionGenerator">Mutation addition generator - random number
		/// generator, which is used to generate random addition values, which are used to
		/// add to chromosome's genes during mutation.</param>
		/// <param name="values">Values used to initialize the chromosome.</param>
		/// 
		/// <remarks><para>The constructor initializes the new chromosome with specified <paramref name="values">values</paramref>.
		/// </para></remarks>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">Invalid length of values array.</exception>
		/// 
		public DoubleArrayChromosome(
			IRandomNumberGenerator chromosomeGenerator,
			IRandomNumberGenerator mutationMultiplierGenerator,
			IRandomNumberGenerator mutationAdditionGenerator,
			double[] values)
		{
			if ((values.Length < 2) || (values.Length > MaxLength))
				throw new ArgumentOutOfRangeException("Invalid length of values array.");

			// save parameters
			this.chromosomeGenerator = chromosomeGenerator;
			this.mutationMultiplierGenerator = mutationMultiplierGenerator;
			this.mutationAdditionGenerator = mutationAdditionGenerator;
			this.length = values.Length;

			// copy specified values
			val = (double[])values.Clone();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DoubleArrayChromosome"/> class.
		/// </summary>
		/// 
		/// <param name="source">Source chromosome to copy.</param>
		/// 
		/// <remarks><para>This is a copy constructor, which creates the exact copy
		/// of specified chromosome.</para></remarks>
		/// 
		public DoubleArrayChromosome(DoubleArrayChromosome source)
		{
			this.chromosomeGenerator = source.chromosomeGenerator;
			this.mutationMultiplierGenerator = source.mutationMultiplierGenerator;
			this.mutationAdditionGenerator = source.mutationAdditionGenerator;
			this.length = source.length;
			this.fitness = source.fitness;
			this.mutationBalancer = source.mutationBalancer;
			this.crossoverBalancer = source.crossoverBalancer;

			// copy genes
			val = (double[])source.val.Clone();
		}

		/// <summary>
		/// Get string representation of the chromosome.
		/// </summary>
		/// 
		/// <returns>Returns string representation of the chromosome.</returns>
		/// 
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			// append first gene
			sb.Append(val[0]);
			// append all other genes
			for (int i = 1; i < length; i++)
			{
				sb.Append(' ');
				sb.Append(val[i]);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Generate random chromosome value.
		/// </summary>
		/// 
		/// <remarks><para>Regenerates chromosome's value using random number generator.</para>
		/// </remarks>
		///
		public override void Generate()
		{
			for (int i = 0; i < length; i++)
			{
				// generate next value
				val[i] = chromosomeGenerator.Next();
			}
		}

		/// <summary>
		/// Create new random chromosome with same parameters (factory method).
		/// </summary>
		/// 
		/// <remarks><para>The method creates new chromosome of the same type, but randomly
		/// initialized. The method is useful as factory method for those classes, which work
		/// with chromosome's interface, but not with particular chromosome type.</para></remarks>
		///
		public override IChromosome CreateNew()
		{
			return new DoubleArrayChromosome(chromosomeGenerator, mutationMultiplierGenerator, mutationAdditionGenerator, length);
		}

		/// <summary>
		/// Clone the chromosome.
		/// </summary>
		/// 
		/// <returns>Return's clone of the chromosome.</returns>
		/// 
		/// <remarks><para>The method clones the chromosome returning the exact copy of it.</para>
		/// </remarks>
		///
		public override IChromosome Clone()
		{
			return new DoubleArrayChromosome(this);
		}

		/// <summary>
		/// Mutation operator.
		/// </summary>
		/// 
		/// <remarks><para>The method performs chromosome's mutation, adding random number
		/// to chromosome's gene or multiplying the gene by random number. These random
		/// numbers are generated with help of <see cref="mutationMultiplierGenerator">mutation
		/// multiplier</see> and <see cref="mutationAdditionGenerator">mutation
		/// addition</see> generators.</para>
		/// 
		/// <para>The exact type of mutation applied to the particular gene
		/// is selected randomly each time and depends on <see cref="MutationBalancer"/>.
		/// Before mutation is done a random number is generated in [0, 1] range - if the
		/// random number is smaller than <see cref="MutationBalancer"/>, then multiplication
		/// mutation is done, otherwise addition mutation.
		/// </para></remarks>
		/// 
		public override void Mutate()
		{
			int mutationGene = rand.Next(length);

			if (rand.NextDouble() < mutationBalancer)
			{
				val[mutationGene] *= mutationMultiplierGenerator.Next();
			}
			else
			{
				val[mutationGene] += mutationAdditionGenerator.Next();
			}
		}

		/// <summary>
		/// Crossover operator.
		/// </summary>
		/// 
		/// <param name="pair">Pair chromosome to crossover with.</param>
		/// 
		/// <remarks><para>The method performs crossover between two chromosomes, selecting
		/// randomly the exact type of crossover to perform, which depends on <see cref="CrossoverBalancer"/>.
		/// Before crossover is done a random number is generated in [0, 1] range - if the
		/// random number is smaller than <see cref="CrossoverBalancer"/>, then the first crossover
		/// type is used, otherwise second type is used.</para>
		/// 
		/// <para>The <b>first crossover type</b> is based on interchanging
		/// range of genes (array elements) between these chromosomes and is known
		/// as one point crossover. A crossover point is selected randomly and chromosomes
		/// interchange genes, which start from the selected point.</para>
		/// 
		/// <para>The <b>second crossover type</b> is aimed to produce one child, which genes'
		/// values are between corresponding genes of parents, and another child, which genes'
		/// values are outside of the range formed by corresponding genes of parents. 
		/// Let take, for example, two genes with 1.0 and 3.0 valueû (of course chromosomes have
		/// more genes, but for simplicity lets think about one). First of all we randomly choose
		/// a factor in the [0, 1] range, let's take 0.4. Then, for each pair of genes (we have
		/// one pair) we calculate difference value, which is 2.0 in our case. In the result we’ll
		/// have two children – one between and one outside of the range formed by parents genes' values.
		/// We may have 1.8 and 3.8 children, or we may have 0.2 and 2.2 children. As we can see
		/// we add/subtract (chosen randomly) <i>difference * factor</i>. So, this gives us exploration
		/// in between and in near outside. The randomly chosen factor is applied to all genes
		/// of the chromosomes participating in crossover.</para>
		/// </remarks>
		///
		public override void Crossover(IChromosome pair)
		{
			DoubleArrayChromosome p = (DoubleArrayChromosome)pair;

			// check for correct pair
			if ((p != null) && (p.length == length))
			{
				if (rand.NextDouble() < crossoverBalancer)
				{
					// crossover point
					int crossOverPoint = rand.Next(length - 1) + 1;
					// length of chromosome to be crossed
					int crossOverLength = length - crossOverPoint;
					// temporary array
					double[] temp = new double[crossOverLength];

					// copy part of first (this) chromosome to temp
					Array.Copy(val, crossOverPoint, temp, 0, crossOverLength);
					// copy part of second (pair) chromosome to the first
					Array.Copy(p.val, crossOverPoint, val, crossOverPoint, crossOverLength);
					// copy temp to the second
					Array.Copy(temp, 0, p.val, crossOverPoint, crossOverLength);
				}
				else
				{
					double[] pairVal = p.val;

					double factor = rand.NextDouble();
					if (rand.Next(2) == 0)
						factor = -factor;

					for (int i = 0; i < length; i++)
					{
						double portion = (val[i] - pairVal[i]) * factor;

						val[i] -= portion;
						pairVal[i] += portion;
					}
				}
			}
		}
	}

	[Serializable]
	public abstract class Neuron
	{
		/// <summary>
		/// Neuron's inputs count.
		/// </summary>
		protected int inputsCount = 0;

		/// <summary>
		/// Nouron's wieghts.
		/// </summary>
		protected double[] weights = null;

		/// <summary>
		/// Neuron's output value.
		/// </summary>
		protected double output = 0;

		/// <summary>
		/// Random number generator.
		/// </summary>
		/// 
		/// <remarks>The generator is used for neuron's weights randomization.</remarks>
		/// 
		protected static ThreadSafeRandom rand = new ThreadSafeRandom();

		/// <summary>
		/// Random generator range.
		/// </summary>
		/// 
		/// <remarks>Sets the range of random generator. Affects initial values of neuron's weight.
		/// Default value is [0, 1].</remarks>
		/// 

		// --------------------MODIFIED BY YUGUORUI---------------------------------------------- //
		protected static Range randRange = new Range(0.0f, 1.0f);
		//protected static Range randRange = new Range(-5f,5f);
		// --------------------MODIFIED END------------------------------------------------------ //
		/// <summary>
		/// Random number generator.
		/// </summary>
		/// 
		/// <remarks>The property allows to initialize random generator with a custom seed. The generator is
		/// used for neuron's weights randomization.</remarks>
		/// 
		public static ThreadSafeRandom RandGenerator
		{
			get { return rand; }
			set
			{
				if (value != null)
				{
					rand = value;
				}
			}
		}

		/// <summary>
		/// Random generator range.
		/// </summary>
		/// 
		/// <remarks>Sets the range of random generator. Affects initial values of neuron's weight.
		/// Default value is [0, 1].</remarks>
		/// 
		public static Range RandRange
		{
			get { return randRange; }
			set { randRange = value; }
		}

		/// <summary>
		/// Neuron's inputs count.
		/// </summary>
		public int InputsCount
		{
			get { return inputsCount; }
		}

		/// <summary>
		/// Neuron's output value.
		/// </summary>
		/// 
		/// <remarks>The calculation way of neuron's output value is determined by inherited class.</remarks>
		/// 
		public double Output
		{
			get { return output; }
		}


		/// <summary>
		/// Neuron's weights.
		/// </summary>
		public double[] Weights
		{
			get { return weights; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Neuron"/> class.
		/// </summary>
		///
		/// <param name="inputs">Neuron's inputs count.</param>
		/// 
		/// <remarks>The new neuron will be randomized (see <see cref="Randomize"/> method)
		/// after it is created.</remarks>
		///
		protected Neuron(int inputs)
		{
			// allocate weights
			inputsCount = Math.Max(1, inputs);
			weights = new double[inputsCount];
			// randomize the neuron
			Randomize();
		}

		/// <summary>
		/// Randomize neuron.
		/// </summary>
		/// 
		/// <remarks>Initialize neuron's weights with random values within the range specified
		/// by <see cref="RandRange"/>.</remarks>
		/// 
		public virtual void Randomize()
		{
			double d = randRange.Length;

			// randomize weights
			for (int i = 0; i < inputsCount; i++)
				weights[i] = rand.NextDouble() * d + randRange.Min;
		}

		/// <summary>
		/// Computes output value of neuron.
		/// </summary>
		/// 
		/// <param name="input">Input vector.</param>
		/// 
		/// <returns>Returns neuron's output value.</returns>
		/// 
		/// <remarks>The actual neuron's output value is determined by inherited class.
		/// The output value is also stored in <see cref="Output"/> property.</remarks>
		/// 
		public abstract double Compute(double[] input);
	}

	public interface IActivationFunction
	{
		/// <summary>
		/// Calculates function value.
		/// </summary>
		///
		/// <param name="x">Function input value.</param>
		/// 
		/// <returns>Function output value, <i>f(x)</i>.</returns>
		///
		/// <remarks>The method calculates function value at point <paramref name="x"/>.</remarks>
		///
		double Function(double x);

		/// <summary>
		/// Calculates function derivative.
		/// </summary>
		/// 
		/// <param name="x">Function input value.</param>
		/// 
		/// <returns>Function derivative, <i>f'(x)</i>.</returns>
		/// 
		/// <remarks>The method calculates function derivative at point <paramref name="x"/>.</remarks>
		///
		double Derivative(double x);

		/// <summary>
		/// Calculates function derivative.
		/// </summary>
		/// 
		/// <param name="y">Function output value - the value, which was obtained
		/// with the help of <see cref="Function"/> method.</param>
		/// 
		/// <returns>Function derivative, <i>f'(x)</i>.</returns>
		/// 
		/// <remarks><para>The method calculates the same derivative value as the
		/// <see cref="Derivative"/> method, but it takes not the input <b>x</b> value
		/// itself, but the function value, which was calculated previously with
		/// the help of <see cref="Function"/> method.</para>
		/// 
		/// <para><note>Some applications require as function value, as derivative value,
		/// so they can save the amount of calculations using this method to calculate derivative.</note></para>
		/// </remarks>
		/// 
		double Derivative2(double y);
	}

	[Serializable]
	public class ActivationNeuron : Neuron
	{
		/// <summary>
		/// Threshold value.
		/// </summary>
		/// 
		/// <remarks>The value is added to inputs weighted sum before it is passed to activation
		/// function.</remarks>
		/// 
		protected double threshold = 0.0;

		/// <summary>
		/// Activation function.
		/// </summary>
		/// 
		/// <remarks>The function is applied to inputs weighted sum plus
		/// threshold value.</remarks>
		/// 
		protected IActivationFunction function = null;

		/// <summary>
		/// Threshold value.
		/// </summary>
		/// 
		/// <remarks>The value is added to inputs weighted sum before it is passed to activation
		/// function.</remarks>
		/// 
		public double Threshold
		{
			get { return threshold; }
			set { threshold = value; }
		}

		/// <summary>
		/// Neuron's activation function.
		/// </summary>
		/// 
		public IActivationFunction ActivationFunction
		{
			get { return function; }
			set { function = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActivationNeuron"/> class.
		/// </summary>
		/// 
		/// <param name="inputs">Neuron's inputs count.</param>
		/// <param name="function">Neuron's activation function.</param>
		/// 
		public ActivationNeuron(int inputs, IActivationFunction function)
			: base(inputs)
		{
			this.function = function;
		}

		/// <summary>
		/// Randomize neuron.
		/// </summary>
		/// 
		/// <remarks>Calls base class <see cref="Neuron.Randomize">Randomize</see> method
		/// to randomize neuron's weights and then randomizes threshold's value.</remarks>
		/// 
		public override void Randomize()
		{
			// randomize weights
			base.Randomize();
			// randomize threshold
			threshold = rand.NextDouble() * (randRange.Length) + randRange.Min;
		}

		/// <summary>
		/// Computes output value of neuron.
		/// </summary>
		/// 
		/// <param name="input">Input vector.</param>
		/// 
		/// <returns>Returns neuron's output value.</returns>
		/// 
		/// <remarks><para>The output value of activation neuron is equal to value
		/// of nueron's activation function, which parameter is weighted sum
		/// of its inputs plus threshold value. The output value is also stored
		/// in <see cref="Neuron.Output">Output</see> property.</para>
		/// 
		/// <para><note>The method may be called safely from multiple threads to compute neuron's
		/// output value for the specified input values. However, the value of
		/// <see cref="Neuron.Output"/> property in multi-threaded environment is not predictable,
		/// since it may hold neuron's output computed from any of the caller threads. Multi-threaded
		/// access to the method is useful in those cases when it is required to improve performance
		/// by utilizing several threads and the computation is based on the immediate return value
		/// of the method, but not on neuron's output property.</note></para>
		/// </remarks>
		/// 
		/// <exception cref="ArgumentException">Wrong length of the input vector, which is not
		/// equal to the <see cref="Neuron.InputsCount">expected value</see>.</exception>
		/// 
		public override double Compute(double[] input)
		{
			// check for corrent input vector
			if (input.Length != inputsCount)
				throw new ArgumentException("Wrong length of the input vector.");

			// initial sum value
			double sum = 0.0;

			// compute weighted sum of inputs
			for (int i = 0; i < weights.Length; i++)
			{
				sum += weights[i] * input[i];
			}
			sum += threshold;

			// local variable to avoid mutlithreaded conflicts
			double output = function.Function(sum);
			// assign output property as well (works correctly for single threaded usage)
			this.output = output;

			return output;
		}
	}

	[Serializable]
	public abstract class Layer
	{
		/// <summary>
		/// Layer's inputs count.
		/// </summary>
		protected int inputsCount = 0;

		/// <summary>
		/// Layer's neurons count.
		/// </summary>
		protected int neuronsCount = 0;

		/// <summary>
		/// Layer's neurons.
		/// </summary>
		protected Neuron[] neurons;

		/// <summary>
		/// Layer's output vector.
		/// </summary>
		protected double[] output;

		/// <summary>
		/// Layer's inputs count.
		/// </summary>
		public int InputsCount
		{
			get { return inputsCount; }
		}

		/// <summary>
		/// Layer's neurons.
		/// </summary>
		/// 
		public Neuron[] Neurons
		{
			get { return neurons; }
		}

		/// <summary>
		/// Layer's output vector.
		/// </summary>
		/// 
		/// <remarks><para>The calculation way of layer's output vector is determined by neurons,
		/// which comprise the layer.</para>
		/// 
		/// <para><note>The property is not initialized (equals to <see langword="null"/>) until
		/// <see cref="Compute"/> method is called.</note></para>
		/// </remarks>
		/// 
		public double[] Output
		{
			get { return output; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Layer"/> class.
		/// </summary>
		/// 
		/// <param name="neuronsCount">Layer's neurons count.</param>
		/// <param name="inputsCount">Layer's inputs count.</param>
		/// 
		/// <remarks>Protected contructor, which initializes <see cref="inputsCount"/>,
		/// <see cref="neuronsCount"/> and <see cref="neurons"/> members.</remarks>
		/// 
		protected Layer(int neuronsCount, int inputsCount)
		{
			this.inputsCount = Math.Max(1, inputsCount);
			this.neuronsCount = Math.Max(1, neuronsCount);
			// create collection of neurons
			neurons = new Neuron[this.neuronsCount];
		}

		/// <summary>
		/// Compute output vector of the layer.
		/// </summary>
		/// 
		/// <param name="input">Input vector.</param>
		/// 
		/// <returns>Returns layer's output vector.</returns>
		/// 
		/// <remarks><para>The actual layer's output vector is determined by neurons,
		/// which comprise the layer - consists of output values of layer's neurons.
		/// The output vector is also stored in <see cref="Output"/> property.</para>
		/// 
		/// <para><note>The method may be called safely from multiple threads to compute layer's
		/// output value for the specified input values. However, the value of
		/// <see cref="Output"/> property in multi-threaded environment is not predictable,
		/// since it may hold layer's output computed from any of the caller threads. Multi-threaded
		/// access to the method is useful in those cases when it is required to improve performance
		/// by utilizing several threads and the computation is based on the immediate return value
		/// of the method, but not on layer's output property.</note></para>
		/// </remarks>
		/// 
		public virtual double[] Compute(double[] input)
		{
			// local variable to avoid mutlithread conflicts
			double[] output = new double[neuronsCount];

			// compute each neuron
			for (int i = 0; i < neurons.Length; i++)
				output[i] = neurons[i].Compute(input);

			// assign output property as well (works correctly for single threaded usage)
			this.output = output;

			return output;
		}

		/// <summary>
		/// Randomize neurons of the layer.
		/// </summary>
		/// 
		/// <remarks>Randomizes layer's neurons by calling <see cref="Neuron.Randomize"/> method
		/// of each neuron.</remarks>
		/// 
		public virtual void Randomize()
		{
			foreach (Neuron neuron in neurons)
				neuron.Randomize();
		}
	}

	[Serializable]
	public class ActivationLayer : Layer
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="ActivationLayer"/> class.
		/// </summary>
		/// 
		/// <param name="neuronsCount">Layer's neurons count.</param>
		/// <param name="inputsCount">Layer's inputs count.</param>
		/// <param name="function">Activation function of neurons of the layer.</param>
		/// 
		/// <remarks>The new layer is randomized (see <see cref="ActivationNeuron.Randomize"/>
		/// method) after it is created.</remarks>
		/// 
		public ActivationLayer(int neuronsCount, int inputsCount, IActivationFunction function)
			: base(neuronsCount, inputsCount)
		{
			// create each neuron
			for (int i = 0; i < neurons.Length; i++)
				neurons[i] = new ActivationNeuron(inputsCount, function);
		}

		/// <summary>
		/// Set new activation function for all neurons of the layer.
		/// </summary>
		/// 
		/// <param name="function">Activation function to set.</param>
		/// 
		/// <remarks><para>The methods sets new activation function for each neuron by setting
		/// their <see cref="ActivationNeuron.ActivationFunction"/> property.</para></remarks>
		/// 
		public void SetActivationFunction(IActivationFunction function)
		{
			for (int i = 0; i < neurons.Length; i++)
			{
				((ActivationNeuron)neurons[i]).ActivationFunction = function;
			}
		}
	}

	[Serializable]
	public abstract class Network
	{
		/// <summary>
		/// Network's inputs count.
		/// </summary>
		protected int inputsCount;

		/// <summary>
		/// Network's layers count.
		/// </summary>
		protected int layersCount;

		/// <summary>
		/// Network's layers.
		/// </summary>
		protected Layer[] layers;

		/// <summary>
		/// Network's output vector.
		/// </summary>
		protected double[] output;

		/// <summary>
		/// Network's inputs count.
		/// </summary>
		public int InputsCount
		{
			get { return inputsCount; }
		}

		/// <summary>
		/// Network's layers.
		/// </summary>
		public Layer[] Layers
		{
			get { return layers; }
		}

		/// <summary>
		/// Network's output vector.
		/// </summary>
		/// 
		/// <remarks><para>The calculation way of network's output vector is determined by
		/// layers, which comprise the network.</para>
		/// 
		/// <para><note>The property is not initialized (equals to <see langword="null"/>) until
		/// <see cref="Compute"/> method is called.</note></para>
		/// </remarks>
		/// 
		public double[] Output
		{
			get { return output; }
		}



		/// <summary>
		/// Initializes a new instance of the <see cref="Network"/> class.
		/// </summary>
		/// 
		/// <param name="inputsCount">Network's inputs count.</param>
		/// <param name="layersCount">Network's layers count.</param>
		/// 
		/// <remarks>Protected constructor, which initializes <see cref="inputsCount"/>,
		/// <see cref="layersCount"/> and <see cref="layers"/> members.</remarks>
		/// 
		protected Network(int inputsCount, int layersCount)
		{
			this.inputsCount = Math.Max(1, inputsCount);
			this.layersCount = Math.Max(1, layersCount);
			// create collection of layers
			this.layers = new Layer[this.layersCount];
		}

		/// <summary>
		/// Compute output vector of the network.
		/// </summary>
		/// 
		/// <param name="input">Input vector.</param>
		/// 
		/// <returns>Returns network's output vector.</returns>
		/// 
		/// <remarks><para>The actual network's output vecor is determined by layers,
		/// which comprise the layer - represents an output vector of the last layer
		/// of the network. The output vector is also stored in <see cref="Output"/> property.</para>
		/// 
		/// <para><note>The method may be called safely from multiple threads to compute network's
		/// output value for the specified input values. However, the value of
		/// <see cref="Output"/> property in multi-threaded environment is not predictable,
		/// since it may hold network's output computed from any of the caller threads. Multi-threaded
		/// access to the method is useful in those cases when it is required to improve performance
		/// by utilizing several threads and the computation is based on the immediate return value
		/// of the method, but not on network's output property.</note></para>
		/// </remarks>
		/// 
		public virtual double[] Compute(double[] input)
		{
			// local variable to avoid mutlithread conflicts
			double[] output = input;

			// compute each layer
			for (int i = 0; i < layers.Length; i++)
			{
				output = layers[i].Compute(output);
			}

			// assign output property as well (works correctly for single threaded usage)
			this.output = output;

			return output;
		}

		/// <summary>
		/// Randomize layers of the network.
		/// </summary>
		/// 
		/// <remarks>Randomizes network's layers by calling <see cref="Layer.Randomize"/> method
		/// of each layer.</remarks>
		/// 
		public virtual void Randomize()
		{
			foreach (Layer layer in layers)
			{
				layer.Randomize();
			}
		}

		/// <summary>
		/// Save network to specified file.
		/// </summary>
		/// 
		/// <param name="fileName">File name to save network into.</param>
		/// 
		/// <remarks><para>The neural network is saved using .NET serialization (binary formatter is used).</para></remarks>
		/// 
		public void Save(string fileName)
		{
			FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
			Save(stream);
			stream.Dispose();
		}

		/// <summary>
		/// Save network to specified file.
		/// </summary>
		/// 
		/// <param name="stream">Stream to save network into.</param>
		/// 
		/// <remarks><para>The neural network is saved using .NET serialization (binary formatter is used).</para></remarks>
		/// 
		public void Save(Stream stream)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, this);
		}

		/// <summary>
		/// Load network from specified file.
		/// </summary>
		/// 
		/// <param name="fileName">File name to load network from.</param>
		/// 
		/// <returns>Returns instance of <see cref="Network"/> class with all properties initialized from file.</returns>
		/// 
		/// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
		/// 
		public static Network Load(string fileName)
		{
			FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			Network network = Load(stream);
			stream.Dispose();

			return network;
		}

		/// <summary>
		/// Load network from specified file.
		/// </summary>
		/// 
		/// <param name="stream">Stream to load network from.</param>
		/// 
		/// <returns>Returns instance of <see cref="Network"/> class with all properties initialized from file.</returns>
		/// 
		/// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
		/// 
		public static Network Load(Stream stream)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			Network network = (Network)formatter.Deserialize(stream);
			return network;
		}
	}

    /// <summary>
    /// 线性的激活函数
    /// </summary>
	[Serializable]
	public class LineFunction : IActivationFunction, ICloneable
	{
		private double gradient;

        public LineFunction(double gra = 1)
        {
            gradient = gra;
        }
		public object Clone()
		{
            return new LineFunction(gradient);
		}

		public double Derivative(double x)
		{
            return 1;
		}

		public double Derivative2(double y)
		{
            return 0;
		}

		public double Function(double x)
		{
            return x*gradient;
		}
	}

	[Serializable]
	public class SigmoidFunction : IActivationFunction, ICloneable
	{
		// sigmoid's alpha value
		private double alpha = 2;

		/// <summary>
		/// Sigmoid's alpha value.
		/// </summary>
		/// 
		/// <remarks><para>The value determines steepness of the function. Increasing value of
		/// this property changes sigmoid to look more like a threshold function. Decreasing
		/// value of this property makes sigmoid to be very smooth (slowly growing from its
		/// minimum value to its maximum value).</para>
		///
		/// <para>Default value is set to <b>2</b>.</para>
		/// </remarks>
		/// 
		public double Alpha
		{
			get { return alpha; }
			set { alpha = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SigmoidFunction"/> class.
		/// </summary>
		public SigmoidFunction() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SigmoidFunction"/> class.
		/// </summary>
		/// 
		/// <param name="alpha">Sigmoid's alpha value.</param>
		/// 
		public SigmoidFunction(double alpha)
		{
			this.alpha = alpha;
		}


		/// <summary>
		/// Calculates function value.
		/// </summary>
		///
		/// <param name="x">Function input value.</param>
		/// 
		/// <returns>Function output value, <i>f(x)</i>.</returns>
		///
		/// <remarks>The method calculates function value at point <paramref name="x"/>.</remarks>
		///
		public double Function(double x)
		{
			return (1 / (1 + Math.Exp(-alpha * x)));
		}

		/// <summary>
		/// Calculates function derivative.
		/// </summary>
		/// 
		/// <param name="x">Function input value.</param>
		/// 
		/// <returns>Function derivative, <i>f'(x)</i>.</returns>
		/// 
		/// <remarks>The method calculates function derivative at point <paramref name="x"/>.</remarks>
		///
		public double Derivative(double x)
		{
			double y = Function(x);

			return (alpha * y * (1 - y));
		}

		/// <summary>
		/// Calculates function derivative.
		/// </summary>
		/// 
		/// <param name="y">Function output value - the value, which was obtained
		/// with the help of <see cref="Function"/> method.</param>
		/// 
		/// <returns>Function derivative, <i>f'(x)</i>.</returns>
		/// 
		/// <remarks><para>The method calculates the same derivative value as the
		/// <see cref="Derivative"/> method, but it takes not the input <b>x</b> value
		/// itself, but the function value, which was calculated previously with
		/// the help of <see cref="Function"/> method.</para>
		/// 
		/// <para><note>Some applications require as function value, as derivative value,
		/// so they can save the amount of calculations using this method to calculate derivative.</note></para>
		/// </remarks>
		/// 
		public double Derivative2(double y)
		{
			return (alpha * y * (1 - y));
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// 
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		/// 
		public object Clone()
		{
			return new SigmoidFunction(alpha);
		}
	}


	/// <summary>
	/// Bipolar sigmoid activation function.
	/// </summary>
	///
	/// <remarks><para>The class represents bipolar sigmoid activation function with
	/// the next expression:
	/// <code lang="none">
	///                2
	/// f(x) = ------------------ - 1
	///        1 + exp(-alpha * x)
	///
	///           2 * alpha * exp(-alpha * x )
	/// f'(x) = -------------------------------- = alpha * (1 - f(x)^2) / 2
	///           (1 + exp(-alpha * x))^2
	/// </code>
	/// </para>
	/// 
	/// <para>Output range of the function: <b>[-1, 1]</b>.</para>
	/// 
	/// <para>Functions graph:</para>
	/// <img src="img/neuro/sigmoid_bipolar.bmp" width="242" height="172" />
	/// </remarks>
	/// 
	[Serializable]
	public class BipolarSigmoidFunction : IActivationFunction, ICloneable
	{
		// sigmoid's alpha value
		private double alpha = 2;

		/// <summary>
		/// Sigmoid's alpha value.
		/// </summary>
		///
		/// <remarks><para>The value determines steepness of the function. Increasing value of
		/// this property changes sigmoid to look more like a threshold function. Decreasing
		/// value of this property makes sigmoid to be very smooth (slowly growing from its
		/// minimum value to its maximum value).</para>
		///
		/// <para>Default value is set to <b>2</b>.</para>
		/// </remarks>
		/// 
		public double Alpha
		{
			get { return alpha; }
			set { alpha = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SigmoidFunction"/> class.
		/// </summary>
		public BipolarSigmoidFunction() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BipolarSigmoidFunction"/> class.
		/// </summary>
		/// 
		/// <param name="alpha">Sigmoid's alpha value.</param>
		/// 
		public BipolarSigmoidFunction(double alpha)
		{
			this.alpha = alpha;
		}

		/// <summary>
		/// Calculates function value.
		/// </summary>
		///
		/// <param name="x">Function input value.</param>
		/// 
		/// <returns>Function output value, <i>f(x)</i>.</returns>
		///
		/// <remarks>The method calculates function value at point <paramref name="x"/>.</remarks>
		///
		public double Function(double x)
		{
			return ((2 / (1 + Math.Exp(-alpha * x))) - 1);
		}

		/// <summary>
		/// Calculates function derivative.
		/// </summary>
		/// 
		/// <param name="x">Function input value.</param>
		/// 
		/// <returns>Function derivative, <i>f'(x)</i>.</returns>
		/// 
		/// <remarks>The method calculates function derivative at point <paramref name="x"/>.</remarks>
		///
		public double Derivative(double x)
		{
			double y = Function(x);

			return (alpha * (1 - y * y) / 2);
		}

		/// <summary>
		/// Calculates function derivative.
		/// </summary>
		/// 
		/// <param name="y">Function output value - the value, which was obtained
		/// with the help of <see cref="Function"/> method.</param>
		/// 
		/// <returns>Function derivative, <i>f'(x)</i>.</returns>
		///
		/// <remarks><para>The method calculates the same derivative value as the
		/// <see cref="Derivative"/> method, but it takes not the input <b>x</b> value
		/// itself, but the function value, which was calculated previously with
		/// the help of <see cref="Function"/> method.</para>
		/// 
		/// <para><note>Some applications require as function value, as derivative value,
		/// so they can save the amount of calculations using this method to calculate derivative.</note></para>
		/// </remarks>
		/// 
		public double Derivative2(double y)
		{
			return (alpha * (1 - y * y) / 2);
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// 
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		/// 
		public object Clone()
		{
			return new BipolarSigmoidFunction(alpha);
		}
	}

	/// <summary>
	/// Activation network.
	/// </summary>
	/// 
	/// <remarks><para>Activation network is a base for multi-layer neural network
	/// with activation functions. It consists of <see cref="ActivationLayer">activation
	/// layers</see>.</para>
	///
	/// <para>Sample usage:</para>
	/// <code>
	/// // create activation network
	///	ActivationNetwork network = new ActivationNetwork(
	///		new SigmoidFunction( ), // sigmoid activation function
	///		3,                      // 3 inputs
	///		4, 1 );                 // 2 layers:
	///                             // 4 neurons in the firs layer
	///                             // 1 neuron in the second layer
	///	</code>
	/// </remarks>
	/// 
	[Serializable]
	public class ActivationNetwork : Network
	{


		/// <summary>
		/// Initializes a new instance of the <see cref="ActivationNetwork"/> class.
		/// </summary>
		/// 
		/// <param name="function">Activation function of neurons of the network.</param>
		/// <param name="inputsCount">Network's inputs count.</param>
		/// <param name="neuronsCount">Array, which specifies the amount of neurons in
		/// each layer of the neural network.</param>
		/// 
		/// <remarks>The new network is randomized (see <see cref="ActivationNeuron.Randomize"/>
		/// method) after it is created.</remarks>
		/// 
		public ActivationNetwork(IActivationFunction function, int inputsCount, params int[] neuronsCount)
			: base(inputsCount, neuronsCount.Length)
		{
			// create each layer
			for (int i = 0; i < layers.Length; i++)
			{
				layers[i] = new ActivationLayer(
					// neurons count in the layer
					neuronsCount[i],
					// inputs count of the layer
					(i == 0) ? inputsCount : neuronsCount[i - 1],
					// activation function of the layer
					function);
			}
		}

		/// <summary>
		/// Set new activation function for all neurons of the network.
		/// </summary>
		/// 
		/// <param name="function">Activation function to set.</param>
		/// 
		/// <remarks><para>The method sets new activation function for all neurons by calling
		/// <see cref="ActivationLayer.SetActivationFunction"/> method for each layer of the network.</para></remarks>
		/// 
		public void SetActivationFunction(IActivationFunction function)
		{
			for (int i = 0; i < layers.Length; i++)
			{
				((ActivationLayer)layers[i]).SetActivationFunction(function);
			}
		}
	}


	[Serializable]
	public struct IntRange
	{
		private int min, max;

		/// <summary>
		/// Minimum value of the range.
		/// </summary>
		/// 
		/// <remarks><para>The property represents minimum value (left side limit) or the range -
		/// [<b>min</b>, max].</para></remarks>
		/// 
		public int Min
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
		public int Max
		{
			get { return max; }
			set { max = value; }
		}

		/// <summary>
		/// Length of the range (deffirence between maximum and minimum values).
		/// </summary>
		public int Length
		{
			get { return max - min; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IntRange"/> structure.
		/// </summary>
		/// 
		/// <param name="min">Minimum value of the range.</param>
		/// <param name="max">Maximum value of the range.</param>
		/// 
		public IntRange(int min, int max)
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
		public bool IsInside(int x)
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
		public bool IsInside(IntRange range)
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
		public bool IsOverlapping(IntRange range)
		{
			return ((IsInside(range.min)) || (IsInside(range.max)) ||
					 (range.IsInside(min)) || (range.IsInside(max)));
		}

		/// <summary>
		/// Implicit conversion to <see cref="Range"/>.
		/// </summary>
		/// 
		/// <param name="range">Integer range to convert to single precision range.</param>
		/// 
		/// <returns>Returns new single precision range which min/max values are implicitly converted
		/// to floats from min/max values of the specified integer range.</returns>
		/// 
		public static implicit operator Range(IntRange range)
		{
			return new Range(range.Min, range.Max);
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
		public static bool operator ==(IntRange range1, IntRange range2)
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
		public static bool operator !=(IntRange range1, IntRange range2)
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
			return (obj is IntRange) ? (this == (IntRange)obj) : false;
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


}