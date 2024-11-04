using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public float turnSpeed = 100.0f;
    [HideInInspector]
    public float actualSpeed = 0.0f;
    float horizontalInput;
    float verticalInput;

    void Update()
    {
        // Captura os inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        // Move o personagem para frente e para trás
        transform.Translate(Vector3.forward * Time.deltaTime * speed * verticalInput);
        // Rotaciona o personagem
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
        // Armazena a velocidade atual cálculos posteriores
        actualSpeed = speed * verticalInput * Time.deltaTime;
    }
}
