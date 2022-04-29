using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.VisualBasic;

namespace DbBenchmark
{
    [SimpleJob(RunStrategy.Monitoring, targetCount: 1)]
    public class DbBenchmarkPerQuery
    {
        //[Params(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15.1,16,17,18,19,20,21,22)] public double qNumber;
        [Params(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15.1,16,17,18,22)] public double qNumber;
        //[Params(19,20,21)] public double BADqNumber;
        //[Params(1)] public double qNumber;

        private string host = "localhost";
        //private string host = "10.114.22.60";
        private const string LogFile = @"c:\tmp\TimeLog.txt";
        public string SqliteQuery =>
            "SELECT   L_RETURNFLAG,   L_LINESTATUS,   L_QUANTITY,   L_EXTENDEDPRICE,   L_DISCOUNT,   L_TAX  FROM LINEITEM WHERE   L_SHIPDATE <= '1997-09-01'";

        public int BlockSize = 1000000;
        
        private string[] possibSeq =Queries.GetPossibilityTest();
        private string[] clusterixFullSeq =Queries.GetFullClusterixTest(); 
        private string[] clusterixPossibSeq =Queries.GetClusterixPossibilityTest();

        //[Benchmark]
        public void CalciteMysql()
        {
            File.AppendAllText(LogFile,$"\n{qNumber}\t{DateTime.Now:O}\t");
            var calciteDb = new Calcite.Database(
                "AdapterFile=java;AdapterRunArgs=-jar Calcirix-jar-with-dependencies.jar;" +
                @"WorkingDirectory=calcite;" +
                "DataSourceUrl=jdbc:mysql://127.0.0.1/tpch_0?useCursorFetch=true&defaultFetchSize=100000;Username=root;Schema=s;DriverClassName=com.mysql.cj.jdbc.Driver");
           
                calciteDb.SelectBlocks(Queries.GetQuery(qNumber), BlockSize);
                File.AppendAllText(LogFile,$"{DateTime.Now:O}");
        }

       
        [Benchmark]
        public void CalciteCSV()
        {
            File.AppendAllText(LogFile,$"\n{qNumber}\t{DateTime.Now:O}\t");
            var calciteDb = new Calcite.Database(
                "AdapterFile=java;AdapterRunArgs=-jar Calcirix-jar-with-dependencies.jar;" +
                @"WorkingDirectory=calcite;" +
                "DataSourceUrl=E:"+ "\\" + "\\1Studing\\00main\\TPC-H\\data;Username=root;Schema=s;DriverClassName=csv");
            calciteDb.SelectBlocks(QueriesCalcite.GetQuery(qNumber), BlockSize);
            File.AppendAllText(LogFile,$"{DateTime.Now:O}");
        }

        //[Benchmark]
        public void PostgresPRO()
        {
            File.AppendAllText(LogFile,$"\n{qNumber}\t{DateTime.Now:O}\t");
            var pgpDatabase = new Postgres.Database(
                "User ID=postgres;Password=root;Host=" + host  + ";Port=5433;Database=tpch;Pooling=true;CommandTimeout=18000");
            pgpDatabase.SelectBlocks(Queries.GetQuery(qNumber), BlockSize);
            File.AppendAllText(LogFile,$"{DateTime.Now:O}");
        }

        //[Benchmark]
        public void Mysql()
        {
            File.AppendAllText(LogFile,$"\n{qNumber}\t{DateTime.Now:O}\t");
            var mySqlDatabase = new Mysql.Database(
                "User ID=root;Password=root;Host=" + host  + ";Port=3306;Database=tpch;ConnectionTimeout=18000;DefaultCommandTimeout=18000");
            mySqlDatabase.SelectBlocks(Queries.GetQuery(qNumber), BlockSize);
            File.AppendAllText(LogFile,$"{DateTime.Now:O}");

        }

        //[Benchmark]
        public void PostgreSQL()
        {
            File.AppendAllText(LogFile,$"\n{qNumber}\t{DateTime.Now:O}\t");
            var pgpDatabase = new Postgres.Database(
                "User ID=postgres;Password=root;Host=" + host + ";Port=5432;Database=tpch;Pooling=true;CommandTimeout=18000");
            pgpDatabase.SelectBlocks(Queries.GetQuery(qNumber), BlockSize);
            //pgpDatabase.SelectBlocks("SELECT * FROM nation", BlockSize);
            File.AppendAllText(LogFile,$"{DateTime.Now:O}");

        }

        //[Benchmark]
        //public void PostgreSQL()
        //{
        //    var pgpDatabase = new Postgres.Database(
        //        "User ID=root;Password=;Host=" + host + ";Port=5432;Database=tpch;Pooling=true;");
        //    pgpDatabase.SelectBlocks(Queries.GetQuery(qNumber), BlockSize);

        //}

    }
}
