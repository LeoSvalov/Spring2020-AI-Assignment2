using System.Collections.Generic;
using System;
using GeneticAlgo;
namespace GeneticAlgo
{
    public class dna<T>
    {
        public T[,] Genes { get; private set; }
        public  float  fitness { get; private set; }

        
        private Random random;
        private Func<T> getRandomGene;
        private Func<int, float> fitnessFunction;
        public dna(int sizeX, int sizeY, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction , bool shouldInitGenes = true)
        {
            Genes = new T[sizeX,sizeY];
            this.random = random;
            this.getRandomGene = getRandomGene;
            this.fitnessFunction = fitnessFunction;
            if (shouldInitGenes)
            {
                for (int i = 0; i < sizeY; i++)
                {
                    for (int j = 0; j < sizeX; j++)
                    {
                        Genes[i,j] = getRandomGene();
                    }
                }
            }
            
        }

        public float CalculateFitness(int index)
        {
            float Fitness = fitnessFunction(index);
            return Fitness;
        }

        public dna<T> Crossover(dna<T> otherParent)
        {
            dna<T> child = new dna<T>(Genes.GetLength(1), Genes.GetLength(0),random, getRandomGene, fitnessFunction, false);

            for (int i = 0; i < Genes.GetLength(0); i++)
            {
                for (int j = 0; j < Genes.GetLength(1); j++)
                {


                    if (random.NextDouble() < 0.5)
                    {
                        child.Genes[i,j] = Genes[i,j];
                    }
                    else
                    {
                        child.Genes[i,j] = otherParent.Genes[i,j];
                    }
                }
            }
            return child;
        }

        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Genes.GetLength(0); i++)
            {
                for (int j = 0; j < Genes.GetLength(1); j++)
                {
                    if (random.NextDouble() < mutationRate)
                    {
                        Genes[i,j] = getRandomGene();
                    }
                }
            }
        }
        
    }
}