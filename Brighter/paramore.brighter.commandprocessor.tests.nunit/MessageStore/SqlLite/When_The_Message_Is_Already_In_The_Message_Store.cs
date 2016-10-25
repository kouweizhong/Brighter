#region Licence

/* The MIT License (MIT)
Copyright � 2014 Francesco Pighi <francesco.pighi@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the �Software�), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED �AS IS�, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System;
using Microsoft.Data.Sqlite;
using nUnitShouldAdapter;
using NUnit.Specifications;
using paramore.brighter.commandprocessor.Logging;
using paramore.brighter.commandprocessor.messagestore.sqllite;

namespace paramore.brighter.commandprocessor.tests.nunit.MessageStore.SqlLite
{
    [Subject(typeof(SqlLiteMessageStore))]
    public class When_The_Message_Is_Already_In_The_Message_Store : ContextSpecification
    {
        private static SqlLiteTestHelper _sqlLiteTestHelper;
        private static SqliteConnection _sqliteConnection;
        private static SqlLiteMessageStore _sSqlMessageStore;
        private static Exception s_exception;
        private static Message s_messageEarliest;
        private static Message s_storedMessage;

        private Cleanup _cleanup = () => CleanUpDb();

        private Establish _context = () =>
        {
            _sqlLiteTestHelper = new SqlLiteTestHelper();
            _sqliteConnection = _sqlLiteTestHelper.CreateMessageStoreConnection();
            _sSqlMessageStore = new SqlLiteMessageStore(new SqlLiteMessageStoreConfiguration(_sqlLiteTestHelper.ConnectionString, _sqlLiteTestHelper.TableName_Messages), new LogProvider.NoOpLogger());
            s_messageEarliest = new Message(new MessageHeader(Guid.NewGuid(), "test_topic", MessageType.MT_DOCUMENT),
                new MessageBody("message body"));
            _sSqlMessageStore.Add(s_messageEarliest);
        };

        private Because _of = () => { s_exception = Catch.Exception(() => _sSqlMessageStore.Add(s_messageEarliest)); };
        private It _should_ignore_the_duplcate_key_and_still_succeed = () => { s_exception.ShouldBeNull(); };

        private static void CleanUpDb()
        {
            _sqliteConnection?.Dispose();
            _sqlLiteTestHelper.CleanUpDb();
        }
    }
}