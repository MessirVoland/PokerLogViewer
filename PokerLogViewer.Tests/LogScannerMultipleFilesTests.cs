using Xunit;
using System.IO;
using System.Text.Json;

public class LogScannerMultipleFilesTests
{
    [Fact]
    public void Should_Parse_Multiple_Files()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "poker_test_multi");
        Directory.CreateDirectory(tempDir);

        var hand = new PokerHand
        {
            HandID = 1,
            TableName = "Test",
            Players = new() { "A" },
            Winners = new() { "A" },
            WinAmount = "10$"
        };

        var json = JsonSerializer.Serialize(new[] { hand });

        File.WriteAllText(Path.Combine(tempDir, "1.json"), json);
        File.WriteAllText(Path.Combine(tempDir, "2.json"), json);

        int handsCount = 0;
        int filesCount = 0;

        var service = new LogScannerService();

        service.Start(
            tempDir,
            h => handsCount++,
            c => filesCount = c,
            error => Assert.Fail(error)
        );

        Thread.Sleep(500);

        Assert.True(handsCount >= 2);
        Assert.True(filesCount >= 2);
    }
}