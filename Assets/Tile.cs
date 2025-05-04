using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile : MonoBehaviour
{
    public int count;

    public Texture2D heightmap;
    public Texture2D gradient;
    public Vector2 rect;

    public int _width, _height;

    public Vector2 pos;

    public Tile neighbor_UP;
    public Tile neighbor_DOWN;
    public Tile neighbor_LEFT;
    public Tile neighbor_RIGHT;
    public Tile spawnedTileRight;
    public Tile spawnedTileUp;
    public Tile spawnedTileUpRight;

    public bool tidalLock;
    public bool seasonal;

    public float scale;

    float range;

    public float sunStrength, sunStrength2, topoScale, seaLevel, velocity, latitude, temp, height, axialTilt;
    public float moisture, airMoisture;

    public float tempLat;

    public float leftVel = 0;
    public float rightVel = 0;
    public float upVel = 0;
    public float downVel = 0;
    public float angle = 0;

    public float exTemp = 0;
    public float finalTemp = 0;

    public float vapour = 0;

    public float summerMoisture = 0;
    public float summerTemp = 0;
    public float winterMoisture = 0;
    public float winterTemp = 0;

    bool viewTemp;
    bool viewMoist;
    bool viewHeight;
    bool viewVelo;
    bool viewComfort;
    bool viewNight;
    bool viewRange;

    bool viewShadow;
    bool viewClouds;

    float rand;

    public float seaSize;
    public float itczNorth;
    public float itczSouth;

    public float distFromSea;
    public float heightLake;
    public bool seaConnected;
    public float heightRiver;
    public bool river;

    public List<Tile> tiles;
    public List<float> heightsRiver;
    public float highestRiver;
    public Tile lowestTile;

    [SerializeField] private SpriteRenderer _renderer;

    [SerializeField] private Color _coldColor;

    public Color _bioColor;

    // Start is called before the first frame update
    void Start()
    {
        rect = new Vector2(Mathf.RoundToInt(pos.x * 2048 / _width), Mathf.RoundToInt(pos.y * 1024 / _height));
        height = heightmap.GetPixel((int)rect.x, (int)rect.y).grayscale * topoScale;

        scale = (40075 / _width);

        //float x = 5 * Mathf.Cos(Mathf.Lerp(-90, 270, pos.y / _width) * Mathf.Deg2Rad) * Mathf.Cos(Mathf.Lerp(0, 360, pos.x / _width) * Mathf.Deg2Rad);
        //float z = 5 * Mathf.Cos(Mathf.Lerp(-90, 270, pos.y / _width) * Mathf.Deg2Rad) * Mathf.Sin(Mathf.Lerp(0, 360, pos.x / _width) * Mathf.Deg2Rad);
        //float y = 5 * Mathf.Sin(Mathf.Lerp(-90, 270, pos.y / _width) * Mathf.Deg2Rad);

        //transform.position = new Vector3(x, y, z);
        //transform.LookAt(Vector3.zero);

        rand = Random.Range(1, 10);

        range = float.PositiveInfinity;

        if (height < seaLevel)
        {
            if (pos.x == 1)
            {
                seaSize = 1;
            }
        }
        else
        {
            if (neighbor_LEFT.height < seaLevel)
            {
                neighbor_LEFT.seaSize = 1;
            }
        }
        if (pos.y == _height / 2)
        {
            itczSouth = 1;
        }
        if (pos.y == _height / 2 + 1)
        {
            itczNorth = 1;
        }

        //seaSize = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (count < 0)
        {
            if (height < seaLevel)
            {
                if (neighbor_LEFT.height < seaLevel && neighbor_LEFT.seaSize < seaSize)
                {
                    neighbor_LEFT.seaSize = seaSize + 1;
                }
                if (neighbor_RIGHT.height < seaLevel && neighbor_RIGHT.seaSize < seaSize)
                {
                    neighbor_RIGHT.seaSize = seaSize;
                }
            }

            if (neighbor_DOWN.height < seaLevel && neighbor_DOWN.itczSouth < itczSouth)
            {
                neighbor_DOWN.itczSouth = itczSouth;
            }
            if (neighbor_DOWN.height >= seaLevel && neighbor_DOWN.itczSouth < itczSouth)
            {
                neighbor_DOWN.itczSouth = itczSouth + 1;
            }
            if (neighbor_UP.itczSouth < itczSouth)
            {
                neighbor_UP.itczSouth = itczSouth;
            }

            if (neighbor_UP.height < seaLevel && neighbor_UP.itczNorth < itczNorth)
            {
                neighbor_UP.itczNorth = itczNorth;
            }
            if (neighbor_UP.height >= seaLevel && neighbor_UP.itczNorth < itczNorth)
            {
                neighbor_UP.itczNorth = itczNorth + 1;
            }
            if (neighbor_DOWN.itczNorth < itczNorth)
            {
                neighbor_DOWN.itczNorth = itczNorth;
            }

            if (height >= seaLevel)
            {
                if (neighbor_LEFT.height < seaLevel || neighbor_RIGHT.height < seaLevel || neighbor_UP.height < seaLevel || neighbor_DOWN.height < seaLevel)
                {
                    distFromSea = 1;
                }
            }

            if (height > seaLevel && neighbor_LEFT.distFromSea <= neighbor_RIGHT.distFromSea
                                  && neighbor_LEFT.distFromSea <= neighbor_UP.distFromSea
                                  && neighbor_LEFT.distFromSea <= neighbor_DOWN.distFromSea)
            {
                distFromSea = neighbor_LEFT.distFromSea + 1;
            }
            if (height > seaLevel && neighbor_RIGHT.distFromSea <= neighbor_LEFT.distFromSea
                                  && neighbor_RIGHT.distFromSea <= neighbor_UP.distFromSea
                                  && neighbor_RIGHT.distFromSea <= neighbor_DOWN.distFromSea)
            {
                distFromSea = neighbor_RIGHT.distFromSea + 1;
            }
            if (height > seaLevel && neighbor_UP.distFromSea <= neighbor_LEFT.distFromSea
                                  && neighbor_UP.distFromSea <= neighbor_RIGHT.distFromSea
                                  && neighbor_UP.distFromSea <= neighbor_DOWN.distFromSea)
            {
                distFromSea = neighbor_UP.distFromSea + 1;
            }
            if (height > seaLevel && neighbor_DOWN.distFromSea <= neighbor_LEFT.distFromSea
                                  && neighbor_DOWN.distFromSea <= neighbor_UP.distFromSea
                                  && neighbor_DOWN.distFromSea <= neighbor_RIGHT.distFromSea)
            {
                distFromSea = neighbor_DOWN.distFromSea + 1;
            }

            _bioColor = Color.Lerp(Color.black, Color.white, distFromSea / 50);

            _renderer.color = _bioColor;

            count++;
        }

        if (count == 0)
        {
            distFromSea = distFromSea * scale;

            latitude = Mathf.Lerp(0, 90, (pos.y - _height / 2) / (_height / 2 - 1));
            if (pos.y < _height / 2)
            {
                latitude = Mathf.Lerp(90, 0, Mathf.Abs(pos.y / (_height / 2 - 1)));
            }

            tempLat = Mathf.Lerp(0, 90, ((pos.y - _height / 2) - (itczNorth * scale / 111 / 10)) / (_height / 2 - 1));
            if (pos.y <= _height / 2 + (itczNorth * scale / 111 / 5))
            {
                tempLat = Mathf.Lerp(90, 0, Mathf.Abs((pos.y - (itczNorth * scale / 111 / 10)) / (_height / 2 - 1)));
            }

            if (height >= seaLevel)
            {
                //if (neighbor_LEFT.height < seaLevel)
                //{
                    //neighbor_LEFT.seaSize = 1;
                //}
                if (tidalLock)
                {
                    float terminator = pos.x / _width;
                    if (terminator < 0.5)
                    {
                        temp = Mathf.Lerp(-100, 100, terminator * 2) + sunStrength;
                    }
                    else
                    {
                        temp = Mathf.Lerp(100, -100, (terminator - 0.5f) * 2) + sunStrength;
                    }
                }
                else
                {
                    temp = Mathf.Clamp(Mathf.Lerp(1, 0, tempLat / 90) * sunStrength * 5 - 30 - 20f * (height - seaLevel) / 2000, -100, sunStrength * 3);
                    if (latitude > 45)
                    {
                        //rightVel = Mathf.Lerp(1, 0, (latitude - 45) / 15) * 100 - 100;
                    }
                }
            }
            else
            {
                if (tidalLock)
                {
                    float terminator = pos.x / _width;
                    if (terminator < 0.5)
                    {
                        temp = Mathf.Lerp(-100, 100, terminator * 2) + sunStrength;
                    }
                    else
                    {
                        temp = Mathf.Lerp(100, -100, (terminator - 0.5f) * 2) + sunStrength;
                    }
                }
                else
                {
                    temp = Mathf.Clamp(Mathf.Lerp(1, 0, tempLat / 90) * sunStrength * 4 - 30, -100, sunStrength * 1.33333333333f);
                    float seaTemp = Mathf.Lerp(0, 1, latitude / 30);
                    exTemp = Mathf.Clamp(Mathf.Lerp(1, 0, tempLat / 90) * sunStrength, 0, sunStrength * 2);
                    float seaDepth = 1;
                    if (seaSize < 50 * (720 / _width) + seaTemp)
                    {
                        seaDepth = 0;
                    }
                    if (tempLat <= 90)
                    {
                        //moisture = Mathf.Clamp((Mathf.Lerp(1, 0, tempLat / 30) + Mathf.Lerp(0, 1, (tempLat - 30) / 30)) * 2000 * (sunStrength / 10) * Mathf.Clamp(seaDepth, 0, 1), 0, 10000);
                    }
                    else
                    {
                        //moisture = Mathf.Clamp(Mathf.Lerp(1, 0.1f, (tempLat - 60) / 30) * 4000 * (sunStrength / 10) * Mathf.Clamp(seaDepth, 0, 1), 0, 10000);
                    }
                }
            }

            _renderer.color = Color.Lerp(Color.black, Color.white, Mathf.Abs((count + 200) / 300));

            if (height >= seaLevel)
            {
                if (temp < -15)
                {
                    _bioColor = Color.white;
                }
                else
                {
                    //_bioColor = gradient.GetPixel((int)moisture / 10, (int)((temp + 30) / 2 * 10));
                    _bioColor = Color.Lerp(Color.cyan, Color.red, Mathf.Abs(temp / 30));
                }
            }
            else
            {
                float freezePoint = (seaLevel - height) / 2 + (temp + 20) * 100;
                if (freezePoint < 1000)
                {
                    _bioColor = new Color(0.9f, 0.9f, 0.9f);
                }
                else
                {
                    //_bioColor = Color.Lerp(Color.black, Color.blue, Mathf.Abs(height / seaLevel));
                    _bioColor = Color.Lerp(Color.black, Color.white, Mathf.Abs(tempLat / 90));
                }
            }
            if (viewShadow)
            {
                if (neighbor_LEFT.height > height && height > seaLevel)
                {
                    _bioColor = _bioColor / ((neighbor_LEFT.height - height) / 4000 + 1);
                }

                if (neighbor_UP.height > height && height > seaLevel)
                {
                    _bioColor = _bioColor / ((neighbor_LEFT.height - height) / 4000 + 1);
                }
            }

            _renderer.color = _bioColor;

            count++;
        }

        if(count > 0 && count <= 10)
        {
            airMoisture = 1;
            exTemp = 1;

            count++;
        }

        if (count > 10 && count < 210)
        {
            Simulate();

            count++;
        }

        if (count == 210)
        {
            if (height >= seaLevel)
            {
                exTemp = exTemp - 6.5f * (height - seaLevel) / 1000;
            }

            if (exTemp >= 0)
            {
                finalTemp = temp + Mathf.Clamp(exTemp, 0, 30);
            }
            else
            {
                finalTemp = temp;
            }

            if (latitude >= 45 && moisture < 1000)
            {
                //moisture = 1000;
            }
            if (pos.y < _height / 2)
            {
                winterMoisture = moisture;
                winterTemp = finalTemp;
            }
            else
            {
                summerMoisture = moisture;
                summerTemp = finalTemp;
            }

            if (seasonal)
            {
                moisture = 0;
                exTemp = 0;
                finalTemp = 0;

                rightVel = 0;
                leftVel = 0;
                upVel = 0;
                downVel = 0;

                tempLat = Mathf.Lerp(0, 90, ((pos.y - _height / 2) + (itczSouth * scale / 111 / 10)) / (_height / 2 - 1));
                if (pos.y < _height / 2 - (itczSouth * scale / 111 / 5))
                {
                    tempLat = Mathf.Lerp(90, 0, Mathf.Abs((pos.y + (itczSouth * scale / 111 / 10)) / (_height / 2 - 1)));
                }

                if (height >= seaLevel)
                {
                    //if (neighbor_LEFT.height < seaLevel)
                    //{
                        //neighbor_LEFT.seaSize = 1;
                    //}
                    if (tidalLock)
                    {
                        float terminator = pos.x / _width;
                        if (terminator < 0.5)
                        {
                            temp = Mathf.Lerp(-100, 100, terminator * 2) + sunStrength;
                        }
                        else
                        {
                            temp = Mathf.Lerp(100, -100, (terminator - 0.5f) * 2) + sunStrength;
                        }
                    }
                    else
                    {
                        temp = Mathf.Clamp(Mathf.Lerp(1, 0, tempLat / 90) * sunStrength * 5 - 30 - 20f * (height - seaLevel) / 2000, -100, sunStrength * 3);
                        if (latitude > 45)
                        {
                            //rightVel = Mathf.Lerp(1, 0, (latitude - 45) / 15) * 100 - 100;
                        }
                    }
                }
                else
                {
                    if (tidalLock)
                    {
                        float terminator = pos.x / _width;
                        if (terminator < 0.5)
                        {
                            temp = Mathf.Lerp(-100, 100, terminator * 2) + sunStrength;
                        }
                        else
                        {
                            temp = Mathf.Lerp(100, -100, (terminator - 0.5f) * 2) + sunStrength;
                        }
                    }
                    else
                    {
                        temp = Mathf.Clamp(Mathf.Lerp(1, 0, tempLat / 90) * sunStrength * 4 - 30, -100, sunStrength * 1.33333333333f);
                        float seaTemp = Mathf.Lerp(0, 1, latitude / 30);
                        exTemp = Mathf.Clamp(Mathf.Lerp(1, 0, tempLat / 90) * sunStrength, 0, sunStrength * 2);
                        float seaDepth = 1;
                        if (seaSize < 50 * (720 / _width) + seaTemp)
                        {
                            seaDepth = 0;
                        }
                        if (tempLat <= 90)
                        {
                            //moisture = Mathf.Clamp((Mathf.Lerp(1, 0, tempLat / 30) + Mathf.Lerp(0, 1, (tempLat - 30) / 30)) * 2000 * (sunStrength / 10) * Mathf.Clamp(seaDepth, 0, 1), 0, 10000);
                        }
                        else
                        {
                            //moisture = Mathf.Clamp(Mathf.Lerp(1, 0.1f, (tempLat - 60) / 30) * 4000 * (sunStrength / 10) * Mathf.Clamp(seaDepth, 0, 1), 0, 10000);
                        }
                    }
                }

                count++;
            }
            else
            {
                count = 410;
            }
        }

        if (count > 210 && count <= 220)
        {
            airMoisture = 1;
            exTemp = 1;

            count++;
        }

        if (count > 220 && count < 420)
        {
            Simulate();

            count++;
        }

        if (count == 420)
        {
            if (height >= seaLevel)
            {
                exTemp = exTemp - 6.5f * (height - seaLevel) / 1000;
            }

            if (exTemp >= 0)
            {
                finalTemp = temp + Mathf.Clamp(exTemp, 0, 30);
            }
            else
            {
                finalTemp = temp;
            }

            if (latitude >= 45 && moisture < 1000)
            {
                //moisture = 1000;
            }
            if (pos.y < _height / 2)
            {
                summerMoisture = moisture;
                summerTemp = finalTemp;
            }
            else
            {
                winterMoisture = moisture;
                winterTemp = finalTemp;
            }

            //if (winterTemp > summerTemp)
            //{
                //float tempTemp = winterTemp;
                //float tempMoist = winterMoisture;
                //winterTemp = summerTemp;
                //winterMoisture = summerMoisture;
                //summerTemp = tempTemp;
                //summerMoisture = tempMoist;
            //}

            _width = _width * 2;
            _height = _height * 2;
            pos.x = pos.x * 2;
            pos.y = pos.y * 2;

            transform.position = new Vector2(transform.position.x * 2, transform.position.y);
            spawnedTileRight = Instantiate(this, new Vector2(transform.position.x + transform.lossyScale.x, transform.position.y), Quaternion.identity, transform);
            spawnedTileRight.transform.localScale = Vector3.one;
            spawnedTileRight.count = 427;
            spawnedTileRight.pos.x = pos.x + 1;
            spawnedTileRight.rect = new Vector2(Mathf.RoundToInt(pos.x * 2048 / _width), Mathf.RoundToInt(pos.y * 1024 / _height));
            spawnedTileRight.height = heightmap.GetPixel((int)rect.x, (int)rect.y).grayscale * topoScale;
            if (seasonal)
            {
                spawnedTileRight.summerTemp = (summerTemp + neighbor_RIGHT.summerTemp) / 2;
                spawnedTileRight.summerMoisture = (summerMoisture + neighbor_RIGHT.summerMoisture) / 2;
                spawnedTileRight.winterTemp = (winterTemp + neighbor_RIGHT.winterTemp) / 2;
                spawnedTileRight.winterMoisture = (winterMoisture + neighbor_RIGHT.winterMoisture) / 2;
            }
            else
            {
                spawnedTileRight.temp = (temp + neighbor_RIGHT.temp) / 2;
                spawnedTileRight.moisture = (moisture + neighbor_RIGHT.moisture) / 2;
                spawnedTileRight.heightLake = (heightLake + neighbor_RIGHT.heightLake) / 2;
                spawnedTileRight.heightRiver = (heightRiver + neighbor_RIGHT.heightRiver) / 2;
            }

            count++;
        }

        if (count == 421)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y * 2);
            spawnedTileUp = Instantiate(this, new Vector2(transform.position.x, transform.position.y + transform.lossyScale.y), Quaternion.identity, transform);
            spawnedTileUp.transform.localScale = Vector3.one;
            spawnedTileUp.count = 427;
            spawnedTileUp.pos.y = pos.y + 1;
            spawnedTileUp.rect = new Vector2(Mathf.RoundToInt(pos.x * 2048 / _width), Mathf.RoundToInt(pos.y * 1024 / _height));
            spawnedTileUp.height = heightmap.GetPixel((int)rect.x, (int)rect.y).grayscale * topoScale;
            if (seasonal)
            {
            spawnedTileUp.summerTemp = (summerTemp + neighbor_UP.summerTemp) / 2;
            spawnedTileUp.summerMoisture = (summerMoisture + neighbor_UP.summerMoisture) / 2;
            spawnedTileUp.winterTemp = (winterTemp + neighbor_UP.winterTemp) / 2;
            spawnedTileUp.winterMoisture = (winterMoisture + neighbor_UP.winterMoisture) / 2;
            }
            else
            {
            spawnedTileUp.temp = (temp + neighbor_UP.temp) / 2;
            spawnedTileUp.moisture = (moisture + neighbor_UP.moisture) / 2;
            spawnedTileUp.heightLake = (heightLake + neighbor_UP.heightLake) / 2;
            spawnedTileUp.heightRiver = (heightRiver + neighbor_UP.heightRiver) / 2;
            }

            count++;
        }

        if (count == 422)
        {
            spawnedTileUpRight = transform.GetChild(1).GetChild(0).GetComponent<Tile>();
            spawnedTileUpRight.count = 427;
            spawnedTileUpRight.pos.y = pos.y + 1;
            spawnedTileUpRight.pos.x = pos.x + 1;
            spawnedTileUpRight.rect = new Vector2(Mathf.RoundToInt(pos.x * 2048 / _width), Mathf.RoundToInt(pos.y * 1024 / _height));
            spawnedTileUpRight.height = heightmap.GetPixel((int)rect.x, (int)rect.y).grayscale * topoScale;
            if (seasonal)
            {
            spawnedTileUpRight.summerTemp = (summerTemp + neighbor_UP.summerTemp + neighbor_RIGHT.summerTemp) / 3;
            spawnedTileUpRight.summerMoisture = (summerMoisture + neighbor_UP.summerMoisture + neighbor_RIGHT.summerMoisture) / 3;
            spawnedTileUpRight.winterTemp = (winterTemp + neighbor_UP.winterTemp + neighbor_RIGHT.winterTemp) / 3;
            spawnedTileUpRight.winterMoisture = (winterMoisture + neighbor_UP.winterMoisture + neighbor_RIGHT.winterMoisture) / 3;
            }
            else
            {
            spawnedTileUpRight.temp = (temp + neighbor_UP.temp + neighbor_RIGHT.temp) / 3;
            spawnedTileUpRight.moisture = (moisture + neighbor_UP.moisture + neighbor_RIGHT.moisture) / 3;
            spawnedTileUpRight.heightLake = (heightLake + neighbor_UP.heightLake + neighbor_RIGHT.heightLake) / 3;
            spawnedTileUpRight.heightRiver = (heightRiver + neighbor_UP.heightRiver + neighbor_RIGHT.heightRiver) / 3;
            }

            if (finalTemp <= 0 && finalTemp > -1)
            {
            GameObject.Find("Manager").GetComponent<GridManager>().zeroDeg.Add(this);
            }

            if (heightRiver > 0 && heightRiver < 1000 && height > seaLevel + 4000)
            {
            river = true;
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, -height / 10000);

            count++;
        }
    }

    void LateUpdate()
    {
        if (count == 423)
        {
            spawnedTileRight.neighbor_RIGHT = neighbor_RIGHT;
            spawnedTileRight.neighbor_LEFT = this;
            spawnedTileRight.neighbor_UP = spawnedTileUpRight;
            spawnedTileRight.neighbor_DOWN = neighbor_DOWN.spawnedTileUpRight;

            if (seasonal)
            {
                spawnedTileRight.summerTemp = (spawnedTileRight.neighbor_LEFT.summerTemp + spawnedTileRight.neighbor_RIGHT.summerTemp) / 2;
                spawnedTileRight.summerMoisture = (spawnedTileRight.neighbor_LEFT.summerMoisture + spawnedTileRight.neighbor_RIGHT.summerMoisture) / 2;
                spawnedTileRight.winterTemp = (spawnedTileRight.neighbor_LEFT.winterTemp + spawnedTileRight.neighbor_RIGHT.winterTemp) / 2;
                spawnedTileRight.winterMoisture = (spawnedTileRight.neighbor_LEFT.winterMoisture + spawnedTileRight.neighbor_RIGHT.winterMoisture) / 2;
                spawnedTileRight.heightLake = (spawnedTileRight.neighbor_LEFT.heightLake + spawnedTileRight.neighbor_RIGHT.heightLake) / 2;
            }

            count++;
        }

        if (count == 424)
        {
            spawnedTileUp.neighbor_RIGHT = spawnedTileUpRight;
            spawnedTileUp.neighbor_LEFT = neighbor_LEFT.spawnedTileUpRight;
            spawnedTileUp.neighbor_UP = neighbor_UP;
            spawnedTileUp.neighbor_DOWN = this;

            if (seasonal)
            {
                spawnedTileUp.summerTemp = (spawnedTileUp.neighbor_DOWN.summerTemp + spawnedTileUp.neighbor_UP.summerTemp) / 2;
                spawnedTileUp.summerMoisture = (spawnedTileUp.neighbor_DOWN.summerMoisture + spawnedTileUp.neighbor_UP.summerMoisture) / 2;
                spawnedTileUp.winterTemp = (spawnedTileUp.neighbor_DOWN.winterTemp + spawnedTileUp.neighbor_UP.winterTemp) / 2;
                spawnedTileUp.winterMoisture = (spawnedTileUp.neighbor_DOWN.winterMoisture + spawnedTileUp.neighbor_UP.winterMoisture) / 2;
                spawnedTileUp.heightLake = (spawnedTileUp.neighbor_DOWN.heightLake + spawnedTileUp.neighbor_UP.heightLake) / 2;
            }

            count++;
        }

        if (count == 425)
        {
            spawnedTileUpRight.neighbor_RIGHT = neighbor_RIGHT.spawnedTileUp;
            spawnedTileUpRight.neighbor_LEFT = spawnedTileUp;
            spawnedTileUpRight.neighbor_UP = neighbor_UP.spawnedTileRight;
            spawnedTileUpRight.neighbor_DOWN = spawnedTileRight;

            if (seasonal)
            {
                spawnedTileUpRight.summerTemp = (summerTemp + neighbor_UP.summerTemp + neighbor_RIGHT.summerTemp) / 3;
                spawnedTileUpRight.summerMoisture = (summerMoisture + neighbor_UP.summerMoisture + neighbor_RIGHT.summerMoisture) / 3;
                spawnedTileUpRight.winterTemp = (winterTemp + neighbor_UP.winterTemp + neighbor_RIGHT.winterTemp) / 3;
                spawnedTileUpRight.winterMoisture = (winterMoisture + neighbor_UP.winterMoisture + neighbor_RIGHT.winterMoisture) / 3;
                spawnedTileUpRight.heightLake = (heightLake + neighbor_UP.heightLake + neighbor_RIGHT.heightLake) / 3;
            }

            count++;
        }

        if (count == 426)
        {
            neighbor_RIGHT = spawnedTileRight;
            neighbor_LEFT = neighbor_LEFT.spawnedTileRight;
            neighbor_UP = spawnedTileUp;
            neighbor_DOWN = neighbor_DOWN.spawnedTileUp;

            count++;
        }

        if (count == 427)
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                viewTemp = true;
                viewMoist = false;
                viewHeight = false;
                viewVelo = false;
                viewComfort = false;
                viewNight = false;
                viewRange = false;

            }
            if (Input.GetKeyUp(KeyCode.M))
            {
                viewTemp = false;
                viewMoist = true;
                viewHeight = false;
                viewVelo = false;
                viewComfort = false;
                viewNight = false;
                viewRange = false;
            }
            if (Input.GetKeyUp(KeyCode.H))
            {
                viewTemp = false;
                viewMoist = false;
                viewHeight = true;
                viewVelo = false;
                viewComfort = false;
                viewNight = false;
                viewRange = false;
            }
            if (Input.GetKeyUp(KeyCode.V))
            {
                viewTemp = false;
                viewMoist = false;
                viewHeight = false;
                viewVelo = true;
                viewComfort = false;
                viewNight = false;
                viewRange = false;
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                viewTemp = false;
                viewMoist = false;
                viewHeight = false;
                viewVelo = false;
                viewComfort = true;
                viewNight = false;
                viewRange = false;
            }
            if (Input.GetKeyUp(KeyCode.L))
            {
                viewTemp = false;
                viewMoist = false;
                viewHeight = false;
                viewVelo = false;
                viewComfort = false;
                viewNight = true;
                viewRange = false;
            }
            if (Input.GetKeyUp(KeyCode.R))
            {
                viewTemp = false;
                viewMoist = false;
                viewHeight = false;
                viewVelo = false;
                viewComfort = false;
                viewNight = false;
                viewRange = true;

                range = float.PositiveInfinity;
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                viewTemp = false;
                viewMoist = false;
                viewHeight = false;
                viewVelo = false;
                viewComfort = false;
                viewNight = false;
                viewRange = false;
            }

            if (viewTemp)
            {
                var tempColor = _coldColor;

                if (seasonal)
                {
                    finalTemp = (summerTemp + winterTemp) / 2;
                }

                if (finalTemp < -24) { tempColor = new Color(1, 1, 1); }
                if (finalTemp >= -24) { tempColor = new Color(1, 0.75f, 1); }
                if (finalTemp >= -22) { tempColor = new Color(1, 0.5f, 1); }
                if (finalTemp >= -20) { tempColor = new Color(1, 0.25f, 1); }
                if (finalTemp >= -18) { tempColor = new Color(1, 0, 1); }
                if (finalTemp >= -16) { tempColor = new Color(0.75f, 0, 1); }
                if (finalTemp >= -14) { tempColor = new Color(0.5f, 0, 1); }
                if (finalTemp >= -12) { tempColor = new Color(0.25f, 0, 1); }
                if (finalTemp >= -10) { tempColor = new Color(0, 0, 1); }
                if (finalTemp >= -8) { tempColor = new Color(0, 0.25f, 1); }
                if (finalTemp >= -6) { tempColor = new Color(0, 0.5f, 1); }
                if (finalTemp >= -4) { tempColor = new Color(0, 0.75f, 1); }
                if (finalTemp >= -2) { tempColor = new Color(0, 1, 1); }
                if (finalTemp >= 0) { tempColor = new Color(0, 1, 0.75f); }
                if (finalTemp >= 2) { tempColor = new Color(0, 1, 0.5f); }
                if (finalTemp >= 4) { tempColor = new Color(0, 1, 0.25f); }
                if (finalTemp >= 6) { tempColor = new Color(0, 1, 0); }
                if (finalTemp >= 8) { tempColor = new Color(0.25f, 1, 0); }
                if (finalTemp >= 10) { tempColor = new Color(0.5f, 1, 0); }
                if (finalTemp >= 12) { tempColor = new Color(0.75f, 1, 0); }
                if (finalTemp >= 14) { tempColor = new Color(1, 1, 0); }
                if (finalTemp >= 16) { tempColor = new Color(1, 0.75f, 0); }
                if (finalTemp >= 18) { tempColor = new Color(1, 0.5f, 0); }
                if (finalTemp >= 20) { tempColor = new Color(1, 0.25f, 0); }
                if (finalTemp >= 22) { tempColor = new Color(1, 0, 0); }
                if (finalTemp >= 24) { tempColor = new Color(0.75f, 0, 0); }
                if (finalTemp >= 26) { tempColor = new Color(0.5f, 0, 0); }
                if (finalTemp >= 28) { tempColor = new Color(0.25f, 0, 0); }
                if (finalTemp >= 30) { tempColor = new Color(0, 0, 0); }

                if (height < seaLevel)
                {
                    tempColor = tempColor * 0.75f;
                }

                _renderer.color = tempColor;
            }
            else if (viewMoist)
            {
                var moistureColor = new Color(0.5f, 0.5f, 0.5f);

                if (seasonal)
                {
                    moisture = (summerMoisture + winterMoisture) / 2;
                }

                if (moisture > 200) { moistureColor = new Color(0.25f, 0.75f, 0.75f); }
                if (moisture > 400) { moistureColor = new Color(0, 1, 1); }
                if (moisture > 600) { moistureColor = new Color(0, 0.75f, 1); }
                if (moisture > 800) { moistureColor = new Color(0, 0.5f, 1); }
                if (moisture > 1000) { moistureColor = new Color(0, 0.25f, 1); }
                if (moisture > 1500) { moistureColor = new Color(0, 0, 1); }
                if (moisture > 2000) { moistureColor = new Color(0.25f, 0, 1); }
                if (moisture > 3000) { moistureColor = new Color(0.5f, 0, 1); }
                if (moisture > 4000) { moistureColor = new Color(0.75f, 0, 1); }
                if (moisture > 5000) { moistureColor = new Color(1, 0, 1); }
                if (moisture > 6000) { moistureColor = new Color(1, 0, 0.75f); }
                if (moisture > 7000) { moistureColor = new Color(1, 0, 0.5f); }
                if (moisture > 8000) { moistureColor = new Color(1, 0, 0.25f); }
                if (moisture > 9000) { moistureColor = new Color(1, 0, 0); }
                if (moisture > 9500) { moistureColor = new Color(1, 0.25f, 0.25f); }
                if (moisture > 9750) { moistureColor = new Color(1, 0.5f, 0.5f); }
                if (moisture > 9900) { moistureColor = new Color(1, 0.75f, 0.75f); }
                if (moisture > 9999) { moistureColor = new Color(1, 1, 1); }
                if (height < seaLevel)
                {
                    moistureColor = moistureColor * 0.75f;
                }

                _renderer.color = moistureColor;
            }
            else if (viewHeight)
            {
                var heightColor = Color.Lerp(Color.white, Color.black, seaLevel / height * 2 - 1);

                if (Input.GetKeyUp(KeyCode.S))
                {
                    viewShadow = !viewShadow;
                }
                if (viewShadow)
                {
                    if (height >= seaLevel)
                    {
                        float slope = Mathf.Abs((Mathf.Abs(neighbor_LEFT.height - height) + Mathf.Abs(neighbor_RIGHT.height - height) + Mathf.Abs(neighbor_UP.height - height) + Mathf.Abs(neighbor_DOWN.height - height)) / 4);
                        if (slope >= 0) { heightColor = new Color(0, 0, 1); }
                        if (slope > 100) { heightColor = new Color(0, 0.25f, 1); }
                        if (slope > 200) { heightColor = new Color(0, 0.5f, 1); }
                        if (slope > 300) { heightColor = new Color(0, 0.75f, 1); }
                        if (slope > 400) { heightColor = new Color(0, 1, 1); }
                        if (slope > 500) { heightColor = new Color(0, 1, 0.75f); }
                        if (slope > 600) { heightColor = new Color(0, 1, 0.5f); }
                        if (slope > 700) { heightColor = new Color(0, 1, 0.25f); }
                        if (slope > 800) { heightColor = new Color(0, 1, 0); }
                        if (slope > 900) { heightColor = new Color(0.25f, 1, 0); }
                        if (slope > 1000) { heightColor = new Color(0.5f, 1, 0); }
                        if (slope > 1100) { heightColor = new Color(0.75f, 1, 0); }
                        if (slope > 1200) { heightColor = new Color(1, 1, 0); }
                        if (slope > 1300) { heightColor = new Color(1, 0.75f, 0); }
                        if (slope > 1400) { heightColor = new Color(1, 0.5f, 0); }
                        if (slope > 1500) { heightColor = new Color(1, 0.25f, 0); }
                        if (slope > 1600) { heightColor = new Color(1, 0, 0); }
                    }
                    else
                    {
                        heightColor = Color.black;
                    }
                }
                else
                {
                    if (height - seaLevel > 0) { heightColor = new Color(0, 0.5f, 1); }
                    if (height - seaLevel > 100) { heightColor = new Color(0, 0.75f, 1); }
                    if (height - seaLevel > 200) { heightColor = new Color(0, 1, 1); }
                    if (height - seaLevel > 300) { heightColor = new Color(0, 1, 0.75f); }
                    if (height - seaLevel > 400) { heightColor = new Color(0, 1, 0.5f); }
                    if (height - seaLevel > 500) { heightColor = new Color(0, 1, 0.25f); }
                    if (height - seaLevel > 600) { heightColor = new Color(0, 1, 0); }
                    if (height - seaLevel > 700) { heightColor = new Color(0.25f, 1, 0); }
                    if (height - seaLevel > 800) { heightColor = new Color(0.5f, 1, 0); }
                    if (height - seaLevel > 900) { heightColor = new Color(0.75f, 1, 0); }
                    if (height - seaLevel > 1000) { heightColor = new Color(1, 1, 0); }
                    if (height - seaLevel > 1500) { heightColor = new Color(1, 0.75f, 0); }
                    if (height - seaLevel > 2000) { heightColor = new Color(1, 0.5f, 0); }
                    if (height - seaLevel > 3000) { heightColor = new Color(1, 0.25f, 0); }
                    if (height - seaLevel > 4000) { heightColor = new Color(1, 0, 0); }
                    if (height - seaLevel > 5000) { heightColor = new Color(1, 0.25f, 0.25f); }
                    if (height - seaLevel > 6000) { heightColor = new Color(1, 0.5f, 0.5f); }
                    if (height - seaLevel > 7000) { heightColor = new Color(1, 0.75f, 0.75f); }
                    if (height - seaLevel > 8000) { heightColor = new Color(1, 1, 1); }
                    if (height < seaLevel)
                    {
                        heightColor = Color.Lerp(Color.black, Color.blue, Mathf.Abs(height / seaLevel));
                    }
                }

                _renderer.color = heightColor;
            }
            else if (viewVelo)
            {
                float pol = Mathf.Atan((upVel - downVel) / (rightVel - leftVel)) * Mathf.Rad2Deg;
                if (rightVel - leftVel >= 0 && upVel - downVel < 0)
                {
                    pol = 180 + pol;
                }
                if (rightVel - leftVel >= 0 && upVel - downVel >= 0)
                {
                    pol = 180 + pol;
                }
                angle = pol;

                float rad = Mathf.Sqrt(Mathf.Pow(rightVel - leftVel, 2) + Mathf.Pow(upVel - downVel, 2));

                var velColor = Color.HSVToRGB(pol / 360, 1, 1);//new Color(0.5f + leftVel / 200 - rightVel / 200, 0.5f + upVel / 200 - downVel / 200, 0);

                _renderer.color = velColor;

                transform.rotation = Quaternion.AngleAxis(pol, Vector3.forward);

                if (pos.x % 2 == 0 && pos.y % 2 == 0)
                {
                    transform.localScale = new Vector2(0.045f * rad / 10, 0.025f * rad / 100);
                }
                else
                {
                    transform.localScale = new Vector2(0, 0);
                }
            }
            else if (viewComfort)
            {
                if (seasonal)
                {
                    moisture = (summerMoisture + winterMoisture) / 2;
                    finalTemp = (summerTemp - 10) * (winterTemp - 10);
                }

                float moistVal = 0;
                if (summerMoisture > winterMoisture)
                {
                    moistVal = (10000 - winterMoisture - summerMoisture / 2) / 10;
                }
                if (summerMoisture < winterMoisture)
                {
                    moistVal = (10000 - summerMoisture - winterMoisture / 2) / 10;
                }
                if (moisture < 100)
                {
                    moistVal = 10000;
                }
                float tempVal = 0;
                if (winterTemp - 10 > 0)
                {
                    tempVal = ((summerTemp - 10) * (winterTemp - 10)) * 5;
                }
                if (summerTemp - 10 < 0)
                {
                    tempVal = Mathf.Abs((summerTemp - 10) * (winterTemp - 10)) * 5;
                }
                float heightVal = (Mathf.Abs(neighbor_LEFT.height - height) + Mathf.Abs(neighbor_RIGHT.height - height)
                                + Mathf.Abs(neighbor_UP.height - height) + Mathf.Abs(neighbor_DOWN.height - height));
                float seaVal = 0;
                if (neighbor_LEFT.height < seaLevel || neighbor_RIGHT.height < seaLevel || neighbor_UP.height < seaLevel || neighbor_DOWN.height < seaLevel)
                {
                    seaVal = 1000;
                }

                var civColour = Color.Lerp(Color.green, Color.red, (moistVal + tempVal + heightVal - seaVal) / 4000);

                if (height < seaLevel)
                {
                    civColour = Color.Lerp(Color.black, Color.blue, Mathf.Abs(height / seaLevel));
                }

                _renderer.color = civColour;
            }
            else if (viewNight)
            {
                if (seasonal)
                {
                    moisture = (summerMoisture + winterMoisture) / 2;
                    finalTemp = (summerTemp - 10) * (winterTemp - 10);
                }

                float moistVal = 0;
                if (summerMoisture > winterMoisture)
                {
                    moistVal = (10000 - winterMoisture - summerMoisture / 2) / 10;
                }
                if (summerMoisture < winterMoisture)
                {
                    moistVal = (10000 - summerMoisture - winterMoisture / 2) / 10;
                }
                if (moisture < 100)
                {
                    moistVal = 10000;
                }
                float tempVal = 0;
                if (winterTemp - 10 > 0)
                {
                    tempVal = ((summerTemp - 10) * (winterTemp - 10)) * 5;
                }
                if (summerTemp - 10 < 0)
                {
                    tempVal = Mathf.Abs((summerTemp - 10) * (winterTemp - 10)) * 5;
                }
                float heightVal = (Mathf.Abs(neighbor_LEFT.height - height) + Mathf.Abs(neighbor_RIGHT.height - height)
                                + Mathf.Abs(neighbor_UP.height - height) + Mathf.Abs(neighbor_DOWN.height - height));
                float seaVal = 0;
                if (neighbor_LEFT.height < seaLevel || neighbor_RIGHT.height < seaLevel || neighbor_UP.height < seaLevel || neighbor_DOWN.height < seaLevel)
                {
                    seaVal = 1000;
                }

                var civColour = Color.Lerp(Color.white, Color.black, (tempVal + moistVal + heightVal - seaVal) / 4000 * rand);

                if (height < seaLevel)
                {
                    civColour = new Color(0, 0, 0.1f);
                }

                _renderer.color = civColour;
            }
            else if (viewRange && !Input.GetKey(KeyCode.R))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 worldPoint;

                    worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    if (worldPoint.x < transform.position.x + 0.04444445f && worldPoint.x > transform.position.x - 0.04444445f
                        && worldPoint.y < transform.position.y + 0.04444445f && worldPoint.y > transform.position.y - 0.04444445f)
                    {
                        GameObject.Find("Manager").GetComponent<GridManager>().startTile = this;

                        range = 0;
                    }
                }

                if (height < seaLevel)
                {
                    if (neighbor_LEFT.range < range)
                    {
                        range = neighbor_LEFT.range + Mathf.Lerp(1.5f - Mathf.Lerp(0, 0.75f, rightVel), 0, latitude / 90);
                    }
                    if (neighbor_RIGHT.range < range)
                    {
                        range = neighbor_RIGHT.range + Mathf.Lerp(1.5f - Mathf.Lerp(0, 0.75f, leftVel), 0, latitude / 90);
                    }
                    if (neighbor_UP.range < range)
                    {
                        range = neighbor_UP.range + 1.5f - Mathf.Lerp(0, 0.75f, downVel);
                    }
                    if (neighbor_DOWN.range < range)
                    {
                        range = neighbor_DOWN.range + 1.5f - Mathf.Lerp(0, 0.75f, upVel);
                    }
                }
                else
                {
                    if (neighbor_LEFT.range < range)
                    {
                        if (neighbor_LEFT.height < height)
                        {
                            range = neighbor_LEFT.range + Mathf.Lerp(3 + (height - neighbor_LEFT.height) / 1000, 0, latitude / 90);
                        }
                        else
                        {
                            range = neighbor_LEFT.range + Mathf.Lerp(3 + (neighbor_LEFT.height - height) / 1000, 0, latitude / 90);
                        }
                    }
                    if (neighbor_RIGHT.range < range)
                    {
                        if (neighbor_RIGHT.height < height)
                        {
                            range = neighbor_RIGHT.range + Mathf.Lerp(3 + (height - neighbor_RIGHT.height) / 1000, 0, latitude / 90);
                        }
                        else
                        {
                            range = neighbor_RIGHT.range + Mathf.Lerp(3 + (neighbor_RIGHT.height - height) / 1000, 0, latitude / 90);
                        }
                    }
                    if (neighbor_UP.range < range)
                    {
                        if (neighbor_UP.height < height)
                        {
                            range = neighbor_UP.range + 3 + (height - neighbor_UP.height) / 1000;
                        }
                        else
                        {
                            range = neighbor_UP.range + 3 + (neighbor_UP.height - height) / 1000;
                        }
                    }
                    if (neighbor_DOWN.range < range)
                    {
                        if (neighbor_DOWN.height < height)
                        {
                            range = neighbor_DOWN.range + 3 + (height - neighbor_DOWN.height) / 1000;
                        }
                        else
                        {
                            range = neighbor_DOWN.range + 3 + (neighbor_DOWN.height - height) / 1000;
                        }
                    }
                }


                if (range < 7) { _renderer.color = new Color(0, 0, 1); }
                if (range >= 7) { _renderer.color = new Color(0, 0.25f, 1); }
                if (range >= 7 * 2) { _renderer.color = new Color(0, 0.5f, 1); }
                if (range >= 7 * 3) { _renderer.color = new Color(0, 0.75f, 1); }
                if (range >= 7 * 4) { _renderer.color = new Color(0, 1, 1); }
                if (range >= 7 * 5) { _renderer.color = new Color(0, 1, 0.75f); }
                if (range >= 7 * 6) { _renderer.color = new Color(0, 1, 0.5f); }
                if (range >= 7 * 7) { _renderer.color = new Color(0, 1, 0.25f); }
                if (range >= 7 * 8) { _renderer.color = new Color(0, 1, 0); }
                if (range >= 7 * 9) { _renderer.color = new Color(0.25f, 1, 0); }
                if (range >= 7 * 10) { _renderer.color = new Color(0.5f, 1, 0); }
                if (range >= 7 * 11) { _renderer.color = new Color(0.75f, 1, 0); }
                if (range >= 7 * 12) { _renderer.color = new Color(1, 1, 0); }
                if (range >= 7 * 13) { _renderer.color = new Color(1, 0.75f, 0); }
                if (range >= 7 * 14) { _renderer.color = new Color(1, 0.5f, 0); }
                if (range >= 7 * 15) { _renderer.color = new Color(1, 0.25f, 0); }
                if (range >= 7 * 16) { _renderer.color = new Color(1, 0, 0); }

                if (height < seaLevel)
                {
                    _renderer.color = _renderer.color * 0.75f;
                }
            }
            else
            {
                if (seasonal)
                {
                    if (height >= seaLevel)
                    {
                        _bioColor = Color.black;
                        //arid
                        float moistureThreshhold = (summerMoisture + 1) / (winterMoisture + 1);
                        float precipThreshold = 0;
                        if (moistureThreshhold >= 0.7)
                        {
                            precipThreshold = (summerMoisture + winterMoisture) / 2 + 200 + 1000;
                        }
                        if (moistureThreshhold >= 0.3 && moistureThreshhold < 0.7)
                        {
                            precipThreshold = (summerMoisture + winterMoisture) / 2 + 200 + 500;
                        }
                        if (moistureThreshhold < 0.3)
                        {
                            precipThreshold = (summerMoisture + winterMoisture) / 2 + 200;
                        }
                        //tropical
                        if (winterTemp >= 22)
                        {
                            //tropical rainforest
                            if (summerMoisture > 9000 && winterMoisture > 9000)
                            {
                                _bioColor = Color.blue;
                            }
                            //tropical monsooon
                            else if (winterMoisture < 9000 && winterMoisture > 5000 - ((summerMoisture + winterMoisture) / 25))
                            {
                                _bioColor = new Color(0.3f, 0.3f, 1);
                            }
                            //tropical savanna
                            else
                            {
                                _bioColor = new Color(0.4f, 0.6f, 1);
                            }
                        }
                        //arid
                        else if (summerMoisture + winterMoisture < precipThreshold / 2)
                        {
                            if ((summerTemp + winterTemp) / 2 >= 10)
                            {
                                _bioColor = Color.red;
                            }
                            if ((summerTemp + winterTemp) / 2 < 10)
                            {
                                _bioColor = new Color(1, 0.5f, 0.5f);
                            }
                        }
                        //semi arid
                        else if (summerMoisture + winterMoisture >= precipThreshold / 2 && summerMoisture + winterMoisture < precipThreshold)
                        {
                            if ((summerTemp + winterTemp) / 2 >= 10)
                            {
                                _bioColor = new Color(1, 0.5f, 0);
                            }
                            if ((summerTemp + winterTemp) / 2 < 10)
                            {
                                _bioColor = new Color(1, 1, 0);
                            }
                        }
                        //temperate
                        else if (winterTemp >= 0 && winterTemp < 22)
                        {
                            _bioColor = new Color(0, 1, 0);
                            //mediterranean
                            if (summerMoisture < 1000 && winterMoisture < 5000)
                            {
                                if (summerTemp >= 18)
                                {
                                    _bioColor = new Color(0.8f, 0.8f, 0);
                                }
                                if (summerTemp < 18)
                                {
                                    _bioColor = new Color(0.7f, 0.7f, 0);
                                }
                            }
                            //humid subtropical
                            else if (summerTemp >= 18)
                            {
                                _bioColor = new Color(0.8f, 0.9f, 0);
                            }
                        }
                        //continental
                        else if (summerTemp >= -10 && winterTemp < 0)
                        {
                            if (summerTemp >= 10)
                            {
                                _bioColor = new Color(0, 1, 1);
                            }
                            if (summerTemp < 10)
                            {
                                _bioColor = new Color(0, 0.8f, 0.8f);
                            }
                            if (summerTemp < 0)
                            {
                                _bioColor = new Color(0, 0.6f, 0.6f);
                            }
                            if (winterTemp < -10)
                            {
                                _bioColor = new Color(1, 0, 1);
                            }
                        }
                        //polar
                        if (summerTemp < -10)
                        {
                            _bioColor = new Color(0.6f, 0.6f, 0.6f);
                        }
                        if (summerTemp < -15)
                        {
                            _bioColor = new Color(0.8f, 0.8f, 0.8f);
                        }
                    }
                    else
                    {
                        _bioColor = Color.white;
                    }

                    if (Input.GetKeyUp(KeyCode.S))
                    {
                        viewShadow = !viewShadow;
                    }
                    if (viewShadow && height >= seaLevel)
                    {
                        _bioColor = Color.Lerp(Color.yellow, Color.green, (summerMoisture + winterMoisture) / 2 / 10000);
                        _bioColor = Color.Lerp(Color.white, _bioColor, ((summerTemp + winterTemp) / 2 + 30) / 60);
                        float slope = Mathf.Abs((Mathf.Abs(neighbor_LEFT.height - height) + Mathf.Abs(neighbor_RIGHT.height - height) + Mathf.Abs(neighbor_UP.height - height) + Mathf.Abs(neighbor_DOWN.height - height)) / 4);
                        _bioColor = Color.Lerp(_bioColor, Color.grey, (slope) / 2000);

                        if (neighbor_LEFT.height > height && height > seaLevel)
                        {
                            _bioColor = _bioColor / ((neighbor_LEFT.height - height) / 4000 + 1);
                        }

                        if (neighbor_UP.height > height && height > seaLevel)
                        {
                            _bioColor = _bioColor / ((neighbor_LEFT.height - height) / 4000 + 1);
                        }
                    }
                }
                else
                {
                    if (height >= seaLevel)
                    {
                        if (temp < -15)
                        {
                            _bioColor = Color.white;
                        }
                        else
                        {
                            _bioColor = gradient.GetPixel((int)moisture / 10, (int)((finalTemp + 30) / 2 * 10));
                        }
                    }
                    else
                    {
                        float freezePoint = (seaLevel - height) / 2 + (temp + 20) * 100;
                        if (freezePoint < 1000)
                        {
                            _bioColor = new Color(0.9f, 0.9f, 0.9f);
                        }
                        else
                        {
                            _bioColor = Color.Lerp(Color.black, Color.blue, Mathf.Abs(height / seaLevel));
                        }
                    }

                    if (Input.GetKeyUp(KeyCode.S))
                    {
                        viewShadow = !viewShadow;
                    }
                    if (viewShadow)
                    {
                        if (neighbor_LEFT.height > height && height > seaLevel)
                        {
                            _bioColor = _bioColor / ((neighbor_LEFT.height - height) / 4000 + 1);
                        }

                        if (neighbor_UP.height > height && height > seaLevel)
                        {
                            _bioColor = _bioColor / ((neighbor_LEFT.height - height) / 4000 + 1);
                        }
                    }
                }

                _renderer.color = _bioColor;
            }
        }
    }

    void Simulate()
    {
        float vel = 0 * (Mathf.Lerp(0, 1, latitude / 30) + Mathf.Lerp(1, 0, (latitude - 30) / 30));
        float rain = 0.003f;
        float evap = 0.001f * Mathf.Lerp(0, 1, (temp + exTemp) / 50) * Mathf.Lerp(0, 1, latitude / 30);
        float horiVel = Mathf.Lerp(1, 1, latitude / 30) + Mathf.Lerp(1, 1, (latitude - 30) / 30);
        float mountainEffect = 50;

        if (height < seaLevel)
        {
            moisture = 10000;

            float seaTemp = Mathf.Lerp(0, 1, latitude / 30);
            float seaDepth = 1;
            if (seaSize < 5 * (720 / _width))
            {
                seaDepth = 0;// + seaTemp;
            }
            //if (neighbor_LEFT.height < seaLevel)
            //{
                //neighbor_LEFT.seaSize = 1;
            //}

            if (tidalLock)
            {
                float terminator = pos.x / _width;
                if (terminator < 0.5)
                {
                    terminator = Mathf.Lerp(0, 1, terminator * 2);
                }
                else
                {
                    terminator = Mathf.Lerp(1, 0, (terminator - 0.5f) * 2);
                }
                if (airMoisture < Mathf.Clamp(Mathf.Lerp(0, 1, terminator) * 2000 * (sunStrength / 10) * seaDepth * 3, 0, 10000))
                {
                    airMoisture = Mathf.Clamp(Mathf.Lerp(0, 1, terminator) * 2000 * (sunStrength / 10) * seaDepth * 3, 0, 10000);// - Mathf.Lerp(1000, 0, (seaLevel - height) / 1000);
                }
            }
            else
            {
                if (tempLat <= 90 && airMoisture < Mathf.Clamp(Mathf.Lerp(1, 0, tempLat / 90) * 2000 * (sunStrength / 10) * seaDepth * 3, 0, 10000))
                {
                    airMoisture = Mathf.Clamp(Mathf.Lerp(1, 0, tempLat / 90) * 2000 * (sunStrength / 10) * seaDepth * 3, 0, 10000);// - Mathf.Lerp(1000, 0, (seaLevel - height) / 1000);
                }
                if (latitude <= 2)
                {
                    //moisture = 10000;
                }
            }

            if (leftVel < Mathf.Lerp(1, 0, latitude / 45) * 100)
            {
                leftVel = Mathf.Lerp(1, 0, latitude / 45) * 100;
            }
            else if (rightVel < (Mathf.Lerp(0, 1, (latitude - 30) / 15)) * 100)
            {
                rightVel = (Mathf.Lerp(0, 1, (latitude - 30) / 15)) * 100;// + Mathf.Lerp(1, 0, (latitude - 45) / 15)) * 100 - 100;
            }
            if (neighbor_LEFT.height >= seaLevel)
            {
                if (pos.y > _height / 2)
                {
                    upVel = 1 * 100;
                }
                else
                {
                    downVel = 1 * 100;
                }
            }
            if (neighbor_RIGHT.height >= seaLevel)
            {
                if (pos.y > _height / 2)
                {
                    downVel = 1 * 100;
                }
                else
                {
                    upVel = 1 * 100;
                }
            }

            if (neighbor_LEFT.upVel > upVel)
            {
                upVel = neighbor_LEFT.upVel * 0.92f;
            }
            if (neighbor_LEFT.downVel > downVel)
            {
                downVel = neighbor_LEFT.downVel * 0.92f;
            }
            if (neighbor_RIGHT.upVel > upVel)
            {
                upVel = neighbor_RIGHT.upVel * 0.92f;
            }
            if (neighbor_RIGHT.downVel > downVel)
            {
                downVel = neighbor_RIGHT.downVel * 0.92f;
            }
            if (neighbor_UP.leftVel > leftVel)
            {
                leftVel = neighbor_UP.leftVel * 0.92f;
            }
            if (neighbor_UP.rightVel > rightVel)
            {
                rightVel = neighbor_UP.rightVel * 0.92f;
            }
            if (neighbor_UP.upVel > upVel)
            {
                upVel = neighbor_UP.upVel * 0.92f;
            }
            if (neighbor_UP.downVel > downVel)
            {
                downVel = neighbor_UP.downVel * 0.92f;
            }
            if (neighbor_DOWN.leftVel > leftVel)
            {
                leftVel = neighbor_DOWN.leftVel * 0.92f;
            }
            if (neighbor_DOWN.rightVel > rightVel)
            {
                rightVel = neighbor_DOWN.rightVel * 0.92f;
            }
            if (neighbor_DOWN.upVel > upVel)
            {
                upVel = neighbor_DOWN.upVel * 0.92f;
            }
            if (neighbor_DOWN.downVel > downVel)
            {
                downVel = neighbor_DOWN.downVel * 0.92f;
            }

            leftVel = Mathf.Clamp(leftVel, 0, 100);
            rightVel = Mathf.Clamp(rightVel, 0, 100);
            upVel = Mathf.Clamp(upVel, 0, 100);
            downVel = Mathf.Clamp(downVel, 0, 100);

            neighbor_LEFT.airMoisture += (airMoisture - neighbor_LEFT.airMoisture) * leftVel / 100;
            neighbor_RIGHT.airMoisture += (airMoisture - neighbor_RIGHT.airMoisture) * rightVel / 100;
            neighbor_UP.airMoisture += (airMoisture - neighbor_UP.airMoisture) * upVel / 100;
            neighbor_DOWN.airMoisture += (airMoisture - neighbor_DOWN.airMoisture) * downVel / 100;

            if (neighbor_LEFT.exTemp < exTemp)
            {
                neighbor_LEFT.exTemp += (exTemp - neighbor_LEFT.exTemp) * leftVel / 100;
            }
            if (neighbor_RIGHT.exTemp < exTemp)
            {
                neighbor_RIGHT.exTemp += (exTemp - neighbor_RIGHT.exTemp) * rightVel / 100;
            }
            if (neighbor_UP.exTemp < exTemp)
            {
                neighbor_UP.exTemp += (exTemp - neighbor_UP.exTemp) * upVel / 100;
            }
            if (neighbor_DOWN.exTemp < exTemp)
            {
                neighbor_DOWN.exTemp += (exTemp - neighbor_DOWN.exTemp) * downVel / 100;
            }

            //airMoisture -= 1;

            if (exTemp > temp)
            {
                exTemp -= (exTemp - temp) / 100;
            }
            if (exTemp < temp)
            {
                exTemp += (temp - exTemp) / 100;
            }

            float rightM = 0;
            if (neighbor_RIGHT.height >= seaLevel)
            {
                float leftVelVal = neighbor_RIGHT.leftVel / (neighbor_RIGHT.leftVel + neighbor_RIGHT.rightVel + neighbor_RIGHT.upVel + neighbor_RIGHT.downVel);
                if (float.IsNaN(leftVelVal)) { leftVelVal = 1; }
                rightM = Mathf.Clamp(leftVelVal * neighbor_RIGHT.airMoisture * horiVel, 1, 10000);
            }

            float leftM = 0;
            if (neighbor_LEFT.height >= seaLevel)
            {
                float rightVelVal = neighbor_LEFT.rightVel / (neighbor_LEFT.leftVel + neighbor_LEFT.rightVel + neighbor_LEFT.upVel + neighbor_LEFT.downVel);
                if (float.IsNaN(rightVelVal)) { rightVelVal = 1; }
                leftM = Mathf.Clamp(rightVelVal * neighbor_LEFT.airMoisture * horiVel, 1, 10000);
            }

            float upM = 0;
            if (neighbor_UP.height >= seaLevel)
            {
                float downVelVal = neighbor_UP.downVel / (neighbor_UP.leftVel + neighbor_UP.rightVel + neighbor_UP.upVel + neighbor_UP.downVel);
                if (float.IsNaN(downVelVal)) { downVelVal = 1; }
                upM = Mathf.Clamp(downVelVal * neighbor_UP.airMoisture, 1, 10000);
            }

            float downM = 0;
            if (neighbor_DOWN.height >= seaLevel)
            {
                float upVelVal = neighbor_DOWN.upVel / (neighbor_DOWN.leftVel + neighbor_DOWN.rightVel + neighbor_DOWN.upVel + neighbor_DOWN.downVel);
                if (float.IsNaN(upVelVal)) { upVelVal = 1; }
                downM = Mathf.Clamp(upVelVal * neighbor_DOWN.airMoisture, 1, 10000);
            }

            airMoisture += Mathf.Clamp(rightM + leftM + upM + downM, 1, 10000);

            airMoisture = Mathf.Clamp(airMoisture, 1, 10000);
            exTemp = Mathf.Clamp(exTemp, 1, 30);
        }
        if (height >= seaLevel)
        {
            if (neighbor_RIGHT.rightVel > rightVel)
            {
                rightVel = neighbor_RIGHT.rightVel * 0.9f;
            }
            if (neighbor_RIGHT.leftVel > leftVel)
            {
                leftVel = neighbor_RIGHT.leftVel * 0.99f;
            }
            if (neighbor_RIGHT.upVel > upVel)
            {
                upVel = neighbor_RIGHT.upVel * 0.75f;
            }
            if (neighbor_RIGHT.downVel > downVel)
            {
                downVel = neighbor_RIGHT.downVel * 0.75f;
            }
            if (neighbor_LEFT.rightVel > rightVel)
            {
                rightVel = neighbor_LEFT.rightVel * 0.99f;
            }
            if (neighbor_LEFT.leftVel > leftVel)
            {
                leftVel = neighbor_LEFT.leftVel * 0.9f;
            }
            if (neighbor_LEFT.downVel > downVel)
            {
                downVel = neighbor_LEFT.downVel * 0.75f;
            }
            if (neighbor_LEFT.upVel > upVel)
            {
                upVel = neighbor_LEFT.upVel * 0.75f;
            }
            if (neighbor_DOWN.upVel > upVel)
            {
                upVel = neighbor_DOWN.upVel * 0.9f;
            }
            if (neighbor_UP.downVel > downVel)
            {
                downVel = neighbor_UP.downVel * 0.9f;
            }

            if (neighbor_RIGHT.height < seaLevel)
            {
                seaConnected = true;
            }
            float leftVelVal = 0;
            float rightM = 0;
            float rightR = 0;
            float rightT = 0;
            if (neighbor_RIGHT.height < height)
            {
                leftVelVal = neighbor_RIGHT.leftVel / Mathf.Clamp((neighbor_RIGHT.leftVel + neighbor_RIGHT.rightVel + neighbor_RIGHT.upVel + neighbor_RIGHT.downVel), 1, 100);
                if (float.IsNaN(leftVelVal)) { leftVelVal = 1; }
                float mountEff = ((height - neighbor_RIGHT.height) / (20000));
                rightM = Mathf.Clamp(leftVelVal * neighbor_RIGHT.airMoisture * horiVel, 1, 10000);
                rightR = Mathf.Clamp(leftVelVal * neighbor_RIGHT.airMoisture * rain * scale * (1 + mountEff * mountainEffect), 1, 10000);
                rightT = Mathf.Clamp(leftVelVal * neighbor_RIGHT.exTemp - scale / 111 / 0.5f, 0, 20);

                if (neighbor_RIGHT.seaConnected)
                {
                    seaConnected = true;
                }

                neighbor_RIGHT.heightRiver = height - neighbor_RIGHT.height + heightRiver;
            }
            if (neighbor_RIGHT.height >= height)
            {
                leftVelVal = neighbor_RIGHT.leftVel / Mathf.Clamp((neighbor_RIGHT.leftVel + neighbor_RIGHT.rightVel + neighbor_RIGHT.upVel + neighbor_RIGHT.downVel), 1, 100);
                if (float.IsNaN(leftVelVal)) { leftVelVal = 1; }
                rightM = Mathf.Clamp(leftVelVal * neighbor_RIGHT.airMoisture * horiVel, 1, 10000);
                rightR = Mathf.Clamp(leftVelVal * neighbor_RIGHT.airMoisture * rain * scale, 1, 10000);
                rightT = Mathf.Clamp(leftVelVal * neighbor_RIGHT.exTemp - scale / 111 / 0.5f, 0, 20);

                neighbor_RIGHT.heightLake = height - neighbor_RIGHT.height + heightLake;
            }

            if (neighbor_LEFT.height < seaLevel)
            {
                seaConnected = true;
            }
            float rightVelVal = 0;
            float leftM = 0;
            float leftR = 0;
            float leftT = 0;
            if (neighbor_LEFT.height < height)
            {
                rightVelVal = neighbor_LEFT.rightVel / Mathf.Clamp((neighbor_LEFT.leftVel + neighbor_LEFT.rightVel + neighbor_LEFT.upVel + neighbor_LEFT.downVel), 1, 100);
                if (float.IsNaN(rightVelVal)) { rightVelVal = 1; }
                float mountEff = ((height - neighbor_LEFT.height) / (20000));
                leftM = Mathf.Clamp(rightVelVal * neighbor_LEFT.airMoisture * horiVel, 1, 10000);
                leftR = Mathf.Clamp(rightVelVal * neighbor_LEFT.airMoisture * rain * scale * (1 + mountEff * mountainEffect), 1, 10000);
                leftT = Mathf.Clamp(rightVelVal * neighbor_LEFT.exTemp - scale / 111 / 1, 0, 20);

                if (neighbor_LEFT.seaConnected)
                {
                    seaConnected = true;
                }

                neighbor_LEFT.heightRiver = height - neighbor_LEFT.height + heightRiver;
            }
            if (neighbor_LEFT.height >= height)
            {
                rightVelVal = neighbor_LEFT.rightVel / Mathf.Clamp((neighbor_LEFT.leftVel + neighbor_LEFT.rightVel + neighbor_LEFT.upVel + neighbor_LEFT.downVel), 1, 100);
                if (float.IsNaN(rightVelVal)) { rightVelVal = 1; }
                leftM = Mathf.Clamp(rightVelVal * neighbor_LEFT.airMoisture * horiVel, 1, 10000);
                leftR = Mathf.Clamp(rightVelVal * neighbor_LEFT.airMoisture * rain * scale, 1, 10000);
                leftT = Mathf.Clamp(rightVelVal * neighbor_LEFT.exTemp - scale / 111 / 1, 0, 20);

                neighbor_LEFT.heightLake = height - neighbor_LEFT.height + heightLake;
            }

            if (neighbor_UP.height < seaLevel)
            {
                seaConnected = true;
            }
            float downVelVal = 0;
            float upM = 0;
            float upR = 0;
            float upT = 0;
            if (neighbor_UP.height < height)
            {
                downVelVal = neighbor_UP.downVel / Mathf.Clamp((neighbor_UP.leftVel + neighbor_UP.rightVel + neighbor_UP.upVel + neighbor_UP.downVel), 1, 100);
                if (float.IsNaN(downVelVal)) { downVelVal = 1; }
                float mountEff = ((height - neighbor_UP.height) / (20000));
                upM = Mathf.Clamp(downVelVal * neighbor_UP.airMoisture, 1, 10000);
                upR = Mathf.Clamp(downVelVal * neighbor_UP.airMoisture * rain * scale * (1 + mountEff * mountainEffect), 1, 10000);
                upT = Mathf.Clamp(downVelVal * neighbor_UP.exTemp - scale / 111 / 1, 0, 20);

                if (neighbor_UP.seaConnected)
                {
                    seaConnected = true;
                }

                neighbor_UP.heightRiver = height - neighbor_UP.height + heightRiver;
            }
            if (neighbor_UP.height >= height)
            {
                downVelVal = neighbor_UP.downVel / Mathf.Clamp((neighbor_UP.leftVel + neighbor_UP.rightVel + neighbor_UP.upVel + neighbor_UP.downVel), 1, 100);
                if (float.IsNaN(downVelVal)) { downVelVal = 1; }
                upM = Mathf.Clamp(downVelVal * neighbor_UP.airMoisture, 1, 10000);
                upR = Mathf.Clamp(downVelVal * neighbor_UP.airMoisture * rain * scale, 1, 10000);
                upT = Mathf.Clamp(downVelVal * neighbor_UP.exTemp - scale / 111 / 1, 0, 20);

                neighbor_UP.heightLake = height - neighbor_UP.height + heightLake;
            }

            if (neighbor_DOWN.height < seaLevel)
            {
                seaConnected = true;
            }
            float upVelVal = 0;
            float downM = 0;
            float downR = 0;
            float downT = 0;
            if (neighbor_DOWN.height < height)
            {
                upVelVal = neighbor_DOWN.upVel / Mathf.Clamp((neighbor_DOWN.leftVel + neighbor_DOWN.rightVel + neighbor_DOWN.upVel + neighbor_DOWN.downVel), 1, 100);
                if (float.IsNaN(upVelVal)) { upVelVal = 1; }
                float mountEff = ((height - neighbor_DOWN.height) / (20000));
                downM = Mathf.Clamp(upVelVal * neighbor_DOWN.airMoisture, 1, 10000);
                downR = Mathf.Clamp(upVelVal * neighbor_DOWN.airMoisture * rain * scale * (1 + mountEff * mountainEffect), 1, 10000);
                downT = Mathf.Clamp(upVelVal * neighbor_DOWN.exTemp - scale / 111 / 1, 0, 20);

                if (neighbor_DOWN.seaConnected)
                {
                    seaConnected = true;
                }

                neighbor_DOWN.heightRiver = height - neighbor_DOWN.height + heightRiver;
            }
            if (neighbor_DOWN.height >= height)
            {
                upVelVal = neighbor_DOWN.upVel / Mathf.Clamp((neighbor_DOWN.leftVel + neighbor_DOWN.rightVel + neighbor_DOWN.upVel + neighbor_DOWN.downVel), 1, 100);
                if (float.IsNaN(upVelVal)) { upVelVal = 1; }
                downM = Mathf.Clamp(upVelVal * neighbor_DOWN.airMoisture, 1, 10000);
                downR = Mathf.Clamp(upVelVal * neighbor_DOWN.airMoisture * rain * scale, 1, 10000);
                downT = Mathf.Clamp(upVelVal * neighbor_DOWN.exTemp - scale / 111 / 1, 0, 20);

                neighbor_DOWN.heightLake = height - neighbor_DOWN.height + heightLake;
            }

            airMoisture = Mathf.Clamp((rightM + leftM + upM + downM), 1, 10000);
            airMoisture = Mathf.Clamp(airMoisture - rightR - leftR - upR - downR, 1, 10000);
            exTemp = rightT + leftT + upT + downT * 0.25f;

            exTemp = Mathf.Clamp(exTemp, 1, 20);

            moisture = (rightR + leftR + upR + downR) * Mathf.Lerp(1, 0.01f, (temp + exTemp) / 50) * (Mathf.Lerp(100, 0.02f, tempLat / 10)
                                                                                                    + Mathf.Lerp(0.02f, 0, (tempLat - 10) / 10)
                                                                                                    + Mathf.Lerp(0, 0.02f, (tempLat - 20) / 10)
                                                                                                    + Mathf.Lerp(0.02f, 10, (tempLat - 30) / 30));
            //moisture -= moisture * evap * scale;
            //moisture = Mathf.Abs(leftVel - rightVel) * 10;
            //moisture = airMoisture / 2;
            moisture = Mathf.Clamp(moisture, 1, 10000);
            //airMoisture = Mathf.Clamp(airMoisture + moisture * evap * scale, 1, 10000);
        }

        if (height >= seaLevel)
        {
            if (moisture < 1000)
            {
                if (temp + exTemp > 5)
                {
                    _bioColor = new Color(1, 0, 0);
                }
                else
                {
                    _bioColor = new Color(1, 0.5f, 0.5f);
                }
            }
            else if (moisture < 9000)
            {
                if (temp + exTemp > 5)
                {
                    _bioColor = new Color(0, 1, 0);
                }
                else
                {
                    _bioColor = new Color(0.5f, 1, 0.5f);
                }
            }
            else
            {
                if (temp + exTemp > 25)
                {
                    _bioColor = new Color(0, 0.5f, 0);
                }
                else if (temp + exTemp > 10)
                {
                    _bioColor = new Color(0.125f, 0.5f, 0.125f);
                }
                else
                {
                    _bioColor = new Color(0.25f, 0.5f, 0.25f);
                }
            }
            if (temp + exTemp < -7)
            {
                _bioColor = new Color(1, 1, 1);
            }

            if (neighbor_LEFT.height > height && height > seaLevel)
            {
                _bioColor = _bioColor / ((neighbor_LEFT.height - height) / 4000 + 1);
            }

            if (neighbor_UP.height > height && height > seaLevel)
            {
                _bioColor = _bioColor / ((neighbor_LEFT.height - height) / 4000 + 1);
            }
        }
        else
        {
            _bioColor = Color.Lerp(Color.black, Color.blue, airMoisture / 10000);
            _bioColor = Color.Lerp(_bioColor, Color.red, exTemp / 30);
        }

        _renderer.color = _bioColor;
    }
}
