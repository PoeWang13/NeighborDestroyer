﻿using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

//[Serializable]
//public class SwapTile
//{
//	public int order;
//	public List<Tile> swapTile = new List<Tile>();

//	public SwapTile(int order)
//    {
//		this.order = order;
//	}
//	public void AddTile(int order, Tile tile)
//	{
//		swapTile.Add(tile);
//	}
//}
public class Board_Manager : MonoBehaviour
{
    private static Board_Manager instance;
	public static Board_Manager Instance { get => instance; }

	private const float DoTweenDuration = 0.25f;
	[SerializeField] private bool shrinkingBoard;
	[SerializeField] private Tile boardTilePrefab;
    [SerializeField] private Transform boardTileParent;
    [SerializeField] private List<Item> boardItemList = new List<Item>();

	private int score;
	private int scoreAdd = 0;
    private int width;
	private int height;
	private bool waitForChoosing;
	private Tile choosedTile;
	private List<Tile> choosedTiles = new List<Tile>();

	private Tile[,] myTile;
	private Dictionary<int, List<Tile>> swapTiles = new Dictionary<int, List<Tile>>();
	public List<Item> BoardItemList { get => boardItemList; }
	public Tile[,] Tiles { get => myTile; }
	public int Width { get => width; }
	public int Height { get => height; }
	public int ScoreAdd { get => scoreAdd; }
	public int Score { get => score; set => score = value; }
	public bool WaitForChoosing { get => waitForChoosing; }

	[ContextMenu("Set Board")]
	private void SetBoard()
	{
        for (int e = boardTileParent.childCount - 1; e >= 0 ; e--)
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
        if (!shrinkingBoard)
		{
			SwapTilesList();
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
	private void SwapTilesList()
	{
		swapTiles.Clear();
		for (int e = 0; e < width; e++)
		{
			int order = e;
			swapTiles.Add(order, new List<Tile>());
		}
	}
	public void ChooseTile(Tile tile)
    {
        if (waitForChoosing)
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
	void CheckBoardForConnecting()
	{
		waitForChoosing = true;
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
				SendAnotherCoordinateToTile(h, e, new Vector2Int(h, yeni));

				yeni++;
            }
		}
		LearnNeighbors();
		DOTween.To(value => { }, startValue: 0, endValue: 1, duration: DoTweenDuration * 2)
                .OnComplete(() =>
				{
					SendLeftSide();
				});
	}
	private void SendLeftSide()
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
							SendAnotherCoordinateToTile(e, c, new Vector2Int(e - movingLine, c));
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
	}
	private void GoDownTile()
	{
		SwapTilesList();
		int totalSwapTiles = 0;
		for (int e = 0; e < Width; e++)
		{
			for (int c = 0; c < Height; c++)
			{
				if (myTile[e, c] == null)
				{
					totalSwapTiles++;
					var tile = Instantiate(boardTilePrefab, new Vector3(e, Height + swapTiles[e].Count, 0), Quaternion.identity, boardTileParent);
					//tile.name = "Tile : " + e + " --- " + c;
					//myTile[e, c] = tile;
					Item item = boardItemList[Random.Range(0, boardItemList.Count)];
					tile.SetMyTile(item);

					swapTiles[e].Add(tile);
					tile.gameObject.SetActive(false);
				}
				else
				{
					if (swapTiles[e].Count > 0)
					{
						SendAnotherCoordinateToTile(e, c, new Vector2Int(e, c - swapTiles[e].Count));
					}
				}
			}
		}
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
								LearnNeighbors();
								ClearChoosedTile();
							}
						});
					}
				}
			});
	}
	private Tile SendAnotherCoordinateToTile(int oldX, int oldY, Vector2Int newCoordinate)
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
	public void SetScore()
    {
		score += scoreAdd;
		scoreAdd = 0;
	}
}