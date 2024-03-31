using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionScreen : MonoBehaviour
{
    [SerializeField] GameObject screen;
    [SerializeField] Image image;

    [SerializeField] private Color lightColor = new Color(0,0,0,0);
    [SerializeField] private Color darkColor = new Color(0, 0, 0, 1);
    private Color currentColor;

    private float animationTimer = 0;
    private const float AnimationTime = 4f;

    public static Action AnimationComplete;

    private void Start()
    {
        //Darken();
        //AnimationComplete += Lighten;
    }

    public void Lighten()
    {
        StartCoroutine(Animate(darkColor, lightColor));
    }

    public void Darken()
    {
        StartCoroutine(Animate(lightColor,darkColor));
    }
    private IEnumerator Animate(Color fromColor, Color toColor)
    {
        screen.SetActive(true);
        animationTimer = 0;
        image.color = fromColor;
        while (animationTimer < AnimationTime)
        {
            image.color = Color.Lerp(fromColor, toColor, animationTimer/AnimationTime);
            animationTimer += Time.unscaledDeltaTime;
            yield return null;
        }
        image.color = toColor;
        screen.SetActive(false);
        AnimationComplete?.Invoke();
    }



}
