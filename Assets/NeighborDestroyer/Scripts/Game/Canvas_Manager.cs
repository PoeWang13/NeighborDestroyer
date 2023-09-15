using TMPro;
using DG.Tweening;
using UnityEngine;
using System.Text;

public class Canvas_Manager : MonoBehaviour
{
	private static Canvas_Manager instance;
	public static Canvas_Manager Instance { get => instance; }
    private const float DoTweenDuration = 0.25f;

    [SerializeField] private GameObject panelLevelComplete;
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textLevelComplete;
    [SerializeField] private TextMeshProUGUI textScoreLevelComplete;
    [SerializeField] private TextMeshProUGUI textStep;

    private StringBuilder sb = new StringBuilder();
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
        textScore.text = "Score : " + 0;
    }
    /// <summary>
    /// Set our match score.
    /// </summary>
    public void SetScore()
    {
        DOTween.To(value => 
            {
                textScore.text = "Score : " + (Board_Manager.Instance.Score + (int)value); 
            }, 
            startValue: 0, 
            endValue: Board_Manager.Instance.ScoreAdd, duration: DoTweenDuration).OnComplete(() => 
            { 
                Board_Manager.Instance.SetScore();
                Board_Manager.Instance.SetScore();
            });
    }
    public void LevelWin()
    {
        LevelFinish("Level Win");
    }
    public void LevelLost()
    {
        LevelFinish("Level Lost");
    }
    private void LevelFinish(string levelStatus)
    {
        SetActiveLevelFinishPanel(true);
        textLevelComplete.text = levelStatus;

        SetText("Score", Board_Manager.Instance.Score, textScoreLevelComplete);
        SetText("Step", Board_Manager.Instance.Step, textStep);
    }
    private void SetText(string title, int value, TextMeshProUGUI textPro)
    {
        sb.Length = 0;
        sb.Append(title);
        sb.AppendLine();
        sb.Append(value);
        textPro.text = sb.ToString();
    }
    public void SetActiveLevelFinishPanel(bool isActive)
    {
        panelLevelComplete.SetActive(isActive);
    }
}