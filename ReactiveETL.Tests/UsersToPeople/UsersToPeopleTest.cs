namespace ReactiveETL.Tests
{
    using Infrastructure;
    using Shouldly;
    using Xunit;

    public class UsersToPeopleFixture : BaseUserToPeopleTest
    {
        [Fact]
        public void CanCopyTableWithTransform()
        {
            var result =
                Input.Query("test", UsersToPeopleActions.SelectAllUsers)
                .Transform(UsersToPeopleActions.SplitUserName)
                .DbCommand("test", UsersToPeopleActions.WritePeople)
                .Execute();

            UsersToPeopleActions.VerifyResult(result);
        }


        [Fact]
        public void CanCopyTableWithTransformAndConnection()
        {
            using (var conn = Use.Connection("test"))
            {
                // Please note that you cannot use the connection in the query operation because 
                // the data reader will still be open when you will attemp to input data
                // Adding a buffered input query operation should solve this but will load 
                // all data in memory before executing the pipeline
                var result =
                    Input.Query("test", UsersToPeopleActions.SelectAllUsers)
                        .Transform(UsersToPeopleActions.SplitUserName)
                        .DbCommand(conn, UsersToPeopleActions.WritePeople)
                        .Execute();

                UsersToPeopleActions.VerifyResult(result);
            }
        }

        [Fact]
        public void CanCopyTableWithTransformThreaded()
        {
            var result =
                Input.Query("test", UsersToPeopleActions.SelectAllUsers)
                .Transform(UsersToPeopleActions.SplitUserName)
                .DbCommand("test", UsersToPeopleActions.WritePeople)
                .ExecuteInThread();
            // Check that we effectively start the process in another thread
            result.Completed.ShouldBe(false);
            // wait for completion
            result.Thread.Join();
            UsersToPeopleActions.VerifyResult(result);
        }
    }
}
