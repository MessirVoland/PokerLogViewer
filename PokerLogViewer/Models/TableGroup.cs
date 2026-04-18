using System.Collections.ObjectModel;

public class TableGroup
{
    public string TableName { get; set; }
    public ObservableCollection<PokerHand> Hands { get; set; } = new();
}
