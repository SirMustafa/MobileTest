using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] GameObject TheBomb;

    public void OnBomb(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Vector3 spawnPos = transform.position;

            spawnPos.x = Mathf.Round(spawnPos.x - 0.5f) + 0.5f;
            spawnPos.z = Mathf.Round(spawnPos.z - 0.5f) + 0.5f;

            Instantiate(TheBomb, spawnPos, Quaternion.identity);
        }
    }
}