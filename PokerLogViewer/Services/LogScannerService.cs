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
            int count = 0;

            try
            {
                var files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    try
                    {
                        var json = File.ReadAllText(file);

                        var hands = JsonSerializer.Deserialize<PokerHand[]>(json);

                        if (hands != null)
                        {
                            foreach (var hand in hands)
                            {
                                onHand?.Invoke(hand);
                            }
                        }

                        count++;
                    }
                    catch
                    {
                        // битый файл - просто пропускаем
                    }
                }

                onComplete?.Invoke(count);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        });

        _thread.IsBackground = true;
        _thread.Start();
    }
}