using System;
using UnityEngine;

public class LuchadorSpawner : MonoBehaviour
{
    public GameObject luchadorPrefab;
    public GameObject playerHealthBar;
    public Transform[] spawnPoints;
    public string[] luchadorNames;
    public string[] luchadorCsvFileNames;
    public Sprite[] luchadorSprites;
    public int score;
    public int lastLucha;

    void Start()
    {
        lastLucha = Array.IndexOf(luchadorNames, GlobalGameState.prevLuchadorName);
        SpawnLuchadors();
        UpdateHeathBar(playerHealthBar, GlobalGameState.playerHealth/GlobalGameState.MAX_PLAYER_HEALTH);
    }

    void SpawnLuchadors()
    {
        if (score == 0)
        {
            SpawnLuchador(0, spawnPoints[0].position);
        }
        else if (score < 10)
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
        obj.GetComponent<OverworldLuchador>().Initialize(luchadorNames[index], luchadorCsvFileNames[index], luchadorSprites[index]);
    }

    void UpdateHeathBar(GameObject healthBar, float healthRatio)
    {
        healthBar.GetComponentsInChildren<Transform>()[1].localScale = new Vector3(healthRatio, 1, 1);
        healthBar.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(-0.5f*(1.0f-healthRatio), 0, 0);
    }
}
