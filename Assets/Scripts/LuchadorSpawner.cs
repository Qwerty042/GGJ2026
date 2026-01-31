using System;
using UnityEngine;
using TMPro;

public class LuchadorSpawner : MonoBehaviour
{
    public GameObject luchadorPrefab;
    public GameObject playerHealthBar;
    public Transform[] spawnPoints;
    public string[] luchadorNames;
    public string[] luchadorCsvFileNames;
    public Sprite[] luchadorSprites;
    public int lastLucha;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI nombreText;
    public TextMeshProUGUI descriptionText;


    void Start()
    {
        lastLucha = Array.IndexOf(luchadorNames, GlobalGameState.prevLuchadorName);
        SpawnLuchadors();
        UpdateHeathBar(playerHealthBar, GlobalGameState.playerHealth/GlobalGameState.MAX_PLAYER_HEALTH);
        Debug.Log(GlobalGameState.playerScore);
        scoreText.text = $"Win Streak: {GlobalGameState.playerScore}";

        nombreText.text = "";
        descriptionText.text = "";
    }

    void SpawnLuchadors()
    {
        if (GlobalGameState.playerScore == 0)
        {
            SpawnLuchador(0, spawnPoints[0].position);
        }
        else if (GlobalGameState.playerScore < 10)
        {
            int[] selectedLuchas = new int[3] { -1, -1, -1 };
            int i = 0;
            while (i < 3)
            {
                int n = UnityEngine.Random.Range(0, 6);
                if (n == lastLucha || System.Array.Exists(selectedLuchas, x => x == n)) continue;
                selectedLuchas[i] = n;
                i++;
            }

            SpawnLuchador(selectedLuchas[0], spawnPoints[0].position);
            SpawnLuchador(selectedLuchas[1], spawnPoints[1].position);
            SpawnLuchador(selectedLuchas[2], spawnPoints[2].position);
        }
        else
        {
            int[] selectedLuchas = new int[3] { -1, -1, -1 };
            int i = 0;
            while (i < 3)
            {
                int n = UnityEngine.Random.Range(0, 7);
                if (n == lastLucha || System.Array.Exists(selectedLuchas, x => x == n)) continue;
                selectedLuchas[i] = n;
                i++;
            }

            SpawnLuchador(selectedLuchas[0], spawnPoints[0].position);
            SpawnLuchador(selectedLuchas[1], spawnPoints[1].position);
            SpawnLuchador(selectedLuchas[2], spawnPoints[2].position);
        }
        
    }
  
    void SpawnLuchador(int index, Vector3 position)
    {
        var obj = Instantiate(luchadorPrefab, position, Quaternion.identity);
        obj.transform.localScale = Vector3.one;

        // Ensure the SpriteRenderer is on the right Sorting Layer
        var sr = obj.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Luchadors"; // create this layer if needed
        sr.sortingOrder = 10;

        // Ensure prefab has a BoxCollider2D for mouse-over
        if (obj.GetComponent<Collider2D>() == null)
            obj.gameObject.AddComponent<BoxCollider2D>();

        obj.GetComponent<OverworldLuchador>().Initialize(
            luchadorNames[index],
            luchadorCsvFileNames[index],
            luchadorSprites[index]
        );

        obj.GetComponent<LuchadorHover>().nombreText = nombreText;
        obj.GetComponent<LuchadorHover>().descriptionText = descriptionText;
        obj.GetComponent<LuchadorHover>().SetName(luchadorNames[index]);

        Debug.Log($"Spawned {luchadorNames[index]} at {position}");
    }

    void UpdateHeathBar(GameObject healthBar, float healthRatio)
    {
        healthBar.GetComponentsInChildren<Transform>()[1].localScale = new Vector3(healthRatio, 1, 1);
        healthBar.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(-0.5f*(1.0f-healthRatio), 0, 0);
    }
}
