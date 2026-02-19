using System;
using UnityEngine;

namespace UI
{
    public class Coins : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _coinsText;
        
        private void Start() => GameManager.CoinsManager.OnCoinsChange += ChangeCoins;

        private void OnValidate()
        {
            if (!_coinsText) TryGetComponent(out _coinsText);
        }

        private void ChangeCoins(uint coins)
        {
            if (!_coinsText) throw new MissingFieldException(nameof(_coinsText));
            _coinsText.text = coins.ToString();
        }
    }
}