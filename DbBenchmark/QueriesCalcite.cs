using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBenchmark
{
    static class QueriesCalcite
    {

        #region Queries

        public static string Q1 =>
            //интервал изменён на 90 дней вместо 108
            @"
        select
            l_returnflag, 
            l_linestatus,
            sum(l_quantity) as sum_qty, 
            sum(l_extendedprice) as sum_base_price, 
            sum(l_extendedprice * (1 - l_discount)) as sum_disc_price, 
            sum(l_extendedprice * (1 - l_discount) * (1 + l_tax)) as sum_charge, 
            avg(l_quantity) as avg_qty, 
            avg(l_extendedprice) as avg_price, 
            avg(l_discount) as avg_disc, 
            count(*) as count_order 
        from 
            s.lineitem
        where 
            l_shipdate <= DATE '1998-12-01' - interval '90' day group by 
            l_returnflag, l_linestatus 
        order by 
            l_returnflag, l_linestatus;
		";
       
        public static string Q1_clusterix =>
            @"

		"; 
        
        public static string Q2 =>
            @"
        select 
        	s_acctbal, 
        	s_name, 
        	n_name, 
        	p_partkey, 
        	p_mfgr, 
        	s_address, 
        	s_phone, 
        	s_comment 
        from 
        	s.part, 
        	s.supplier, 
        	s.partsupp, 
        	s.nation, 
        	s.region 
        where 
        	p_partkey = ps_partkey and 
        	s_suppkey = ps_suppkey and 
        	p_size = 30 and 
        	p_type like '%STEEL' and 
        	s_nationkey = n_nationkey and 
        	n_regionkey = r_regionkey and 
        	r_name = 'ASIA' and 
        	ps_supplycost = (
        		select 
        			min(ps_supplycost) 
        		from 
        			s.partsupp, 
        			s.supplier, 
        			s.nation, 
        			s.region 
        		where 
        			p_partkey = ps_partkey and 
        			s_suppkey = ps_suppkey and 
        			s_nationkey = n_nationkey and 
        			n_regionkey = r_regionkey and 
        			r_name = 'ASIA'
        	) 
        order by 
        	s_acctbal desc, 
        	n_name, s_name, 
        	p_partkey limit 100;

		";

        public static string Q2_clusterix =>
            @"
		SELECT
			S_ACCTBAL,
			S_NAME,
			N_NAME,
			P_PARTKEY,
			P_MFGR,
			S_ADDRESS,
			S_PHONE,
			S_COMMENT
		FROM
			part,
			supplier,
			partsupp,
			s.nation,
			region
		WHERE
			P_PARTKEY = PS_PARTKEY
			AND S_SUPPKEY = PS_SUPPKEY
			AND P_SIZE = 48
			AND P_TYPE LIKE '%NICKEL'
			AND S_NATIONKEY = N_NATIONKEY
			AND N_REGIONKEY = R_REGIONKEY
			AND R_NAME = 'AMERICA'
			AND PS_SUPPLYCOST = (
				SELECT
					MIN(PS_SUPPLYCOST)
				FROM
					partsupp,
					supplier,
					s.nation,
					region
				WHERE
					P_PARTKEY = PS_PARTKEY
					AND S_SUPPKEY = PS_SUPPKEY
					AND S_NATIONKEY = N_NATIONKEY
					AND N_REGIONKEY = R_REGIONKEY
					AND R_NAME = 'AMERICA'
			)
		ORDER BY
			S_ACCTBAL DESC,
			N_NAME,
			S_NAME,
			P_PARTKEY;
		";

        public static string Q3 =>
            @"
        select 
        	l_orderkey, 
        	sum(l_extendedprice * (1 - l_discount)) as revenue, 
        	o_orderdate, 
        	o_shippriority 
        from 
        	s.customer, 
        	s.orders, 
        	s.lineitem 
        where 
        	c_mktsegment = 'AUTOMOBILE' and 
        	c_custkey = o_custkey and 
        	l_orderkey = o_orderkey and 
        	o_orderdate < date '1995-03-13' and 
        	l_shipdate > date '1995-03-13' 
        group by 
        	l_orderkey, 
        	o_orderdate, 
        	o_shippriority 
        order by 
        	revenue desc, 
        	o_orderdate limit 10;
        
		";

        public static string Q3_clusterix =>
            @"
        SELECT
            L_ORDERKEY,
            SUM(L_EXTENDEDPRICE * (1 - L_DISCOUNT)) AS REVENUE,
            O_ORDERDATE,
            O_SHIPPRIORITY
        FROM
            s.customer,
            orders,
            s.lineitem
        WHERE
            C_MKTSEGMENT = 'HOUSEHOLD'
            AND C_CUSTKEY = O_CUSTKEY
            AND L_ORDERKEY = O_ORDERKEY
            AND O_ORDERDATE < '1995-03-31'
            AND L_SHIPDATE > '1995-03-31'
        GROUP BY
            L_ORDERKEY,
            O_ORDERDATE,
            O_SHIPPRIORITY
        ORDER BY
            REVENUE DESC,
            O_ORDERDATE;
        ";

        public static string Q4 =>
            @"
        select 
        	o_orderpriority, 
        	count(*) as order_count 
        from orders 
        where 
        	o_orderdate >= date '1995-01-01' and 
        	o_orderdate < date '1995-01-01' + interval '3' month and 
        	exists (
        		select * 
        		from s.lineitem 
        		where 
        			l_orderkey = o_orderkey and 
        			l_commitdate < l_receiptdate
        	) 
        group by o_orderpriority 
        order by o_orderpriority;
        
		";
        
        public static string Q4_clusterix => 
            @"
        select 
            o_orderpriority, 
            count(*) as order_count 
        from 
            orders 
        where 
            o_orderdate >= date '1995-01-01' and 
            o_orderdate < date '1995-01-01' + interval '3' month and 
            exists (
                select * 
                from 
                    s.lineitem
                where 
                    l_orderkey = o_orderkey and 
                    l_commitdate < l_receiptdate
            ) 
        group by o_orderpriority 
        order by o_orderpriority;
           ";
                               
        //Данные не совпадают, обдумать
        public static string Q4_NoExist =>
            @"
        SELECT 
            O_ORDERPRIORITY,
           COUNT(O_ORDERPRIORITY) AS ORDER_COUNT
        FROM 
           (
        SELECT
            L_ORDERKEY,
            O_ORDERPRIORITY
        FROM
            orders,
            s.lineitem
        WHERE
            L_ORDERKEY = O_ORDERKEY
            AND L_COMMITDATE < L_RECEIPTDATE
            AND O_ORDERDATE >= date '1996-02-01'
            AND O_ORDERDATE < date '1996-02-01' + INTERVAL '3' MONTH			
        GROUP BY
            L_ORDERKEY
            )
        GROUP BY
            O_ORDERPRIORITY
        ORDER BY
            O_ORDERPRIORITY;
        ";

        public static string Q5 =>
            @"
        select 
        	n_name, 
        	sum(l_extendedprice * (1 - l_discount)) as revenue 
        from 
        	s.customer, 
        	s.orders, 
        	s.lineitem, 
        	s.supplier, 
        	s.nation, 
        	s.region 
        where 
        	c_custkey = o_custkey and 
        	l_orderkey = o_orderkey and 
        	l_suppkey = s_suppkey and 
        	c_nationkey = s_nationkey and 
        	s_nationkey = n_nationkey and 
        	n_regionkey = r_regionkey and 
        	r_name = 'MIDDLE EAST' and 
        	o_orderdate >= date '1994-01-01' and 
        	o_orderdate < date '1994-01-01' + interval '1' year 
        group by n_name 
        order by revenue desc;

		";

        public static string Q5_clusterix =>
            @"
        SELECT
            N_NAME,
            SUM(L_EXTENDEDPRICE * (1 - L_DISCOUNT)) AS REVENUE
        FROM
            s.customer,
            orders,
            s.lineitem,
            supplier,
            s.nation,
            region
        WHERE
            C_CUSTKEY = O_CUSTKEY
            AND L_ORDERKEY = O_ORDERKEY
            AND L_SUPPKEY = S_SUPPKEY
            AND C_NATIONKEY = S_NATIONKEY
            AND S_NATIONKEY = N_NATIONKEY
            AND N_REGIONKEY = R_REGIONKEY
            AND R_NAME = 'MIDDLE EAST'
            AND O_ORDERDATE >= date '1994-01-01'
            AND O_ORDERDATE < date '1994-01-01' + INTERVAL '1' YEAR
        GROUP BY
            N_NAME
        ORDER BY
            REVENUE DESC;
        ";

        public static string Q6 =>
            @"
        select
        	sum(l_extendedprice * l_discount) as revenue
        from
        	s.lineitem
        where
        	l_shipdate >= date '1994-01-01'
        	and l_shipdate < date '1994-01-01' + interval '1' year
        	and l_discount between 0.06 - 0.01 and 0.06 + 0.01
        	and l_quantity < 24;
		";

        public static string Q6_clusterix =>
            @"
        SELECT
            SUM(L_EXTENDEDPRICE * L_DISCOUNT) AS REVENUE
        FROM
            s.lineitem
        WHERE
            L_SHIPDATE >= date '1997-01-01'
            AND L_SHIPDATE < date '1997-01-01' + INTERVAL '1' YEAR
            AND L_DISCOUNT BETWEEN 0.07 - 0.01 AND 0.07 + 0.01
            AND L_QUANTITY < 24;		
        ";

        public static string Q7 =>
            @"
        select 
        	supp_nation, 
        	cust_nation, 
        	l_year, 
        	sum(volume) as revenue 
        from (
        	select 
        		n1.n_name as supp_nation, 
        		n2.n_name as cust_nation, 
        		extract(year from l_shipdate) as l_year, 
        		l_extendedprice * (1 - l_discount) as volume 
        	from 
        		s.supplier, 
        		s.lineitem, 
        		s.orders, 
        		s.customer, 
        		s.nation n1, 
        		s.nation n2 
        	where 
        		s_suppkey = l_suppkey and 
        		o_orderkey = l_orderkey and 
        		c_custkey = o_custkey and 
        		s_nationkey = n1.n_nationkey and 
        		c_nationkey = n2.n_nationkey and 
        		(
        			(n1.n_name = 'JAPAN' and n2.n_name = 'INDIA') or 
        			(n1.n_name = 'INDIA' and n2.n_name = 'JAPAN')
        		) and 
        		l_shipdate between date '1995-01-01' and date '1996-12-31'
        	) as shipping 
        group by 
        	supp_nation, 
        	cust_nation, 
        	l_year 
        order by 
        	supp_nation, 
        	cust_nation, 
        	l_year;

		";

        public static string Q7_clusterix =>
            @"
        SELECT
            SUPP_NATION,
            CUST_NATION,
            L_YEAR,
            SUM(VOLUME) AS REVENUE
        FROM
            (
                SELECT
                    N1.N_NAME AS SUPP_NATION,
                    N2.N_NAME AS CUST_NATION,
                    EXTRACT(YEAR FROM L_SHIPDATE) AS L_YEAR,
                    L_EXTENDEDPRICE * (1 - L_DISCOUNT) AS VOLUME
                FROM
                    supplier,
                    s.lineitem,
                    orders,
                    s.customer,
                    s.nation N1,
                    s.nation N2
                WHERE
                    S_SUPPKEY = L_SUPPKEY
                    AND O_ORDERKEY = L_ORDERKEY
                    AND C_CUSTKEY = O_CUSTKEY
                    AND S_NATIONKEY = N1.N_NATIONKEY
                    AND C_NATIONKEY = N2.N_NATIONKEY
                    AND (
                        (N1.N_NAME = 'IRAQ' AND N2.N_NAME = 'ALGERIA')
                        OR (N1.N_NAME = 'ALGERIA' AND N2.N_NAME = 'IRAQ')
                    )
                    AND L_SHIPDATE BETWEEN '1995-01-01' AND '1996-12-31'
            ) AS SHIPPING
        GROUP BY
            SUPP_NATION,
            CUST_NATION,
            L_YEAR
        ORDER BY
            SUPP_NATION,
            CUST_NATION,
            L_YEAR;		
        ";

        public static string Q8 =>
            @"
        select 
        	o_year, 
        	sum(
        		case 
        			when nation = 'INDIA' 
        			then volume 
        			else 0 
        		end
        		) / sum(volume) as mkt_share 
        from (
        	select 
        		extract(year from o_orderdate) as o_year,	
        		l_extendedprice * (1 - l_discount) as volume, 
        		n2.n_name as nation 
        	from 
        		s.part, 
        		s.supplier, 
        		s.lineitem, 
        		s.orders, 
        		s.customer, 
        		s.nation n1, 
        		s.nation n2, 
        		s.region 
        	where 
        		p_partkey = l_partkey and 
        		s_suppkey = l_suppkey and 
        		l_orderkey = o_orderkey and 
        		o_custkey = c_custkey and 
        		c_nationkey = n1.n_nationkey and 
        		n1.n_regionkey = r_regionkey and 
        		r_name = 'ASIA'	and 
        		s_nationkey = n2.n_nationkey and 
        		o_orderdate between date '1995-01-01' and date '1996-12-31'and 
        		p_type = 'SMALL PLATED COPPER'
        	) as all_nations 
        group by o_year 
        order by o_year;
		";

        public static string Q8_clusterix =>
            @"
        SELECT
            O_YEAR,
            SUM(CASE
                WHEN s.nation = 'IRAN' THEN VOLUME
                ELSE 0
            END) / SUM(VOLUME) AS MKT_SHARE
        FROM
            (
                SELECT
                    EXTRACT(YEAR FROM O_ORDERDATE) AS O_YEAR,
                    L_EXTENDEDPRICE * (1 - L_DISCOUNT) AS VOLUME,
                    N2.N_NAME AS s.nation
                FROM
                    part,
                    supplier,
                    s.lineitem,
                    orders,
                    s.customer,
                    s.nation N1,
                    s.nation N2,
                    region
                WHERE
                    P_PARTKEY = L_PARTKEY
                    AND S_SUPPKEY = L_SUPPKEY
                    AND L_ORDERKEY = O_ORDERKEY
                    AND O_CUSTKEY = C_CUSTKEY
                    AND C_NATIONKEY = N1.N_NATIONKEY
                    AND N1.N_REGIONKEY = R_REGIONKEY
                    AND R_NAME = 'MIDDLE EAST'
                    AND S_NATIONKEY = N2.N_NATIONKEY
                    AND O_ORDERDATE BETWEEN '1995-01-01' AND '1996-12-31'
                    AND P_TYPE = 'STANDARD BRUSHED BRASS'
            ) AS ALL_NATIONS
        GROUP BY
            O_YEAR
        ORDER BY
            O_YEAR;
        ";

        public static string Q9 =>
            @"
        select 
        	nation, 
        	o_year, 
        	sum(amount) as sum_profit 
        from (
        	select 
        	n_name as nation, 
        	extract(year from o_orderdate) as o_year, 
        	l_extendedprice * (1 - l_discount) - ps_supplycost * l_quantity as amount 
        from 
        	s.part, 
        	s.supplier, 
        	s.lineitem, 
        	s.partsupp, 
        	s.orders, 
        	s.nation 
        where 
        	s_suppkey = l_suppkey and 
        	ps_suppkey = l_suppkey and 
        	ps_partkey = l_partkey and 
        	p_partkey = l_partkey and 
        	o_orderkey = l_orderkey and 
        	s_nationkey = n_nationkey and 
        	p_name like '%dim%') as profit 
        group by 
        	nation, 
        	o_year 
        order by 
        	nation, 
        	o_year desc;
        
		";

        //пусто в постгрес
        public static string Q9_clusterix =>
            @"
        SELECT
            s.nation,
            O_YEAR,
            SUM(AMOUNT) AS SUM_PROFIT
        FROM
            (
                SELECT
                    N_NAME AS s.nation,
                    EXTRACT(YEAR FROM O_ORDERDATE) AS O_YEAR,
                    L_EXTENDEDPRICE * (1 - L_DISCOUNT) - PS_SUPPLYCOST * L_QUANTITY AS AMOUNT
                FROM
                    part,
                    supplier,
                    s.lineitem,
                    partsupp,
                    orders,
                    s.nation
                WHERE
                    S_SUPPKEY = L_SUPPKEY
                    AND PS_SUPPKEY = L_SUPPKEY
                    AND PS_PARTKEY = L_PARTKEY
                    AND P_PARTKEY = L_PARTKEY
                    AND O_ORDERKEY = L_ORDERKEY
                    AND S_NATIONKEY = N_NATIONKEY
                    AND P_NAME LIKE '%SNOW%'
            ) AS PROFIT
        GROUP BY
            s.nation,
            O_YEAR
        ORDER BY
            s.nation,
            O_YEAR DESC;
        ";

        public static string Q10 =>
            @"
        select c_custkey,
        	c_name,
        	sum(l_extendedprice * (1 - l_discount)) as revenue,
        	c_acctbal,
        	n_name,
        	c_address,
        	c_phone,
        	c_comment
        from
        	s.customer,
        	s.orders,
        	s.lineitem,
        	s.nation
        where
        	c_custkey = o_custkey
        	and l_orderkey = o_orderkey
        	and o_orderdate >= date '1993-08-01'
        	and o_orderdate < date '1993-08-01' + interval '3' month
        	and l_returnflag = 'R'
        	and c_nationkey = n_nationkey
        group by
        	c_custkey,
        	c_name,
        	c_acctbal,
        	c_phone,
        	n_name,
        	c_address,
        	c_comment
        order by
        	revenue desc
        limit 20;

		";

        public static string Q10_clusterix =>
            @"
        SELECT
            C_CUSTKEY,
            C_NAME,
            SUM(L_EXTENDEDPRICE * (1 - L_DISCOUNT)) AS REVENUE,
            C_ACCTBAL,
            N_NAME,
            C_ADDRESS,
            C_PHONE,
            C_COMMENT
        FROM
            s.customer,
            orders,
            s.lineitem,
            s.nation
        WHERE
            C_CUSTKEY = O_CUSTKEY
            AND L_ORDERKEY = O_ORDERKEY
            AND O_ORDERDATE >= date '1994-04-01'
            AND O_ORDERDATE < date '1994-04-01' + INTERVAL '3' MONTH
            AND L_RETURNFLAG = 'R'
            AND C_NATIONKEY = N_NATIONKEY
        GROUP BY
            C_CUSTKEY,
            C_NAME,
            C_ACCTBAL,
            C_PHONE,
            N_NAME,
            C_ADDRESS,
            C_COMMENT
        ORDER BY
            REVENUE DESC;
        ";

        public static string Q11 =>
            @"
        select 
        	ps_partkey, 
        	sum(ps_supplycost * ps_availqty) as test 
        from 
        	s.partsupp, 
        	s.supplier, 
        	s.nation 
        where 
        	ps_suppkey = s_suppkey and 
        	s_nationkey = n_nationkey and 
        	n_name = 'MOZAMBIQUE' 
        group by ps_partkey 
        	having sum(ps_supplycost * ps_availqty) >
        	(	
        		select 
        			sum(ps_supplycost * ps_availqty) * 0.0001000000 
        		from 
        			s.partsupp, 
        			s.supplier, 
        			s.nation 
        		where 
        			ps_suppkey = s_suppkey and 
        			s_nationkey = n_nationkey and 
        			n_name = 'MOZAMBIQUE'
        	) 
        order by test desc;

		";

        public static string Q11_clusterix =>
            @"
        SELECT
            PS_PARTKEY,
            SUM(PS_SUPPLYCOST * PS_AVAILQTY) AS VALUE
        FROM
            partsupp,
            supplier,
            s.nation
        WHERE
            PS_SUPPKEY = S_SUPPKEY
            AND S_NATIONKEY = N_NATIONKEY
            AND N_NAME = 'ALGERIA'
        GROUP BY
            PS_PARTKEY HAVING
                SUM(PS_SUPPLYCOST * PS_AVAILQTY) > (
                    SELECT
                        SUM(PS_SUPPLYCOST * PS_AVAILQTY) * 0.0001000000
                    FROM
                        partsupp,
                        supplier,
                        s.nation
                    WHERE
                        PS_SUPPKEY = S_SUPPKEY
                        AND S_NATIONKEY = N_NATIONKEY
                        AND N_NAME = 'ALGERIA'
                )
        ORDER BY
            VALUE DESC;
        ";

        public static string Q12 =>
            @"
        select 
        	l_shipmode, 
        	sum(
        		case 
        			when 
        				o_orderpriority = '1-URGENT' or 
        				o_orderpriority = '2-HIGH' 
        			then 1 
        			else 0 
        		end
        		) as high_line_count, 
        	sum(
        		case 
        			when 
        				o_orderpriority <> '1-URGENT' and 
        				o_orderpriority <> '2-HIGH' 
        			then 1 
        			else 0 
        		end
        		) as low_line_count 
        from 
        	s.orders, 
        	s.lineitem 
        where 
        	o_orderkey = l_orderkey and
        	l_shipmode in ('RAIL', 'FOB') and 
        	l_commitdate < l_receiptdate and 
        	l_shipdate < l_commitdate and 
        	l_receiptdate >= date '1997-01-01' 
        	and l_receiptdate < date '1997-01-01' + interval '1' year 
        group by l_shipmode 
        order by l_shipmode;
		";

        public static string Q12_clusterix =>
            @"
        SELECT
            L_SHIPMODE,
            SUM(CASE
                WHEN O_ORDERPRIORITY = '1-URGENT'
                    OR O_ORDERPRIORITY = '2-HIGH'
                    THEN 1
                ELSE 0
            END) AS HIGH_LINE_COUNT,
            SUM(CASE
                WHEN O_ORDERPRIORITY <> '1-URGENT'
                    AND O_ORDERPRIORITY <> '2-HIGH'
                    THEN 1
                ELSE 0
            END) AS LOW_LINE_COUNT
        FROM
            orders,
            s.lineitem
        WHERE
            O_ORDERKEY = L_ORDERKEY
            AND L_SHIPMODE IN ('AIR', 'SHIP')
            AND L_COMMITDATE < L_RECEIPTDATE
            AND L_SHIPDATE < L_COMMITDATE
            AND L_RECEIPTDATE >= date '1994-01-01'
            AND L_RECEIPTDATE < date '1994-01-01' + INTERVAL '1' YEAR
        GROUP BY
            L_SHIPMODE
        ORDER BY
            L_SHIPMODE;		
        ";

        public static string Q13 =>
            @"
        select 
        	c_count, 
        	count(*) as custdist 
        from (
        	select 
        		c_custkey, 
        		count(o_orderkey) as c_count 
        	from 
        		s.customer left outer join s.orders 
        			on c_custkey = o_custkey and 
        			o_comment not like '%pending%deposits%' 
        	group by c_custkey
        	) c_orders
        group by c_count 
        order by custdist desc, c_count desc;

		";

        public static string Q13_clusterix =>
            @"
        SELECT
            C_COUNT,
            COUNT(*) AS CUSTDIST
        FROM
            (
                SELECT
                    C_CUSTKEY,
                    COUNT(O_ORDERKEY) AS C_COUNT
                FROM
                    s.customer LEFT OUTER JOIN orders ON
                        C_CUSTKEY = O_CUSTKEY
                        AND O_COMMENT NOT LIKE '%SPECIAL%REQUESTS%'
                GROUP BY
                    C_CUSTKEY
            ) AS C_ORDERS
        GROUP BY
            C_COUNT
        ORDER BY
            CUSTDIST DESC,
            C_COUNT DESC;		
        ";

        public static string Q14 =>
            @"
        select 
        	100.00 * 
        	sum(
        		case 
        			when p_type like 'PROMO%' 
        			then l_extendedprice * (1 - l_discount) 
        			else 0 
        		end)
        			/
        	sum(
        		l_extendedprice * (1 - l_discount)
        	) as promo_revenue 
        from 
        	s.lineitem, 
        	s.part 
        where 
        	l_partkey = p_partkey and 
        	l_shipdate >= date '1996-12-01' and 
        	l_shipdate < date '1996-12-01' + interval '1' month;

		";

        public static string Q14_clusterix =>
            @"
        SELECT
            100.00 * SUM(CASE
                WHEN P_TYPE LIKE 'PROMO%'
                    THEN L_EXTENDEDPRICE * (1 - L_DISCOUNT)
                ELSE 0
            END) / SUM(L_EXTENDEDPRICE * (1 - L_DISCOUNT)) AS PROMO_REVENUE
        FROM
            s.lineitem,
            part
        WHERE
            L_PARTKEY = P_PARTKEY
            AND L_SHIPDATE >= date '1995-01-01'
            AND L_SHIPDATE < date '1995-01-01' + INTERVAL '1' MONTH;
        ";

        //Обсудить
        public static string Q15 =>
            @"
        create view REVENUE0 (supplier_no, total_revenue) as 
        select
            l_suppkey, 
            sum(l_extendedprice * (1 - l_discount)) 
        from 
            s.lineitem 
        where 
            l_shipdate >= date '1997-07-01' and 
            l_shipdate < date '1997-07-01' + interval '3' month 
        group by l_suppkey; 
        select s_suppkey, s_name, s_address, s_phone, total_revenue 
        from supplier, REVENUE0 
        where 
            s_suppkey = supplier_no and 
            total_revenue = ( 
                select max(total_revenue) 
                from 
                    REVENUE0
             ) 
        order by s_suppkey; 
        drop view REVENUE0;
            ";
        
        public static string Q15_NoView =>
            @"       
        select
            l_suppkey, 
            sum(l_extendedprice * (1 - l_discount)) 
        from 
            s.lineitem 
        where 
            l_shipdate >= date '1997-07-01' and 
            l_shipdate < date '1997-07-01' + interval '3' month 
        group by l_suppkey; 
        select s_suppkey, s_name, s_address, s_phone, total_revenue 
        from s.supplier, REVENUE0 
        where 
            s_suppkey = supplier_no and 
            total_revenue = ( 
                select max(total_revenue) 
                from 
                    REVENUE0
             ) 
        order by s_suppkey;
            ";

        public static string Q16 =>
            @"
        select 
            p_brand, 
            p_type, 
            p_size, 
            count(distinct ps_suppkey) as supplier_cnt 
        from 
            s.partsupp, 
            s.part 
        where 
            p_partkey = ps_partkey and 
            p_brand <> 'Brand#34' and 
            p_type not like 'LARGE BRUSHED%' and 
            p_size in (48, 19, 12, 4, 41, 7, 21, 39) and 
            ps_suppkey not in (
                select s_suppkey 
                from s.supplier 
                where 
                    s_comment like '%s.customer%Complaints%'
            )
        group by 
            p_brand, 
            p_type, 
            p_size 
        order by 
            supplier_cnt desc,
            p_brand, 
            p_type, 
            p_size;
        ";

        public static string Q17 =>
            @"
            select 
                sum(l_extendedprice) / 7.0 as avg_yearly 
            from 
                s.lineitem, 
                s.part 
            where 
                p_partkey = l_partkey and 
                p_brand = 'Brand#44' and 
                p_container = 'WRAP PKG' and 
                l_quantity < (
                    select 0.2 * avg(l_quantity) 
                    from s.lineitem 
                    where l_partkey = p_partkey
                );            
            ";

        public static string Q18 =>
            @"
        select 
            c_name, 
            c_custkey, 
            o_orderkey,  
            o_orderdate, 
            o_totalprice,  
            sum(l_quantity) 
        from 
            s.customer,  
            s.orders,  
            s.lineitem 
        where 
            o_orderkey in (
                select 
                    l_orderkey 
                from s.lineitem 
                group by l_orderkey having sum(l_quantity) > 314
            ) and 
            c_custkey = o_custkey and  
            o_orderkey = l_orderkey 
        group by 
            c_name, 
            c_custkey, 
            o_orderkey, 
            o_orderdate, 
            o_totalprice 
        order by 
            o_totalprice desc, 
            o_orderdate limit 100;
            ";

        //непонятно
        public static string Q19 =>
            @"
        select 
            sum(l_extendedprice* (1 - l_discount)) as revenue 
        from 
            s.lineitem, 
            s.part 
        where 
            (p_partkey = l_partkey and 
            p_brand = 'Brand#52' and 
            p_container in ('SM CASE', 'SM BOX', 'SM PACK', 'SM PKG') and 
            l_quantity >= 4 and     
            l_quantity <= 4 + 10 and 
            p_size between 1 and 5 and 
            l_shipmode in ('AIR', 'AIR REG') and 
            l_shipinstruct = 'DELIVER IN PERSON'
            ) or 
            (p_partkey = l_partkey and 
             p_brand = 'Brand#11' and 
             p_container in ('MED BAG', 'MED BOX', 'MED PKG', 'MED PACK') and 
             l_quantity >= 18 and 
             l_quantity <= 18 + 10 and 
             p_size between 1 and 10 and 
             l_shipmode in ('AIR', 'AIR REG') and 
             l_shipinstruct = 'DELIVER IN PERSON' 
            ) or 
            (p_partkey = l_partkey and 
             p_brand = 'Brand#51' and 
             p_container in ('LG CASE', 'LG BOX', 'LG PACK', 'LG PKG') and 
             l_quantity >= 29 and 
             l_quantity <= 29 + 10 and 
             p_size between 1 and 15 
             and l_shipmode in ('AIR', 'AIR REG') and 
             l_shipinstruct = 'DELIVER IN PERSON'
            );";

        public static string Q20 =>
            @"
            select 
                s_name,  
                s_address  
            from  
                s.supplier,  
                s.nation  
            where  
                s_suppkey in ( 
                    select ps_suppkey 
                    from s.partsupp 
                    where
                        ps_partkey in (
                            select p_partkey 
                            from s.part 
                            where p_name like 'green%'
                        ) and 
                        ps_availqty > (
                            select 0.5 * sum(l_quantity) 
                            from s.lineitem 
                            where 
                                l_partkey = ps_partkey and 
                                l_suppkey = ps_suppkey and 
                                l_shipdate >= date '1993-01-01' and 
                                l_shipdate < date '1993-01-01' + interval '1' year
                        )
                ) and 
                s_nationkey = n_nationkey and 
                n_name = 'ALGERIA' 
            order by s_name;
            ";

        //непонятно
        public static string Q21 =>
            @"
            select
                s_name, 
                count(*) as numwait 
            from 
                s.supplier,  
                s.lineitem l1, 
                s.orders, 
                s.nation 
            where 
                s_suppkey = l1.l_suppkey and   
                o_orderkey = l1.l_orderkey and  
                o_orderstatus = 'F' and  
                l1.l_receiptdate > l1.l_commitdate and  
                exists (  
                    select * 
                    from s.lineitem l2 
                    where 
                        l2.l_orderkey = l1.l_orderkey and 
                        l2.l_suppkey <> l1.l_suppkey
                ) and 
                not exists (
                    select * 
                    from s.lineitem l3 
                    where 
                        l3.l_orderkey = l1.l_orderkey and  
                        l3.l_suppkey <> l1.l_suppkey and  
                        l3.l_receiptdate > l3.l_commitdate
                ) and  
                s_nationkey = n_nationkey and  
                n_name = 'EGYPT'  
            group by s_name 
            order by 
                numwait desc, 
                s_name limit 100;";

        public static string Q22 =>
            @"
            select 
                cntrycode, 
                count(*) as numcust,  
                sum(c_acctbal) as totacctbal  
            from (
                select 
                    substring (c_phone from 1 for 2) as cntrycode, 
                    c_acctbal 
                from s.customer 
                where 
                    substring(c_phone from 1 for 2) in ('20', '40', '22', '30', '39', '42', '21') and 
                    c_acctbal > ( 
                        select avg(c_acctbal) 
                        from s.customer 
                        where c_acctbal > 0.00 and 
                        substring(c_phone from 1 for 2) in ('20', '40', '22', '30', '39', '42', '21')
                    ) and 
                    not exists ( 
                        select * 
                        from s.orders 
                        where 
                            o_custkey = c_custkey
                    )
            ) as custsale 
            group by cntrycode 
            order by cntrycode;    
            ";


        #endregion

        //132-48=84
        private static int[] tpchFullTestSequence = new int[] {
            21, 3, 18, 5, 11, 7, 6, 20, 17, 12, 16, 15, 13, 10, 2, 8, 14, 19, 9, 22, 1, 4,
            6, 17, 14, 16, 19, 10, 9, 2, 15, 8, 5, 22, 12, 7, 13, 18, 1, 4, 20, 3, 11, 21,
            8, 5, 4, 6, 17, 7, 1, 18, 22, 14, 9, 10, 15, 11, 20, 2, 21, 19, 13, 16, 12, 3,
            5, 21, 14, 19, 15, 17, 12, 6, 4, 9, 8, 16, 11, 2, 10, 18, 1, 13, 7, 22, 3, 20,
            21, 15, 4, 6, 7, 16, 19, 18, 14, 22, 11, 13, 3, 1, 2, 5, 8, 20, 12, 17, 10, 9,
            10, 3, 15, 13, 6, 8, 9, 7, 4, 11, 22, 18, 12, 1, 5, 16, 2, 14, 19, 20, 17, 21,
            //18, 8, 20, 21, 2, 4, 22, 17, 1, 11, 9, 19, 3, 13, 5, 7, 10, 16, 6, 14, 15, 12,
            //19, 1, 15, 17, 5, 8, 9, 12, 14, 7, 4, 3, 20, 16, 6, 22, 10, 13, 2, 21, 18, 11,
            //8, 13, 2, 20, 17, 3, 6, 21, 18, 11, 19, 10, 15, 4, 22, 1, 7, 12, 9, 14, 5, 16,
            //6, 15, 18, 17, 12, 1, 7, 2, 22, 13, 21, 10, 14, 9, 3, 16, 20, 19, 11, 4, 8, 5,
            //15, 14, 18, 17, 10, 20, 16, 11, 1, 8, 4, 22, 5, 12, 3, 9, 21, 2, 13, 6, 19, 7,
            //1, 7, 16, 17, 18, 22, 12, 6, 8, 9, 11, 4, 2, 5, 20, 21, 13, 10, 19, 3, 14, 15,
            //21, 17, 7, 3, 1, 10, 12, 22, 9, 16, 6, 11, 2, 4, 5, 14, 8, 20, 13, 18, 15, 19,
            //2, 9, 5, 4, 18, 1, 20, 15, 16, 17, 7, 21, 13, 14, 19, 8, 22, 11, 10, 3, 12, 6,
            //16, 9, 17, 8, 14, 11, 10, 12, 6, 21, 7, 3, 15, 5, 22, 20, 1, 13, 19, 2, 4, 18,
            //1, 3, 6, 5, 2, 16, 14, 22, 17, 20, 4, 9, 10, 11, 15, 8, 12, 19, 18, 13, 7, 21,
            //3, 16, 5, 11, 21, 9, 2, 15, 10, 18, 17, 7, 8, 19, 14, 13, 1, 4, 22, 20, 6, 12,
            //14, 4, 13, 5, 21, 11, 8, 6, 3, 17, 2, 20, 1, 19, 10, 9, 12, 18, 15, 7, 22, 16,
            //4, 12, 22, 14, 5, 15, 16, 2, 8, 10, 17, 9, 21, 7, 3, 6, 13, 18, 11, 20, 19, 1,
            //16, 15, 14, 13, 4, 22, 18, 19, 7, 1, 12, 17, 5, 10, 20, 3, 9, 21, 11, 2, 6, 8,
            //20, 14, 21, 12, 15, 17, 4, 19, 13, 10, 11, 1, 16, 5, 18, 7, 8, 22, 9, 6, 3, 2,
            //16, 14, 13, 2, 21, 10, 11, 4, 1, 22, 18, 12, 19, 5, 7, 8, 6, 3, 15, 20, 9, 17,
            //18, 15, 9, 14, 12, 2, 8, 11, 22, 21, 16, 1, 6, 17, 5, 10, 19, 4, 20, 13, 3, 7,
            //7, 3, 10, 14, 13, 21, 18, 6, 20, 4, 9, 8, 22, 15, 2, 1, 5, 12, 19, 17, 11, 16,
            //18, 1, 13, 7, 16, 10, 14, 2, 19, 5, 21, 11, 22, 15, 8, 17, 20, 3, 4, 12, 6, 9,
            //13, 2, 22, 5, 11, 21, 20, 14, 7, 10, 4, 9, 19, 18, 6, 3, 1, 8, 15, 12, 17, 16,
            //14, 17, 21, 8, 2, 9, 6, 4, 5, 13, 22, 7, 15, 3, 1, 18, 16, 11, 10, 12, 20, 19,
            //10, 22, 1, 12, 13, 18, 21, 20, 2, 14, 16, 7, 15, 3, 4, 17, 5, 19, 6, 8, 9, 11,
            //10, 8, 9, 18, 12, 6, 1, 5, 20, 11, 17, 22, 16, 3, 13, 2, 15, 21, 14, 19, 7, 4,
            //7, 17, 22, 5, 3, 10, 13, 18, 9, 1, 14, 15, 21, 19, 16, 12, 8, 6, 11, 20, 4, 2,
            //2, 9, 21, 3, 4, 7, 1, 11, 16, 5, 20, 19, 18, 8, 17, 13, 10, 12, 15, 6, 14, 22,
            //15, 12, 8, 4, 22, 13, 16, 17, 18, 3, 7, 5, 6, 1, 9, 11, 21, 10, 14, 20, 19, 2,
            //15, 16, 2, 11, 17, 7, 5, 14, 20, 4, 21, 3, 10, 9, 12, 8, 13, 6, 18, 19, 22, 1,
            //1, 13, 11, 3, 4, 21, 6, 14, 15, 22, 18, 9, 7, 5, 10, 20, 12, 16, 17, 8, 19, 2,
            //14, 17, 22, 20, 8, 16, 5, 10, 1, 13, 2, 21, 12, 9, 4, 18, 3, 7, 6, 19, 15, 11,
            //9, 17, 7, 4, 5, 13, 21, 18, 11, 3, 22, 1, 6, 16, 20, 14, 15, 10, 8, 2, 12, 19,
            //13, 14, 5, 22, 19, 11, 9, 6, 18, 15, 8, 10, 7, 4, 17, 16, 3, 1, 12, 2, 21, 20,
            //20, 5, 4, 14, 11, 1, 6, 16, 8, 22, 7, 3, 2, 12, 21, 19, 17, 13, 10, 15, 18, 9,
            //3, 7, 14, 15, 6, 5, 21, 20, 18, 10, 4, 16, 19, 1, 13, 9, 8, 17, 11, 12, 22, 2,
            //13, 15, 17, 1, 22, 11, 3, 4, 7, 20, 14, 21, 9, 8, 2, 18, 16, 6, 10, 12, 5, 19

            };

        private static int tpchAllQueriesCount = 22;
        private static int tpchClusterixQueriesCount = 14;

        public static string[] GetFullTest()
        {
            List<string> fullTest = new List<string>();
            for (int i = 0; i < tpchFullTestSequence.Length; i++)
            {
                fullTest.Add(GetQuery(tpchFullTestSequence[i]));
            }
            return fullTest.ToArray();
        }

        public static string[] GetFullClusterixTest()
        {
            List<string> fullClusterixTest = new List<string>();
            for (int i = 0; i < tpchFullTestSequence.Length; i++)
            {
                if (tpchFullTestSequence[i] < 15)
                {
                    fullClusterixTest.Add( GetQuery(tpchFullTestSequence[i]));
                }
            }
            return fullClusterixTest.ToArray();
        }

        public static string[] GetPossibilityTest()
        {
            List<string> possibilityTest = new List<string>();
            for (int i = 0; i < tpchAllQueriesCount; i++)
            {
                possibilityTest.Add(GetQuery(i+1));
            }
            return possibilityTest.ToArray();
        }


        public static string[] GetClusterixPossibilityTest()
        {
            List<string> clusterixPossibilityTest = new List<string>();
            for (int i = 0; i < tpchClusterixQueriesCount; i++)
            {
                clusterixPossibilityTest.Add(GetQuery(i+1));
            }
            return clusterixPossibilityTest.ToArray();
            
        }

        public static string GetQuery(double number)
        {
            switch (number)
            {
                case 1: return Q1;
                case 2: return Q2;
                case 3: return Q3;
                case 4: return Q4;
                case 4.1: return Q4_NoExist;
                case 5: return Q5;
                case 6: return Q6;
                case 7: return Q7;
                case 8: return Q8;
                case 9: return Q9;
                case 10: return Q10;
                case 11: return Q11;
                case 12: return Q12;
                case 13: return Q13;
                case 14: return Q14;
                case 15: return Q15;
                case 15.1: return Q15_NoView;
                case 16: return Q16;
                case 17: return Q17;
                case 18: return Q18;
                case 19: return Q19;
                case 20: return Q20;
                case 21: return Q21;
                case 22: return Q22;
                default: return "SELECT * FROM s.nation;";
            }
        }
    }
}
