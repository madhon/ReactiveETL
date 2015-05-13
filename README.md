# ReactiveETL
Reactive ETL is a rewrite of Rhino ETL using the reactive extensions for .Net.

Here is an example of a simple pipeline that reads data from a table, transform the data, and insert the result in another table.
```C#
var result =
          Input.Query("input", "SELECT * FROM Users")
                .Transform(
                    row =>
                        {
                            string name = (string)row["name"];
                            row["FirstName"] = name.Split()[0];
                            row["LastName"] = name.Split()[1];
                            return row;
                        }
                )
                .DbCommand("output", (cmd, row) =>
                    {
                        cmd.CommandText = @"INSERT INTO People (UserId, FirstName, LastName, Email) VALUES (@UserId, @FirstName, @LastName, @Email)";
                        cmd.AddParameter("UserId", row["Id"]);
                        cmd.AddParameter("FirstName", row["FirstName"]);
                        cmd.AddParameter("LastName", row["LastName"]);
                        cmd.AddParameter("Email", row["Email"]);
                    })
                .Execute();
```
