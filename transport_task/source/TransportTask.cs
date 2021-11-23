using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;

namespace transport_task.source
{
    public class TransportTask
    {
        private TransportTable _transportTable;

        public TransportTask()
        {
            _transportTable = new TransportTable();
        }

        public TransportTask(TransportTable transportTable)
        {
            _transportTable = transportTable ?? throw new ArgumentNullException("Устанавливаемая таблица пустая");
        }

        /// <summary>
        /// Возвращает транспортную таблицу
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public TransportTable GetTransportTable()
        {
            return _transportTable ?? throw new ArgumentNullException("Таблица не была установлена");
        }

        /// <summary>
        /// Устанавливает транспортную таблицу
        /// </summary>
        /// <param name="transportTable"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetTransportTable(TransportTable transportTable)
        {
            _transportTable = transportTable ?? throw new ArgumentNullException("Устанавливаемая таблица пустая");
        }

        /// <summary>
        /// Решает задачу
        /// </summary>
        /// <param name="initialPlanMethod"></param>
        public void Solve(string initialPlanMethod)
        {
            Console.WriteLine("Исходная задача");
            PrintTransportTable(_transportTable);
            Console.WriteLine();
            Preprocessing();
            TransportTable initialPlan = CalculateInitialPlan(initialPlanMethod);

            List<KeyValuePair<int, int>> filledCells = initialPlan.GetFilledCells();

            int dif = Math.Abs(filledCells.Count - initialPlan.GetNeeds().Count - initialPlan.GetReserves().Count + 1);
            if (dif > 0)
            {
                Console.WriteLine("План вырожденный, добавляем базисные нули");
                for (int i = 0; i < dif; i++)
                {
                    // Добавляем базисные нули
                    KeyValuePair<int, int> indexMinEl = GetIndexOfMinPriceEl(initialPlan);
                    int price = initialPlan[indexMinEl.Key][indexMinEl.Value].Key;
                    initialPlan[indexMinEl.Key][indexMinEl.Value] = new KeyValuePair<int, int>(price, 0);
                    
                    PrintTransportTable(initialPlan, indexMinEl.Key, indexMinEl.Value);

                }
                Console.WriteLine("План невырожденный");
            }

            Console.WriteLine("Проверка на оптимальность");
            List<List<int>> potentials = new List<List<int>>();
            if (!IsOptimal(initialPlan, ref potentials))
            {
                PrintPotentials(potentials);
                Console.WriteLine("План не оптимален");
            }
            else
            {
                Console.WriteLine("План оптимален");
            }
            
            /*while (!IsOptimal(initialPlan, ref potentials))
            {
                
                KeyValuePair<int, int> indexMaxEl = GetIndexOfMaxPotential(potentials);
                List<KeyValuePair<int, int>> loop = GetLoopByRoot(initialPlan, indexMaxEl);
            }*/
        }

        private void PrintPotentials(List<List<int>> potentials)
        {
            KeyValuePair<int, int> indexMaxEl = GetIndexOfMaxPotential(potentials);

            for (int i = 0; i < potentials.Count; i++)
            {
                for (int j = 0; j < potentials[i].Count; j++)
                {
                    if (i == indexMaxEl.Key && j == indexMaxEl.Value)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    
                    Console.Write("{0,5}", potentials[i][j] + " ");

                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        private List<KeyValuePair<int, int>> GetLoopByRoot(TransportTable transportTable,
            KeyValuePair<int, int> indexMaxEl)
        {
            List<KeyValuePair<int, int>> loop = new List<KeyValuePair<int, int>>();

            return loop;
        }

        private KeyValuePair<int, int> GetIndexOfMaxPotential(List<List<int>> potentials)
        {
            int max = int.MinValue;
            int iIndex = -1;
            int jIndex = -1;
            for (int i = 0; i < potentials.Count; i++)
            {
                for (int j = 0; j < potentials[i].Count; j++)
                {
                    if (potentials[i][j] > max)
                    {
                        max = potentials[i][j];
                        iIndex = i;
                        jIndex = j;
                    }
                }
            }

            return new KeyValuePair<int, int>(iIndex, jIndex);
        }

        private bool IsOptimal(TransportTable transportTable, ref List<List<int>> potentials)
        {
            List<KeyValuePair<KeyValuePair<int, int>, int>> equations =
                new List<KeyValuePair<KeyValuePair<int, int>, int>>();
            for (int i = 0; i < transportTable.Count; i++)
            {
                for (int j = 0; j < transportTable[i].Count; j++)
                {
                    if (transportTable[i][j].Value == -1)
                    {
                        continue;
                    }

                    KeyValuePair<int, int> index = new KeyValuePair<int, int>(i, j);
                    equations.Add(new KeyValuePair<KeyValuePair<int, int>, int>(index, transportTable[i][j].Key));
                }
            }

            List<int> u = new List<int>();
            for (int i = 0; i < transportTable.GetReserves().Count; i++)
            {
                u.Add(-1);
            }

            List<int> v = new List<int>();
            for (int i = 0; i < transportTable.GetNeeds().Count; i++)
            {
                v.Add(-1);
            }
            
            u[0] = 0;

            PrintSystemEquations(equations);
            PrintUAndV(u, v);
            while (!IsPotentialsSolved(u, v))
            {
                foreach (var item in equations)
                {
                    int uIndex = item.Key.Key;
                    int vIndex = item.Key.Value;
                    int c = item.Value;

                    if (u[uIndex] == -1 && v[vIndex] == -1 || u[uIndex] != -1 && v[vIndex] != -1)
                    {
                        continue;
                    }

                    if (u[uIndex] != -1)
                    {
                        v[vIndex] = c - u[uIndex];
                    }
                    else if (v[vIndex] != -1)
                    {
                        u[uIndex] = c - v[vIndex];
                    }
                }
            }
            
            PrintUAndV(u, v);

            bool negative = false;
            for (int i = 0; i < transportTable.Count; i++)
            {
                potentials.Add(new List<int>());
                for (int j = 0; j < transportTable[i].Count; j++)
                {
                    int result = u[i] + v[j] - transportTable[i][j].Key;
                    if (result < 0)
                    {
                        negative = true;
                    }
                    potentials[i].Add(result);
                }
            }

            return negative == false;
        }

        private void PrintUAndV(List<int> ints, List<int> list)
        {
            Console.Write("U: ");
            foreach (var item in ints)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
            
            Console.Write("V: ");
            foreach (var item in list)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }

        private void PrintSystemEquations(List<KeyValuePair<KeyValuePair<int, int>, int>> equations)
        {
            foreach (var item in equations)
            {
                Console.WriteLine("u" + (item.Key.Key + 1) + " + v" + (item.Key.Value + 1) + " = " + item.Value);
            }
        }

        private bool IsPotentialsSolved(List<int> u, List<int> v)
        {
            foreach (var item in u)
            {
                if (item == -1)
                {
                    return false;
                }
            }

            foreach (var item in v)
            {
                if (item == -1)
                {
                    return false;
                }
            }

            return true;
        }

        private KeyValuePair<int, int> GetIndexOfMinPriceEl(TransportTable transportTable)
        {
            int minEl = int.MaxValue;
            int indexI = -1, indexJ = -1;

            for (int i = 0; i < transportTable.Count; i++)
            {
                for (int j = 0; j < transportTable[i].Count; j++)
                {
                    if (transportTable[i][j].Key != 0 && transportTable[i][j].Value == -1 &&
                        transportTable[i][j].Key < minEl)
                    {
                        minEl = transportTable[i][j].Key;
                        indexI = i;
                        indexJ = j;
                    }
                }
            }

            return new KeyValuePair<int, int>(indexI, indexJ);
        }

        /// <summary>
        /// Производит предобработку решения задачи
        /// </summary>
        private void Preprocessing()
        {
            int sumReserves = 0;
            int sumNeeds = 0;
            if (!IsBalanced(ref sumReserves, ref sumNeeds))
            {
                Console.WriteLine("Задача несбалансированная (" + sumReserves + " != " + sumNeeds + ")");
                AddDummy(sumReserves, sumNeeds);
                PrintTransportTable(_transportTable);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Задача сбалансированная");
            }
        }

        /// <summary>
        /// Проверяет задачу на сбалансированность
        /// </summary>
        /// <param name="sumReserves"></param>
        /// <param name="sumNeeds"></param>
        /// <returns></returns>
        private bool IsBalanced(ref int sumReserves, ref int sumNeeds)
        {
            List<int> reserves = _transportTable.GetReserves();
            List<int> needs = _transportTable.GetNeeds();

            foreach (var reserve in reserves)
            {
                sumReserves += reserve;
            }

            foreach (var need in needs)
            {
                sumNeeds += need;
            }

            return sumReserves == sumNeeds;
        }

        /// <summary>
        /// Добавление фиктивного поставщика или потребителя
        /// </summary>
        private void AddDummy(int sumReserves, int sumNeeds)
        {
            if (sumReserves > sumNeeds)
            {
                AddDummyConsumer(sumReserves - sumNeeds);
            }
            else
            {
                AddDummySupplier(sumNeeds - sumReserves);
            }
        }

        /// <summary>
        /// Добавляет фиктивного потребителя
        /// </summary>
        private void AddDummyConsumer(int need)
        {
            List<KeyValuePair<int, int>> consumer = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < _transportTable.Count; i++)
            {
                consumer.Add(new KeyValuePair<int, int>(0, -1));
            }

            _transportTable.AddConsumer(consumer);
            _transportTable.AddNeed(need);
            Console.WriteLine("Добавляем " + _transportTable.GetNeeds().Count + " потребителя");
        }

        /// <summary>
        /// Добавляет фиктивного поставщика
        /// </summary>
        private void AddDummySupplier(int reserve)
        {
            List<KeyValuePair<int, int>> supplier = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < _transportTable.Count; i++)
            {
                supplier.Add(new KeyValuePair<int, int>(0, -1));
            }

            _transportTable.AddSupplier(supplier);
            _transportTable.AddReserve(reserve);
            Console.WriteLine("Добавляем " + _transportTable.GetReserves().Count + " поставщика");
        }

        /// <summary>
        /// Считает начальный план методом северо-западного угла
        /// </summary>
        /// <returns></returns>
        private TransportTable CalculateInitialPlanByNorthwestCorner()
        {
            TransportTable plan = _transportTable;

            Console.WriteLine("Нахождение начального плана методом северо-западного угла");
            PrintTransportTable(plan, 0, 0);
            Console.WriteLine();

            int iteration = 0;
            int i = 0, j = 0;
            while (i < plan.Count || j < plan[0].Count)
            {
                iteration++;
                int price = plan[i][j].Key;
                int reserve = plan.GetReserves()[i];
                int need = plan.GetNeeds()[j];

                plan[i][j] = new KeyValuePair<int, int>(price, Math.Min(reserve, need));

                if (reserve < need)
                {
                    plan.GetReserves()[i] = 0;
                    plan.GetNeeds()[j] -= reserve;
                    i++;
                }
                else if (reserve > need)
                {
                    plan.GetNeeds()[j] = 0;
                    plan.GetReserves()[i] -= need;
                    j++;
                }
                else
                {
                    plan.GetReserves()[i] = 0;
                    plan.GetNeeds()[j] = 0;
                    i++;
                    j++;
                }

                Console.WriteLine("Итерация " + iteration);
                PrintTransportTable(plan, i, j);
                Console.WriteLine();
            }

            return plan;
        }

        private void PrintTransportTable(TransportTable plan)
        {
            Console.Write("{0,7}", " ");
            for (int i = 0; i < plan[0].Count; i++)
            {
                Console.Write("{0,7}", "B" + (i + 1) + " ");
            }

            Console.WriteLine();

            for (int i = 0; i < plan.Count; i++)
            {
                Console.Write("{0,7}", "A" + (i + 1) + " ");

                for (int j = 0; j < plan[i].Count; j++)
                {
                    Console.Write("{0,7}", plan[i][j].Key + "/" + plan[i][j].Value + " ");
                }

                Console.WriteLine("{0,7}", plan.GetReserves()[i]);
            }

            Console.Write("{0,7}", " ");

            foreach (var value in plan.GetNeeds())
            {
                Console.Write("{0,7}", value + " ");
            }

            Console.WriteLine();
        }

        private void PrintTransportTable(TransportTable plan, int indexLine, int indexColumn)
        {
            Console.Write("{0,7}", " ");
            for (int i = 0; i < plan[0].Count; i++)
            {
                Console.Write("{0,7}", "B" + (i + 1) + " ");
            }

            Console.WriteLine();

            for (int i = 0; i < plan.Count; i++)
            {
                Console.Write("{0,7}", "A" + (i + 1) + " ");

                for (int j = 0; j < plan[i].Count; j++)
                {
                    if (i == indexLine && j == indexColumn)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.Write("{0,7}", plan[i][j].Key + "/" + plan[i][j].Value + " ");

                    Console.ResetColor();
                }

                if (i == indexLine)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.WriteLine("{0,7}", plan.GetReserves()[i]);
                Console.ResetColor();
            }

            Console.Write("{0,7}", " ");

            for (int j = 0; j < plan.GetNeeds().Count; j++)
            {
                if (j == indexColumn)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.Write("{0,7}", plan.GetNeeds()[j] + " ");
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Считает начальный план методом минимального элемента
        /// </summary>
        /// <returns></returns>
        private TransportTable CalculateInitialPlanByMinimalElement()
        {
            TransportTable plan = _transportTable;

            int iteration = 0;
            while (!IsSolved(plan))
            {
                iteration++;

                KeyValuePair<int, int> coords = GetIndexOfNotFilledMinPriceEl(plan);

                int price = plan[coords.Key][coords.Value].Key;
                int reserve = plan.GetReserves()[coords.Key];
                int need = plan.GetNeeds()[coords.Value];

                plan[coords.Key][coords.Value] = new KeyValuePair<int, int>(price, Math.Min(reserve, need));

                if (reserve < need)
                {
                    plan.GetReserves()[coords.Key] = 0;
                    plan.GetNeeds()[coords.Value] -= reserve;
                }
                else if (reserve > need)
                {
                    plan.GetNeeds()[coords.Value] = 0;
                    plan.GetReserves()[coords.Key] -= need;
                }
                else
                {
                    plan.GetReserves()[coords.Key] = 0;
                    plan.GetNeeds()[coords.Value] = 0;
                }

                Console.WriteLine("Итерация " + iteration);
                PrintTransportTable(plan, coords.Key, coords.Value);
                Console.WriteLine();
            }

            return plan;
        }

        /// <summary>
        /// Возвращает индекс минимального элемента в таблице
        /// </summary>
        /// <param name="transportTable"></param>
        /// <returns></returns>
        private KeyValuePair<int, int> GetIndexOfNotFilledMinPriceEl(TransportTable transportTable)
        {
            int minEl = int.MaxValue;
            int indexI = -1, indexJ = -1;

            for (int i = 0; i < transportTable.Count; i++)
            {
                if (transportTable.GetReserves()[i] == 0)
                {
                    continue;
                }

                for (int j = 0; j < transportTable[i].Count; j++)
                {
                    if (transportTable.GetNeeds()[j] == 0)
                    {
                        continue;
                    }

                    if (transportTable[i][j].Key != 0 && transportTable[i][j].Value == -1 &&
                        transportTable[i][j].Key < minEl)
                    {
                        minEl = transportTable[i][j].Key;
                        indexI = i;
                        indexJ = j;
                    }
                }
            }

            if (indexI == -1 || indexJ == -1 && !IsSolved(transportTable))
            {
                for (int i = 0; i < transportTable.Count; i++)
                {
                    if (transportTable.GetReserves()[i] == 0)
                    {
                        continue;
                    }

                    for (int j = 0; j < transportTable[i].Count; j++)
                    {
                        if (transportTable.GetNeeds()[j] == 0)
                        {
                            continue;
                        }

                        if (transportTable[i][j].Value == -1 && transportTable[i][j].Key < minEl)
                        {
                            minEl = transportTable[i][j].Key;
                            indexI = i;
                            indexJ = j;
                        }
                    }
                }
            }

            return new KeyValuePair<int, int>(indexI, indexJ);
        }

        /// <summary>
        /// Проверяет задачу на решенность
        /// </summary>
        /// <param name="transportTable"></param>
        /// <returns></returns>
        private bool IsSolved(TransportTable transportTable)
        {
            List<int> reserves = transportTable.GetReserves();
            List<int> needs = transportTable.GetNeeds();

            foreach (var item in reserves)
            {
                if (item != 0)
                {
                    return false;
                }
            }

            foreach (var item in needs)
            {
                if (item != 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Считает начальный план
        /// </summary>
        /// <param name="initialPlanMethod"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private TransportTable CalculateInitialPlan(string initialPlanMethod)
        {
            switch (initialPlanMethod)
            {
                case "NorthwestCorner":
                    return CalculateInitialPlanByNorthwestCorner();
                case "MinimalElement":
                    return CalculateInitialPlanByMinimalElement();
                default: throw new ArgumentException("Неопознанный метод построения начального плана");
            }
        }
    }
}