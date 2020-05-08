using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNoteService.Model;
using System;
using System.Collections.Generic;

namespace MyNoteService.DataLayer.SQL.Test
{
    [TestClass]
    public class TagRepositoryTest
    {
        private string _connectionString = "Data Source=DESKTOP-H9KPRLI;Initial Catalog=notesdb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private List<int> createdTagIDs = new List<int>();

        [TestMethod]
        public void CreateEntity_AddTag_TagWhithActualIdReturned()
        {
            // arrange
            var tagRepository = new TsqlTagRepository(_connectionString);
            var testedTag = new Tag()
            {
                TagID = -1,
                TagName = "createTestTag"
            };

            // act
            var addedTag = tagRepository.CreateEntity(testedTag);

            // assert
            Assert.AreEqual("createTestTag", addedTag.TagName);
            Assert.AreNotEqual(-1, addedTag.TagID);
            createdTagIDs.Add(addedTag.TagID);
        }

        [TestMethod]
        public void DeleteEntity_RemoveTag()
        {
            // arrange
            var tagRepository = new TsqlTagRepository(_connectionString);
            var testedTag = new Tag()
            {
                TagID = -1,
                TagName = "deleteTestTag"
            };
            testedTag = tagRepository.CreateEntity(testedTag);
            bool flag;

            // act
            try
            {
                tagRepository.DeleteEntity(testedTag.TagID);
                flag = true;
            }
            catch (Exception)
            {
                flag = false;
            }

            // assert
            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void EditEntity_EditTagName()
        {
            // arrange
            var tagRepository = new TsqlTagRepository(_connectionString);
            var testedTag = new Tag()
            {
                TagID = -1,
                TagName = "editTestTag"
            };
            testedTag = tagRepository.CreateEntity(testedTag);
            createdTagIDs.Add(testedTag.TagID);


            // act
            testedTag.TagName = "Edited_editTestTag";
            tagRepository.EditEntity(testedTag);
            testedTag = tagRepository.GetEntityByID(testedTag.TagID);

            // assert
            Assert.AreEqual("Edited_editTestTag", testedTag.TagName);
        }

        [TestCleanup]
        public void Clean()
        {
            var tagRepository = new TsqlTagRepository(_connectionString);
            foreach(var id in createdTagIDs)
            {
                tagRepository.DeleteEntity(id);
            }
        }
    }
}
