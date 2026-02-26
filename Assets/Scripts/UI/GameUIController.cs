using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    [RequireComponent(typeof(UIDocument))]
    public class GameUIController : MonoBehaviour
    {
        private Label _hudCoinsLabel;

        private VisualElement _victoryCoinIcon;
        private VisualElement _defeatCoinIcon;
        private float _overlayCoinsRotation;

        private VisualElement _victoryOverlay;
        private Label _victoryCoinsLabel;

        private VisualElement _defeatOverlay;
        private Label _defeatCoinsLabel;

        private VisualElement _networkOverlay;

        private const string HiddenClass = "overlay--hidden";
        private const string PreAnimateClass = "overlay--pre-animate";

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _hudCoinsLabel = root.Q<Label>("hud-coins-label");

            _victoryOverlay = root.Q<VisualElement>("victory-overlay");
            _victoryCoinsLabel = root.Q<Label>("victory-coins-label");
            _victoryCoinIcon = _victoryOverlay.Q<VisualElement>(className: "overlay__coins-icon");
            root.Q<Button>("victory-continue-btn").clicked += Continue;

            _defeatOverlay = root.Q<VisualElement>("defeat-overlay");
            _defeatCoinsLabel = root.Q<Label>("defeat-coins-label");
            _defeatCoinIcon = _defeatOverlay.Q<VisualElement>(className: "overlay__coins-icon");
            root.Q<Button>("defeat-continue-btn").clicked += Continue;

            _networkOverlay = root.Q<VisualElement>("network-overlay");
            root.Q<Button>("network-retry-btn").clicked += OnRetry;
            root.Q<Button>("network-offline-btn").clicked += OnOffline;
        }

        private void Update()
        {
            _overlayCoinsRotation = (_overlayCoinsRotation + 180f * Time.unscaledDeltaTime) % 360f;
            var rotate = new Rotate(_overlayCoinsRotation);
            _victoryCoinIcon.style.rotate = rotate;
            _defeatCoinIcon.style.rotate = rotate;
        }

        private void Start()
        {
            GameManager.OnDie += OnDie;
            GameManager.OnFinish += OnFinish;
            GameManager.OnNetworkError += OnNetworkError;
            GameManager.CoinsManager.OnCoinsChange += OnCoinsChange;
        }

        private void OnCoinsChange(uint coins)
        {
            var text = coins.ToString();
            _hudCoinsLabel.text = text;
            _victoryCoinsLabel.text = text;
            _defeatCoinsLabel.text = text;
        }

        private void OnDie()
        {
            Time.timeScale = 0;
            _ = SendCoinsThenShowOverlay(_defeatOverlay);
        }

        private void OnFinish()
        {
            Time.timeScale = 0;
            _ = SendCoinsThenShowOverlay(_victoryOverlay);
        }

        private async Awaitable SendCoinsThenShowOverlay(VisualElement overlay)
        {
            await GameManager.RunWithRetry(() =>
                GameManager.NetworkManager.SendCoinsAsync(GameManager.CoinsManager.Coins));
            ShowOverlay(overlay);
        }

        private void OnNetworkError() => ShowOverlay(_networkOverlay);

        private static void ShowOverlay(VisualElement overlay)
        {
            overlay.RemoveFromClassList(HiddenClass);
            overlay.AddToClassList(PreAnimateClass);
            overlay.schedule.Execute(() => overlay.RemoveFromClassList(PreAnimateClass));
        }

        private static void HideOverlay(VisualElement overlay)
        {
            overlay.AddToClassList(HiddenClass);
            overlay.RemoveFromClassList(PreAnimateClass);
        }

        private static void Continue()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnRetry()
        {
            HideOverlay(_networkOverlay);
            GameManager.RetryConnection();
        }

        private void OnOffline()
        {
            HideOverlay(_networkOverlay);
            GameManager.GoOffline();
        }
    }
}
