using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class Board_Manager : MonoBehaviour
{
    private static Board_Manager instance;
	public static Board_Manager Instance { get => instance; }

	private const float DoTweenDuration = 0.25f;

	[SerializeField] private bool shrinkingBoard;
	[SerializeField] private int scoreLimit;
	[SerializeField] private int stepLimit;
	[SerializeField] private Tile boardTilePrefab;
    [SerializeField] private Transform boardTileParent;
    [SerializeField] private List<Item> boardItemList = new List<Item>();

	private int stepLimitMax;
	private int score;
	private int scoreAdd = 0;
    private int width;
	private int height;
	private int totalSwapTiles = 0;
	private bool gameStart;
	private bool waitForChoosing;
	private Tile choosedTile;
	private List<Tile> choosedTiles = new List<Tile>();

	private Tile[,] myTile;
	private Dictionary<int, List<Tile>> swapTiles = new Dictionary<int, List<Tile>>();
	public Tile[,] Tiles { get => myTile; }
	public int Width { get => width; }
	public int Height { get => height; }
	public int ScoreAdd { get => scoreAdd; }
	public int Score { get => score; set => score = value; }
	public int Step { get => stepLimitMax; }

	[ContextMenu("Set Board")]
	private void SetBoard()
	{
		Canvas_Manager.Instance.SetActiveLevelFinishPanel(false);
		for (int e = boardTileParent.childCount - 1; e >= 0; e--)
		{
			Destroy(boardTileParent.GetChild(e).gameObject);
		}
		SetBoard(Game_Manager.Instance.BoardSize);
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
    public void SetBoard(Vector2Int boardSize)
	{
		width = boardSize.x;
		height = boardSize.y;
		myTile = new Tile[width, height];
		choosedTiles.Clear();
		score = 0;
		stepLimitMax = 0;
		gameStart = true;

		if (!shrinkingBoard)
		{
			SetSwapTilesList();
		}

		for (int h = 0; h < Width; h++)
		{
			for (int e = 0; e < Height; e++)
			{
				var tile = Instantiate(boardTilePrefab, new Vector3(h, e, 0), Quaternion.identity, boardTileParent);
				tile.name = "Tile : " + h + " --- " + e;
				myTile[h, e] = tile;
				Item item = boardItemList[Random.Range(0, boardItemList.Count)];
				tile.SetMyTile(item);
				tile.SetMyCoordinate(new Vector2Int(h, e));
			}
		}
        foreach (var tile in myTile)
        {

			tile.SetMyNeighbors();
		}
	}
	public void ChooseTile(Tile tile)
	{
		if (waitForChoosing)
		{
			return;
		}
		if (!gameStart)
		{
			return;
		}
		if (choosedTile == null)
		{
			SetChoosedTile(tile);
		}
        else if (choosedTile != tile)
		{
			choosedTiles.Clear();
			SetChoosedTile(tile);
		}
		else if (choosedTile == tile)
		{
			CheckBoardForConnecting();
		}
	}
	/// <summary>
	/// Check choosed tiles for same tiles.
	/// </summary>
	private void CheckBoardForConnecting()
	{
		waitForChoosing = true;
		stepLimitMax++;
		for (int h = 0; h < choosedTiles.Count; h++)
        {
			var myNeighbors = choosedTiles[h].MyNeighbors;
            for (int e = 0; e < myNeighbors.Length; e++)
            {
                if (myNeighbors[e] == null)
                {
					continue;
                }
                if (choosedTiles.Contains(myNeighbors[e]))
				{
					continue;
				}
                if (myNeighbors[e].MyItem == choosedTile.MyItem)
				{
					choosedTiles.Add(myNeighbors[e]);
				}
            }
		}
		CheckAndDestroyConnectedTiles();
	}
	/// <summary>
	/// Destroy choosedTiles if choosedTiles amount bigger than 2.
	/// </summary>
	private void CheckAndDestroyConnectedTiles()
	{
		if (choosedTiles.Count > 2)
		{
			for (int h = 0; h < choosedTiles.Count; h++)
			{
				scoreAdd += choosedTiles[h].MyItem.point;
				myTile[choosedTiles[h].MyCoordinate.x, choosedTiles[h].MyCoordinate.y] = null;
				Destroy(choosedTiles[h].gameObject);
			}
			Canvas_Manager.Instance.SetScore();
			if (shrinkingBoard)
			{
				ShrinkBoard();
			}
			else
			{
				GoDownTile();
			}
		}
		else
		{
			ClearChoosedTile();
		}
	}
	/// <summary>
	/// Send down tiles if their below tiles coordinate is null
	/// </summary>
	private void ShrinkBoard()
	{
		for (int h = 0; h < Width; h++)
		{
			int yeni = 0;
			for (int e = 0; e < Height; e++)
			{
                if (myTile[h, e] == null)
                {
					continue;
                }
				SendTileToAnotherCoordinate(h, e, new Vector2Int(h, yeni));

				yeni++;
            }
		}
		LearnNeighbors();
		SendLeftSide();
	}
	/// <summary>
	/// If board has empty column, move board's right side.
	/// </summary>
	private void SendLeftSide()
	{
		DOTween.To(value => { }, startValue: 0, endValue: 1, duration: DoTweenDuration * 2)
				.OnComplete(() =>
				{
					int movingLine = 0;
					bool isLineMoved = false;
					for (int h = 0; h < Width; h++)
					{
						if (myTile[h, 0] == null)
						{
							movingLine++;
							isLineMoved = true;
						}
						else
						{
							if (movingLine > 0)
							{
								for (int e = h; e < Width; e++)
								{
									for (int c = 0; c < Height; c++)
									{
										if (myTile[e, c] == null)
										{
											continue;
										}
										SendTileToAnotherCoordinate(e, c, new Vector2Int(e - movingLine, c));
									}
								}
								movingLine = 0;
							}
						}
					}
					if (isLineMoved)
					{
						LearnNeighbors();
					}
					ClearChoosedTile();
					CheckGameFinish();
					CheckOtherMoves();
				});
	}
	/// <summary>
	/// Send down tiles if their below tiles is null.
	/// </summary>
	private void GoDownTile()
	{
		SetSwapTilesList();
		for (int e = 0; e < Width; e++)
		{
			for (int c = 0; c < Height; c++)
			{
				if (myTile[e, c] == null)
				{
					totalSwapTiles++;
					var tile = Instantiate(boardTilePrefab, new Vector3(e, Height + swapTiles[e].Count, 0), Quaternion.identity, boardTileParent);
					Item item = boardItemList[Random.Range(0, boardItemList.Count)];
					tile.SetMyTile(item);

					swapTiles[e].Add(tile);
					tile.gameObject.SetActive(false);
				}
				else
				{
					if (swapTiles[e].Count > 0)
					{
						SendTileToAnotherCoordinate(e, c, new Vector2Int(e, c - swapTiles[e].Count));
					}
				}
			}
		}
		SendDownNewTiles();
	}
	/// <summary>
	/// Send new tiles to their new coordinate.
	/// </summary>
	private void SendDownNewTiles()
	{
		DOTween.To(value => { },
			startValue: 0, endValue: 1, duration: DoTweenDuration * 2)
			.OnComplete(() =>
			{
				for (int e = 0; e < Width; e++)
				{
					int swapTilesCount = swapTiles[e].Count;
					for (int c = 0; c < swapTilesCount; c++)
					{
						swapTiles[e][c].name = "Tile : " + e + " --- " + c;
						Vector2Int newCoordinate = new Vector2Int(e, Height - swapTiles[e].Count + c);
						myTile[e, newCoordinate.y] = swapTiles[e][c];
						swapTiles[e][c].SetMyCoordinate(newCoordinate);
						swapTiles[e][c].gameObject.SetActive(true);
						Vector3 newPos = new Vector3(newCoordinate.x, newCoordinate.y, 0);
						swapTiles[e][c].transform.DOMove(newPos, DoTweenDuration).OnComplete(() =>
						{
							totalSwapTiles--;
							if (totalSwapTiles == 0)
							{
								CheckGameFinish();
								LearnNeighbors();
								ClearChoosedTile();
								CheckOtherMoves();
							}
						});
					}
				}
			});
	}
	private Tile SendTileToAnotherCoordinate(int oldX, int oldY, Vector2Int newCoordinate)
	{
		Tile tile = myTile[oldX, oldY];
		myTile[oldX, oldY] = null;
		myTile[newCoordinate.x, newCoordinate.y] = tile;
		tile.SetMyCoordinate(newCoordinate);
		Vector3 newPos = new Vector3(newCoordinate.x, newCoordinate.y, 0);
		tile.transform.DOMove(newPos, DoTweenDuration);

		return tile;
	}
	private void LearnNeighbors()
	{
		Debug.Log("Learn Neighbors");
		for (int h = 0; h < Width; h++)
		{
			for (int e = 0; e < Height; e++)
			{
				if (myTile[h, e] == null)
				{
					continue;
				}
				myTile[h, e].SetMyNeighbors();
			}
		}
	}
	/// <summary>
	/// Clear swap tiles list for next step.
	/// </summary>
	private void SetSwapTilesList()
	{
		swapTiles.Clear();
		for (int e = 0; e < width; e++)
		{
			int order = e;
			swapTiles.Add(order, new List<Tile>());
		}
	}
	private void SetChoosedTile(Tile tile)
	{
		choosedTile = tile;
		choosedTiles.Add(choosedTile);
	}
	private void ClearChoosedTile()
	{
		waitForChoosing = false;
		choosedTile = null;
		choosedTiles.Clear();
	}
	private void CheckGameFinish()
	{
		Debug.Log("Check Game Finish");
		if (stepLimit == 0) // Sadece belli bir score geçilmesi isteniyor
		{
			CheckScoreLimit();
		}
		else // Belli bir step içinde belli bir score geçilmesi isteniyor.
		{
			if (stepLimitMax >= stepLimit)
			{
				if (!CheckScoreLimit())
				{
					gameStart = false;
					Canvas_Manager.Instance.LevelLost();
				}
			}
			else
			{
				CheckScoreLimit();
			}
		}
	}
	private bool CheckScoreLimit()
	{
		if (score >= scoreLimit)
		{
			gameStart = false;
			Canvas_Manager.Instance.LevelWin();
			return true;
		}
		return false;

	}
	private void CheckOtherMoves()
	{
		List<Tile> checkedTiles = new List<Tile>();
		bool foundMoves = false;
		// Control all tiles for move.
		for (int e = 0; e < Width && !foundMoves; e++)
        {
            for (int h = 0; h < Height && !foundMoves; h++)
            {
                if (myTile[e, h] == null)
                {
                    continue;
                }
                if (checkedTiles.Contains(myTile[e, h]))
				{
					// Tile checked so dont check anymore.
					continue;
				}
				checkedTiles.Add(myTile[e, h]);

				// Check new tile has moves
				List<Tile> checkedNeighborTiles = new List<Tile>();
				checkedNeighborTiles.Add(myTile[e, h]);
                for (int i = 0; i < checkedNeighborTiles.Count && !foundMoves; i++)
				{
					var myNeighbors = checkedNeighborTiles[i].MyNeighbors;
					for (int c = 0; c < myNeighbors.Length && !foundMoves; c++)
					{
						if (myNeighbors[c] == null)
						{
							continue;
						}
						if (checkedNeighborTiles.Contains(myNeighbors[c]))
						{
							continue;
						}
						if (myNeighbors[c].MyItem == checkedNeighborTiles[0].MyItem)
						{
							checkedTiles.Add(myNeighbors[c]);
							checkedNeighborTiles.Add(myNeighbors[c]);
                            if (checkedNeighborTiles.Count > 2)
                            {
								foundMoves = true;
								Debug.Log("Found Moves");
							}
						}
					}
                }
			}
        }
        if (!foundMoves)
		{
			// There is not move for game so game is over.
			Canvas_Manager.Instance.LevelLost();
		}
	}
	public void SetScore()
    {
		score += scoreAdd;
		scoreAdd = 0;
	}
}