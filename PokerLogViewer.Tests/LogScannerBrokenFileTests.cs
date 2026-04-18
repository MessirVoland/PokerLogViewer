using Xunit;
using System.IO;

public class LogScannerBrokenFileTests
{
    [Fact]
    public void Should_Skip_Broken_File_Without_Crash()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "poker_test_broken");
        Directory.CreateDirectory(tempDir);

        var badFile = Path.Combine(tempDir, "bad.json");
        File.WriteAllText(badFile, "{ this is not json }");

        var service = new LogScannerService();

        bool finished = false;

        service.Start(
            tempDir,
            hand => { },
            count => finished = true,
            error => { }
        );

        Thread.Sleep(300);

        Assert.True(finished);
    }
}