using UnityEngine;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class MapGenerator : MonoBehaviour
{
    [Header("맵 크기 설정")]
    [SerializeField] private int mapWidth = 80;          // 맵 가로 크기
    [SerializeField] private int mapHeight = 80;         // 맵 세로 크기
    [SerializeField] public float CellSize = 2f;        // 각 셀의 크기
    
    [Header("프리팹 참조")]
    [SerializeField] private GameObject floorPrefab;     // 바닥
    [SerializeField] private GameObject wallPrefab;      // 벽
    [SerializeField] private GameObject playerPrefab;  // 플레이어 프리팹 추가
    
    [Header("경로 설정")]
    [SerializeField] private int pathWidth = 2;          // 경로 너비 최소 2 이상으로 설정
    
    [Header("타일 설정")]
    [SerializeField] private float wallHeight = 4f;  // 벽 높이 설정 추가
    
    private Cell[,] grid;                               // 맵 그리드
    private Vector2Int startPos;                        // 시작 위치
    private Vector2Int bossPos;                         // 보스 위치
    public List<Vector2Int> MainPath { get; private set; }

    public delegate void MapGeneratedHandler();
    public event MapGeneratedHandler OnMapGenerated;

    public delegate void PlayerSpawnedHandler(GameObject player);
    public event PlayerSpawnedHandler OnPlayerSpawned;

    private NavMeshSurface navMeshSurface;

    private void Awake()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    private void Start()
    {
        GenerateMap();
    }
    
    // 맵 생성 메인 함수
    public void GenerateMap()
    {
        InitializeGrid();
        GeneratePath();
        CreateMapMesh();
        
        // 경로 생성 완료 후 이벤트 발생
        OnMapGenerated?.Invoke();
    }
    
    private void InitializeGrid()
    {
        grid = new Cell[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                grid[x, z] = new Cell { type = CellType.Wall };
            }
        }
        
        // 시작 위치 설정 (왼쪽 아래)
        startPos = new Vector2Int(5, 5);
        // 보스 위치 설정 (오른쪽 위)
        bossPos = new Vector2Int(mapWidth - 5, mapHeight - 5);
    }
    
    // 경로 생성
    private void GeneratePath()
    {
        MainPath = new List<Vector2Int>();
        Vector2Int currentPos = startPos;
        MainPath.Add(currentPos);

        // 맵을 여러 구역으로 나누어 경유점 생성
        int numSections = Random.Range(4, 7);  // 경로를 4~6개의 구역으로 나눔
        float sectionWidth = (bossPos.x - startPos.x) / numSections;
        
        List<Vector2Int> waypoints = new List<Vector2Int>();
        waypoints.Add(startPos);

        // 각 구역마다 랜덤한 경유점 생성
        for (int i = 1; i < numSections; i++)
        {
            int minX = startPos.x + Mathf.RoundToInt(sectionWidth * i);
            int maxX = startPos.x + Mathf.RoundToInt(sectionWidth * (i + 0.8f));
            
            Vector2Int waypoint = new Vector2Int(
                Random.Range(minX, maxX),
                Random.Range(10, mapHeight - 10)
            );
            waypoints.Add(waypoint);
        }
        waypoints.Add(bossPos);

        // 경유점들을 베지어 곡선으로 연결
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Vector2Int start = waypoints[i];
            Vector2Int end = waypoints[i + 1];
            
            // 중간 제어점 생성
            Vector2Int controlPoint1 = start + new Vector2Int(
                Random.Range(5, 15),
                Random.Range(-10, 10)
            );
            
            Vector2Int controlPoint2 = end + new Vector2Int(
                Random.Range(-15, -5),
                Random.Range(-10, 10)
            );

            // 베지어 곡선을 따라 경로 포인트 생성
            int steps = 20;
            for (int j = 0; j <= steps; j++)
            {
                float t = j / (float)steps;
                Vector2 bezierPoint = CalculateBezierPoint(t, start, controlPoint1, controlPoint2, end);
                Vector2Int pathPoint = new Vector2Int(
                    Mathf.RoundToInt(bezierPoint.x),
                    Mathf.RoundToInt(bezierPoint.y)
                );
                
                // 경로가 맵 경계 안에 있도록 조정
                pathPoint.x = Mathf.Clamp(pathPoint.x, pathWidth + 2, mapWidth - pathWidth - 2);
                pathPoint.y = Mathf.Clamp(pathPoint.y, pathWidth + 2, mapHeight - pathWidth - 2);
                
                if (!MainPath.Contains(pathPoint))
                {
                    MainPath.Add(pathPoint);
                }
            }
        }

        // 경로 주변을 바닥으로 설정
        foreach (Vector2Int pathPos in MainPath)
        {
            for (int x = -pathWidth; x <= pathWidth; x++)
            {
                for (int y = -pathWidth; y <= pathWidth; y++)
                {
                    Vector2Int pos = pathPos + new Vector2Int(x, y);
                    if (IsInBounds(pos))
                    {
                        grid[pos.x, pos.y].type = CellType.Floor;
                    }
                }
            }
        }

        // 경로 주변에 랜덤한 추가 공간 생성
        AddRandomRooms();
    }

    // 베지어 곡선 계산 함수
    private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;

        return point;
    }

    // 랜덤한 추가 공간 생성
    private void AddRandomRooms()
    {
        int numRooms = Random.Range(3, 6);  // 추가할 방의 개수

        for (int i = 0; i < numRooms; i++)
        {
            // 메인 경로에서 랜덤한 위치 선택
            Vector2Int roomCenter = MainPath[Random.Range(0, MainPath.Count)];
            
            // 방 크기 설정
            int roomWidth = Random.Range(4, 8);
            int roomHeight = Random.Range(4, 8);

            // 방 생성
            for (int x = -roomWidth; x <= roomWidth; x++)
            {
                for (int y = -roomHeight; y <= roomHeight; y++)
                {
                    Vector2Int pos = roomCenter + new Vector2Int(x, y);
                    if (IsInBounds(pos))
                    {
                        grid[pos.x, pos.y].type = CellType.Floor;
                    }
                }
            }
        }
    }
    
    // 맵 메시 생성
    private void CreateMapMesh()
    {
        // 기존 맵 오브젝트 제거
        Transform mapHolder = transform.Find("MapHolder");
        if (mapHolder != null) Destroy(mapHolder.gameObject);
        
        // 새 맵 홀더 생성
        mapHolder = new GameObject("MapHolder").transform;
        mapHolder.parent = transform;
        
        // 맵 생성
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                Vector3 worldPos = new Vector3(x * CellSize, 0, z * CellSize);
                
                if (grid[x, z].type == CellType.Floor)
                {
                    GameObject floor = Instantiate(floorPrefab, worldPos, Quaternion.identity, mapHolder);
                    floor.transform.localScale = new Vector3(CellSize, 1, CellSize);
                }
                else
                {
                    // 벽 생성 위치와 스케일 수정
                    Vector3 wallPos = worldPos + Vector3.up * (wallHeight / 2);
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, mapHolder);
                    wall.transform.localScale = new Vector3(CellSize, wallHeight, CellSize);
                }
            }
        }

        // NavMesh 베이크
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        // NavMesh 생성 후 플레이어 소환
        Vector3 playerSpawnPos = new Vector3(startPos.x * CellSize, 1f, startPos.y * CellSize);
        GameObject player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        OnPlayerSpawned?.Invoke(player);  // 이벤트 발생
    }
    
    // 그리드 좌표를 월드 좌표로 변환
    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * CellSize, 0, gridPos.y * CellSize);
    }
    
    // 좌표가 맵 범위 내인지 확인
    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < mapWidth && pos.y >= 0 && pos.y < mapHeight;
    }
}

public enum CellType
{
    Wall,
    Floor
}

public class Cell
{
    public CellType type;
} 