#region Copyright
/*
 * Copyright 2021 Roman Klassen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DbBenchmark.Calcite
{
    public class Database
    {
        public Database(string connectionString)
        {
            ConnectionString = connectionString;
        }


        public string ConnectionString { get; protected set; }

        private Process StartCalciteProcess(string query)
        {
            var arguments = new StringBuilder(ConnectionString.Length + query.Length + 100);
            var adapterArgs = ConnectionStringParser.GetAdapterRunArgs(ConnectionString);
            if (!string.IsNullOrWhiteSpace(adapterArgs))
            {
                arguments.Append(adapterArgs);
                arguments.Append(" ");
            }
            
            arguments.Append("\"");
            arguments.Append(ConnectionString);

            var schema = ConnectionStringParser.GetSchema(ConnectionString);
            if (string.IsNullOrWhiteSpace(schema))
            {
                schema = "s";
                arguments.Append(";Schema="+schema);
            }

            arguments.Append(";Query=");
            arguments.Append(InsertSchemaToQuery(schema, query));
            arguments.Append("\"");
            //Console.WriteLine(arguments.ToString().Split(";")[7]);
            var processInfo = new ProcessStartInfo(ConnectionStringParser.GetAdapterFile(ConnectionString))
            {
                Arguments = arguments.ToString(),
                CreateNoWindow = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = ConnectionStringParser.GetWorkingDirectory(ConnectionString)
            };

            return Process.Start(processInfo);
        }

        private string InsertSchemaToQuery(string schema, string query)
        {
            if (query.Contains(";"))
                query = query.Replace(';', ' ');

            foreach (var c in new char[]{'\n','\r','\t'})
            {
                query = query.Replace(c, ' ');
            }

            var start = query.ToLower().IndexOf("from");
            if (start < 0) return query;
            start += 4;
            
            var sb = new StringBuilder(query.Length + 20);
            sb.Append(query.Substring(0, start));

            var end = query.ToLower().IndexOf("where");
            var part = end > 0 ? query.Substring(start, end - start) : query.Substring(start);
            

            if (part.Contains(schema + ".")) return query; //схема уже содержится в запросе
            if (part.ToLower().Contains("join")) return query; //управление происходит в менеджере отношений

            var tables = part.Split(',').Select(x => x.Trim()).ToArray();
            for (var i = 0; i < tables.Length; i++)
            {
                tables[i] = $"{schema}.{tables[i]}";
            }

            sb.Append(" ");
            sb.Append(string.Join(" ,", tables));

            if (end > 0)
            {
                sb.Append(" ");
                sb.Append(query.Substring(end));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Выборка по блокам из БД
        /// </summary>
        /// <param name="query">Запрос к БД</param>
        /// <param name="blockSize">Размер блока в строках</param>
        public void SelectBlocks(string query, int blockSize)
        {

            Process process = null;
            try
            {
                process = StartCalciteProcess(query);
                //process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
                //process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                //process.BeginErrorReadLine();
                //process.BeginOutputReadLine();
                var sb = new StringBuilder();
                string line;
                uint count = 0;
                var sendcount = 0;
                byte[] sendBuffer = null;

                while (!string.IsNullOrWhiteSpace(line = process?.StandardOutput.ReadLine()))
                {
                    sb.AppendLine(line);
                    count++;

                    if (count % blockSize == 0) //отправка блока
                    {
                        if (sendBuffer != null)
                        {
                            OnBlockReaded(sendBuffer, orderNumber: sendcount++);
                        }

                        var dest = new char[sb.Length];
                        sb.CopyTo(0, dest, 0,sb.Length);
                        sendBuffer = Encoding.UTF8.GetBytes(dest);
                        sb = new StringBuilder();
                    }
                }

                //отправка последнего блока
                if (sb.Length > 0)
                {
                    if (sendBuffer != null)
                    {
                        OnBlockReaded(sendBuffer, orderNumber: sendcount++);
                    }
                    OnBlockReaded(Encoding.UTF8.GetBytes(sb.Remove(sb.Length - 1, 1).ToString()), true, sendcount);
                }
                else
                {
                    OnBlockReaded(sendBuffer ?? new byte[0], true, sendcount);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.ToString());
            }
            finally
            {
                //Console.WriteLine(process?.StandardOutput.ReadToEnd());
                Console.WriteLine(process?.StandardError.ReadToEnd());
                //process?.CancelErrorRead();
                process?.Close();
            }
        }




        #region Events

        
        protected void OnBlockReaded(byte[] rows, bool isLast = false, int orderNumber = 0)
        {
        }

        #endregion

    }
}