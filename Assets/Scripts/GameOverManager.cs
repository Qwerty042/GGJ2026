using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI gameOverText;      // Your main Game Over text
    public TextMeshProUGUI instructionsText;  // The text that fades in
    public TextMeshProUGUI bestText;          // The text showing high score / new record

    [Header("Settings")]
    public float inputDelay = 2f;        // Seconds before input is accepted
    public string nextSceneName = "EnemySelect";
    public float fadeDuration = 1f;      // How long the fade takes

    private bool canAcceptInput = false;

    public AudioSource gameOverSource;

    void Start()
    {
        // Set Game Over text
        if (gameOverText != null)
        {
            gameOverText.text = $"Win Streak: {GlobalGameState.playerScore}";
        }

        // Hide instructions at start
        if (instructionsText != null)
        {
            var color = instructionsText.color;
            color.a = 0f;
            instructionsText.color = color;
        }

        // Handle high score
        HandleHighScore();

        gameOverSource.volume = 1.0f;

        // Start the input delay
        StartCoroutine(EnableInputAfterDelay());
    }

    void Update()
    {
        if (canAcceptInput && Input.anyKeyDown)
        {
            // Reset score and health
            GlobalGameState.playerScore = 0;
            GlobalGameState.playerHealth = GlobalGameState.MAX_PLAYER_HEALTH;
            
            try
            {
                AudioManager.Instance.SetVolume("Background", 1.0f);
                gameOverSource.volume = 0;
            }
            catch
            {
                Debug.Log("No audio manager");
            }

            // Load next scene
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private System.Collections.IEnumerator EnableInputAfterDelay()
    {
        yield return new WaitForSeconds(inputDelay);
        canAcceptInput = true;

        // Fade in instructions if assigned
        if (instructionsText != null)
        {
            StartCoroutine(FadeInText(instructionsText, fadeDuration));
        }
    }

    private System.Collections.IEnumerator FadeInText(TextMeshProUGUI text, float duration)
    {
        float elapsed = 0f;
        Color color = text.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            text.color = color;
            yield return null;
        }

        // Ensure fully visible at the end
        color.a = 1f;
        text.color = color;
    }

    private void HandleHighScore()
    {
        if (bestText == null) return;

        int currentScore = GlobalGameState.playerScore;
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > highScore)
        {
            // New record!
            PlayerPrefs.SetInt("HighScore", currentScore);
            bestText.text = "New Record!";
        }
        else
        {
            bestText.text = $"Best: {highScore}";
        }

        PlayerPrefs.Save();
    }
}