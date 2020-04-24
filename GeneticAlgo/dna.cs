using System;

namespace GeneticAlgo
{
    public class Dna<T>
    {
        
        /* List of rectangles in the image. There are 64 rectangles(genes) on the image, index of list shows the rectangle position on the image,
           where 8 rectangles in 1 row, we go from left-up rectangle, to right-bottom rectangle. */
        public T[] Genes { get; } 
        public  float  Fitness { get; set; }
        private readonly Random _random;
        private readonly Func<int,bool,T,T> _getRandomGene;
        private readonly Func<int, float> _fitnessFunction;
        
        /* Constructor */
        public Dna(int size, Random random, Func<int,bool,T,T> getRandomGene, Func<int, float> fitnessFunction , bool shouldInitGenes = true)
        {
            Genes = new T[size];
            this._random = random;
            this._getRandomGene = getRandomGene;
            this._fitnessFunction = fitnessFunction;
            if (!shouldInitGenes) return;
            for (var i = 0; i < size; i++)
            {
                Genes[i] = getRandomGene(i,false,default);
            }
        }

        public float CalculateFitness(int index)
        {
            float fitness = _fitnessFunction(index);
            return fitness;
        }
        
        /* The crossover between 2 images, it takes rectangle from 1st if random double < 0.5 else it takes from 2nd parent */
        public Dna<T> Crossover(Dna<T> otherParent)
        {
            Dna<T> child = new Dna<T>(Genes.Length,_random, _getRandomGene, _fitnessFunction, false);
            for (int i = 0; i < Genes.Length; i++)
            {
                double n = _random.NextDouble();
                if (n < 0.5)
                {
                    child.Genes[i] = Genes[i];
                }
                else 
                {
                    child.Genes[i] = otherParent.Genes[i];
                }
              
            }
            return child;
        }

        /* The mutation is generate new color for the particular rectangle if the random number is less than the constant mutation rate */
        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (_random.NextDouble() < mutationRate)
                {
                    var tmp = _getRandomGene(i,true,Genes[i]);
                    Genes[i] = tmp;
                }
            }
        }
    }
}