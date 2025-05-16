using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] List<GameObject> Panels = new List<GameObject>();
    int previousPanelIndex = 0;

    private void Start()
    {
        Application.targetFrameRate = 60;
        Panels[0].SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ChangePanel(int panelIndex)
    {
        Panels[previousPanelIndex].SetActive(false);
        Panels[panelIndex].SetActive(true);
        previousPanelIndex = panelIndex;
    }
}