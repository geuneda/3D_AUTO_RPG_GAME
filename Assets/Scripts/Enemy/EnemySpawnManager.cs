using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] private GameObject normalEnemyPrefab;
    [SerializeField] private GameObject eliteEnemyPrefab;
    [SerializeField] private GameObject bossEnemyPrefab;
    
    [Header("스폰 설정")]
    [SerializeField] private int normalEnemiesPerSection = 3;
    [SerializeField] private int eliteEnemiesPerSection = 1;
    
    private MapGenerator mapGenerator;
    
    private void Start()
    {
        mapGenerator = FindFirstObjectByType<MapGenerator>();
        if (mapGenerator != null)
        {
            mapGenerator.OnMapGenerated += SpawnEnemies;
        }
    }
    
    public void SpawnEnemies()
    {
        if (mapGenerator == null || mapGenerator.MainPath == null || mapGenerator.MainPath.Count == 0)
        {
            Debug.LogError("EnemySpawnManager : SpawnEnemies 확인바람");
            return;
        }
        
        List<Vector2Int> path = mapGenerator.MainPath;
        int pathLength = path.Count;
        int numSections = pathLength / 20;
        
        for (int i = 0; i < numSections; i++)
        {
            int sectionStart = i * 20;
            int sectionEnd = Mathf.Min(sectionStart + 20, pathLength);
            
            for (int j = 0; j < normalEnemiesPerSection; j++)
            {
                SpawnEnemy(path, sectionStart, sectionEnd, EnemyType.Normal);
            }
            
            for (int j = 0; j < eliteEnemiesPerSection; j++)
            {
                SpawnEnemy(path, sectionStart, sectionEnd, EnemyType.Elite);
            }
        }
        
        if (pathLength > 0)
        {
            Vector3 bossPosition = new Vector3(path[pathLength - 1].x * mapGenerator.CellSize, 0, 
                                             path[pathLength - 1].y * mapGenerator.CellSize);
            Instantiate(bossEnemyPrefab, bossPosition, Quaternion.identity);
        }
    }
    
    private void SpawnEnemy(List<Vector2Int> path, int startIndex, int endIndex, EnemyType type)
    {
        int randomIndex = Random.Range(startIndex, endIndex);
        Vector2Int spawnPoint = path[randomIndex];
        
        Vector2Int offset = new Vector2Int(
            Random.Range(-2, 3),
            Random.Range(-2, 3)
        );
        
        Vector3 spawnPosition = new Vector3(
            (spawnPoint.x + offset.x) * mapGenerator.CellSize,
            1f,
            (spawnPoint.y + offset.y) * mapGenerator.CellSize
        );
        
        GameObject prefab = type == EnemyType.Normal ? normalEnemyPrefab : eliteEnemyPrefab;
        GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        if (enemy.TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.Warp(spawnPosition);
        }
    }
} 