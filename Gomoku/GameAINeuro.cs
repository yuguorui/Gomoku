using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Gomoku
{
    [Serializable]
    class Strategy : ActivationNetwork, IComparable
    {
        private IActivationFunction activationFunction;
        private int[] neuronsCount;
        private readonly int totalNumberOfWeights;
        public double Fitness { get; set; }

        public Strategy(IActivationFunction activationFunction, int inputsCount, params int[] neuronsCount)
            : base(activationFunction, inputsCount, neuronsCount)
        {
            Fitness = 0;
            this.activationFunction = activationFunction;
            this.inputsCount = inputsCount;
            this.neuronsCount = neuronsCount;
            totalNumberOfWeights = CalculateNetworkSize();
        }

        public double Compute(GameState gs)
        {
            double[] input = gs;
            return Compute(input).ToArray()[0];
        }
        public int CompareTo(object obj)
        {
            Strategy strategy = obj as Strategy;
            if (Fitness > strategy.Fitness)
                return -1;
            else if (Fitness == strategy.Fitness)

                return 0;
            else
                return 1;
        }

        public Strategy CreateNew()
        {
            return new Strategy(activationFunction, inputsCount, neuronsCount);
        }

        // Create and initialize genetic population
        private int CalculateNetworkSize()
        {
            // caclculate total amount of weight in neural network
            int networkSize = 0;

            for (int i = 0; i < Layers.Length; i++)
            {
                Layer layer = Layers[i];

                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    // sum all weights and threshold
                    networkSize += layer.Neurons[j].Weights.Length + 1;
                }
            }

            return networkSize;
        }

        public DoubleArrayChromosome ToDoubleArrarChromosome()
        {
            double[] chromosomeGenes = new double[totalNumberOfWeights];

            // asign new weights and thresholds to network from the given chromosome
            int count = 0;
            for (int i = 0, layersCount = Layers.Length; i < layersCount; i++)
            {
                Layer layer = Layers[i];

                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    ActivationNeuron neuron = layer.Neurons[j] as ActivationNeuron;

                    for (int k = 0; k < neuron.Weights.Length; k++)
                    {
                        chromosomeGenes[count++] = neuron.Weights[k];
                    }
                    chromosomeGenes[count++] = neuron.Threshold;
                }
            }

            return new DoubleArrayChromosome(new UniformGenerator(new Range(-1f, 1f)),
                new ExponentialGenerator(1),
                new UniformGenerator(new Range(-0.5f, 0.5f)),
                chromosomeGenes);
        }

        public void SetWithDoubleArrayChromosome(DoubleArrayChromosome daChromosome)
        {
            int count = 0;
            double[] chromosomeGenes = daChromosome.Value;
            // asign new weights and thresholds to network from the given chromosome
            for (int i = 0, layersCount = Layers.Length; i < layersCount; i++)
            {
                Layer layer = Layers[i];

                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    ActivationNeuron neuron = layer.Neurons[j] as ActivationNeuron;

                    for (int k = 0; k < neuron.Weights.Length; k++)
                    {
                        neuron.Weights[k] = chromosomeGenes[count++];
                    }
                    neuron.Threshold = chromosomeGenes[count++];
                }
            }
        }


        public void Crossover(Strategy strategy)
        {
            if (strategy != null)
            {
                DoubleArrayChromosome c1 = ToDoubleArrarChromosome();
                DoubleArrayChromosome c2 = strategy.ToDoubleArrarChromosome();
                c1.Crossover(c2);
                SetWithDoubleArrayChromosome(c1);
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public void Mutate()
        {
            DoubleArrayChromosome c = ToDoubleArrarChromosome();
            c.Mutate();
            SetWithDoubleArrayChromosome(c);
        }

        public Strategy Clone()
        {
            DoubleArrayChromosome daChromosome = ToDoubleArrarChromosome();
            Strategy s = new Strategy(activationFunction, inputsCount, neuronsCount);
            s.SetWithDoubleArrayChromosome(daChromosome);
            return s;
        }
    }
    class GameAINeuroEvolutionaryLearning
    {
        public List<Strategy> population;
        private int size;
        private double crossoverRate = 0.75;
        private double mutationRate = 0.10;
        private static ThreadSafeRandom rand;
        private bool autoShuffling = false;
        public Strategy BestChromosome
        {
            get;
            private set;
        }


        public GameAINeuroEvolutionaryLearning(int size)
        {
            population = new List<Strategy>();
            this.size = size;
            rand = new ThreadSafeRandom();
            Strategy strategy = new Strategy(
                //new SigmoidFunction(),
                new LineFunction(),
                Settings.MAX_CHESSES,
                Settings.NETWORK_STRUCT);
            population.Add(strategy);
            for (int i = 0; i < size - 1; i++)
            {
                population.Add(strategy.CreateNew());
            }
        }

        public GameAINeuroEvolutionaryLearning(int size, FileStream file)
        {
            population = new List<Strategy>();
            this.size = size;
            rand = new ThreadSafeRandom();
            Strategy strategy = (Strategy)Network.Load(file);
            population.Add(strategy);
            for (int i = 0; i < size - 1; i++)
            {
                population.Add(strategy.CreateNew());
            }
        }

        public void Crossover()
        {
            for (int i = 1; i < size; i += 2)
            {
                // generate next random number and check if we need to do crossover
                if (rand.NextDouble() <= crossoverRate)
                {
                    // clone both ancestors
                    Strategy c1 = population[i - 1].Clone();
                    Strategy c2 = population[i].Clone();

                    // do crossover
                    c1.Crossover(c2);

                    // add two new offsprings to the population
                    population.Add(c1);
                    population.Add(c2);
                }
            }
        }

        public void Mutate()
        {
            for (int i = 0; i < size; i++)
            {
                // generate next random number and check if we need to do mutation
                if (rand.NextDouble() <= mutationRate)
                {
                    // clone the chromosome
                    Strategy c = population[i].Clone();
                    // mutate it
                    c.Mutate();
                    // add mutant to the population
                    population.Add(c);
                }
            }
        }

        public void Regenerate()
        {
            Strategy ancestor = population[0];

            // clear population
            population.Clear();
            // add chromosomes to the population
            for (int i = 0; i < size; i++)
            {
                // create new chromosome
                Strategy c = ancestor.CreateNew();
                // calculate it's fitness
                // add it to population
                population.Add(c);
            }
            Evaluate();
        }

        public void Selection()
        {
            population.Sort();
            population.RemoveRange(size, population.Count - size);
            FindBestChromosome();
        }

        public void RunEpochs(int times)
        {
            for (int i = 0; i < times; i++)
            {
                Crossover();
                Mutate();

                Evaluate();

                Selection();


                if (autoShuffling)
                    Shuffle();
            }

        }

        public void RunEpoch()
        {
            Crossover();
            Mutate();

            Evaluate();

            Selection();


            if (autoShuffling)
                Shuffle();
        }

        /// <summary>
        /// 针对整个群体进行适应度的评估
        /// </summary>
        public void Evaluate()
        {
            for (int i = 0; i < population.Count; i++)
            {
                int fit = 0;
                for (int j = 0; j < Settings.GAME_MATCHES; j++)
                {
                    fit += GameAI.ComputerVSComputer(
                        population[i].Compute,
                        population[rand.Next(population.Count)].Compute);
                }
                population[i].Fitness = fit;
            }
        }

        public void Shuffle()
        {
            // current population size
            int size = population.Count;
            // create temporary copy of the population
            List<Strategy> tempPopulation = population.GetRange(0, size);
            // clear current population and refill it randomly
            population.Clear();

            while (size > 0)
            {
                int i = rand.Next(size);

                population.Add(tempPopulation[i]);
                tempPopulation.RemoveAt(i);

                size--;
            }
        }

        private void FindBestChromosome()
        {
            BestChromosome = population[0];
            double fitnessMax = BestChromosome.Fitness;

            for (int i = 1; i < size; i++)
            {
                double fitness = population[i].Fitness;
                // check for max
                if (fitness > fitnessMax)
                {
                    fitnessMax = fitness;
                    BestChromosome = population[i];
                }
            }
        }
    }
}

