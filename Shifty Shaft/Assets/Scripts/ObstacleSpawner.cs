using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObstacleSpawner : MonoBehaviour
{
    [Serializable]
    private struct SpawnPoint
    {
        public Vector3 position;
        public Vector3 eulerAngles;
    }

    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private Color[] colors;
    [SerializeField] private float distanceBetweenObstacles;
    [SerializeField] private float distanceFromFirstObstacle;
    [SerializeField] private int obstaclesPerColor;

    private Dictionary<string, ObjectPool<GameObject>> obstaclePools = new Dictionary<string, ObjectPool<GameObject>>();
    private Color currentColor;

    private float nextSpawnPositionZ;
    private int spawnCounter = 0;

    // Start is called before the first frame update
    void Start()
    {   
        RenderSettings.fogStartDistance = GameObject.Find("Player").transform.position.z - Camera.main.transform.position.z;
        RenderSettings.fogEndDistance =  RenderSettings.fogStartDistance + distanceFromFirstObstacle;

        int obstaclesToSpawn = Mathf.FloorToInt(distanceFromFirstObstacle / distanceBetweenObstacles) + 3;

        nextSpawnPositionZ = distanceFromFirstObstacle;

        foreach (GameObject obstacle in obstacles)
        {
            ObjectPool<GameObject> obstaclePool = new ObjectPool<GameObject>(() =>
            {
                return Instantiate(obstacle, transform);
            }, obstacle =>
            {
                obstacle.SetActive(true);
            }, obstacle =>
            {
                obstacle.SetActive(false);
            }, obstacle =>
            {
                Destroy(obstacle);
            }, false, Mathf.FloorToInt(obstaclesToSpawn / obstacles.Length), obstaclesToSpawn);

            obstaclePools.Add(obstacle.tag, obstaclePool);
        }

        ChangeCurrentColor();

        for (int i = 0; i < obstaclesToSpawn; i++)
            SpawnRandomObstacle();
    }

    private void ChangeCurrentColor()
    {
        Color randomColor = colors[UnityEngine.Random.Range(0, colors.Length)];

        if (randomColor == currentColor)
            ChangeCurrentColor();
        else
            currentColor = randomColor;
    }

    private void SpawnRandomObstacle()
    {
        string randomTag = obstacles[UnityEngine.Random.Range(0, obstacles.Length)].tag;
        GameObject randomObstacle = obstaclePools[randomTag].Get();

        SpawnPoint randomSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        randomSpawnPoint.position.z = nextSpawnPositionZ;

        randomObstacle.transform.localPosition = randomSpawnPoint.position;
        randomObstacle.transform.localEulerAngles = randomSpawnPoint.eulerAngles;
        randomObstacle.GetComponent<Renderer>().material.color = currentColor;

        nextSpawnPositionZ += distanceBetweenObstacles;
        spawnCounter++;

        if (spawnCounter == obstaclesPerColor)
        {
            ChangeCurrentColor();
            spawnCounter = 0;
        }
    }

    public IEnumerator Release(GameObject obstacle)
    {
        yield return new WaitForSeconds(1f);

        obstaclePools[obstacle.tag].Release(obstacle);
        SpawnRandomObstacle();
    }

    public void FloatingOrigin(float playerPositionZ)
    {
        foreach (Transform child in transform)
        {
            Vector3 position = child.localPosition;
            position.z -= playerPositionZ;
            child.localPosition = position;
        }

        nextSpawnPositionZ -= playerPositionZ;
    }
}
