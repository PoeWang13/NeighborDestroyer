using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
	[SerializeField] private Vector2Int myCoordinate;
	[SerializeField] private BoardTile myBoardTile;
	[SerializeField] private Image myImageIcon;
	[SerializeField] private Image myImageLock;

	[SerializeField] private TextMeshProUGUI myTextLock;

	public BoardTile MyBoardTile { get => myBoardTile; }
	public Vector2Int MyCoordinate { get => myCoordinate; }


	[SerializeField] private Tile[] myNeighbors = new Tile[4];
	public Tile[] MyNeighbors { get => myNeighbors; }

	private void Start()
    {
	}
    /// <summary>
    /// Set and show tile's item and lockCount.
    /// </summary>
    /// <param name="boardTile">Our tile's item and lockCount</param>
    public void SetMyBoardTile(BoardTile boardTile)
	{
		myBoardTile = boardTile;
		myImageIcon.sprite = myBoardTile.item.icon;


		myTextLock.text = myBoardTile.lockCount.ToString();
		myTextLock.gameObject.SetActive(myBoardTile.lockCount > 1);
		myImageLock.gameObject.SetActive(myBoardTile.lockCount != 0);

		if (myBoardTile.lockCount >= 1)
		{
			myImageLock.sprite = Canvas_Manager.Instance.LockIcon;
		}
        if (myBoardTile.isLocked)
        {
			Destroy(GetComponent<Button>());
        }
	}
}