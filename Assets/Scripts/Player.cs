using NUnit.Framework;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class Player : MonoBehaviour
{
    [SerializeField] public List<Unit> army;

    [SerializeField] private float speed = 20f;
    [SerializeField] private float sensitivity = 10f;

    private void Start()
    {

    }

    private void Update()
    {
        CameraMovement();
    }

    private void CameraMovement()
    {
        if(Input.GetMouseButton(1))
        {
            float rotHoriz = Input.GetAxis("Mouse X") * sensitivity;
            float rotVert = Input.GetAxis("Mouse Y") * sensitivity;

            transform.Rotate(new Vector3(-rotVert, 0, 0));
            transform.Rotate(new Vector3(0, rotHoriz, 0), Space.World);
        }

        float horiz = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float vert = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float depth = Input.GetAxis("Depth") * speed * Time.deltaTime;
        Vector3 movement;

        movement = new Vector3(horiz, vert, depth);

        transform.position += movement;
    }
}
