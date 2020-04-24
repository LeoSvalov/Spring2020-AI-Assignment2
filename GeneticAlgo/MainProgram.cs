using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace GeneticAlgo
{
    
    internal static class MainProgram
    {
        private static GeneticAlgorithm<MyRectangle> _ga;
        private static readonly Image Target = Image.FromFile("/Users/levsvalov/RiderProjects/GeneticAlgo/GeneticAlgo/512x512/4.jpg");
        private static readonly Bitmap TargetImage = new Bitmap(Target);
        
        public static void Main()
        {
            // Algorithm's parameters;
            int populationSize = 50;
            float mutationRate = 0.005f;
            int elitism = 10;
            int maxGeneration = 5000;
            float desiredFitness = 0.7f;
            int dnaSize = 64;
            Random random = new Random();
            
            /*Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();*/
           
            Algo(populationSize,dnaSize, elitism,mutationRate,random,maxGeneration,desiredFitness);
            /*stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("RunTime " + elapsedTime);*/
            
        }

        /* Main method that contains the algorithm itself */
        private static void Algo(int populationSize, int dnaSize, int elitism, float mutationRate,Random random, int maxGeneration, float desiredFitness)
        {
            
            _ga = new GeneticAlgorithm<MyRectangle>(populationSize, dnaSize, random, GetRandomGene, FitnessFunction, elitism, mutationRate);
            while (_ga.BestFitness<desiredFitness && _ga.GenerationNumber<=maxGeneration)
            {
                _ga.NewGeneration();
                Console.Out.WriteLine(_ga.GenerationNumber + " - " + _ga.BestFitness);
                DrawOutput(_ga.BestGenes);
            }
            DrawOutput(_ga.BestGenes);
            Console.Out.WriteLine("Done!");
        }
        
        /* The method for drawing the result in the output image. */
        private static void DrawOutput(IEnumerable<MyRectangle> result)
        {
            int width = 512;
            int height = 512;
            Bitmap outputImage = new Bitmap(width,height);
           
            foreach (var t in result)
            {
                int initialX = t.Pivot.X;
                int initialY = t.Pivot.Y; 
                for (int j = 0; j < t.Height; j++)
                {
                    for (int k = 0; k < t.Width; k++)
                    {
                        outputImage.SetPixel(initialX+k,initialY+j, t.Color);
                    }
                }
            }
            
            Image output = Image.FromHbitmap(outputImage.GetHbitmap());
            output.Save("output.png");
        }
        
        /* The method that determines the similarity of the 2 colors according to given tolerance.
           Each color fields(except alpha) difference should be less than the tolerance */
        private static bool AreColorsSimilar(Color c1, Color c2, int tolerance)
        {
            return Math.Abs(c1.R - c2.R) < tolerance &&
                   Math.Abs(c1.G - c2.G) < tolerance &&
                   Math.Abs(c1.B - c2.B) < tolerance;
        }
        
        /* The method for calculating the fitness score for the given gene.(there is given index in list of genes)
           It is computed comparing pixel-by-pixel with the source image.*/
        private static float FitnessFunction(int index)
        {
            int totalScore = 0;
            var dna = _ga.Population[index];

            foreach (var t in dna.Genes)
            {
                var initialX = t.Pivot.X;
                var initialY = t.Pivot.Y;

                for (var j = 0; j < t.Height; j++)
                {
                    for (var k = 0; k < t.Width; k++)
                    {
                        var targetColor = TargetImage.GetPixel(initialX + k, initialY + j);
                        if (AreColorsSimilar(targetColor, t.Color, 30))
                        {
                            totalScore++;
                        }
                    }
                }
            }
            float score = totalScore;
            score /= 512 * 512;
            score = (float) (Math.Pow(2, score) - 1); 
            
            return score;
        }

        
        /* The method that returns new Rectangle either for initialization of population, where it will random all color fields,
           or for the mutation, where only 1 color field is being randomized. 
           As argument we get sector number - it's the sequence number of the rectangle on the image, we go from left to the right, on each row there 8 sectors. */
        private static MyRectangle GetRandomGene(int sectorNumber, bool isMutation, MyRectangle mutatedGene)
        {
            Random random = new Random();
            var yFlag = (sectorNumber) / 8;
            var xFlag = sectorNumber - yFlag * 8;
            Point pivot = new Point(Math.Max(xFlag * 64 - 1, 0), Math.Max((yFlag) * 64 - 1, 0));
            var width = 64;
            var height = 64;
            Color color;
            if (isMutation)
            {
                var randomField = random.Next(1, 4);  /* random field: 1 - red, 2 - green, 3 - blue  */
                var value = random.Next(255);
                int r, g, b;
                switch (randomField)
                {
                    case 1:
                        r = value;
                        g = mutatedGene.Color.G;
                        b = mutatedGene.Color.B;
                        break;
                    case 2:
                        r = mutatedGene.Color.R;
                        g = value;
                        b = mutatedGene.Color.B;
                        break;
                    default:
                        r = mutatedGene.Color.R;
                        g = mutatedGene.Color.G;
                        b = value;
                        break;
                }
                color = Color.FromArgb(255, r, g, b);
            }
            else
            {
                color = Color.FromArgb(255, random.Next(255), random.Next(255), random.Next(255));
            }
            MyRectangle rectangle = new MyRectangle(pivot, width, height, color);
            return rectangle;
        }
        
    }
}