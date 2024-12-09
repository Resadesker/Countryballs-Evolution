using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class RandomFly : MonoBehaviour
{
    public float speed = 35f; // Adjust this to control the speed of movement
    public float angleRange = 45f; // Adjust this to control the range of angles the object can move within
    public float fadeDuration = 1f; // Adjust this to control the duration of the fade out effect
    private TMP_Text _text;
    private Image _image;
    [SerializeField] private bool isImage = false;

    void Start()
    {
        // Generate a random direction (left or right)
        int direction = Random.Range(0, 2) < 1 ? -1 : 1;

        // Generate a random angle within the specified range
        float angle = Random.Range(-angleRange, angleRange);

        if (!isImage) _text = GetComponent<TMP_Text>();
        else _image = GetComponent<Image>();

        // Calculate the movement vector based on the angle
        Vector3 directionVector = Quaternion.Euler(0, 0, angle) * Vector3.right * direction;

        // Apply the movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = directionVector.normalized * speed;
        }
        else
        {
            Debug.LogWarning("No Rigidbody2D component found. Add a Rigidbody2D component to the object.");
        }
        
        StartCoroutine(FadeOutAndDestroy());
    }
    
    IEnumerator FadeOutAndDestroy()
    {
        yield return new WaitForSeconds(1.8f);
        float elapsedTime = 0f;
        Color initialColor;
        if (!isImage) initialColor = _text.color;
        else initialColor = _image.color;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            if (!isImage) _text.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            else _image.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}