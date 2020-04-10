using System.Collections.Generic;
using System;
using System.Drawing;
using GeneticAlgo;

namespace GeneticAlgo
{
    public class GeneticAlgorithm<T>
    {
        public List<dna<T>> Population { get; private set; }
        public int GenerationNumber { get; private set; }
        public float BestFitness { get; private set; }
        public T[,] BestGenes { get; private set; }
        public int Elitism;
        public float MutationRate;
        private List<dna<T>> newPopulation;
        private Random random;
        private float fitnessSum;
        public GeneticAlgorithm(int populationSize, int dnaSizeX,int dnaSizeY, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction, int elitism, float mutationRate = 0.01f)
        {
            GenerationNumber = 1;
            Elitism = elitism;
            MutationRate = mutationRate;
            Population = new List<dna<T>>(populationSize);
            newPopulation = new List<dna<T>>(populationSize);
            this.random = random;
            BestGenes = new T[dnaSizeX,dnaSizeY];
            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new dna<T>(dnaSizeX,dnaSizeY,random,getRandomGene,fitnessFunction,true));
            }
        }

        public void NewGeneration()
        {
            if (Population.Count<=0)
            {
                return;
            }

            CalculateFitness();
            Population.Sort(CompareDna);
            newPopulation.Clear();
            for (int i = 0; i < Population.Count; i++)
            {
                if (i< Elitism)
                {
                    newPopulation.Add(Population[i]);
                }
                else
                {
                    var parents= ChooseParents();
                    dna<T> parent1 = parents.Item1;
                    dna<T> parent2 = parents.Item2; 
                    
                    dna<T> child = parent1.Crossover(parent2);
                    child.Mutate(MutationRate);
                    newPopulation.Add(child);
                }
            }

            List<dna<T>> tmpList = Population;
            Population = newPopulation;
            newPopulation = tmpList;
            GenerationNumber++;
        }

        public int CompareDna(dna<T> a, dna<T> b)
        {
            if (a.fitness > b.fitness)
            {
                return -1;
            }else if (a.fitness < b.fitness)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void CalculateFitness()
        {
            fitnessSum = 0;
            dna<T> best = Population[0];
            
            for (int i = 0; i < Population.Count; i++)
            {
                fitnessSum += Population[i].CalculateFitness(i);
                if (Population[i].fitness > best.fitness)
                {
                    best = Population[i];
                }
            }
            BestFitness = best.fitness;
            for (int i = 0; i < best.Genes.GetLength(0); i++)
            {
                for (int j = 0; j < best.Genes.GetLength(1); j++)
                {
                    BestGenes[i, j] = best.Genes[i, j];
                }
            }
        }

        private Tuple<dna<T>,dna<T>> ChooseParents()
        {
            float max1 = -1;
            int ind_1 = -1;
            int ind_2 = -1;
            float max2 =  -1;
            
            for (int i = 0; i < Population.Count; i++)
            {
                if (Population[i].fitness > max1) 
                {
                    max2 = max1;
                    ind_2 = ind_1;
                    ind_1 = i;
                    max1 = Population[i].fitness; 
                }else if (Population[i].fitness > max2) 
                {
                    max2 = Population[i].fitness;
                    ind_2 = i;
                } 
            }
            return new Tuple<dna<T>, dna<T>>(Population[ind_1], Population[ind_2]);
        }
        
    }
}