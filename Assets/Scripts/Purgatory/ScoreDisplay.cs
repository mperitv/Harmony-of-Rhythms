using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    public void Refresh(int score)
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.text = $"Score {score}";
    }
}
