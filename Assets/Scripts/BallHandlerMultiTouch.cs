using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandlerMultiTouch : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay;
    [SerializeField] private float respawnDelay;
    

    private Rigidbody2D currentBallRigidBody;
    private SpringJoint2D currentBallSprintJoint;
    private Camera mainCamera;
    private bool isDragging;
    
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
    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame 
    void Update()
    {
        if(currentBallRigidBody == null){return;}
        
        if(Touch.activeTouches.Count == 0)
        {
            if(isDragging)
            {
                LaunchBall();
            }
            isDragging = false;

            return;
        } //bir yere  basılmadığında kod çalışmasın diye
        
        isDragging = true;
        currentBallRigidBody.isKinematic = true;  //topu tutunca yer çekimi fln etkilenmemesi için kinematike çevirme. (normalde dinamik)
        
        Vector2 touchPositions = new Vector2();
        
        foreach(Touch touch in Touch.activeTouches)
        {
            touchPositions += touch.screenPosition;
        }

        touchPositions /= Touch.activeTouches.Count;
        
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPositions);  //pixel konummunu oyun konumuna dönüştürmek için.
        currentBallRigidBody.position = worldPosition;
        
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity); //spawn komutu
        currentBallRigidBody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSprintJoint = ballInstance.GetComponent<SpringJoint2D>();
        currentBallSprintJoint.connectedBody = pivot;
    }

    private void LaunchBall()
    {
        currentBallRigidBody.isKinematic = false; //topu tutmadığımızda ilk hal dinamikte kalması için
        currentBallRigidBody = null;

        Invoke(nameof(DetachBall),detachDelay);

    }

    private void DetachBall()
    {
        currentBallSprintJoint.enabled = false;
        currentBallSprintJoint = null;

        Invoke(nameof(SpawnNewBall),respawnDelay);
    }
}
