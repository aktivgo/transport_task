using System;
using System.Collections.Generic;
using System.IO;

namespace transport_task.source
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            TransportTask task = new TransportTask();

            Console.Write("Введите название файла: ");
            string fileName = Console.ReadLine() ?? throw new InvalidOperationException();

            TransportTable table = ReadTransportTable(fileName);
            task.SetTransportTable(table);

            Console.WriteLine("Выберите метод построения начального плана\n" +
                              "1. Северо-западного угла\n" +
                              "2. Минимального элемента");
            int method = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
            task.Solve(method == 1 ? "NorthwestCorner" : "MinimalElement");
        }

        private static TransportTable ReadTransportTable(string fileName)
        {
            TransportTable transportTable = new TransportTable();

            List<List<KeyValuePair<int, int>>> table = new List<List<KeyValuePair<int, int>>>();
            List<int> reserves = new List<int>();
            List<int> needs = new List<int>();

            using (StreamReader sr = new StreamReader("../../data/" + fileName))
            {
                char[] separators = { '\r', '\n' };
                string[] read = sr.ReadToEnd().Split(separators, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < read.Length; i++)
                {
                    string[] values = read[i].Split();

                    if (i == read.Length - 1)
                    {
                        foreach (var value in values)
                        {
                            needs.Add(int.Parse(value));
                        }

                        continue;
                    }

                    List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();
                    for (int j = 0; j < values.Length; j++)
                    {
                        if (j == values.Length - 1)
                        {
                            reserves.Add(int.Parse(values[j]));
                            continue;
                        }

                        pairs.Add(new KeyValuePair<int, int>(int.Parse(values[j]), 0));
                    }

                    table.Add(pairs);
                }
            }

            transportTable.SetTable(table);
            transportTable.SetReserves(reserves);
            transportTable.SetNeeds(needs);

            return transportTable;
        }
    }
}