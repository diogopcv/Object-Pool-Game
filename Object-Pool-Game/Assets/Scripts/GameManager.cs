using System.Collections;
using UnityEngine;

/// <summary>
/// Manager class of the Game
/// </summary>
public class GameManager : MonoBehaviour
{
    private readonly float waitPickCoinTime = 0.5f; // interval between turns

    public static GameManager _Instance; // singleton instance

    // Singleton Initialization
    void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(LoopGame());
    }

    /// <summary>
    /// Main loop game co-routine. 
    /// </summary>
    /// <returns></returns>
    IEnumerator LoopGame()
    {
        while (true)
        {
            // Get a random available hole/slot
            int slotIndex = HolePoolManager._Instance.GetRandomAvailableSlot();
            if (slotIndex < 0)
                yield return null;

            // Get a available coin from the coin pool
            CoinController coin = CoinPoolManager._Instance.GetCoin();

            // initialization of the coin
            coin.InitCoin(slotIndex);

            // start the movement of the coin
            coin.StartMovement();

            // wait for the next pick
            yield return new WaitForSeconds(waitPickCoinTime);
        }
    }
}
