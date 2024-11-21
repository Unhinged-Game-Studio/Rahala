using UnityEngine;

public class Friendlyship : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemyball"))
        {
            Debug.Log("Hit by enemyball");
        }
    }
}
