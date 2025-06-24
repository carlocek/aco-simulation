using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(LineRenderer))]
public class AnimatedEdge : MonoBehaviour
{
    private LineRenderer lr;
    private float fadeDuration = 1f;
    private float lifeTime = 0.1f; // time before starting to fade

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void AnimateEdge(Vector3 start, Vector3 end, float duration, Boolean fade = true)
    {
        StartCoroutine(AnimateRoutine(start, end, duration, fade));
    }

    private IEnumerator AnimateRoutine(Vector3 start, Vector3 end, float duration, Boolean fade = true)
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
        if (fade==true)
            StartCoroutine(FadeOutAfterDelay());
    }

    private IEnumerator FadeOutAfterDelay()
    {
        yield return new WaitForSeconds(lifeTime);

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

