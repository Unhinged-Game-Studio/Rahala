using UnityEngine;

public class CoinCollision : MonoBehaviour
{
    private PlayerStats _playerStats;

    void Start()
    {
        _playerStats = PlayerStats.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (this.CompareTag("Gold"))
                _playerStats.coinCollected(50);
            if (this.CompareTag("Silver"))
                _playerStats.coinCollected(20);
            if (this.CompareTag("Brass"))
                _playerStats.coinCollected(5);
            if (this.CompareTag("DarkGold"))
                _playerStats.coinCollected(1);
            Destroy(this.gameObject);
        }
    }
}
