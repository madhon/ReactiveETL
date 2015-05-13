using System.IO;
using FileHelpers;
using Shouldly;

namespace ReactiveETL.Tests.Files
{
    /// <summary>
    /// Summary description for FileTest
    /// </summary>
    public class FileTest
    {
        [DelimitedRecord(";"), IgnoreFirst]
        private class FileReadPoco
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Blabla { get; set; }
        }

        [Fact]
        public void FileReadTest()
        {
            using (var file = Helper.LoadFromRessourceFileToStreamReader("FileReadTest.txt"))
            {
                string content = file.ReadToEnd();
                file.BaseStream.Position = 0;
                var filecontent = Input
                    .ReadFile<FileReadPoco>(file)
                    .WriteFile<FileReadPoco>("resultfile.txt")
                    .Execute();

                File.Exists("resultfile.txt").ShouldBe(true);
                filecontent.Count.ShouldBe(2);
                File.Delete("resultfile.txt");
            }
        }
    }
}
