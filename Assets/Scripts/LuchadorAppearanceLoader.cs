using UnityEngine;

public class LuchadorAppearanceLoader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private SpriteRenderer thumbSprite;

    void Start()
    {
        string luchadorName = GlobalGameState.nextLuchadorName;

        if (!LuchadorSprites.Map.TryGetValue(luchadorName, out var files))
        {
            Debug.LogError($"No sprite mapping for luchador: {luchadorName}");
            return;
        }

        Sprite main  = Resources.Load<Sprite>($"Luchadors/{files[0]}");
        Sprite thumb = Resources.Load<Sprite>($"Luchadors/{files[1]}");

        if (main == null || thumb == null)
        {
            Debug.LogError($"Failed to load sprites for {luchadorName}");
            return;
        }

        mainSprite.sprite = main;
        thumbSprite.sprite = thumb;
    }
}
