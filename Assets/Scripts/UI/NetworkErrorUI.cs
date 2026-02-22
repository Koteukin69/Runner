using UnityEngine;
using UnityEngine.UI;

public class NetworkErrorUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _offlineButton;
    [SerializeField] private Button _retryButton;

    private void Start()
    {
        GameManager.OnNetworkError += Show;
        _offlineButton.onClick.AddListener(OnOffline);
        _retryButton.onClick.AddListener(OnRetry);
    }

    private void Show() => _panel.SetActive(true);

    private void OnOffline()
    {
        _panel.SetActive(false);
        GameManager.GoOffline();
    }

    private void OnRetry()
    {
        _panel.SetActive(false);
        GameManager.RetryConnection();
    }
}
