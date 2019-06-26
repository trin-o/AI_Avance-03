using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class EenemyMovement : BaseAgent
{
    [Header("Displacement Up and Down")]
    [SerializeField] private bool UpDown;
    [SerializeField] private float yDisplacement;

    float positionY;
    float m_positionY;

    void Start()
    {
        positionY = transform.position.y;
        m_positionY = positionY;
    }

    // Update is called once per frame
    void Update()
    {
        if (UpDown)
        {
            float newY = UpAndDown();
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    float UpAndDown()
    {
        m_positionY += yDisplacement;

        float limit = 1.5f;

        if (m_positionY > positionY + limit)
        {
            yDisplacement = -yDisplacement;
        }
        else if (m_positionY < positionY - limit)
        {
            yDisplacement = -yDisplacement;
        }

        return m_positionY;
    }
}
