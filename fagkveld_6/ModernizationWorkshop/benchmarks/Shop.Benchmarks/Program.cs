using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<MaterializeVsStreamBenchmarks>();
BenchmarkRunner.Run<LinqVsYieldBenchmarks>();

public class MaterializeVsStreamBenchmarks
{
    [Params(10, 1000, 100_000)]
    public int N;

    [Benchmark]
    public int Materialize_List()
    {
        var list = new List<int>(N);
        for (int i = 0; i < N; i++) list.Add(i);
        var sum = 0;
        foreach (var x in list) sum += x;
        return sum;
    }

    [Benchmark]
    public async Task<int> Stream_AsyncEnumerable()
    {
        var sum = 0;
        await foreach (var x in Generate(N)) sum += x;
        return sum;
    }

    private static async IAsyncEnumerable<int> Generate(int n)
    {
        for (int i = 0; i < n; i++)
        {
            yield return i;
            if ((i & 0xFF) == 0) await Task.Yield();
        }
    }
}

public class LinqVsYieldBenchmarks
{
    [Params(100, 10_000, 100_000)]
    public int N;

    [Benchmark]
    public int LinqToList_Materialize()
    {
        var result = Enumerable.Range(0, N)
            .Where(x => x % 2 == 0)
            .Select(x => x * 2)
            .ToList(); // Materializes the entire sequence
        
        var sum = 0;
        foreach (var item in result)
            sum += item;
        return sum;
    }

    [Benchmark]
    public int YieldSequence_Streaming()
    {
        var sum = 0;
        foreach (var item in GenerateEvenDoubles(N))
            sum += item;
        return sum;
    }

    private static IEnumerable<int> GenerateEvenDoubles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (i % 2 == 0)
                yield return i * 2;
        }
    }
}
 
