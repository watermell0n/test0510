using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ex : MonoBehaviour
{
    public GameObject mapTile;

    const int SIZE = 8;
    const int PATH_LENGTH = 22;
    int[,] mapTiles = new int[SIZE, SIZE];  // 1=Ÿ�� �������� ���
    Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    private List<Vector2Int> pathTiles = new List<Vector2Int>();
    private HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

    void Start()
    {
        generateMap();
        printGrid();
    }

    private void generateMap()
    {
        // �ʱ�ȭ
        pathTiles.Clear();
        visited.Clear();
        mapTiles = new int[SIZE, SIZE];

        Vector2Int startTile, endTile;  // �Ա�, �ⱸ

        do
        {
            startTile = generateEdgeTile();
            endTile = generateEdgeTile();
        }
        while (startTile == endTile || startTile.x == endTile.x || startTile.y == endTile.y);

        Debug.Log(startTile);
        Debug.Log(endTile);

        bool success = false;
        int attempts = 0;

        while (!success && attempts < 20)
        {
            pathTiles.Clear();
            visited.Clear();

            pathTiles.Add(startTile);
            visited.Add(startTile);

            success = generatePath(startTile, endTile);
            attempts++;
        }

        if (success)
        {
            pathTiles.Add(endTile);
            foreach (var tile in pathTiles)
            {
                mapTiles[tile.x, tile.y] = 1;
            }
        }
        else
        {
            Debug.Log("��� ���� ����");
        }
    }

    // �Ա�, �ⱸ Ÿ�� ���� ����
    private Vector2Int generateEdgeTile() 
    {
        bool ran = Random.value < 0.5f;

        if (ran)
        {
            int x = (Random.value < 0.5f) ? 0 : 7;
            int y = (Random.value < 0.5f) ? 1 : 6;
            return new Vector2Int(x, y);
        }
        else
        {
            int y = (Random.value < 0.5f) ? 0 : 7;
            int x = (Random.value < 0.5f) ? 1 : 6;
            return new Vector2Int(x, y);
        }
    }

    private bool generatePath(Vector2Int current, Vector2Int end)
    {
        if (pathTiles.Count == PATH_LENGTH - 1)   // ��� 22ĭ �ϼ�
        {
            foreach (var dir in directions)
            {
                if (current + dir == end) return true;  // ������ ��ΰ� �ⱸ�� ����Ǿ� �ִ��� Ȯ��
            }
            return false;
        }

        Vector2Int[] sortedDirs = SortedDirections(current, end);
        //Shuffle(directions);
        foreach (var dir in sortedDirs)
        {
            Vector2Int next = current + dir;
            if (isValid(next))
            {
                pathTiles.Add(next);
                visited.Add(next);

                if (generatePath(next, end)) return true;

                // ��ΰ� ���� ��� �ǵ��ư��� ����
                pathTiles.RemoveAt(pathTiles.Count - 1);
                visited.Remove(next);
            }
        }
        return false;   // ������ ��θ� ã�� ���� ���
    }

    // �ⱸ�� ����� ������ ����ġ�� �Ű� ������
    private Vector2Int[] SortedDirections(Vector2Int current, Vector2Int end)
    {
        List<Vector2Int> dirList = new List<Vector2Int>(directions);
        dirList.Sort((a, b) =>  // Ŀ���� ����
        {
            Vector2Int next1 = current + a;
            Vector2Int next2 = current + b;
            float dist1 = Vector2Int.Distance(next1, end);
            float dist2 = Vector2Int.Distance(next2, end);
            return dist1.CompareTo(dist2);  // �������� ����
        });
        return dirList.ToArray();   // list�� �迭�� ��ȯ�� ��ȯ
    }

    // �Ǽ�-������ ���� �˰��� ���
    private void Shuffle(Vector2Int[] array) 
    {
        for (int i = 0; i < array.Length; i++)
        {
            int ran = Random.Range(i, array.Length);
            (array[i], array[ran]) = (array[ran], array[i]);
        }
    }

    // ���� Ÿ�� ��ȿ�� �˻�
    bool isValid(Vector2Int tile)   
    {
        if (tile.x < 1 || tile.x > 6 || tile.y < 1 || tile.y > 6) return false;    // ���� ���
        if (visited.Contains(tile)) return false;   // �̹� ��ο� ���ԵǾ� �ִ� ���

        // ��ο��� 2X2 ������ �����Ǵ��� Ȯ��
        int neighbors = 0;
        foreach (var dir in directions)
        {
            var sub = tile + dir;
            if (pathTiles.Contains(sub)) neighbors++;
        }
        return neighbors <= 1;

        //return true;
    }

    void printGrid()
    {
        string output = "";
        for (int y = 0; y < SIZE; y++)
        {
            for (int x = 0; x < SIZE; x++)
            {
                output += mapTiles[y, x] == 1 ? "��" : "��";
            }
            output += "\n";
        }
        Debug.Log(output);
    }
}
