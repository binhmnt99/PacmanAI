using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISimulator : MonoBehaviour
{
    // Các hằng số cho biểu đồ của trò chơi Pacman
    private const int Width = 10;
    private const int Height = 10;

    // Tạo một mảng 2D để biểu diễn bản đồ của trò chơi
    private char[,] gameMap = new char[Width, Height];

    // Định nghĩa các ký tự cho các phần tử trên bản đồ
    private char wallChar = '#';
    private char pacmanChar = 'P';
    private char ghostChar = 'G';
    private char emptyChar = ' ';

    // Vị trí bắt đầu của Ghost
    private int ghostX = 1;
    private int ghostY = 6;
    private int pacmanX = 7;
    private int pacmanY = 5;

    // Tìm đường đi ngẫu nhiên cho Ghost
    private System.Random random = new System.Random();
    private int direction;

    // Biến đếm thời gian để kiểm soát tốc độ cập nhật
    private float timer = 0f;
    public float updateInterval = 0.5f;

    private List<int> usedValues = new List<int>();
    private int randomNumber;

    private List<Vector2Int> validNeighborPositions = new List<Vector2Int>();

    private void Start()
    {
        // Thiết lập bản đồ với các ký tự khác nhau
        //Top - Left -> Bottom - Right
        gameMap[0, 0] = wallChar;
        gameMap[0, 1] = wallChar;
        gameMap[0, 2] = wallChar;
        gameMap[0, 3] = wallChar;
        gameMap[0, 4] = wallChar;
        gameMap[0, 5] = wallChar;
        gameMap[0, 6] = wallChar;
        gameMap[0, 7] = wallChar;
        gameMap[0, 8] = wallChar;
        gameMap[0, 9] = wallChar;
        gameMap[1, 0] = wallChar;
        gameMap[1, 1] = emptyChar;
        gameMap[1, 2] = emptyChar;
        gameMap[1, 3] = emptyChar;
        gameMap[1, 4] = emptyChar;
        gameMap[1, 5] = emptyChar;
        gameMap[1, 6] = ghostChar;
        gameMap[1, 7] = wallChar;
        gameMap[1, 8] = emptyChar;
        gameMap[1, 9] = wallChar;
        gameMap[2, 0] = wallChar;
        gameMap[2, 1] = emptyChar;
        gameMap[2, 2] = emptyChar;
        gameMap[2, 3] = emptyChar;
        gameMap[2, 4] = emptyChar;
        gameMap[2, 5] = wallChar;
        gameMap[2, 6] = emptyChar;
        gameMap[2, 7] = wallChar;
        gameMap[2, 8] = emptyChar;
        gameMap[2, 9] = wallChar;
        gameMap[3, 0] = wallChar;
        gameMap[3, 1] = emptyChar;
        gameMap[3, 2] = emptyChar;
        gameMap[3, 3] = emptyChar;
        gameMap[3, 4] = emptyChar;
        gameMap[3, 5] = wallChar;
        gameMap[3, 6] = wallChar;
        gameMap[3, 7] = emptyChar;
        gameMap[3, 8] = emptyChar;
        gameMap[3, 9] = wallChar;
        gameMap[4, 0] = wallChar;
        gameMap[4, 1] = emptyChar;
        gameMap[4, 2] = emptyChar;
        gameMap[4, 3] = emptyChar;
        gameMap[4, 4] = emptyChar;
        gameMap[4, 5] = wallChar;
        gameMap[4, 6] = emptyChar;
        gameMap[4, 7] = emptyChar;
        gameMap[4, 8] = emptyChar;
        gameMap[4, 9] = wallChar;
        gameMap[5, 0] = wallChar;
        gameMap[5, 1] = emptyChar;
        gameMap[5, 2] = emptyChar;
        gameMap[5, 3] = emptyChar;
        gameMap[5, 4] = emptyChar;
        gameMap[5, 5] = emptyChar;
        gameMap[5, 6] = emptyChar;
        gameMap[5, 7] = emptyChar;
        gameMap[5, 8] = emptyChar;
        gameMap[5, 9] = wallChar;
        gameMap[6, 0] = wallChar;
        gameMap[6, 1] = emptyChar;
        gameMap[6, 2] = emptyChar;
        gameMap[6, 3] = emptyChar;
        gameMap[6, 4] = emptyChar;
        gameMap[6, 5] = emptyChar;
        gameMap[6, 6] = emptyChar;
        gameMap[6, 7] = emptyChar;
        gameMap[6, 8] = emptyChar;
        gameMap[6, 9] = wallChar;
        gameMap[7, 0] = wallChar;
        gameMap[7, 1] = emptyChar;
        gameMap[7, 2] = emptyChar;
        gameMap[7, 3] = wallChar;
        gameMap[7, 4] = wallChar;
        gameMap[7, 5] = wallChar;
        gameMap[7, 6] = emptyChar;
        gameMap[7, 7] = emptyChar;
        gameMap[7, 8] = emptyChar;
        gameMap[7, 9] = wallChar;
        gameMap[8, 0] = wallChar;
        gameMap[8, 1] = emptyChar;
        gameMap[8, 2] = emptyChar;
        gameMap[8, 3] = emptyChar;
        gameMap[8, 4] = wallChar;
        gameMap[8, 5] = pacmanChar;
        gameMap[8, 6] = emptyChar;
        gameMap[8, 7] = emptyChar;
        gameMap[8, 8] = emptyChar;
        gameMap[8, 9] = wallChar;
        gameMap[9, 0] = wallChar;
        gameMap[9, 1] = wallChar;
        gameMap[9, 2] = wallChar;
        gameMap[9, 3] = wallChar;
        gameMap[9, 4] = wallChar;
        gameMap[9, 5] = wallChar;
        gameMap[9, 6] = wallChar;
        gameMap[9, 7] = wallChar;
        gameMap[9, 8] = wallChar;
        gameMap[9, 9] = wallChar;

        // Chọn hướng di chuyển ngẫu nhiên ban đầu
        direction = GetRandomUniqueValue(0, 3);
    }

    public int GetRandomUniqueValue(int minValue, int maxValue)
    {
        int randomValue = Random.Range(minValue, maxValue + 1);

        while (usedValues.Contains(randomValue))
        {
            randomValue = Random.Range(minValue, maxValue + 1);
        }

        usedValues.Add(randomValue);

        // Nếu danh sách đã chứa tất cả các giá trị có thể, hãy xóa danh sách và bắt đầu lại.
        if (usedValues.Count == (maxValue - minValue + 1))
        {
            usedValues.Clear();
        }

        return randomValue;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Kiểm tra xem đã đến thời điểm cập nhật mới hay chưa
        if (timer >= updateInterval)
        {
            // Reset timer
            timer = 0f;

            float distance = CalculateDistance(ghostX, ghostY, pacmanX, pacmanY);
            //Debug.Log("Distance to Pacman: " + distance);
            MoveTowardsPacman();

        }
    }

    private void MoveTowardsPacman()
    {
        // Tìm position gần kề
        FindValidNeighborPositions(new Vector2Int(ghostX, ghostY));

        //Kiểm tra nếu có position gần kề hợp lệ
        if (validNeighborPositions.Count > 0)
        {
            // Chọn một position gần kề hợp lệ ngẫu nhiên
            int randomIndex = Random.Range(0, validNeighborPositions.Count);
            Vector2Int randomPosition = validNeighborPositions[randomIndex];

            // cập nhật Ghost position
            //gameMap[ghostX, ghostY] = emptyChar;
            ghostX = randomPosition.x;
            ghostY = randomPosition.y;
            gameMap[ghostX, ghostY] = 'o';
        }
    }

    private void FindValidNeighborPositions(Vector2Int currentPosition)
    {
        // Clear the list of valid neighbor positions
        validNeighborPositions.Clear();

        // Check the neighboring positions
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPosition = currentPosition + direction;
            if (gameMap[neighborPosition.x, neighborPosition.y] != wallChar)
            {
                validNeighborPositions.Add(neighborPosition);
            }
        }
    }

    private float CalculateDistance(int ghostX, int ghostY, int pacmanX, int pacmanY)
    {
        float dx = pacmanX - ghostX;
        float dy = pacmanY - ghostY;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    private void OnGUI()
    {
        float cellSize = 40f; // Kích thước của mỗi ô trên bản đồ
        float mapWidth = Width * cellSize;
        float mapHeight = Height * cellSize;

        float offsetX = (Screen.width - mapWidth) / 2f;
        float offsetY = (Screen.height - mapHeight) / 2f;

        // Vẽ bản đồ
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                // Tính toán vị trí của ô trên màn hình
                Vector2 position = new Vector2(x * cellSize + offsetX, y * cellSize + offsetY);

                // Vẽ ô trên màn hình dựa trên giá trị của gameMap[x, y]
                switch (gameMap[x, y])
                {
                    case '#':
                        GUI.Label(new Rect(position.x, position.y, cellSize, cellSize), "#");
                        break;
                    case 'P':
                        GUI.Label(new Rect(position.x, position.y, cellSize, cellSize), "P");
                        break;
                    case 'G':
                        GUI.Label(new Rect(position.x, position.y, cellSize, cellSize), "G");
                        break;
                    case 'o':
                        GUI.Label(new Rect(position.x, position.y, cellSize, cellSize), "0");
                        break;
                    default:
                        GUI.Label(new Rect(position.x, position.y, cellSize, cellSize), " ");
                        break;
                }
            }
        }
    }

}
