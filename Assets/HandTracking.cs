using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    public UDPReceive udpReceive;

    public GameObject[] leftHandPoints;  
    public GameObject[] rightHandPoints; 

    private int offset = 70;

    private Vector3 cameraTransform = new Vector3(6, 5, -10);
    

    void Start()
    {
        leftHandPoints = new GameObject[21];
        rightHandPoints = new GameObject[21];
        
        Transform leftPointsParent = GameObject.Find("LeftHand/Points")?.transform;
        Transform rightPointsParent = GameObject.Find("RightHand/Points")?.transform;

        if (leftPointsParent == null || rightPointsParent == null)
        {
            Debug.LogError("LeftHand/Points 또는 RightHand/Points가 맞는지 확인 필요");
            return;
        }

        for (int i = 0; i < 21; i++)
        {
            Transform leftPoint = leftPointsParent.Find($"Point ({i})");
            Transform rightPoint = rightPointsParent.Find($"Point ({i})");

            if (leftPoint != null)
                leftHandPoints[i] = leftPoint.gameObject;
            else
                Debug.LogWarning($"LeftHand point ({i})가 없음");

            if (rightPoint != null)
                rightHandPoints[i] = rightPoint.gameObject;
            else
                Debug.LogWarning($"RightHand point ({i})가 없음");
        }
    }

    void Update()
    {
        HandData handData = udpReceive.handData;
        Quaternion handRotation = Quaternion.Euler(180f, 0f, 180f);

        // 왼손 처리
        if (handData != null && handData.Left != null && handData.Left.Length >= 63)
        {
            for (int i = 0; i < 21; i++)
            {
                float x = -(7 - handData.Left[i * 3] / offset );
                float y = (handData.Left[i * 3 + 1] / offset );
                float z = (handData.Left[i * 3 + 2] / offset );

                Vector3 rawPos = new Vector3(x, y, z);
                Vector3 rotatedPos = handRotation * rawPos;

                if (leftHandPoints != null && i < leftHandPoints.Length && leftHandPoints[i] != null)
                    leftHandPoints[i].transform.localPosition = rotatedPos;
            }
        }

        // 오른손 처리
        if (handData != null && handData.Right != null && handData.Right.Length >= 63)
        {
            for (int i = 0; i < 21; i++)
            {
                float x = -(7 - handData.Right[i * 3] / offset);
                float y = (handData.Right[i * 3 + 1] / offset);
                float z = (handData.Right[i * 3 + 2] / offset);

                Vector3 rawPos = new Vector3(x, y, z);
                Vector3 rotatedPos = handRotation * rawPos;

                if (rightHandPoints != null && i < rightHandPoints.Length && rightHandPoints[i] != null)
                    rightHandPoints[i].transform.localPosition = rotatedPos;
            }
        }

        if ((handData?.Left?.Length ?? 0) < 63 && (handData?.Right?.Length ?? 0) < 63)
        {
            Debug.Log("No hands detected.");
        }
    }
}
