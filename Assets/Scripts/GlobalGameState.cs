using UnityEngine;

public static class GlobalGameState
{
    public const float MAX_PLAYER_HEALTH = 100.0f;
    public static float playerHealth = MAX_PLAYER_HEALTH;
    public static string nextLuchadorName = "El Modismo";
    public static string nextLuchadorCsvFileName = "el_modismo";
    public static string prevLuchadorName;
    public static int playerScore;
}
