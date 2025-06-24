using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(LineRenderer))]
public class AnimatedEdge : MonoBehaviour
{
    private LineRenderer lr;
    public GameObject antSpritePrefab;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void AnimateEdge(Vector3 start, Vector3 end, float drawDuration, float fadeDelay, float fadeDuration, bool fade = true)
    {
        StartCoroutine(AnimateRoutine(start, end, drawDuration, fadeDelay, fadeDuration, fade));
    }

    private IEnumerator AnimateRoutine(Vector3 start, Vector3 end, float duration, float fadeDelay, float fadeDuration, bool fade = true)
    {
        lr.SetPosition(0, start);
        lr.SetPosition(1, start);

        GameObject antVisual = null;
        if (antSpritePrefab != null)
            antVisual = Instantiate(antSpritePrefab, start, Quaternion.identity, transform);

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 pos = Vector3.Lerp(start, end, t);
            lr.SetPosition(1, pos);

            if (antVisual != null)
            {
                antVisual.transform.position = pos;
                antVisual.transform.right = (end - start).normalized; // direzione del movimento
            }

            yield return null;
        }

        lr.SetPosition(1, end);
        if (antVisual != null)
            Destroy(antVisual);
        if (fade)
            StartCoroutine(FadeOutAfterDelay(fadeDelay, fadeDuration));
    }

    private IEnumerator FadeOutAfterDelay(float fadeDelay, float fadeDuration)
    {
        yield return new WaitForSeconds(fadeDelay);

        Color startColor = lr.startColor;
        Color endColor = lr.endColor;
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            Color faded = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1, 0, t));
            lr.startColor = lr.endColor = faded;
            yield return null;
        }

        Destroy(gameObject);
    }
}

