namespace ReactiveETL.Tests;

using System.Data;
using ReactiveETL.Infrastructure;
using System.Collections.Generic;

internal sealed class UsersToPeopleActions
{
    public static string SelectAllUsers => "SELECT * FROM Users";

    public static Row SplitUserName(Row row)
    {
        string name = (string)row["name"];
        row["FirstName"] = name.Split()[0];
        row["LastName"] = name.Split()[1];
        return row;
    }

    public static void WritePeople(IDbCommand cmd, Row row)
    {
        cmd.CommandText =
            @"INSERT INTO People (UserId, FirstName, LastName, Email) VALUES (@UserId, @FirstName, @LastName, @Email)";
        cmd.AddParameter("UserId", row["Id"]);
        cmd.AddParameter("FirstName", row["FirstName"]);
        cmd.AddParameter("LastName", row["LastName"]);
        cmd.AddParameter("Email", row["Email"]);
    }

    public static void VerifyResult(EtlFullResult result)
    {
        result.Completed.ShouldBe(true);
        result.Count.ShouldBe(4);
        result.CountExceptions.ShouldBe(0);
            
        System.Collections.Generic.List<string[]> names = Use.Transaction<System.Collections.Generic.List<string[]>>("test", delegate(IDbCommand cmd)
        {
            var tuples = new List<string[]>();
            cmd.CommandText = "SELECT firstname, lastname from people order by userid";
            using (IDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tuples.Add(new string[] { reader.GetString(0), reader.GetString(1) });
                }
            }
            return tuples;
        });
        BaseUserToPeopleTest.AssertNames(names);
    }
}