using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNoteService.DataLayer.Helpers
{
    public static class DataHelper
    {
        /// <summary>
        /// Метод для определения добавленных и удалённых элементов в коллекции
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldValues">Старая версия коллекции</param>
        /// <param name="newValues">Новая версия коллекции</param>
        /// <returns>Возвращает коллекцию элементов которые были удалены и коллекцию добавленных элементов</returns>
        public static (IEnumerable<T>, IEnumerable<T>) GetDeletedAndAdded<T>(IEnumerable<T> oldValues, IEnumerable<T> newValues)
        {
            oldValues ??= Enumerable.Empty<T>();
            newValues ??= Enumerable.Empty<T>();

            var deletedValues = oldValues.Except(newValues).ToList();
            var addedValues = newValues.Except(oldValues).ToList();
            return (deletedValues, addedValues);
        }
    }
}
