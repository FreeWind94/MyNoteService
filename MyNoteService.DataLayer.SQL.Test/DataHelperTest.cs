using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNoteService.DataLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNoteService.DataLayer.SQL.Test
{
    [TestClass]
    public class DataHelperTest
    {
        [TestMethod]
        public void GetDeletedAndAddedTest()
        {
            var oldValues = new List<int> { 1, 2, 3, 4, 5 };
            var newValues = new List<int> { 3, 4, 5, 6, 7 };

            (var deletedValues, var addedValues) = DataHelper.GetDeletedAndAdded(oldValues, newValues);

            CollectionAssert.AreEqual(new List<int> { 1, 2 }, deletedValues.ToList());
            CollectionAssert.AreEqual(new List<int> { 6, 7 }, addedValues.ToList());
        }

        [TestMethod]
        public void GetDeletedAndAddedTest_NoAdded()
        {
            var oldValues = new List<int> { 1, 2, 3, 4, 5 };
            var newValues = new List<int> { 3, 4, 5 };

            (var deletedValues, var addedValues) = DataHelper.GetDeletedAndAdded(oldValues, newValues);

            CollectionAssert.AreEqual(new List<int> { 1, 2 }, deletedValues.ToList());
            CollectionAssert.AreEqual(new List<int>(), addedValues.ToList());
        }

        [TestMethod]
        public void GetDeletedAndAddedTest_NoDeleted()
        {
            var oldValues = new List<int> { 3, 4, 5 };
            var newValues = new List<int> { 3, 4, 5, 6, 7 };

            (var deletedValues, var addedValues) = DataHelper.GetDeletedAndAdded(oldValues, newValues);

            CollectionAssert.AreEqual(new List<int>(), deletedValues.ToList());
            CollectionAssert.AreEqual(new List<int> { 6, 7 }, addedValues.ToList());
        }
    }
}
