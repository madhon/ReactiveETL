namespace ReactiveETL.Tests.Files
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Shouldly;
    using Xunit;

    public class UsingDALFixture
    {
        private const string expected =
            @"Id	Name	Email
1	ayende	ayende@example.org
2	foo	foo@example.org
3	bar	bar@example.org
4	brak	brak@example.org
5	snar	snar@example.org
";
        [Fact]
        public void CanWriteToFileFromDAL()
        {
            Input
                .From(MySimpleDal.GetUsers())
                .WriteFile<UserRecord>(
                    "users.txt", 
                    ff => ff.HeaderText = "Id\tName\tEmail")
                .Start();
            string actual = File.ReadAllText("users.txt");
            actual.ShouldBe(expected.Replace("\r\n", "\n").Replace("\n", Environment.NewLine));
        }

        [Fact]
        public void CanReadFromFileToDAL()
        {
            MySimpleDal.Users = new List<User>();
            File.WriteAllText("users.txt", expected);

            Input
                .ReadFile<UserRecord>("users.txt")
                .Apply(row => MySimpleDal.Save(row.ToObject<User>()))
                .Start();

            MySimpleDal.Users.Count.ShouldBe(5);
        }
    }
}
