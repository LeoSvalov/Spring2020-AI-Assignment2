using System;
using System.Drawing;
using System.Drawing.Imaging;
using GeneticAlgo;
using static System.Console;

namespace GeneticAlgo
{
    internal class Program
    {
        private static GeneticAlgorithm<Color> ga;
        static Image target = Image.FromFile("/Users/levsvalov/RiderProjects/GeneticAlgo/GeneticAlgo/512x512/kobe2.jpg");
        static Bitmap target_image = new Bitmap(target);
        
        public static void Main(string[] args)
        {
            
            int populationSize = 100;
            float mutationRate = 0.01f;
            int elitism = 20;
            Random random = new Random();
            int max_generation = 500;
            algo(populationSize,elitism,mutationRate,random,max_generation);
        }

        private static void algo(int populationSize,int elitism, float mutationRate,Random random, int max_generation)
        {
            
            Boolean flag = true;
            ga = new GeneticAlgorithm<Color>(populationSize, 512, 512, random, GetRandomGene, FitnessFunction, elitism, mutationRate);
            while (ga.GenerationNumber<=max_generation && flag)
            {
                ga.NewGeneration();
                DrawOutput(ga.BestGenes);

                if (ga.BestFitness == 1)
                {
                    flag = false;
                }
            }
            DrawOutput(ga.BestGenes);
            Console.Out.Write("Done");
        }

        private static void DrawOutput(Color[,] result)
        {
            int width = result.GetLength(1);
            int height = result.GetLength(0);
            Bitmap output_image = new Bitmap(width,height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    output_image.SetPixel(i,j, ga.BestGenes[i,j]);
                }
            }
            Image output = Image.FromHbitmap(output_image.GetHbitmap());
            // here output
            output.Save("output.png");
        }
        
        private static float FitnessFunction(int index)
        {
            float score = 0;
            dna<Color> dna= ga.Population[index];
            for (int i = 0; i < dna.Genes.GetLength(0); i++)
            {
                for (int j = 0; j < dna.Genes.GetLength(1); j++)
                {
                    if (target_image.GetPixel(i, j).Equals(dna.Genes[i, j]))
                    {
                        score += 1;
                    }
                    
                }
            }
            score /= (target_image.Width * target_image.Height);
            score = (float) ((Math.Pow(2, score) - 1) / (2 - 1));
            return score;
            
        }

        private static Color GetRandomGene()
        {
            Random random = new Random();
            int a = 255;
            int r = random.Next(255);
            int g = random.Next(255);
            int b = random.Next(255);
            return Color.FromArgb(a,r,g,b);
        }
        
    }
}