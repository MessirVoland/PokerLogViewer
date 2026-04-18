using System.Collections.Generic;

public class PokerHand
{
    public long HandID { get; set; }

    public string TableName { get; set; }

    public List<string> Players { get; set; }

    public List<string> Winners { get; set; }

    public string WinAmount { get; set; }
}