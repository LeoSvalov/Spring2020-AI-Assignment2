using System.Collections.Generic;
using System;

namespace GeneticAlgo
{
    public class GeneticAlgorithm<T>
    {
        
        public List<Dna<T>> Population { get; private set; } /* Population of the current generation */
        public int GenerationNumber { get; private set; }
        public float BestFitness { get; private set; }
        public T[] BestGenes { get; } /* The intermediate image(or list of genes) that has the best fitness score in the population*/
        private readonly int _elitism; 
        private readonly float _mutationRate;
        private List<Dna<T>> _newPopulation;
        private Random _random;
        
        /* Constructor */
        public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<int,bool,T,T> getRandomGene, Func<int, float> fitnessFunction, int elitism, float mutationRate = 0.01f)
        {
            GenerationNumber = 1;
            _elitism = elitism;
            _mutationRate = mutationRate;
            Population = new List<Dna<T>>(populationSize);
            _newPopulation = new List<Dna<T>>(populationSize);
            this._random = random;
            BestGenes = new T[dnaSize];
            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new Dna<T>(dnaSize,random,getRandomGene,fitnessFunction));
            }
        }

        /* The process of the selection of genes for new generation*/
        public void NewGeneration()
        {
            CalculateFitness();
            Population.Sort(CompareDna);
            _newPopulation.Clear();
            
            for (int i = 0; i < Population.Count; i++)
            {
                if (i < _elitism) /* first(and best as it's sorted) genes of the previous generations are added directly w/o crossover & mutation */
                {
                    _newPopulation.Add(Population[i]);
                }
                else
                {
                    var parents= ChooseParents(); /* choose 2 best parents for crossover*/
                    Dna<T> parent1 = parents.Item1;
                    Dna<T> parent2 = parents.Item2;
                    Dna<T> child = parent1.Crossover(parent2);
                    child.Mutate(_mutationRate);
                    _newPopulation.Add(child);
                }
            }
            List<Dna<T>> tmpList = Population;
            Population = _newPopulation;
            _newPopulation = tmpList;
            GenerationNumber++;
        }

        private int CompareDna(Dna<T> a, Dna<T> b)
        {
            if (a.Fitness > b.Fitness)
            {
                return -1;
            }else if (a.Fitness < b.Fitness)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /* Calculation of fitness score using method that is implemented in MainProgram.cs */
        private void CalculateFitness()
        {
            Dna<T> best = Population[0];
            
            for (var i = 0; i < Population.Count; i++)
            {
                Population[i].Fitness = Population[i].CalculateFitness(i);
                if (Population[i].Fitness > best.Fitness)
                {
                    best = Population[i];
                }
            }
            BestFitness = best.Fitness;
            best.Genes.CopyTo(BestGenes, 0);
        }

        /* Choose 2 images with the highest fitness scores */
        private Tuple<Dna<T>,Dna<T>> ChooseParents()
        {
            float max1 = -1;
            int ind1 = -1;
            int ind2 = -1;
            float max2 =  -1;
 
            for (int i = 0; i < Population.Count; i++)
            {
                if (Population[i].Fitness > max1) 
                {
                    max2 = max1;
                    ind2 = ind1;
                    ind1 = i;
                    max1 = Population[i].Fitness; 
                }else if (Population[i].Fitness > max2)
                {
                    max2 = Population[i].Fitness;
                    ind2 = i;
                }
            }
            return new Tuple<Dna<T>, Dna<T>>(Population[ind1], Population[ind2]);
        }
    }
}