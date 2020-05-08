using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNoteService.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyNoteService.DataLayer.SQL.Test
{
    [TestClass]
    public class UserRepositoryTest
    {
        private string _connectionString = "Data Source=DESKTOP-H9KPRLI;Initial Catalog=notesdb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private List<int> createdUserIDs = new List<int>();

        [TestMethod]
        public void CreateEntity_AddUser_UserWhithActualIdReturned()
        {
            // arrange
            var userRepository = new TsqlUserRepository(_connectionString);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "createTestName",
                UserPassword = "test"
            };

            // act
            var addedUser = userRepository.CreateEntity(testedUser);

            // assert
            Assert.AreEqual("createTestName", addedUser.LoginName);
            Assert.AreEqual("test", addedUser.UserPassword);
            Assert.AreNotEqual(-1, addedUser.UserID);
            createdUserIDs.Add(addedUser.UserID);
        }

        [TestMethod]
        public void DeleteEntity_RemoveUserByID()
        {
            // arrange
            var userRepository = new TsqlUserRepository(_connectionString);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "deleteByIdTestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            bool flag;

            // act
            try
            {
                userRepository.DeleteEntity(testedUser.UserID);
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
        public void DeleteEntity_RemoveUserByLogin()
        {
            // arrange
            var userRepository = new TsqlUserRepository(_connectionString);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "deleteByLoginTest",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            bool flag;

            // act
            try
            {
                userRepository.DeleteEntity("deleteByLoginTest");
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
        public void EditEntity_EditUserNameAndPassword()
        {
            // arrange
            var userRepository = new TsqlUserRepository(_connectionString);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "editTest",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            createdUserIDs.Add(testedUser.UserID);


            // act
            testedUser.LoginName = "Edited_editTest";
            testedUser.UserPassword = "Edited";
            userRepository.EditEntity(testedUser);
            testedUser = userRepository.GetEntityByID(testedUser.UserID);

            // assert
            Assert.AreEqual("Edited_editTest", testedUser.LoginName);
            Assert.AreEqual("Edited", testedUser.UserPassword);
        }

        [TestMethod]
        public void GetEntityByID_GetUserById_UserReturned()
        {
            // arrange
            var userRepository = new TsqlUserRepository(_connectionString);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "getByIdTestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            var testedID = testedUser.UserID;
            createdUserIDs.Add(testedID);

            // act
            var returnedUser = userRepository.GetEntityByID(testedUser.UserID);

            // assert
            Assert.AreEqual(testedID, returnedUser.UserID);
            Assert.AreEqual("getByIdTestName", returnedUser.LoginName);
            Assert.AreEqual("test", returnedUser.UserPassword);
        }

        [TestMethod]
        public void GetEntityByLogin_GetUserByLogin_UserReturned()
        {
            // arrange
            var userRepository = new TsqlUserRepository(_connectionString);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "getByLoginTestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            var testedID = testedUser.UserID;
            createdUserIDs.Add(testedID);

            // act
            var returnedUser = userRepository.GetEntityByLogin("getByLoginTestName");

            // assert
            Assert.AreEqual(testedID, returnedUser.UserID);
            Assert.AreEqual("getByLoginTestName", returnedUser.LoginName);
            Assert.AreEqual("test", returnedUser.UserPassword);
        }

        [TestMethod]
        public void UserAuthorization_ValidAuthorization_UserReturned()
        {
            // arrange
            var userRepository = new TsqlUserRepository(_connectionString);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "authTestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            var testedID = testedUser.UserID;
            createdUserIDs.Add(testedID);

            // act
            var returnedUser = userRepository.UserAuthorization("authTestName", "test");

            // assert
            Assert.AreEqual(testedID, returnedUser.UserID);
            Assert.AreEqual("authTestName", returnedUser.LoginName);
            Assert.AreEqual("test", returnedUser.UserPassword); 
        }

        [TestMethod]
        public void UserAuthorization_InvalidAuthorization_IOExceptionReturned()
        {
            // arrange
            var userRepository = new TsqlUserRepository(_connectionString);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "authTestName2",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            var testedID = testedUser.UserID;
            createdUserIDs.Add(testedID);
            bool flag1 = false, flag2 = false;

            // act
            try
            {
                userRepository.UserAuthorization("authTestName2", "fake");
            }
            catch (IOException)
            {
                flag1 = true;
            }
            try
            {
                userRepository.UserAuthorization("fake", "test");
            }
            catch (IOException)
            {
                flag2 = true;
            }

            // assert
            Assert.IsTrue(flag1);
            Assert.IsTrue(flag2);
        }

        [TestCleanup]
        public void Clean()
        {
            var userRepository = new TsqlUserRepository(_connectionString);
            foreach (var id in createdUserIDs)
            {
                userRepository.DeleteEntity(id);
            }
        }
    }
}
