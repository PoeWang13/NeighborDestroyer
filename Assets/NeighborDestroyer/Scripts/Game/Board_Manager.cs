using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

[Serializable]
public class SwapTile
{
	public List<Tile> swapTile = new List<Tile>();
	public int selectedTilePosYPower;

	public void AddTile(Tile tile)
    {
		selectedTilePosYPower++;
		swapTile.Add(tile);
	}
}
[Serializable]
public class BoardTile
{
	public Item item;
	public int lockCount;
	public bool isLocked;

	public BoardTile(Item item, int lockCount = 0, bool isLocked = false)
	{
		this.item = item;
		this.isLocked = isLocked;
		this.lockCount = lockCount;
	}
}
public class Board_Manager : MonoBehaviour
{
    private static Board_Manager instance;
	public static Board_Manager Instance { get => instance; }

	private const float DoTweenDuration = 0.25f;
    [SerializeField] private List<Item> boardItemList = new List<Item>();

	private int score;
	private int scoreAdd = 0;
    private int scoreMulti = 0;
    private int width;
	private int height;
	private bool waitForChoosing;
	private List<Tile> choosedTiles = new List<Tile>();

	private Tile[,] myTile;
	private List<SwapTile> swapTiles = new List<SwapTile>();
	public List<Item> BoardItemList { get => boardItemList; }
	public Tile[,] Tiles { get => myTile; }
	public int Width { get => width; }
	public int Height { get => height; }
	public int ScoreAdd { get => scoreAdd; }
	public int Score { get => score; set => score = value; }
	public bool WaitForChoosing { get => waitForChoosing; }

	[ContextMenu("Clear Board ChoosedTiles")]
	private void ClearBoardChoosedTiles()
	{
	}
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

    /// <summary>
    /// Create a tile board
    /// </summary>
    public void SetBoard(Vector2Int boardSize, Vector3 boardPosition)
	{
		width = boardSize.x;
		height = boardSize.y;
		myTile = new Tile[width, height];
		choosedTiles.Clear();
		swapTiles.Clear();
		for (int e = 0; e < width; e++)
		{
			swapTiles.Add(new SwapTile());
		}

		Canvas_Manager.Instance.SetBoardView(myTile, boardPosition);
	}
}