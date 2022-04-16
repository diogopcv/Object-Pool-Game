using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D coll2D = Physics2D.OverlapPoint(cam.ScreenToWorldPoint(Input.mousePosition));

            CoinController coin;
            if (coll2D != null && (coin = coll2D.GetComponent<CoinController>()) != null)
            {
                coin.ClickCoin();
            }
        }
#else
        Touch touch;
        if(Input.touchCount > 0 && (touch = Input.GetTouch(0)).phase == TouchPhase.Began)
        {
            Collider2D coll2D = Physics2D.OverlapPoint(cam.ScreenToWorldPoint(touch.position));

            CoinController coin;
            if (coll2D != null && (coin = coll2D.GetComponent<CoinController>()) != null)
            {
                coin.ClickCoin();
            }
        }
#endif
    }
}
