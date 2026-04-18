using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly LogScannerService _scanner = new();
    private readonly FileWatcherService _watcher = new();


    // =========================
    // Путь к папке
    // =========================
    private string _selectedPath;
    public string SelectedPath
    {
        get => _selectedPath;
        set { _selectedPath = value; OnPropertyChanged(); }
    }

    // =========================
    // Статус
    // =========================
    private string _status = "Готов";
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    // =========================
    // Хранение данных
    // =========================
    public ObservableCollection<string> Tables { get; } = new();
    public ObservableCollection<long> HandIds { get; } = new();


    private readonly Dictionary<string, List<PokerHand>> _data = new();

    // =========================
    // Выбранный стол
    // =========================
    private string _selectedTable;
    public string SelectedTable
    {
        get => _selectedTable;
        set
        {
            _selectedTable = value;
            OnPropertyChanged();
            LoadHands();
        }
    }

    // =========================
    // Выбранная рука
    // =========================
    private long _selectedHand;
    public long SelectedHand
    {
        get => _selectedHand;
        set
        {
            _selectedHand = value;
            OnPropertyChanged();
            LoadDetails();
        }
    }

    // =========================
    // Детали
    // =========================
    private string _details;
    public string Details
    {
        get => _details;
        set { _details = value; OnPropertyChanged(); }
    }

    // =========================
    // Команды
    // =========================
    public ICommand SelectFolderCommand { get; }
    public ICommand StartScanCommand { get; }

    public MainViewModel()
    {
        SelectFolderCommand = new RelayCommand(SelectFolder);
        StartScanCommand = new RelayCommand(StartScan);
        _watcher.OnNewFile = HandleNewFile;
    }

    // =========================
    // Выбор папки
    // =========================
    private void SelectFolder()
    {
        var dialog = new FolderBrowserDialog();

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            SelectedPath = dialog.SelectedPath;

            _watcher.Stop();
            _watcher.Start(SelectedPath);
        }
    }

    // =========================
    // Старт сканирования
    // =========================
    private void StartScan()
    {
        if (string.IsNullOrWhiteSpace(SelectedPath))
        {
            Status = "Выберите папку";
            return;
        }

        Status = "Сканирование...";

        Tables.Clear();
        HandIds.Clear();
        _data.Clear();

        _scanner.Start(
            SelectedPath,

            onHand: hand =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (!_data.ContainsKey(hand.TableName))
                    {
                        _data[hand.TableName] = new List<PokerHand>();
                        Tables.Add(hand.TableName);
                    }

                    _data[hand.TableName].Add(hand);
                });
            },

            onComplete: count =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Status = $"Готово ({count} файлов)";
                });
            },

            onError: err =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Status = $"Ошибка: {err}";
                });
            }
        );
    }

    // =========================
    // Загрузка рук для выбранного стола
    // =========================
    private void LoadHands()
    {
        HandIds.Clear();

        if (_selectedTable == null)
            return;

        if (!_data.ContainsKey(_selectedTable))
            return;

        foreach (var hand in _data[_selectedTable])
        {
            HandIds.Add(hand.HandID);
        }
    }

    // =========================
    // Загрузка деталей для выбранной руки
    // =========================
    private void LoadDetails()
    {
        if (_selectedTable == null)
            return;

        if (!_data.ContainsKey(_selectedTable))
            return;

        var hand = _data[_selectedTable]
            .FirstOrDefault(x => x.HandID == _selectedHand);

        if (hand == null)
            return;

        Details =
            $"Table: {hand.TableName}\n" +
            $"HandID: {hand.HandID}\n\n" +
            $"Players: {string.Join(", ", hand.Players)}\n" +
            $"Winners: {string.Join(", ", hand.Winners)}\n" +
            $"Win: {hand.WinAmount}";
    }

    // =========================
    // Уведомление об изменении свойств
    // =========================
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void HandleNewFile(string filePath)
    {
        _scanner.ProcessSingleFile(filePath, hand =>
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!_data.ContainsKey(hand.TableName))
                {
                    _data[hand.TableName] = new List<PokerHand>();
                    Tables.Add(hand.TableName);
                }

                _data[hand.TableName].Add(hand);
            });
        });
    }
}