using System;
using System.Collections.Generic;
using System.Reflection;

namespace transport_task
{
    public class TransportTable
    {
        private List<List<KeyValuePair<int, int>>> _table;
        private List<int> _reserves;
        private List<int> _needs;

        public TransportTable()
        {
            _table = new List<List<KeyValuePair<int, int>>>();
            _reserves = new List<int>();
            _needs = new List<int>();
        }

        public TransportTable(List<List<KeyValuePair<int, int>>> table, List<int> reserves, List<int> needs)
        {
            _table = table ?? throw new ArgumentNullException("Устанавливаемая таблица пустая");
            _reserves = reserves ?? throw new ArgumentNullException("Устанавливаемые запасы пустые");
            _needs = needs ?? throw new ArgumentNullException("Устанавливаемые потребности пустые");
        }

        /// <summary>
        /// Возвращает таблицу
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public List<List<KeyValuePair<int, int>>> GetTable()
        {
            return _table ?? throw new ArgumentNullException("Таблица не была установлена");
        }

        /// <summary>
        /// Устанавливает таблицу
        /// </summary>
        /// <param name="table"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetTable(List<List<KeyValuePair<int, int>>> table)
        {
            _table = table ?? throw new ArgumentNullException("Устанавливаемая таблица пустая");
        }

        /// <summary>
        /// Добавляет поставщика(строку) в таблицу
        /// </summary>
        /// <param name="supplier"></param>
        public void AddSupplier(List<KeyValuePair<int, int>> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException("Устанавливаемый поставщик пустой");
            }

            if (_table.Count != 0 && supplier.Count != _table[0].Count)
            {
                throw new TargetParameterCountException(
                    "Размер устанавливаемого поставщика не совпадает с размером таблицы");
            }

            _table.Add(supplier);
        }

        /// <summary>
        /// Добавляет потребителя(столбец) в таблицу
        /// </summary>
        /// <param name="consumer"></param>
        public void AddConsumer(List<KeyValuePair<int, int>> consumer)
        {
            if (consumer == null)
            {
                throw new ArgumentNullException("Устанавливаемый потребитель пустой");
            }

            if (_table.Count != 0 && consumer.Count != _table.Count)
            {
                throw new TargetParameterCountException(
                    "Размер устанавливаемого потребителя не совпадает с размером таблицы");
            }
        }

        /// <summary>
        /// Возвращает список запасов
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public List<int> GetReserves()
        {
            return _reserves ?? throw new ArgumentNullException("Запасы не были установлены");
        }

        /// <summary>
        /// Устанавливает список запасов
        /// </summary>
        /// <param name="reserves"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetReserves(List<int> reserves)
        {
            _reserves = reserves ?? throw new ArgumentNullException("Устанавливаемые запасы пустые");
        }

        /// <summary>
        /// Добавляет значение запаса в таблицу
        /// </summary>
        /// <param name="value"></param>
        public void AddReserve(int value)
        {
            _reserves.Add(value);
        }

        /// <summary>
        /// Возвращает список потребностей
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public List<int> GetNeeds()
        {
            return _needs ?? throw new ArgumentNullException("Потребности не были установлены");
        }

        /// <summary>
        /// Устанавливает список потребностей
        /// </summary>
        /// <param name="needs"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetNeeds(List<int> needs)
        {
            _needs = needs ?? throw new ArgumentNullException("Устанавливаемые потребности пустые");
        }

        /// <summary>
        /// Добавляет значение потребности в таблицу
        /// </summary>
        /// <param name="value"></param>
        public void AddNeed(int value)
        {
            _needs.Add(value);
        }
    }
}