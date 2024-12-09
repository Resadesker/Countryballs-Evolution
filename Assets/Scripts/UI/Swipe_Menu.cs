using UnityEngine;
using UnityEngine.UI;

public class Swipe_Menu : MonoBehaviour
{
    public Scrollbar scrollbar;
    float[] pos;
    bool isDragging = false;
    float targetScrollPos = 0f;

    void Start()
    {
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            targetScrollPos = scrollbar.value;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            float distance = 1f / (pos.Length - 1f);
            int closestIndex = 0;
            float closestDistance = Mathf.Infinity;
            for (int i = 0; i < pos.Length; i++)
            {
                float dist = Mathf.Abs(scrollbar.value - pos[i]);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestIndex = i;
                }
            }
            targetScrollPos = pos[closestIndex];
        }

        if (isDragging)
        {
            scrollbar.value = Mathf.Clamp01(targetScrollPos + (-Input.GetAxis("Mouse Y") * 0.05f));
        }
        else
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetScrollPos, 0.1f);
        }
    }
}