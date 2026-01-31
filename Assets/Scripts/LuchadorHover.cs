using UnityEngine;
using UnityEngine.SceneManagement;

public class LuchadorHover : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    public float hoverScale = 1.2f; // how much it grows when hovered
    public float lerpSpeed = 10f;   // speed of smooth scaling

    private SpriteRenderer spriteRenderer;
    public Color hoverColor = Color.yellow; // optional glow color
    private Color originalColor;

    public AudioClip hoverSound;
    private AudioSource audioSource;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

    }

    void OnMouseEnter()
    {
        // Set target scale to hover
        targetScale = originalScale * hoverScale;

        // Change color immediately
        if (spriteRenderer != null)
            spriteRenderer.color = hoverColor;

        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    void OnMouseExit()
    {
        // Set target scale back to original
        targetScale = originalScale;

        // Revert color immediately
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked!");
        string thisLuchadorName = GetComponent<OverworldLuchador>().luchadorName;
        AudioManager.Instance.SetVolume(thisLuchadorName, 1.0f);
        GlobalGameState.nextLuchadorCsvFileName = GetComponent<OverworldLuchador>().luchadorCsvFileName;
        GlobalGameState.nextLuchadorName = thisLuchadorName;
        SceneManager.LoadScene("FightScene");
    }

    void Update()
    {
        // Smoothly interpolate the scale toward the target
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * lerpSpeed);
    }
}
