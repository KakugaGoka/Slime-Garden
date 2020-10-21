using UnityEngine;
using System.Collections;

public enum OpenableState {
    Closed,
    Open
}

[RequireComponent(typeof(InteractController))]
public class InteractOpen : MonoBehaviour {
    [Header("Parameters")]
    [Tooltip("The Object that should actually move during the open animation")]
    public GameObject objectToMove;
    [Tooltip("The Opened State Transform")]
    public Transform openPosition;
    [Tooltip("The Closed State Transform")]
    public Transform closedPosition;
    [Tooltip("If the door can be closed or not")]
    public bool canBeClosed = true;
    [Tooltip("Message to be displayed when the Openable cannot be closed and is open")]
    public string cannotCloseMessage = "";
    [Tooltip("The Speed at which the openable opens/closes")]
    [Range(100, 1000)]
    public float speed = 350f;

    [Header("Loot Spawn")]
    [Tooltip("The time between spwaning each item in the list (If they spawn at teh same time the colliders will cause problems)")]
    public float timeBetweenSpawns = 0.1f;
    [Tooltip("The list of items to spawn on open")]
    public GameObject[] spawnItemList;

    bool m_SpawnItems = false;
    bool m_IsOpen = false;
    int m_SpawnCount = 0;
    float m_WaitedTime;
    Vector3 m_PosVelocity = Vector3.zero;
    Vector3 m_RotVelocity = Vector3.zero;
    InteractController m_InteractController;
    OpenableState m_CurrentOpenableState;

    private void Start() {
        m_InteractController = GetComponent<InteractController>();

        m_CurrentOpenableState = OpenableState.Closed;
        if (!objectToMove) { objectToMove = gameObject; }

        m_WaitedTime = timeBetweenSpawns;

        m_InteractController.onInteract += OnInteract;
    }

    private void LateUpdate() {
        if (m_IsOpen && !m_SpawnItems) {
            m_SpawnItems = true;
        }

        if (m_SpawnCount < spawnItemList.Length && m_SpawnItems) {
            m_WaitedTime += Time.deltaTime;
            if (m_WaitedTime >= timeBetweenSpawns && spawnItemList[m_SpawnCount]) {
                Vector3 targetPos = transform.position + new Vector3(0f, 0.5f, 0f);
                GameObject newPickup = Instantiate(spawnItemList[m_SpawnCount], targetPos, transform.rotation);
                float angle = Random.Range(135.0f, 225.0f);
                newPickup.transform.Rotate(new Vector3(0f, newPickup.transform.rotation.y + angle, 0f));
                Rigidbody body = newPickup.GetComponent<Rigidbody>();
                if (body) {
                    body.AddForce((newPickup.transform.forward * 2f) + (newPickup.transform.up * 5f), ForceMode.Impulse);
                }
                m_WaitedTime = 0f;
                m_SpawnCount++;
            }
        }

        if (!canBeClosed && m_CurrentOpenableState == OpenableState.Open && m_InteractController.interactionMessage != cannotCloseMessage) {
            m_InteractController.interactionMessage = cannotCloseMessage;
            return;
        }

        if (m_CurrentOpenableState == OpenableState.Closed && !objectToMove.transform.Equals(closedPosition)) {
            objectToMove.transform.MoveTo(closedPosition, ref m_PosVelocity, ref m_RotVelocity, speed);
        } else if (m_CurrentOpenableState == OpenableState.Open && !objectToMove.transform.Equals(openPosition)) {
            objectToMove.transform.MoveTo(openPosition, ref m_PosVelocity, ref m_RotVelocity, speed);
        }
    }

    void OnInteract(PlayerCharacterController player) {
        if (m_CurrentOpenableState == OpenableState.Open && canBeClosed) {
            m_CurrentOpenableState = OpenableState.Closed;
        } else if (m_CurrentOpenableState == OpenableState.Closed) {
            m_CurrentOpenableState = OpenableState.Open;
        }
    }

    //private IEnumerator OpenThisOpenable() {
    //    while (!objectToMove.transform.Equals(openPosition)) {
    //        if (m_CurrentOpenableState == OpenableState.Closed) { yield break; }
    //        objectToMove.transform.MoveTo(openPosition, ref m_PosVelocity, ref m_RotVelocity, speed, ref m_IsOpen);
    //        if (m_IsOpen) { yield break; }
    //        yield return null;
    //    }
    //    yield break;
    //}

    //private IEnumerator CloseThisOpenable() {
    //    while (!objectToMove.transform.Equals(closedPosition)) {
    //        if (m_CurrentOpenableState == OpenableState.Open) { yield break; }
    //        objectToMove.transform.MoveTo(closedPosition, ref m_PosVelocity, ref m_RotVelocity, speed, ref m_IsOpen);
    //        if (!m_IsOpen) { yield break; }
    //        yield return null;
    //    }
    //    yield break;
    //}
}
