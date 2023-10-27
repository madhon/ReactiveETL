namespace ReactiveETL.Tests.Joins
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Xunit;

    public class JoinFixture : BaseJoinFixture
    {
        [Fact]
        public void InnerJoin()
        {
            var result = Input.From(left).InnerJoin(Input.From(right), "email", MergeRows).Execute();

            List<Row> items = (List<Row>)result.Data;

            items.Count.Should().Be(1);
            items[0]["person_id"].Should().Be(3);
        }

        [Fact]
        public void RightJoin()
        {
            var result = Input.From(left).RightJoin(Input.From(right), "email", MergeRows).Execute();

            List<Row> items = (List<Row>)result.Data;

            items.Count.Should().Be(2);
            items[0]["person_id"].Should().Be(3);
            items[1]["name"].Should().Be(null);
            items[1]["person_id"].Should().Be(5);
        }

        [Fact]
        public void LeftJoin()
        {
            var result = Input.From(left).LeftJoin(Input.From(right), "email", MergeRows).Execute();
            List<Row> items = (List<Row>)result.Data;

            items.Count.Should().Be(2);
            items[0]["person_id"].Should().Be(3);
            items[1]["name"].Should().Be("bar");
        }

        [Fact]
        public void FullJoin()
        {
            var result = Input.From(left).FullJoin(Input.From(right), "email", MergeRows).Execute();
            List<Row> items = (List<Row>)result.Data;

            items.Count.Should().Be(3);
            items[0]["person_id"].Should().Be(3);

            items[1]["name"].Should().Be("bar");

            items[2]["name"].Should().Be(null);
            items[2]["person_id"].Should().Be(5);
        }

        [Fact]
        public void CanJoinOnEnumerable()
        {
            var result = Input.From(left).Join(right, new RowJoinHelper("email").InnerJoinMatch, MergeRows).Execute();
            result.Count.Should().Be(1);
            ((List<Row>)result.Data)[0]["person_id"].Should().Be(3);
       }

        [Fact]
        public void CanUseComplexJoinInProcesses()
        {
            var result =
                Input.From(left)
                .Transform(RowColumnToUpperCase)
                .RightJoin(Input.From(right).Transform(RowColumnToUpperCase), "email", MergeRows)
                .Execute();

            result.Count.Should().Be(2);
            ((List<Row>)result.Data)[0]["name"].Should().Be("FOO");
            ((List<Row>)result.Data)[0]["email"].Should().Be("FOO@EXAMPLE.ORG");
            ((List<Row>)result.Data)[1]["person_id"].Should().Be(5);
        }

        private Row RowColumnToUpperCase(Row row)
        {
            foreach (string column in row.Columns)
            {
                string item = row[column] as string;
                if (item != null)
                    row[column] = item.ToUpper();
            }

            return row;
        }

        private Row MergeRows(Row leftRow, Row rightRow)
        {
            var row = new Row();
            row.Copy(leftRow);
            if (rightRow != null)
                row["person_id"] = rightRow["id"];
            return row;
        }
    }
}
