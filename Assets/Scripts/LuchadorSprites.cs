using UnityEngine;
using System.Collections.Generic;


public static class LuchadorSprites
{
    public static readonly Dictionary<string, string[]> Map =
        new Dictionary<string, string[]>
        {
            { "El Modismo",        new[] { "ModismoSprite", "modismothumb" } },
            { "Nacho Libre",       new[] { "NachoSprite", "Nachothumb" } },
            { "Pantalla Plateada", new[] { "pantallaSprite", "patalathumb" } },
            { "Rey Lucha",         new[] { "ModismoSprite", "reythumb" } },
            { "Ultimo Memer",    new[] { "ModismoSprite", "memerthumb" } },
            { "El Politico",       new[] { "ModismoSprite", "politicothumb" } },
            { "Obscuro",           new[] { "obscurosprite", "obscurothumb" } },
        };
}
