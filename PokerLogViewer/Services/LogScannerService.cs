using System;
using System.IO;
using System.Text.Json;
using System.Threading;

public class LogScannerService
{
    private Thread _thread;

    public void Start(
        string path,
        Action<PokerHand> onHand,
        Action<int> onComplete,
        Action<string> onError)
    {
        _thread = new Thread(() =>
        {
            int processedFiles = 0;
            int failedFiles = 0;

            try
            {
                var files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    try
                    {
                        ProcessSingleFile(file, onHand);
                        processedFiles++;
                    }
                    catch
                    {
                        failedFiles++;
                    }
                }

                onComplete?.Invoke(processedFiles);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        });

        _thread.IsBackground = true;
        _thread.Start();
    }

    // =========================
    // Общий метод обработки файла
    // (используется и scanner, и watcher)
    // =========================
    public void ProcessSingleFile(string filePath, Action<PokerHand> onHand)
    {
        string json;

        using (var fs = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite))
        using (var sr = new StreamReader(fs))
        {
            json = sr.ReadToEnd();
        }

        var hands = JsonSerializer.Deserialize<PokerHand[]>(json);

        if (hands == null)
            return;

        foreach (var hand in hands)
        {
            onHand?.Invoke(hand);
        }
    }
}