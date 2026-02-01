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
            { "Rey Lucha",         new[] { "reysprite", "reythumb" } },
            { "Ultimo Memer",      new[] { "memersprite", "memerthumb" } },
            { "El Politico",       new[] { "politicosprite", "politicothumb" } },
            { "Obscuro",           new[] { "obscurosprite", "obscurothumb" } },
        };
}
