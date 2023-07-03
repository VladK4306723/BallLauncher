using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay;
    [SerializeField] private float respawnDelay;
    
    // [SerializeField] float delayDuration = 0.5f;
    private Rigidbody2D currentBallRigidbode;
    private SpringJoint2D currentBallSprintJoint;
    
    private Camera mainCamera;

    private bool isDraggin;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigidbode == null)
        {
            return;
        }
        
        if (Touch.activeTouches.Count == 0)
        {
            if (isDraggin)
            {
                LauncBall();
            }

            isDraggin = false;
            return;
        }

        isDraggin = true;
        
        currentBallRigidbode.isKinematic = true;

        Vector2 touchPositions = new Vector2();

        foreach (Touch touch in Touch.activeTouches)
        {
            touchPositions += touch.screenPosition;
        }

        touchPositions /= Touch.activeTouches.Count;
        
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        

        Vector3 worldPosition =  mainCamera.ScreenToWorldPoint(touchPosition);

        currentBallRigidbode.position = worldPosition;
        
        
        
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance =  Instantiate(ballPrefab, pivot.position, quaternion.identity);
        currentBallRigidbode = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSprintJoint = ballInstance.GetComponent<SpringJoint2D>();
        currentBallSprintJoint.connectedBody = pivot;
    }

    private void LauncBall()
    {
        currentBallRigidbode.isKinematic = false;
        currentBallRigidbode = null;
        Invoke(nameof(DetachBall), detachDelay);
        
    }

    void DetachBall()
    {
        currentBallSprintJoint.enabled = false;
                currentBallSprintJoint = null;
                
        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}
