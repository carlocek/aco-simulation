using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(LineRenderer))]
public class AnimatedEdge : MonoBehaviour
{
    private LineRenderer lr;
    
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

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            lr.SetPosition(1, Vector3.Lerp(start, end, t));
            yield return null;
        }

        lr.SetPosition(1, end);

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

