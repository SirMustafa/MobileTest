using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SetPlayers : MonoBehaviour
{
    [Header("UI Panels and Groups")]
    [SerializeField] private List<GameObject> panels = new List<GameObject>();
    [SerializeField] private List<CanvasGroup> panelGroups = new List<CanvasGroup>();

    [Header("Player Indicators and Positions")]
    [SerializeField] private List<RectTransform> playerIndicators = new List<RectTransform>();
    [SerializeField] private List<RectTransform> playerPositions = new List<RectTransform>();

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private float inputThreshold = 0.5f;

    private int[] playerIndices = new int[2] { 0, 1 };
    private bool[] isPlayerLocked = new bool[2] { false, false };

    private void OnEnable()
    {
        for (int i = 0; i < playerIndicators.Count && i < playerIndices.Length; i++)
        {
            playerIndicators[i].anchoredPosition = playerPositions[playerIndices[i]].anchoredPosition;
        }
    }

    public void OnMoveP1(InputAction.CallbackContext ctx) => HandleMove(0, ctx);
    public void OnMoveP2(InputAction.CallbackContext ctx) => HandleMove(1, ctx);

    private void HandleMove(int playerNum, InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || isPlayerLocked[playerNum]) return;

        float horizontal = ctx.ReadValue<Vector2>().x;
        if (Mathf.Abs(horizontal) < inputThreshold) return;

        int direction = horizontal > 0 ? 1 : -1;
        int current = playerIndices[playerNum];
        int other = playerIndices[1 - playerNum];

        int next = Mathf.Clamp(current + direction, 0, playerPositions.Count - 1);
        if (next == other)
        {
            int skip = Mathf.Clamp(next + direction, 0, playerPositions.Count - 1);
            if (skip != other && skip != current) next = skip;
            else return;
        }

        if (next != current)
        {
            playerIndices[playerNum] = next;
            playerIndicators[playerNum]
                .DOAnchorPos(playerPositions[next].anchoredPosition, 0.2f);
        }
    }

    public void OnApplyP1(InputAction.CallbackContext ctx) => HandleApply(0, ctx);
    public void OnApplyP2(InputAction.CallbackContext ctx) => HandleApply(1, ctx);

    private void HandleApply(int playerNum, InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;

        if (playerNum == 0) gameData.playerInex1 = playerIndices[0];
        else gameData.playerInex2 = playerIndices[1];

        isPlayerLocked[playerNum] = true;
        if (isPlayerLocked[0] && isPlayerLocked[1])
        {
            SceneManager.LoadScene(1);
        }
    }

    public void SetPlayerCount(int count)
    {
        if (count == 1 && playerIndicators.Count > 1)
        {
            playerIndicators[1].gameObject.SetActive(false);
            playerIndices[1] = -1;
            isPlayerLocked[1] = true;
        }
        SwapPanels(0, 1);
    }

    private void SwapPanels(int from, int to)
    {
        panelGroups[from]
            .DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                panels[from].SetActive(false);
                panels[to].SetActive(true);
                panelGroups[to].alpha = 0f;
                panelGroups[to].DOFade(1f, fadeDuration);
            });
    }

    private void OnDisable()
    {
        SwapPanels(1, 0);
        if (playerIndicators.Count > 1)
        {
            playerIndicators[1].gameObject.SetActive(true);
            playerIndices[1] = 1;
            isPlayerLocked[1] = false;
        }
        isPlayerLocked[0] = false;
    }
}