using UnityEngine;

public class Game_Manager : MonoBehaviour
{
	private static Game_Manager instance;
	public static Game_Manager Instance { get => instance; }
    [SerializeField] private Vector2Int boardSize;
    public Vector2Int BoardSize { get => boardSize; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        Board_Manager.Instance.SetBoard(boardSize);
    }
}