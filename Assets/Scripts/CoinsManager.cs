using System;

public class CoinsManager
{
    public uint Coins { private set; get; }
    public Action<uint> OnCoinsChange;

    public void AddCoins(uint count = 1)
    {
        Coins += count;
        OnCoinsChange?.Invoke(Coins);
    }
}
