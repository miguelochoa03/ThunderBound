using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private float maxSpeed = -5f;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        FollowMousePositionDelayed(maxSpeed);
    }
    private void FollowMousePosition()
    {
        transform.position = GetWorldPositionFromMouse();
    }
    private void FollowMousePositionDelayed(float maxSpeed)
    {
        //transform.position = Vector2.MoveTowards(transform.position, GetWorldPositionFromMouse(), maxSpeed * Time.deltaTime);
        // Only update the X position
        float newX = Mathf.MoveTowards(transform.position.x, GetWorldPositionFromMouse().x, maxSpeed);
        transform.position = new Vector2(newX, transform.position.y);
    }
    private Vector2 GetWorldPositionFromMouse()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
}
