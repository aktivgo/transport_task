using System;
using System.Collections.Generic;

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

            for (int i = 0; i < reserves.Count; i++)
            {
                sumReserves += reserves[i];
                sumNeeds += needs[i];
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
                consumer.Add(new KeyValuePair<int, int>(0, 0));
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
                supplier.Add(new KeyValuePair<int, int>(0, 0));
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

            return plan;
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