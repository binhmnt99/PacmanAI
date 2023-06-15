using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 2f;
    public Transform target;

    public LayerMask wall;
    private Vector3 newPos;

    private Vector3 currentDirection = Vector3.up;
    private Vector3 newDirection = Vector3.zero;

    private int rotate;


    void Start()
    {
        ChooseDirection();
        newDirection = (newPos - transform.position).normalized;
        currentDirection = newDirection;
    }


    void Update()
    {


        if (transform.position != target.position)
        {
            if (transform.position == newPos)
            {
                ChooseDirection();
            }
            else
            {
                currentDirection = (newPos - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, newPos, speed * Time.deltaTime);
            }
        }

    }

    void ChooseDirection()
    {
        // Lấy 4 hướng
        FourDirection[] direction = FindObjectsOfType<FourDirection>();
        float shortestDirection = Mathf.Infinity;
        int countWall = 0;
        Vector3 backPos = Vector3.zero;
        List<Vector3> equalDistances = new List<Vector3>();

        for (int i = 0; i < direction.Length; i++)
        {
            // Kiểm tra va chạm với tường
            var hitCollider = Physics2D.OverlapCircle(direction[i].transform.position, 0.1f, wall);
            // Hướng đang xét
            Vector3 thisDirection = (direction[i].transform.position - transform.position).normalized;
            Debug.Log(direction[i].getDirectionName() + " " + direction[i].getDistance());

            if (hitCollider)
            {
                if (thisDirection != (-1 * currentDirection))
                    countWall++;
            }
            else
            {
                // Kiểm tra quay lại
                if (thisDirection == (-1 * currentDirection))
                {
                    backPos = direction[i].transform.position;
                }
                else
                {
                    // Kiểm tra khoảng cách nhỏ nhất
                    float thisDistance = direction[i].getDistance();
                    if (shortestDirection > thisDistance)
                    {
                        
                        shortestDirection = thisDistance;
                        newPos = direction[i].transform.position;
                    }
                    else if (shortestDirection == thisDistance)
                    {
                        // Cùng khoảng cách nhưng khác hướng
                        equalDistances.Add(direction[i].transform.position);
                    }
                }
            }
        }

        // Khi có 3 tường thì quay lại
        if (countWall == 3)
        {
            newPos = backPos;
        }
        else if (equalDistances.Count >= 2)
        {
            // Nếu có 2 hướng có cùng độ dài, di chuyển ngẫu nhiên giữa chúng
            int randomIndex = Random.Range(0, equalDistances.Count);
            newPos = equalDistances[randomIndex];
        }
    }

}
