using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class PokerDataService
{
    private readonly Dictionary<string, List<PokerHand>> _allData = new();

    public ObservableCollection<string> Tables { get; } = new();
    public ObservableCollection<long> HandIds { get; } = new();

    public string SelectedTable { get; private set; }

    public void AddHand(PokerHand hand)
    {
        if (!_allData.ContainsKey(hand.TableName))
        {
            _allData[hand.TableName] = new List<PokerHand>();
            Tables.Add(hand.TableName);
        }

        _allData[hand.TableName].Add(hand);
    }

    public void SetTable(string tableName)
    {
        SelectedTable = tableName;
        HandIds.Clear();

        if (tableName == null || !_allData.ContainsKey(tableName))
            return;

        foreach (var hand in _allData[tableName])
            HandIds.Add(hand.HandID);
    }

    public PokerHand GetHand(long handId)
    {
        if (SelectedTable == null) return null;

        return _allData[SelectedTable]
            .FirstOrDefault(x => x.HandID == handId);
    }

    public void Clear()
    {
        _allData.Clear();
        Tables.Clear();
        HandIds.Clear();
    }
}