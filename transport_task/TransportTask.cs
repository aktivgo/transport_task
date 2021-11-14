using System;
using System.Collections.Generic;

namespace transport_task
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

        private bool IsBalanced()
        {
            List<int> reserves = _transportTable.GetReserves();
            List<int> needs = _transportTable.GetNeeds();

            int sumReserves = 0;
            int sumNeeds = 0;
            for (int i = 0; i < reserves.Count; i++)
            {
                sumReserves += reserves[i];
                sumNeeds += needs[i];
            }

            return sumReserves == sumNeeds;
        }
    }
}