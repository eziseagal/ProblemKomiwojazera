namespace ProblemKomiwojazera
{
    public class TravelingSalesmanDK
    {
        public class City
        {
            public int Id { get; set; }
            public double X { get; set; }
            public double Y { get; set; }

            public City(int id, double x, double y)
            {
                Id = id;
                X = x;
                Y = y;
            }

            public double DistanceTo(City other)
            {
                var deltaX = X - other.X;
                var deltaY = Y - other.Y;
                return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            }
        }

        public class Chromosome
        {
            public List<int> Gene { get; set; }
            public double Fitness { get; set; } = double.MaxValue;

            public Chromosome(int numberOfCities)
            {
                Gene = new List<int>(new int[numberOfCities]);
            }
        }

        public static class DataGenerator
        {
            public static List<City> GenerateCities(int numCities)
            {
                var rand = new Random();
                return Enumerable.Range(0, numCities)
                                 .Select(i => new City(i, rand.NextDouble() * 100, rand.NextDouble() * 100))
                                 .ToList();
            }
        }

        public static class EvolutionaryAlgorithm
        {
            public static Chromosome Run(List<City> cities, int populationSize, int generations, double mutationRate)
            {
                var rand = new Random();
                var population = InitializePopulation(populationSize, cities.Count, rand);

                for (var generation = 0; generation < generations; generation++)
                {
                    EvaluatePopulation(population, cities);
                    var selectedChromosomes = SelectChromosomes(population);

                    // Upewnij się, że liczba wybranych chromosomów jest parzysta
                    if (selectedChromosomes.Count % 2 != 0)
                    {
                        selectedChromosomes.Add(selectedChromosomes[0]);
                    }

                    population = CrossoverAndMutate(selectedChromosomes, mutationRate, rand);
                }

                EvaluatePopulation(population, cities);
                return population.OrderBy(c => c.Fitness).First();
            }

            private static List<Chromosome> InitializePopulation(int populationSize, int numCities, Random rand)
            {
                return Enumerable.Range(0, populationSize)
                                 .Select(_ =>
                                 {
                                     var chromosome = new Chromosome(numCities)
                                     {
                                         Gene = Enumerable.Range(0, numCities).OrderBy(_ => rand.Next()).ToList()
                                     };
                                     return chromosome;
                                 })
                                 .ToList();
            }

            private static void EvaluatePopulation(List<Chromosome> population, List<City> cities)
            {
                foreach (var chromosome in population)
                {
                    chromosome.Fitness = CalculateTotalDistance(chromosome, cities);
                }
            }

            private static double CalculateTotalDistance(Chromosome chromosome, List<City> cities)
            {
                var totalDistance = 0.0;
                for (var i = 0; i < chromosome.Gene.Count - 1; i++)
                {
                    totalDistance += cities[chromosome.Gene[i]].DistanceTo(cities[chromosome.Gene[i + 1]]);
                }
                // Dodajemy dystans powrotny do pierwszego miasta
                totalDistance += cities[chromosome.Gene.Last()].DistanceTo(cities[chromosome.Gene.First()]);
                return totalDistance;
            }

            private static List<Chromosome> SelectChromosomes(List<Chromosome> population)
            {
                return population.OrderBy(c => c.Fitness).Take(population.Count / 2).ToList();
            }

            private static List<Chromosome> CrossoverAndMutate(List<Chromosome> selectedChromosomes, double mutationRate, Random rand)
            {
                var newPopulation = new List<Chromosome>();

                for (var i = 0; i < selectedChromosomes.Count; i += 2)
                {
                    var parent1 = selectedChromosomes[i];
                    var parent2 = selectedChromosomes[i + 1];
                    var offspring1 = new Chromosome(parent1.Gene.Count);
                    var offspring2 = new Chromosome(parent2.Gene.Count);

                    var crossoverPoint = rand.Next(1, parent1.Gene.Count - 1);

                    offspring1.Gene = parent1.Gene.Take(crossoverPoint).Concat(parent2.Gene.Except(parent1.Gene.Take(crossoverPoint))).ToList();
                    offspring2.Gene = parent2.Gene.Take(crossoverPoint).Concat(parent1.Gene.Except(parent2.Gene.Take(crossoverPoint))).ToList();

                    Mutate(offspring1, mutationRate, rand);
                    Mutate(offspring2, mutationRate, rand);

                    newPopulation.AddRange(new[] { offspring1, offspring2 });
                }

                return newPopulation;
            }

            private static void Mutate(Chromosome chromosome, double mutationRate, Random rand)
            {
                for (var i = 0; i < chromosome.Gene.Count; i++)
                {
                    if (rand.NextDouble() < mutationRate)
                    {
                        var swapIndex = rand.Next(chromosome.Gene.Count);
                        (chromosome.Gene[i], chromosome.Gene[swapIndex]) = (chromosome.Gene[swapIndex], chromosome.Gene[i]);
                    }
                }
            }
        }
    }
}