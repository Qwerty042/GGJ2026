using UnityEngine;
using System.Collections;

public class LuchadorShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        // Store the camera's original local position
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Generate random offsets around the original position
            float x = Random.Range(-0.5f, 0.5f) * magnitude;
            float y = Random.Range(-0.5f, 0.5f) * magnitude;

            // Apply the random offset (keeping the original Z position for 2D)
            transform.localPosition = new Vector3(originalPos.x + x+0.25f, originalPos.y + y+0.25f, originalPos.z);

            elapsed += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Reset the camera to its original position after the shake is done
        transform.localPosition = originalPos;
    }
}
