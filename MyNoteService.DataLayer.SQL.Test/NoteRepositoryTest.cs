using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNoteService.Model;
using MyNoteService.DataLayer;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;

namespace MyNoteService.DataLayer.SQL.Test
{
    [TestClass]
    public class NoteRepositoryTest
    {
        private string _connectionString = "Data Source=DESKTOP-H9KPRLI;Initial Catalog=notesdb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private List<int> createdUserIDs = new List<int>();
        private List<int> createdTagIDs = new List<int>();
        private List<int> createdNoteIDs = new List<int>();

        [TestMethod]
        public void CreateEntity_AddNote_NoteWhithActualIdReturned()
        {
            // arrange
            var tagRepository = new TsqlTagRepository(_connectionString);
            var userRepository = new TsqlUserRepository(_connectionString);
            var noteRepository = new TsqlNoteRepository(_connectionString, userRepository, tagRepository);
            var testedTag = new Tag()
            {
                TagID = -1,
                TagName = "createTestTag"
            };
            testedTag = tagRepository.CreateEntity(testedTag);
            createdTagIDs.Add(testedTag.TagID);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "createTestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            createdUserIDs.Add(testedUser.UserID);

            var testedNote = new Note
            {
                NoteID = -1,
                Aurhor = testedUser,
                Topic = "createTest",
                Text = "text",
                Tags = new Tag[] { testedTag },
                UsersWhithAccess = new User[] { testedUser }
            };

            // act
            var addedNote = noteRepository.CreateEntity(testedNote);

            // assert
            Assert.AreEqual("createTest", addedNote.Topic);
            Assert.AreEqual("text", addedNote.Text);
            Assert.AreNotEqual(-1, addedNote.NoteID);
            createdNoteIDs.Add(addedNote.NoteID);
        }


        [TestMethod]
        public void DeleteEntity_RemoveNote()
        {
            // arrange
            var tagRepository = new TsqlTagRepository(_connectionString);
            var userRepository = new TsqlUserRepository(_connectionString);
            var noteRepository = new TsqlNoteRepository(_connectionString, userRepository, tagRepository);
            var testedTag = new Tag()
            {
                TagID = -1,
                TagName = "deleteTestTag"
            };
            testedTag = tagRepository.CreateEntity(testedTag);
            createdTagIDs.Add(testedTag.TagID);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "deleteTestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            createdUserIDs.Add(testedUser.UserID);

            var testedNote = new Note
            {
                NoteID = -1,
                Aurhor = testedUser,
                Topic = "deleteTest",
                Text = "text",
                Tags = new Tag[] { testedTag },
                UsersWhithAccess = new User[] { testedUser }
            };
            testedNote = noteRepository.CreateEntity(testedNote);
            var testId = testedNote.NoteID;
            createdNoteIDs.Add(testId);
            bool flag;

            // act
            try
            {
                noteRepository.DeleteEntity(testId);
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
        public void EditEntity_EditNote()
        {
            // arrange
            var tagRepository = new TsqlTagRepository(_connectionString);
            var userRepository = new TsqlUserRepository(_connectionString);
            var noteRepository = new TsqlNoteRepository(_connectionString, userRepository, tagRepository);
            var testedTag = new Tag()
            {
                TagID = -1,
                TagName = "editTestTag"
            };
            testedTag = tagRepository.CreateEntity(testedTag);
            createdTagIDs.Add(testedTag.TagID);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "editTestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            createdUserIDs.Add(testedUser.UserID);

            var testedNote = new Note
            {
                NoteID = -1,
                Aurhor = testedUser,
                Topic = "editTest",
                Text = "text",
                Tags = new Tag[] { testedTag },
                UsersWhithAccess = new User[] { testedUser }
            };
            testedNote = noteRepository.CreateEntity(testedNote);
            var testId = testedNote.NoteID;

            // act
            testedNote.Topic = "edited";
            testedNote.Text = "edited";
            noteRepository.EditEntity(testedNote);

            // assert
            Assert.AreEqual(testId, testedNote.NoteID);
            Assert.AreEqual("edited", testedNote.Topic);
            Assert.AreEqual("edited", testedNote.Text);
        }

        [TestMethod]
        public void GetEntityByID_GetNote_NoteReturned()
        {
            // arrange
            var tagRepository = new TsqlTagRepository(_connectionString);
            var userRepository = new TsqlUserRepository(_connectionString);
            var noteRepository = new TsqlNoteRepository(_connectionString, userRepository, tagRepository);
            var testedTag = new Tag()
            {
                TagID = -1,
                TagName = "getTestTag"
            };
            testedTag = tagRepository.CreateEntity(testedTag);
            createdTagIDs.Add(testedTag.TagID);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "getTestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            createdUserIDs.Add(testedUser.UserID);

            var testedNote = new Note
            {
                NoteID = -1,
                Aurhor = testedUser,
                Topic = "getTest",
                Text = "text",
                Tags = new Tag[] { testedTag },
                UsersWhithAccess = new User[] { testedUser }
            };
            testedNote = noteRepository.CreateEntity(testedNote);
            var testId = testedNote.NoteID;

            // act
            var returnedNote = noteRepository.GetEntityByID(testId);

            // assert
            Assert.AreEqual(testId, returnedNote.NoteID);
            Assert.AreEqual("getTest", returnedNote.Topic);
            Assert.AreEqual("text", returnedNote.Text);
            Assert.AreEqual("getTestName", returnedNote.Aurhor.LoginName);
            foreach(var tag in returnedNote.Tags)
            {
                Assert.AreEqual("getTestTag", tag.TagName);
            }
            foreach (var user in returnedNote.UsersWhithAccess)
            {
                Assert.AreEqual("getTestName", user.LoginName);
            }
        }

        [TestMethod]
        public void AddNoteTag_AddSecondTag()
        {
            // arrange
            var tagRepository = new TsqlTagRepository(_connectionString);
            var userRepository = new TsqlUserRepository(_connectionString);
            var noteRepository = new TsqlNoteRepository(_connectionString, userRepository, tagRepository);
            var testedTag = new Tag()
            {
                TagID = -1,
                TagName = "addNTTestTag"
            };
            testedTag = tagRepository.CreateEntity(testedTag);
            createdTagIDs.Add(testedTag.TagID);
            var secondTag = new Tag()
            {
                TagID = -1,
                TagName = "addNTSecondTag"
            };
            secondTag = tagRepository.CreateEntity(secondTag);
            createdTagIDs.Add(secondTag.TagID);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "addNTTestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            createdUserIDs.Add(testedUser.UserID);

            var testedNote = new Note
            {
                NoteID = -1,
                Aurhor = testedUser,
                Topic = "addNTTest",
                Text = "text",
                Tags = new Tag[] { testedTag },
                UsersWhithAccess = new User[] { testedUser }
            };
            testedNote = noteRepository.CreateEntity(testedNote);
            var testId = testedNote.NoteID;

            // act
            noteRepository.AddNoteTag(testedNote.NoteID, secondTag.TagID);
            testedNote = noteRepository.GetEntityByID(testedNote.NoteID);

            // assert
            bool flag = false;
            foreach(var tag in testedNote.Tags)
            {
                if (tag.TagName == "addNTSecondTag")
                {
                    flag = true;
                    break;
                }
            }
            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void RemoveNoteTag_RemoveSecondTag()
        {
            // arrange
            var tagRepository = new TsqlTagRepository(_connectionString);
            var userRepository = new TsqlUserRepository(_connectionString);
            var noteRepository = new TsqlNoteRepository(_connectionString, userRepository, tagRepository);
            var testedTag = new Tag()
            {
                TagID = -1,
                TagName = "remNTTestTag"
            };
            testedTag = tagRepository.CreateEntity(testedTag);
            createdTagIDs.Add(testedTag.TagID);
            var secondTag = new Tag()
            {
                TagID = -1,
                TagName = "remNTSecondTag"
            };
            secondTag = tagRepository.CreateEntity(secondTag);
            createdTagIDs.Add(secondTag.TagID);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "remNTTestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            createdUserIDs.Add(testedUser.UserID);

            var testedNote = new Note
            {
                NoteID = -1,
                Aurhor = testedUser,
                Topic = "remNTTest",
                Text = "text",
                Tags = new Tag[] { testedTag },
                UsersWhithAccess = new User[] { testedUser }
            };
            testedNote = noteRepository.CreateEntity(testedNote);
            bool flag;

            // act
            try
            {
                noteRepository.RemoveNoteTag(testedNote.NoteID, secondTag.TagID);
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
        public void AddUserWhithAccess_AddSecondUserWhithAccess()
        {
            // arrange
            var tagRepository = new TsqlTagRepository(_connectionString);
            var userRepository = new TsqlUserRepository(_connectionString);
            var noteRepository = new TsqlNoteRepository(_connectionString, userRepository, tagRepository);
            var testedTag = new Tag()
            {
                TagID = -1,
                TagName = "addUATestTag"
            };
            testedTag = tagRepository.CreateEntity(testedTag);
            createdTagIDs.Add(testedTag.TagID);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "addUATestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            createdUserIDs.Add(testedUser.UserID);
            var secondUser = new User()
            {
                UserID = -1,
                LoginName = "addUASecond",
                UserPassword = "test"
            };
            secondUser = userRepository.CreateEntity(secondUser);
            createdUserIDs.Add(secondUser.UserID);

            var testedNote = new Note
            {
                NoteID = -1,
                Aurhor = testedUser,
                Topic = "addNTTest",
                Text = "text",
                Tags = new Tag[] { testedTag },
                UsersWhithAccess = new User[] { testedUser }
            };
            testedNote = noteRepository.CreateEntity(testedNote);
            var testId = testedNote.NoteID;

            // act
            noteRepository.AddUserWhithAccess(testedNote.NoteID, secondUser.UserID);
            testedNote = noteRepository.GetEntityByID(testedNote.NoteID);

            // assert
            bool flag = false;
            foreach (var user in testedNote.UsersWhithAccess)
            {
                if (user.LoginName == "addUASecond")
                {
                    flag = true;
                    break;
                }
            }
            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void RemoveUserWhithAccess_RemoveSecondUserWhithAccess()
        {
            // arrange
            var tagRepository = new TsqlTagRepository(_connectionString);
            var userRepository = new TsqlUserRepository(_connectionString);
            var noteRepository = new TsqlNoteRepository(_connectionString, userRepository, tagRepository);
            var testedTag = new Tag()
            {
                TagID = -1,
                TagName = "remUATestTag"
            };
            testedTag = tagRepository.CreateEntity(testedTag);
            createdTagIDs.Add(testedTag.TagID);
            var testedUser = new User()
            {
                UserID = -1,
                LoginName = "remUATestName",
                UserPassword = "test"
            };
            testedUser = userRepository.CreateEntity(testedUser);
            createdUserIDs.Add(testedUser.UserID);
            var secondUser = new User()
            {
                UserID = -1,
                LoginName = "remUASecond",
                UserPassword = "test"
            };
            secondUser = userRepository.CreateEntity(secondUser);
            createdUserIDs.Add(secondUser.UserID);

            var testedNote = new Note
            {
                NoteID = -1,
                Aurhor = testedUser,
                Topic = "remUATest",
                Text = "text",
                Tags = new Tag[] { testedTag },
                UsersWhithAccess = new User[] { testedUser }
            };
            testedNote = noteRepository.CreateEntity(testedNote);
            bool flag;

            // act
            try
            {
                noteRepository.RemoveUserWhithAccess(testedNote.NoteID,secondUser.UserID);
                flag = true;
            }
            catch (Exception)
            {
                flag = false;
            }

            // assert
            Assert.IsTrue(flag);
        }

        [TestCleanup]
        public void Clean()
        {
            var tagRepository = new TsqlTagRepository(_connectionString);
            var userRepository = new TsqlUserRepository(_connectionString);
            var noteRepository = new TsqlNoteRepository(_connectionString, userRepository, tagRepository);

            foreach (var id in createdNoteIDs)
            {
                noteRepository.DeleteEntity(id);
            }

            foreach (var id in createdTagIDs)
            {
                tagRepository.DeleteEntity(id);
            }

            foreach (var id in createdUserIDs)
            {
                userRepository.DeleteEntity(id);
            }
        }
    }
}
