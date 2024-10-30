using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropInstance : MonoBehaviour
{
    [SerializeField] public float damage = 0f;

    public void Hit(float dropPercentage, float score)
    {
        StartCoroutine(OnHitCoroutine());
    }

    private IEnumerator OnHitCoroutine()
    {
        float fadeCounter = 0f;
        // We are still fading out
        while (fadeCounter < 0.25f)
        {
            float alpha = Mathf.Lerp(1.0f, 0.0f, fadeCounter / 0.25f);
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, alpha);
            fadeCounter += Time.deltaTime;
            yield return null;
        }

        // We are done fading out
        Destroy(this.gameObject);
    }
}
