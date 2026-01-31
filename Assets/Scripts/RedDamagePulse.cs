using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class RedDamageOverlay : MonoBehaviour
{
    public IEnumerator Pulse()
    {
        float rate = 5.0f;
        while (gameObject.GetComponent<SpriteRenderer>().color.a < 0.5f)
        {
            Color tmpColor = gameObject.GetComponent<SpriteRenderer>().color;
            tmpColor.a += (rate * Time.deltaTime);
            gameObject.GetComponent<SpriteRenderer>().color = tmpColor;
            yield return null;
        }

        while (gameObject.GetComponent<SpriteRenderer>().color.a > 0f)
        {
            Color tmpColor = gameObject.GetComponent<SpriteRenderer>().color;
            tmpColor.a -= (rate * Time.deltaTime);
            gameObject.GetComponent<SpriteRenderer>().color = tmpColor;
            yield return null;
        }
    }
}
