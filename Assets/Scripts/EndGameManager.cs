using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    [SerializeField] private GameObject _dieUI;
    [SerializeField] private GameObject _finishUI;
    [SerializeField] private Button _dieContinueButton;
    [SerializeField] private Button _finishContinueButton;

    private void OnValidate()
    {
        _dieContinueButton ??= _dieUI.GetComponentInChildren<Button>();
        _finishContinueButton ??= _finishUI.GetComponentInChildren<Button>();
    }
    
    private void Start()
    {
        GameManager.OnDie += EndGame(_dieUI, _dieContinueButton);
        GameManager.OnFinish += EndGame(_finishUI, _finishContinueButton);
    }

    static Action EndGame(GameObject ui, Button button) => () => {
        if (!ui) throw new NullReferenceException(nameof(ui));
        if (!button) throw new NullReferenceException(nameof(button));
        _ = GameManager.NetworkManager.SendCoinsAsync(GameManager.CoinsManager.Coins);
        
        Time.timeScale = 0;
        ui.SetActive(true);
        button.onClick.RemoveListener(Continue);
        button.onClick.AddListener(Continue);
    };

    private static void Continue()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
