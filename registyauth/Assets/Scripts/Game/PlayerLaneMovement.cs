using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLaneMovement : MonoBehaviour
{
    [Header("Posiciones de los carriles (arrastra los 3 marcadores vacíos)")]
    public Transform[] lanePositions = new Transform[3]; // 0 = carril 1 (J), 1 = carril 2 (K), 2 = carril 3 (L)

    [Header("Movimiento")]
    public float moveSpeed = 15f;   // velocidad de desplazamiento entre carriles
    public bool moveOnXAxis = true; // true = carriles en horizontal (mueve X), false = carriles en vertical (mueve Y)

    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return; // no hay teclado detectado

        if (keyboard.jKey.wasPressedThisFrame) SetLane(0);
        if (keyboard.kKey.wasPressedThisFrame) SetLane(1);
        if (keyboard.lKey.wasPressedThisFrame) SetLane(2);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void SetLane(int index)
    {
        if (lanePositions == null || index < 0 || index >= lanePositions.Length || lanePositions[index] == null)
            return;

        if (moveOnXAxis)
            targetPosition = new Vector3(lanePositions[index].position.x, transform.position.y, transform.position.z);
        else
            targetPosition = new Vector3(transform.position.x, lanePositions[index].position.y, transform.position.z);
    }
}