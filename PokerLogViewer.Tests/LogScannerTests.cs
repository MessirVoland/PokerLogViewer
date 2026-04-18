using Xunit;
using System.Text.Json;
using PokerLogViewer;

public class LogScannerTests
{
    [Fact]
    public void Should_Parse_Valid_Json()
    {
        string json = """
        [
            {
                "HandID": 1,
                "TableName": "Berlin #01",
                "Players": ["A", "B"],
                "Winners": ["A"],
                "WinAmount": "100$"
            }
        ]
        """;

        var result = JsonSerializer.Deserialize<PokerHand[]>(json);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public void Should_Not_Fail_On_Empty_Directory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "poker_test_empty");

        Directory.CreateDirectory(tempDir);

        var service = new LogScannerService();

        bool completed = false;

        service.Start(
            tempDir,
            hand => { },
            count => completed = true,
            error => Assert.Fail("Should not throw error")
        );

        Thread.Sleep(300);

        Assert.True(completed);
    }
}