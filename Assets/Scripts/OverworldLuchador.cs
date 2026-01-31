using UnityEngine;

public class OverworldLuchador : MonoBehaviour
{

    public string luchadorName;
    public SpriteRenderer spriteRenderer;

    public void Initialize(string name, Sprite sprite)
    {
        luchadorName = name;
        spriteRenderer.sprite = sprite;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
