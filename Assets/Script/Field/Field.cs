using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] GameObject TilePrefabs;

    const int MaxX = 8;
    const int MaxY = 3;

    public Tile[,] TileArray = new Tile[MaxY, MaxX];

    void Awake()
    {
        FieldSet();
    }
    
    void FieldSet()
    {
        Vector3 vec = transform.position;

        float disX = transform.localScale.x / MaxX;
        float disY = transform.localScale.y / MaxY;

        for (int i = -1; i < 2; i++)
        {
            for (int j = -4; j < 4; j++)
            {
                float x = (disX * j) + (disX * 0.5f);
                float y = disY * i;

                GameObject tile = GameObject.Instantiate(TilePrefabs, gameObject.transform);
                tile.transform.position = new Vector3(vec.x + x, vec.y + y);

                TileArray[i + 1, j + 4] = tile.GetComponent<Tile>();
            }
        }

        //transform.eulerAngles = new Vector3(30, 0, 0);
    }

    public Vector3 GetTileLocate(int x, int y)
    {
        Vector3 vec =TileArray[y, x].transform.position;

        float sizeX = TileArray[y, x].transform.localScale.x * 0.5f;
        float sizeY = TileArray[y, x].transform.localScale.y * 0.5f;

        vec.x += sizeX;
        vec.y += sizeY;

        return vec;
    }
}
