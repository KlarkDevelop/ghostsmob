using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameSessionSettings settings;

    [SerializeField] private GameSessionSettings gameSessionSettings;

    private void Awake()
    {
        settings = gameSessionSettings;
    }
}
