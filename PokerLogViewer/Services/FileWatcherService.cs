using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

public class FileWatcherService
{
    private FileSystemWatcher _watcher;

    public Action<string> OnNewFile;

    // защита от дублей
    private readonly ConcurrentDictionary<string, byte> _processed = new();

    public void Start(string path)
    {
        Stop();

        _watcher = new FileSystemWatcher(path, "*.json")
        {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Size
        };

        _watcher.Created += OnChanged;
        _watcher.Renamed += OnRenamed;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        SafeHandle(e.FullPath);
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        SafeHandle(e.FullPath);
    }

    private void SafeHandle(string path)
    {
        // защита от дублей
        if (!_processed.TryAdd(path, 0))
            return;

        // ждём пока файл допишется
        Thread.Sleep(100);

        try
        {
            if (File.Exists(path))
            {
                OnNewFile?.Invoke(path);
            }
        }
        catch
        {
            // игнорируем временно недоступные файлы
        }
    }

    public void Stop()
    {
        if (_watcher == null) return;

        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        _watcher = null;

        _processed.Clear();
    }
}