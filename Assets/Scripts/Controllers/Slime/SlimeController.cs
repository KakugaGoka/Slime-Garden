using UnityEngine;
using UnityEngine.AI;
using static Unity.Mathematics.math;
using static Mathk;
using System;
using System.Collections;

[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof( SphereCollider ) )]
[RequireComponent( typeof( MeshRenderer ) )]
[Serializable]
public class SlimeController : MainController
{
    private Rigidbody rb;
    private SphereCollider sphereCollider;

    private float maxJumpForce;
    private GameObject player;
    private NavMeshPath navPath;
    private Vector3[] path;

    private Vector3 targetPosition;
    private Vector3 pathTarget;

    [SerializeField]
    public float hunger;

    public float HungerLimit = 30;
    private bool hungry { get { return hunger > HungerLimit; } }
    private float pathDist = 0;
    private float goalDist = 0;

    [SerializeField]
    public float hopping;

    public float jumpTimer = 5;
    public float wanderRange = 5;
    public float jumpForce = 100;

    [SerializeField]
    public float rolling;

    [SerializeField]
    public float floating;

    [SerializeField]
    public float range;

    public bool npc = false;

    private float lastHopTimer;

    private bool grounded {
        get {
            bool g = Physics.Raycast( gameObject.Pos(), Vector3.down, out RaycastHit hit, 0.6f, -1 );
            groundDist = hit.distance;
            return g;
        }
    }

    private float groundDist;

    private MeshRenderer m_Renderer;
    [Serializable]
    public struct Faces {
        public Texture2D happy;
        public Texture2D angry;
        public Texture2D sad;
        public Texture2D hungry;
    }
    public enum CurrentFace { 
        Happy,
        Angry,
        Sad,
        Hungry,
    }
    [HideInInspector]
    public Faces faces;
    public CurrentFace currentFace = CurrentFace.Happy;
    public Color color;
    public float amplitude;
    public float frequency;
    public float brightness;
    public float spinRate;
    public float cellType;
    public float cellDensity;
    public float speckleDensity;
    public float speckleBrightness;
    public bool shaderSet = false;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        sphereCollider = gameObject.GetComponent<SphereCollider>();
        player = GameObject.FindGameObjectWithTag( "Player" );
        navPath = new NavMeshPath();
        m_Renderer = GetComponent<MeshRenderer>();
        faces.happy = Resources.Load<Texture2D>("Textures/Smiley");
        faces.angry = Resources.Load<Texture2D>("Textures/Smiley");
        faces.sad = Resources.Load<Texture2D>("Textures/Smiley");
        faces.hungry = Resources.Load<Texture2D>("Textures/Smiley");
        if (!shaderSet) {
            SlimeShaderController.ResetAllShaderValues(this);
            shaderSet = true;
        }
        SetInShader();
        ChangeFaceTexture(currentFace);
    }

    private void Update()
    {
        if (npc) {
            return;
        }
        hunger += Time.deltaTime;
        lastHopTimer -= Time.deltaTime;
        Wander();
        Navigate();
    }

    private void Navigate()
    {
        if (path != null) {
            if (path.Length > 1) {
                path[0] = ProjectToNavMesh(gameObject.Pos(), 10f);
                pathDist = Vector3.Distance(gameObject.Pos(), path[1]);
                goalDist = Vector3.Distance(gameObject.Pos(), targetPosition);
            }
        }
        if (pathDist < 2 || targetPosition == Vector3.zero) {
            if (NavMesh.CalculatePath( gameObject.Pos(), targetPosition, -1, navPath )) {
                path = navPath.corners;
            }
            ;

            if (path != null) {
                targetPosition = path[path.LastIndex()];
            }
            else {
                targetPosition = Vector3.zero;
            }
        }

        if (lastHopTimer < 0) {
            if (grounded) {
                Hop( path[1] );
            }
            lastHopTimer = jumpTimer * UnityEngine.Random.value + 0.5f;
        }
    }

    private void Wander()
    {
        if (goalDist < 1f) {
            if (hungry) {
                FindFood();
            }
            else {
                targetPosition = FindWanderPoint();
            }
        }

        Balance();
    }

    private void OnCollisionEnter( Collision collision )
    {
        for (int i = 0; i < collision.contactCount; i++) {
            if (collision.relativeVelocity.magnitude > 0.4f && collision.contacts[i].normal.y > 0.8f) {
                rb.angularVelocity *= 0f;
                break;
            }
        }
    }

    private Vector3 ProjectToNavMesh( Vector3 point, float range )
    {
        if (NavMesh.SamplePosition( point, out NavMeshHit hit, range, -1 )) { return hit.position; }
        else { return Vector3.zero; }
    }

    private Vector3 FindWanderPoint()
    {
        Vector3 point = UnityEngine.Random.insideUnitSphere * wanderRange + gameObject.Pos();

        if (NavMesh.SamplePosition( point, out NavMeshHit hit, 10, -1 )) {
            return hit.position;
        }
        return Vector3.zero;
    }

    private void Balance()
    {
        if (path != null && path.Length > 1) {
            Vector3 lookDir = gameObject.lookAt( path[1] );
            rb.AddTorque( cross( transform.forward, lookDir ) );
        }
        rb.AddTorque( cross( transform.up, Vector3.up ) );
        if (grounded) {
            float amount = saturate( dot( Vector3.up, gameObject.transform.up ) );

            rb.angularDrag = 0.05f + amount * 5f;
        }
    }

    private void Hop( Vector3 target )
    {
        Vector3 jumpDir = gameObject.lookAt( target );
        jumpDir = Vector3.ProjectOnPlane( jumpDir, Vector3.up ).normalized;
        Quaternion jumpRot = Quaternion.LookRotation( jumpDir );

        Vector3 jumpVector = normalize( float3( 0, 1, 1 ) );
        jumpVector = jumpRot * jumpVector;

        rb.AddForce( jumpVector * jumpForce );
    }

    private void FindFood()
    {
        var food = GameObject.FindGameObjectsWithTag( "Food" );
        if (food.Length > 0) {
            float[] distances = new float[food.Length];

            for (int i = 0; i < food.Length; i++) {
                distances[i] = distance( gameObject.Pos(), food[i].transform.position );
            }

            float foodDist = Min( distances );
            int index = Array.FindIndex( distances, ( x ) => x == foodDist );
            var chosen = food[index];

            if (chosen != null) {
                targetPosition = chosen.transform.position;

                if (foodDist < 0.7f) {
                    EdibleController foodComp = chosen.GetComponent<EdibleController>();
                    if (foodComp) {
                        foodComp.onEat.Invoke(this);
                    }
                }
            }
        } else {
            targetPosition = FindWanderPoint();
        }
    }

    private void OnDrawGizmos()
    {
        DebugPath( path );
    }

    private void DebugPath( Vector3[] inPath )
    {
        if (inPath != null) {
            for (int i = 0; i < inPath.Length; i++) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere( inPath[i], 0.05f );
                if (i < inPath.LastIndex()) {
                    //Gizmos.color
                    Gizmos.DrawLine( inPath[i], inPath[i + 1] );
                }
            }
        }
    }

    public void SetInShader() {
        m_Renderer.material.SetFloat("_Amplitude", amplitude); //Property Reference for Amplitude found in the shader.
        m_Renderer.material.SetFloat("_Frequency", frequency); //Property Reference for Frequency found in the shader.
        m_Renderer.material.SetFloat("_Brightness", brightness); //Property Reference for Brightness found in the shader.
        m_Renderer.material.SetFloat("_SpinRate", spinRate); //Property Reference for Spin Rate found in the shader.
        m_Renderer.material.SetFloat("_CellType", cellType); //Property Reference for Cell Type found in the shader.
        m_Renderer.material.SetFloat("_CellDensity", cellDensity); //Property Reference for Cell Density found in the shader.
        m_Renderer.material.SetFloat("_SpeckleDensity", speckleDensity); //Property Reference for Speckle Density found in the shader.
        m_Renderer.material.SetFloat("_SpeckleBrightness", speckleBrightness); //Property Reference for Speckle Brightness found in the shader.
        m_Renderer.material.SetColor("_Color", color); //Property Reference for Color found in the shader.
        ChangeFaceTexture(currentFace);
    }

    public void ChangeFaceTexture(CurrentFace face) {
        switch (face) {
            case CurrentFace.Happy:
                if (!faces.happy) { Debug.LogError("Face passed in for SlimeShaderController is null"); }
                m_Renderer.material.SetTexture("_FaceTexture", faces.happy);
                return;
            case CurrentFace.Angry:
                if (!faces.angry) { Debug.LogError("Face passed in for SlimeShaderController is null"); }
                m_Renderer.material.SetTexture("_FaceTexture", faces.angry);
                return;
            case CurrentFace.Sad:
                if (!faces.sad) { Debug.LogError("Face passed in for SlimeShaderController is null"); }
                m_Renderer.material.SetTexture("_FaceTexture", faces.sad);
                return;
            case CurrentFace.Hungry:
                if (!faces.hungry) { Debug.LogError("Face passed in for SlimeShaderController is null"); }
                m_Renderer.material.SetTexture("_FaceTexture", faces.hungry);
                return;
        }
    }
}