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
                AddDummy(sumReserves, sumNeeds);
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
        }

        /// <summary>
        /// Считает начальный план методом северо-западного угла
        /// </summary>
        /// <returns></returns>
        private TransportTable CalculateInitialPlanByNorthwestCorner()
        {
            TransportTable plan = _transportTable;

            return plan;
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