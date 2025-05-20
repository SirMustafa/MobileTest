using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUiManager : MonoBehaviour
{
    [SerializeField] List<GameObject> uiPanel = new List<GameObject>();

    enum UiPanels
    {
        MainPnl,
        PausePnl
    }

    public void OpenPausePanel()
    {
        uiPanel[(int)UiPanels.MainPnl].SetActive(false);
        uiPanel[(int)UiPanels.PausePnl].SetActive(true);
    }

    public void ReturnHome()
    {
        SceneManager.LoadScene(0);
    }
}