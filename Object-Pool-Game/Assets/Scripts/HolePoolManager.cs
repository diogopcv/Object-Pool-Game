using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Hole pool manager
/// </summary>
public class HolePoolManager : MonoBehaviour
{
    public static HolePoolManager _Instance; // singleton instance
    private Transform holePrefab; // hole prefab

    private readonly string prefabPath = "Assets/Prefabs/Hole.prefab"; // hole prefab path
    private readonly Vector2 referenceResolution = new Vector2(1125, 2436); // reference resolution (worst case) used to create hole grid
    private readonly float bottomPercentageSpace = 0.1f; // grid bottom offset (percentage)
    private readonly float topPercentageSpace = 0.1f; // grid top offset (percentage)
    private readonly List<Transform> slots = new List<Transform>(); // hole list (fixed pool)
    private readonly List<int> freeSlotsIndexes = new List<int>(); // indexes of available slots

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
        // Load hole prefab and create the grid of holes

        var operation = Addressables.LoadAssetAsync<GameObject>(prefabPath);
        holePrefab = (operation.WaitForCompletion()).transform;

        InitHolesGrid();
    }

    /// <summary>
    /// Create grid of holes
    /// </summary>        
    private void InitHolesGrid()
    {
        Camera cam = Camera.main;
        Vector2 camLimitBottomLeft = cam.ScreenToWorldPoint(new Vector3(0, Screen.height * bottomPercentageSpace, 0));
        Vector2 camLimitUpRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height * (1 - topPercentageSpace), 0));

        SpriteMask sprHoleMouse = holePrefab.GetComponent<SpriteMask>();
        Vector2 sizeHoleMouse = sprHoleMouse.bounds.size;

        // Vertical size and Horizontal size of the camera box
        Vector2 sizeCam = camLimitUpRight - camLimitBottomLeft;
        // Number of columns and rows of holes
        int columns, rows = (int)(sizeCam.y / sizeHoleMouse.y);

        // calculate of camera reference width based on reference resolution
        float refWidth = (referenceResolution.x / referenceResolution.y) * cam.orthographicSize * 2;
        // calculate the number of columns in even rows
        int columnsPar = (int)(refWidth / sizeHoleMouse.y);
        // calculate the number of columns in odd rows
        int columnsImpar = columnsPar - 1;

        // Y spacing between holes 
        float deltaY = sizeCam.y / rows;
        // calculate X spacing between holes in even rows
        float deltaX = sizeCam.x / columnsPar;

        // Auxiliary variable for defining the position of holes
        Vector2 position;
        // Auxiliary variables for x and y components to define the position of holes
        float posX, posY;
        float offsetX, offsetY = deltaY / 2f;

        int numberHole = 0;
        
        // Holes Creation
        for (int i = 0; i < rows; i++)
        {
            // odd row or even row?
            if (i % 2 == 0)
            {
                columns = columnsPar;
                offsetX = deltaX / 2f;
            }
            else
            {
                columns = columnsImpar;
                offsetX = deltaX;
            }

            // The Y position of holes in a row is based on the Y position of the lower left limit of the camera
            // more offset (half size of the hole), more line-related offset that the hole is
            posY = camLimitBottomLeft.y + offsetY + deltaY * i;
            for (int j = 0; j < columns; j++)
            {
                // The X position of holes in a row is based on the X position of the lower left limit of the camera
                // more offset (half size of the hole), plus column related offset that the hole is
                posX = camLimitBottomLeft.x + offsetX + deltaX * j;
                position = new Vector2(posX, posY);
                Transform go = Instantiate(holePrefab, position, Quaternion.identity, transform);
                slots.Add(go);
                freeSlotsIndexes.Add(numberHole);

                // Specifies through the "sorting order" who the specific mask will filter (avoid filtering neighboring sprites)
                SpriteMask mask = go.GetComponent<SpriteMask>();
                mask.isCustomRangeActive = true;
                mask.frontSortingOrder = numberHole;
                mask.backSortingOrder = numberHole - 1;
                numberHole++;
            }
        }
    }

    /// <summary>
    /// Get a random available hole/slot
    /// </summary>
    /// <returns></returns>
    public int GetRandomAvailableSlot()
    {
        if (freeSlotsIndexes.Count > 0)
        {
            int index = Random.Range(0, freeSlotsIndexes.Count);
            int slotPos = freeSlotsIndexes[index];
            freeSlotsIndexes.RemoveAt(index);
            return slotPos;
        }

        return -1;
    }

    /// <summary>
    /// Realease hole/slot
    /// </summary>
    /// <param name="slot">slot index</param>
    public void ReleaseSlot(int slot)
    {
        freeSlotsIndexes.Add(slot);
    }

    /// <summary>
    /// Get slot postion
    /// </summary>
    /// <param name="indexSlot">slot index</param>
    /// <returns></returns>
    public Vector3 GetSlotPosition(int indexSlot)
    {
        return slots[indexSlot].position;
    }
}
