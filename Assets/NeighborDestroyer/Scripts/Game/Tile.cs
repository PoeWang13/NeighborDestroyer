using UnityEngine;

public class Tile : MonoBehaviour
{
	[SerializeField] private Vector2Int myCoordinate;
	[SerializeField] private Item myItem;
	[SerializeField] private SpriteRenderer mySpriteIcon;
	[SerializeField] private Tile[] myNeighbors = new Tile[4];

	public Item MyItem { get => myItem; }
	public Vector2Int MyCoordinate { get => myCoordinate; }
	public Tile[] MyNeighbors { get => myNeighbors; }

	private void Start()
    {
	}
	/// <summary>
	/// Set and show tile's item and lockCount.
	/// </summary>
	/// <param name="boardTile">Our tile's item and lockCount</param>
	public void SetMyTile(Item item)
	{
		myItem = item;
        if (mySpriteIcon == null)
        {
			mySpriteIcon = GetComponent<SpriteRenderer>();
		}
		mySpriteIcon.sprite = myItem.icon;
	}
	public void SetMyCoordinate(Vector2Int coordinate)
	{
		myCoordinate = coordinate;
	}
	public void SetMyNeighbors()
	{
		Tile north = myCoordinate.y == Board_Manager.Instance.Height - 1 ? null : Board_Manager.Instance.Tiles[myCoordinate.x, myCoordinate.y + 1];
		Tile south = myCoordinate.y == 0 ? null : Board_Manager.Instance.Tiles[myCoordinate.x, myCoordinate.y - 1];
		Tile east = myCoordinate.x == Board_Manager.Instance.Width - 1 ? null : Board_Manager.Instance.Tiles[myCoordinate.x + 1, myCoordinate.y];
		Tile west = myCoordinate.x == 0 ? null : Board_Manager.Instance.Tiles[myCoordinate.x - 1, myCoordinate.y];

		myNeighbors[0] = north;
		myNeighbors[1] = south;
		myNeighbors[2] = east;
		myNeighbors[3] = west;
	}
	private void OnMouseUpAsButton()
    {
		Board_Manager.Instance.ChooseTile(this);
	}
}