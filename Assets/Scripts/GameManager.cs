using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool isTesting;

    void Start()
    {
        Application.targetFrameRate = 60;

        if(isTesting) return;


    }
}