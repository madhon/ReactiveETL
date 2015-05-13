namespace ReactiveETL.Tests
{
    using FileHelpers;

    [DelimitedRecord("\t"), IgnoreFirst]
    public class UserRecord
    {
        public int Id;
        public string Name;
        public string Email;
    }
}