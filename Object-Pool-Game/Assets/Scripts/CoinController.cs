using System.Collections;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private readonly float waitUpTime = 2.0f; // wait time when reachs the up limit
    private readonly float speed = 3f; // coin speed
    private readonly float offsetUp = 0.3f; // offset for up limit
    private readonly float offsetDown = -0.7f; // offset for down limit
   
    private SpriteRenderer spr;
    private int currentSlot = -1; // current slot that coin is associated
    private float yLimitMax = 0f; // up limit
    private float yLimitMin = 0f; // down limit

    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Coin Initialization 
    /// </summary>
    /// <param name="slot">index of the slot/hole that coin will be associated</param>
    public void InitCoin(int slot)
    {
        currentSlot = slot;
        Vector3 position = HolePoolManager._Instance.GetSlotPosition(slot);

        transform.position = position + Vector3.up * offsetDown;
        yLimitMax = position.y + offsetUp;
        yLimitMin = position.y + offsetDown;

        // each hole has a Sprite mask whith a custom interval of sortingOrder based on his index
        spr.sortingOrder = currentSlot;
    }

    /// <summary>
    /// Something clicked the coin. Release.
    /// </summary>
    public void ClickCoin()
    {
        StopAllCoroutines();
        ResetCoin();
    }

    /// <summary>
    /// Start the StateManager co-routine
    /// </summary>
    public void StartMovement()
    {
        StartCoroutine(StateManager());
    }

    /// <summary>
    /// Release the coin
    /// </summary>
    void ResetCoin()
    {
        HolePoolManager._Instance.ReleaseSlot(currentSlot);
        currentSlot = -1;
        transform.position = Vector2.one * 1000f;
        CoinPoolManager._Instance.ReleaseCoin(this);
    }

    /// <summary>
    /// Coin movement co-routine 
    /// </summary>
    /// <returns></returns>
    IEnumerator StateManager()
    {
        yield return StartCoroutine(UpState());
        yield return StartCoroutine(WaitUpState());
        yield return StartCoroutine(DownState());
        ResetCoin();
    }

    /// <summary>
    /// Up movement co-routine
    /// </summary>
    /// <returns></returns>
    IEnumerator UpState()
    {
        while (transform.position.y + speed * Time.deltaTime < yLimitMax) {
            transform.Translate(0, speed * Time.deltaTime, 0);
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, yLimitMax, 0);
    }

    /// <summary>
    /// Down movement co-routine
    /// </summary>
    /// <returns></returns>
    IEnumerator DownState()
    {
        while (transform.position.y - speed * Time.deltaTime > yLimitMin)
        {
            transform.Translate(0, -speed * Time.deltaTime, 0);
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, yLimitMax, 0);
    }

    /// <summary>
    /// WaitUp co-routine
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitUpState()
    {
        yield return new WaitForSeconds(waitUpTime);
    }

}
