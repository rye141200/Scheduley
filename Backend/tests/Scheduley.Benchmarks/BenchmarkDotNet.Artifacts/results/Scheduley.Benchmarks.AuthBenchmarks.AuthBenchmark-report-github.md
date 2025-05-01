```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.5189/23H2/2023Update/SunValley3)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.400
  [Host]   : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method                        | Mean         | Error       | StdDev      | Gen0     | Gen1   | Allocated |
|------------------------------ |-------------:|------------:|------------:|---------:|-------:|----------:|
| &#39;First request - cold cache&#39;  |   1,945.1 μs |   118.39 μs |   347.22 μs |  31.2500 | 7.8125 | 140.65 KB |
| &#39;Second request - warm cache&#39; |     519.0 μs |    32.22 μs |    91.92 μs |  11.7188 | 1.9531 |  52.69 KB |
| &#39;Multiple requests&#39;           |   7,215.3 μs |   489.70 μs | 1,436.20 μs | 140.6250 |      - | 608.36 KB |
| ListUserFiles                 | 455,363.9 μs | 9,037.32 μs | 9,280.66 μs |        - |      - | 161.81 KB |
