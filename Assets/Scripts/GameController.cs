using System.Collections;
using System.Collections.Generic;
using AmazingAssets.CurvedWorld;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CurvedWorldController worldController;
    public GameObject Ship;
    public GameObject OnLand;
    public ParticleSystem explosion;
    public float forwardOffset = 2f;
    public float yOffset = 1f;
    public float forwardOffset2 = 2f;
    public float yOffset2 = 1f;

    public bool inputEnabled = true;

    public float cooldown = 2f; 
    private float lastSwitchTime = -Mathf.Infinity;

    public void SwitchPlayer()
    {
        if (!inputEnabled || Time.time < lastSwitchTime + cooldown) return;

        inputEnabled = false;
        lastSwitchTime = Time.time;
        Debug.Log("hit the ground");

        StartCoroutine(EnableInputAfterDelay(1f));

        Vector3 newPosition = Ship.transform.position +
                              Ship.transform.forward * forwardOffset +
                              Vector3.up * yOffset;

        explosion.transform.position = newPosition;
        OnLand.transform.rotation = Ship.transform.rotation;
        explosion.Play();

        OnLand.transform.position = newPosition;

        Ship.SetActive(false);
        OnLand.SetActive(true);

        worldController.bendPivotPoint = OnLand.transform;
    }

    public void SwitchPlayerreverse()
    {
        if (!inputEnabled || Time.time < lastSwitchTime + cooldown) return;

        inputEnabled = false;
        lastSwitchTime = Time.time;
        Debug.Log("hit the ground");

        StartCoroutine(EnableInputAfterDelay(1f));

        Vector3 newPosition2 = OnLand.transform.position +
                              OnLand.transform.forward * forwardOffset2 +
                              Vector3.up * yOffset2;

        explosion.transform.position = newPosition2;
        Ship.transform.rotation = OnLand.transform.rotation;
        explosion.Play();

        Ship.transform.position = newPosition2;

        Ship.SetActive(true);
        OnLand.SetActive(false);

        worldController.bendPivotPoint = Ship.transform;
    }

    private IEnumerator EnableInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        inputEnabled = true;
    }
}
