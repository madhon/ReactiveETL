namespace ReactiveETL.Tests;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exceptions;
using Shouldly;
using Xunit;

public class ErrorsFixture
{        
    public IEnumerable<User> ListUsers(int numusers)
    {
        for (int i=0 ; i < numusers; i++)
        {
            yield return new User() {Id = i, Email = "1@rhino.com", Name = "User" + i};
        }
    }

    [Fact]
    public void WillReportErrorsWhenThrown()
    {
        int maxElements = 1000;
        int throwAfter = 15;
        int rowCount = 0;

        try
        {
            var result = Input.From(ListUsers(maxElements))
                .Apply(row =>
                {
                    rowCount++;
                    if (rowCount > throwAfter)
                        throw new InvalidDataException("problem");
                })
                .Execute();
        }
        catch (EtlResultException ex)
        {
            ex.EtlResult.CountExceptions.ShouldBe(1);
            var exc = ex.EtlResult.Exceptions.FirstOrDefault();

            exc.ShouldBeAssignableTo<InvalidDataException>();
        }                       
    }
}