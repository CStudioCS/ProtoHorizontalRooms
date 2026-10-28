using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class TransitionEffectManager : MonoBehaviour
{
    [SerializeField] private Camera roomCamera;

    public IEnumerator TransitionEffect(Vector3 roomPosition)
    {
        const float duration = 0.65f;
        float elapsed = 0f;

        Vector3 cameraStart = roomCamera.transform.position;
        Vector3 cameraEnd = roomPosition + new Vector3(0, 0, -10);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f); // Ease out

            roomCamera.transform.position =
                Vector3.Lerp(cameraStart, cameraEnd, eased);

            yield return null;
        }
    }
}
