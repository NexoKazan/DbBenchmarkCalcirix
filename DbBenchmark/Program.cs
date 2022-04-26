using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DbBenchmark
{
    class Program
    {
        //static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
        //    .Run(args, new DebugInProcessConfig());
        static void Main(string[] args)
        {
            //Console.WriteLine(Queries.GetQuery(15.1));
            //new DbBenchmarkPerQuery().CalciteCSV();
            //new DbBenchmarkPerQuery().CalciteCSV();
            //new DbBenchmark().PostgresPRO();
            Console.WriteLine(@"Choose test type: 
                    Use 0 for defaultTest(SELECT * FROM NATION)
                    Use 1 for full test (132 query)                        
                    Use 2 for possibility test (22 query)
                    Use 3 for per Query Test(22 query, each as single benchmark");
            bool toidi = true;
            while (toidi)
            {
                toidi = false;
                switch (Console.ReadLine())
                {
                    case "0":
                    {
                        var summary = BenchmarkRunner.Run<DbBenchmarkDefault>();
                        break;
                    }
                    case "1":
                    {
                        var summary = BenchmarkRunner.Run<DbBenchmarkFull>(null, args);
                        break;
                    }
                    case "2":
                    {
                        var summary = BenchmarkRunner.Run<DbBenchmarkPossib>();
                        break;
                    }
                    case "3":
                    {
                        var summary = BenchmarkRunner.Run<DbBenchmarkPerQuery>();
                        break;
                    }
                    default:
                    {
                        Console.WriteLine(@"Wrong number!

                                Use 0 for defaultTest(SELECT * FROM NATION)
                                Use 1 for full test (132 query)                        
                                Use 2 for possibility test (22 query)
                                Use 3 for per Query Test(22 query, each as single benchmark");
                        toidi = true;
                        break;
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
