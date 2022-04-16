using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Coin pool manager
/// </summary>
public class CoinPoolManager : MonoBehaviour
{
    public static CoinPoolManager _Instance; // singleton instance
    private CoinController coinPrefab; // coin prefab

    private readonly string prefabPath = "Assets/Prefabs/coin.prefab"; // coin prefab path
    private readonly int initSizePool = 10; // pool size
    private readonly List<CoinController> availableCoins = new List<CoinController>(); // coins List (pool)
    private readonly List<CoinController> inUseCoins = new List<CoinController>(); // coins List (pool)

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
        // Load coin prefab and create the pool of coins

        var operation = Addressables.LoadAssetAsync<GameObject>(prefabPath);
        coinPrefab = (operation.WaitForCompletion()).GetComponent<CoinController>();

        for (int i = 0; i < initSizePool; i++)
        {
            CoinController coin = Instantiate(coinPrefab, Vector3.one * 1000f, Quaternion.identity, transform);
            availableCoins.Add(coin);
        }
    }

    /// <summary>
    /// Returns the avaialable coin. Otherwise, return a new one
    /// </summary>
    /// <returns>Avaialable coin</returns>
    public CoinController GetCoin()
    {
        CoinController coin;

        if(availableCoins.Count > 0)
        {
            coin = availableCoins[0];
            inUseCoins.Add(coin);
            availableCoins.RemoveAt(0);
        }
        else
        {
            coin = Instantiate(coinPrefab, Vector3.one * 1000f, Quaternion.identity, transform);
            inUseCoins.Add(coin);
        }

        return coin;
    }

   /// <summary>
   /// Release a coin to be used again.
   /// </summary>
   /// <param name="coin">Coin to be realeased</param>
    public void ReleaseCoin(CoinController coin)
    {
        availableCoins.Add(coin);
        inUseCoins.Remove(coin);
    }
}