#region Copyright
/*
 * Copyright 2017 Roman Klassen
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
using System.Linq;

namespace DbBenchmark
{
    public class ConnectionStringParserBase
    {
        protected static string GetValue(string connectionString, params string[] keyAliases)
        {
            var keyValuePairs = connectionString.Split(';')
                .Where(kvp => kvp.Contains('='))
                .Select(kvp => kvp.Split(new[] {'='}, 2))
                .ToDictionary(kvp => kvp[0].Trim(),
                    kvp => kvp[1].Trim(),
                    StringComparer.InvariantCultureIgnoreCase);
            foreach (var alias in keyAliases)
            {
                string value;
                if (keyValuePairs.TryGetValue(alias, out value))
                    return value;
            }
            return string.Empty;
        }
    }
}