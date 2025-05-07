using NUnit.Framework;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
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
        if (Input.GetMouseButton(1))
        {
            float rotHoriz = Input.GetAxis("Mouse X") * sensitivity;
            float rotVert = Input.GetAxis("Mouse Y") * sensitivity;

            transform.Rotate(new Vector3(-rotVert, 0, 0));
            transform.Rotate(new Vector3(0, rotHoriz, 0), Space.World);
        }

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetAxis("Depth"));
        transform.Translate(movement * speed * Time.deltaTime);
    }
}
