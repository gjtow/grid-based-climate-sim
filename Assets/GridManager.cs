using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile tilePrefab;

    [SerializeField] private Transform _cam;

    public Tile[,] mapTiles;

    public Texture2D heightmap;
    public Texture2D gradient;

    public bool tidalLock;
    public bool seasonal;

    public float velocity = 1, topoScale = 1, seaLevel = 1, axialTilt;
    public float luminosity = 1, orbitalDist = 1, co2Prc = 0.0408f, atmPrs = 1;
    public float orbitalDist2 = 1, seaLevel2 = 1;
    public float temp = 15;
    public float temp2 = 15;

    public int count;

    public Tile startTile;

    public List<Tile> zeroDeg;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
        AssignTileNeighbors();
    }

    void GenerateGrid()
    {
        float baseTemp = Mathf.Pow(luminosity * 3.846E+26f * (1 - 0.3f)
                       / (16 * Mathf.PI * Mathf.Pow(orbitalDist * 149597870700, 2) * 0.00000005670373f)
                       , 0.25f);
        temp = baseTemp + 33 + (co2Prc * atmPrs * 0.045f) - 273.15f - 1.89f + 2.15347f;
        baseTemp = Mathf.Pow(luminosity * 3.846E+26f * (1 - 0.3f)
                       / (16 * Mathf.PI * Mathf.Pow(orbitalDist2 * 149597870700, 2) * 0.00000005670373f)
                       , 0.25f);
        temp2 = baseTemp + 33 + (co2Prc * atmPrs * 0.045f) - 273.15f - 1.89f + 2.15347f;

        mapTiles = new Tile[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x * 16 / (float)_width, y * 9 / (float)_height), Quaternion.identity, null);
                spawnedTile.transform.name = $"Tile {x} {y}";
                spawnedTile.transform.localScale = new Vector3(16 / (float)_width, 9 / (float)_height, 1);

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.pos = new Vector2(x, y);
                spawnedTile._width = _width;
                spawnedTile._height = _height;
                spawnedTile.heightmap = heightmap;
                spawnedTile.gradient = gradient;
                spawnedTile.velocity = velocity;
                spawnedTile.topoScale = topoScale * 20000;
                spawnedTile.seaLevel = seaLevel * 11220;
                spawnedTile.sunStrength = temp;
                spawnedTile.sunStrength2 = temp2;
                spawnedTile.axialTilt = axialTilt;
                spawnedTile.tidalLock = tidalLock;
                spawnedTile.seasonal = seasonal;
                spawnedTile.count = count;

                mapTiles[x, y] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3((8 - 16 / (float)_width / 2) * 2f, (4.5f - 9 / (float)_height / 2) * 2f, -10);
    }

    void AssignTileNeighbors()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Tile tile = mapTiles[x, y];

                if (y < _height - 1)
                {
                    tile.neighbor_UP = mapTiles[x, y + 1];
                }
                if (y > 0)
                {
                    tile.neighbor_DOWN = mapTiles[x, y - 1];
                }
                if (x > 0)
                {
                    tile.neighbor_LEFT = mapTiles[x - 1, y];
                }
                if (x < _width - 1)
                {
                    tile.neighbor_RIGHT = mapTiles[x + 1, y];
                }
                if (x == 0)
                {
                    tile.neighbor_LEFT = mapTiles[_width - 1, y];
                }
                if (x == _width - 1)
                {
                    tile.neighbor_RIGHT = mapTiles[1, y];
                }
                if (tile.neighbor_UP == null)
                {
                    tile.neighbor_UP = tile;
                }
                if (tile.neighbor_DOWN == null)
                {
                    tile.neighbor_DOWN = tile;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("escape"))
        {
            Application.Quit();
        }
    }
}
