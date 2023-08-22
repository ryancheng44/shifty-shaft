using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Menu")]
    [SerializeField] private GameObject shiftyText;
    [SerializeField] private GameObject shaftText;

    [SerializeField] private GameObject leaderboardButton;

    [Header("Game")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject newText;

    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private Animator retryButton;

    public bool isGameOver { get; private set; } = false;
    public bool isGameInProgress { get; private set; } = false;

    private int score = 0;
    private int highScore;

    // Start is called before the first frame update
    public void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        highScore = PlayerPrefs.GetInt("High Score", 0);

        if (!Social.localUser.authenticated)
            Authenticate();
        
        ReportScore(highScore, 1);
    }

    private void Authenticate() {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
                Debug.Log("Authentication of " + Social.localUser.userName + " successful");
            else
                Debug.Log("Authentication failed");
        });
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void GameOver()
    {
        isGameOver = true;

        StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake());

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("High Score", highScore);
            newText.SetActive(true);
        }

        highScoreText.text = "high score: " + highScore.ToString();
        highScoreText.gameObject.SetActive(true);

        pauseButton.SetActive(false);
        retryButton.gameObject.SetActive(true);
    }

    public void GameInProgress() {
        isGameInProgress = true;
        shiftyText.SetActive(false);
        shaftText.SetActive(false);
        scoreText.gameObject.SetActive(true);
        pauseButton.SetActive(true);
        leaderboardButton.SetActive(false);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseButton.SetActive(false);
        resumeButton.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        resumeButton.SetActive(false);
        pauseButton.SetActive(true);
    }

    private void ReportScore(int score, int times)
    {
        if (times > 3)
            return;

        if (!Social.localUser.authenticated) {
            Authenticate();
            ReportScore(score, times++);
        } else {
            Social.ReportScore((long)score, "globalLeaderboard", success =>
            {
                if (success)
                    Debug.Log("Reported score of " + score + " successfully");
                else
                    Debug.Log("Failed to report score");
            });
        }
    }

    public void ShowLeaderboard(int times)
    {
        if (times > 3)
            return;

        if (!Social.localUser.authenticated) {
            Authenticate();
            ShowLeaderboard(times++);
        } else {
            Social.ShowLeaderboardUI();
        }
    }
}
