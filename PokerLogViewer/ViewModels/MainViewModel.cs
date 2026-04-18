using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly LogScannerService _scanner = new();
    private readonly FileWatcherService _watcher = new();
    private readonly PokerDataService _data = new();
    public string SelectFolderText => AppLocalization.Get("SelectFolder");
    public string StartScanText => AppLocalization.Get("StartScan");

    private string _selectedPath;
    public string SelectedPath
    {
        get => _selectedPath;
        set
        {
            _selectedPath = value;
            OnPropertyChanged();
        }
    }

    private string _status = "Ready";
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    public string SearchText { get; set; }
    public System.Collections.ObjectModel.ObservableCollection<string> Tables => _data.Tables;
    public System.Collections.ObjectModel.ObservableCollection<long> HandIds => _data.HandIds;

    private string _selectedTable;
    public string SelectedTable
    {
        get => _selectedTable;
        set
        {
            _selectedTable = value;
            _data.SetTable(value);
            OnPropertyChanged();
        }
    }

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

    private string _details;
    public string Details
    {
        get => _details;
        set { _details = value; OnPropertyChanged(); }
    }

    public ICommand SelectFolderCommand { get; }
    public ICommand StartScanCommand { get; }
    public ICommand SetRuCommand { get; }
    public ICommand SetEnCommand { get; }

    public MainViewModel()
    {
        SelectFolderCommand = new RelayCommand(SelectFolder);
        StartScanCommand = new RelayCommand(StartScan);

        SetRuCommand = new RelayCommand(() => SwitchLanguage("RU"));
        SetEnCommand = new RelayCommand(() => SwitchLanguage("EN"));
    }
    private void SelectFolder()
    {
        var dialog = new FolderBrowserDialog();

        if (dialog.ShowDialog() == DialogResult.OK)
            SelectedPath = dialog.SelectedPath;
    }

    private void StartScan()
    {
        if (string.IsNullOrWhiteSpace(SelectedPath))
        {
            Status = AppLocalization.Get("SelectFolder");
            return;
        }

        Status = AppLocalization.Get("Scanning");
        _data.Clear();

        _scanner.Start(
            SelectedPath,

            onHand: hand =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    _data.AddHand(hand);
                });
            },

            onComplete: count =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Status = $"{AppLocalization.Get("Ready")} ({count})";
                });
            },

            onError: err =>
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Status = $"{AppLocalization.Get("Error")}: {err}";
                });
            }
        );
    }

    private void LoadDetails()
    {
        var hand = _data.GetHand(_selectedHand);

        if (hand == null) return;

        Details =
            $"Table: {hand.TableName}\n" +
            $"HandID: {hand.HandID}\n\n" +
            $"Players: {string.Join(", ", hand.Players)}\n" +
            $"Winners: {string.Join(", ", hand.Winners)}\n" +
            $"Win: {hand.WinAmount}";
    }

    private void SwitchLanguage(string lang)
    {
        AppLocalization.Load(lang);
        Status = AppLocalization.Get("Ready");
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(SelectFolderText));
        OnPropertyChanged(nameof(StartScanText));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}