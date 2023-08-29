using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float magnitude;

    public IEnumerator Shake()
    {
        Vector3 originalPosition = transform.position;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude / 100f;
            float y = Random.Range(-1f, 1f) * magnitude / 100f;

            transform.position += new Vector3(x, y);

            yield return null;
            elapsed += Time.deltaTime;
        }

        transform.position = originalPosition;
    }
}
