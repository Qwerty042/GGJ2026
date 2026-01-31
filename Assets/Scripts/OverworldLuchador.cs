using UnityEngine;

public class OverworldLuchador : MonoBehaviour
{

    public string luchadorName;
    public string luchadorCsvFileName;
    public SpriteRenderer spriteRenderer;

    public void Initialize(string name, string csvFileName, Sprite sprite)
    {
        luchadorName = name;
        luchadorCsvFileName = csvFileName;
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
