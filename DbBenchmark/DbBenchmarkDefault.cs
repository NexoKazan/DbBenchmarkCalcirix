using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace DbBenchmark
{
    [SimpleJob(RunStrategy.Monitoring, targetCount: 5)]
    public class DbBenchmarkDefault
    {

        //public string Query =>
        //    "SELECT  " +
        //        " O_ORDERPRIORITY, " +
        //"   COUNT(O_ORDERPRIORITY) AS ORDER_COUNT " +
        //"FROM  " +
        //"   ( " +
        //"SELECT " +
        //" L_ORDERKEY, " +
        //" O_ORDERPRIORITY " +
        //"FROM " +
        //" ORDERS, " +
        //" LINEITEM " +
        //"WHERE " +
        //" L_ORDERKEY = O_ORDERKEY " +
        //" AND L_COMMITDATE < L_RECEIPTDATE " +
        //" AND O_ORDERDATE >= '1996-02-01' " +
        //" AND O_ORDERDATE < '1996-02-01' + INTERVAL '3' MONTH " +
        //"  " +
        //"GROUP BY " +
        //" L_ORDERKEY " +
        //" ) T " +
        //"GROUP BY " +
        //" O_ORDERPRIORITY " +
        //"ORDER BY " +
        //" O_ORDERPRIORITY";

        //public string Query =>
        //            @"select
        //	o_orderpriority,
        //	count(*) as order_count
        //from
        //	orders
        //where
        //	o_orderdate >= date '1997-07-01'
        //	and o_orderdate < date '1997-07-01' + interval '3' month
        //	and exists (
        //		select
        //			*
        //		from
        //			lineitem
        //		where
        //			l_orderkey = o_orderkey
        //			and l_commitdate < l_receiptdate
        //	)
        //group by
        //	o_orderpriority
        //order by
        //	o_orderpriority;";      
       
        public string SqliteQuery =>
            "SELECT   L_RETURNFLAG,   L_LINESTATUS,   L_QUANTITY,   L_EXTENDEDPRICE,   L_DISCOUNT,   L_TAX  FROM LINEITEM WHERE   L_SHIPDATE <= '1997-09-01'";

        public int BlockSize = 1000000;
        
        private string[] possibSeq =Queries.GetPossibilityTest();
        private string[] clusterixFullSeq =Queries.GetFullClusterixTest(); 
        private string[] clusterixPossibSeq =Queries.GetClusterixPossibilityTest();

        //[Benchmark]
        public void CalciteMysql()
        {
            var calciteDb = new Calcite.Database(
                "AdapterFile=java;AdapterRunArgs=-jar Calcirix-jar-with-dependencies.jar;" +
                @"WorkingDirectory=calcite;" +
                "DataSourceUrl=jdbc:mysql://127.0.0.1/tpch_0?useCursorFetch=true&defaultFetchSize=100000;Username=root;Schema=s;DriverClassName=com.mysql.cj.jdbc.Driver");
           
                calciteDb.SelectBlocks("SELECT * FROM NATION", BlockSize);
            
        }

        //[Benchmark]
        public void Mysql()
        {
            var mySqlDatabase = new Mysql.Database(
                "User ID=root;Password=;Host=localhost;Port=3306;Database=tpch_0");
            mySqlDatabase.SelectBlocks("SELECT * FROM LINEITEM", BlockSize);

        }


        //[Benchmark]
        public void CalciteCSV()
        {
            var calciteDb = new Calcite.Database(
                "AdapterFile=java;AdapterRunArgs=-jar Calcirix-jar-with-dependencies.jar;" +
                @"WorkingDirectory=calcite;" +
                "DataSourceUrl=/DB/tpch;Username=root;Schema=s;DriverClassName=csv");
            calciteDb.SelectBlocks("SELECT * FROM NATION", BlockSize);
            
        }

        //[Benchmark]
        public void PostgresPROPossib()
        {
            var pgpDatabase = new Postgres.Database(
                "User ID=postgres;Password=root;Host=10.114.22.60;Port=5433;Database=tpch;Pooling=true;");
            pgpDatabase.SelectBlocks("SELECT * FROM nation", BlockSize);
        }

        [Benchmark]
        public void PostgresPossib()
        {
            var pgpDatabase = new Postgres.Database(
                "User ID=postgres;Password=root;Host=10.114.22.60;Port=5432;Database=tpch;Pooling=true;");
            pgpDatabase.SelectBlocks("SELECT * FROM nation", BlockSize);
        }
    }
}
