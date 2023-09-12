using System;
using System.Collections.Generic;
using System.Text;

namespace MyNoteService.DataLayer
{
    /// <summary>
    /// Общий интерфейс репозитория данных
    /// </summary>
    /// <typeparam name="T">Класс модели данных</typeparam>
    public interface IRepository <T>
        where T : class
    {
        /// <summary>
        /// получение всех объектов
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetEntities();  //TODO: иногда этот метод выгружает слишком много данных, подумать что с этим делать (перепиливать интерфейсы? добавить пагинацию)
        
        /// <summary>
        /// получение одного объекта по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetEntityByID(int id);
        
        /// <summary>
        /// создание объекта
        /// </summary>
        /// <param name="item"></param>
        T CreateEntity(T item);
        
        /// <summary>
        /// изменение объекта
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        T EditEntity(T item);
        
        /// <summary>
        /// удаление объекта по id
        /// </summary>
        /// <param name="id"></param>
        void DeleteEntity(int id); 
    }
}
