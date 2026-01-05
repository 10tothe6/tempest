using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuParallax : MonoBehaviour
{
    public List<RectTransform> objects;
    public RectTransform cursor;
    public float[] modifiers;
    public float offset;

    void Update()
    {
        cursor.localPosition = new Vector3(Input.mousePosition.x - 960, Input.mousePosition.y - 540, 0);
        offset = (Input.mousePosition.x - 960) / 30;
        
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].localPosition = new Vector3(modifiers[i] * offset, 0, 0);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform currentTransform = transform.GetChild(i).gameObject.GetComponent<RectTransform>();
            if (!objects.Contains(currentTransform) && currentTransform != cursor)
            {
                currentTransform.localPosition = new Vector3(modifiers[modifiers.Length - 1] * offset, 0, 0);
            }
        }
    }
}
