using TMPro;
using DG.Tweening;
using UnityEngine;

public class Canvas_Manager : MonoBehaviour
{
	private static Canvas_Manager instance;
	public static Canvas_Manager Instance { get => instance; }
    private const float DoTweenDuration = 0.25f;

    [SerializeField] private TextMeshProUGUI textScore;
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
            endValue: Board_Manager.Instance.ScoreAdd, duration: DoTweenDuration)
            .OnComplete(() => { Board_Manager.Instance.SetScore(); });
    }
}