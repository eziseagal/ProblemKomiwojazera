using ProblemKomiwojazera;

List<TravelingSalesmanDK.City> cities = TravelingSalesmanDK.DataGenerator.GenerateCities(100);

int populationSize = 100;
int generations = 1000;
double mutationRate = 0.01;
TravelingSalesmanDK.Chromosome bestChromosome = TravelingSalesmanDK.EvolutionaryAlgorithm.Run(cities, populationSize, generations, mutationRate);

Console.WriteLine("Najlepsza znaleziona trasa:");
foreach (int cityIndex in bestChromosome.Gene)
{
    Console.Write($"{cityIndex} ");
}
Console.WriteLine($"\nCałkowita odległość: {bestChromosome.Fitness}");
