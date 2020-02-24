using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    private Tilemap tilemap = null;
    public static CameraFollow Instance;
    public float xMax, xMin, yMax, yMin;
    public float damping = 1;
    public float lookAheadFactor = 3;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;
    public float CameraZ = 0;
    private float m_OffsetZ;
    private Vector3 m_LastTargetPosition;
    private Vector3 m_CurrentVelocity;
    private Vector3 m_LookAheadPos;

    void Start()
    {
        Instance = this;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        tilemap.ResizeBounds();

        SetLimits();

        m_LastTargetPosition = target.position;
        m_OffsetZ = (transform.position - target.position).z;
    }

    private void FixedUpdate()
    {

        UpdateCamera();
        

    }
    public void ResetCamera()
    {
        StartCoroutine(resetCamera());
    }
    private IEnumerator resetCamera()
    {
        transform.position = new Vector3(target.position.x, target.position.y, CameraZ);
        yield return new WaitForEndOfFrame();
        transform.position = new Vector3( target.position.x, target.position.y,  CameraZ);
        m_OffsetZ = 0;
     m_LastTargetPosition = target.position;
     m_CurrentVelocity = Vector3.zero;
     m_LookAheadPos = Vector3.zero;
        transform.position = new Vector3(target.position.x, target.position.y, CameraZ);
        yield return null;
    }
    public void UpdateCamera()
    {
        Vector3 MoveDelta = (target.position - m_LastTargetPosition);

        bool updateLookAheadTarget = Mathf.Abs(MoveDelta.magnitude) > lookAheadMoveThreshold;

        if (updateLookAheadTarget)
        {
            m_LookAheadPos = lookAheadFactor * MoveDelta;
        }
        else
        {
            m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
        }

        Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);


        transform.position = new Vector3(Mathf.Clamp(newPos.x, xMin, xMax), Mathf.Clamp(newPos.y, yMin, yMax), transform.position.z);
        m_LastTargetPosition = target.position;
    }
    public void SetLimits()
    {
        Camera cam = Camera.main;

        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        xMin = tilemap.CellToWorld(tilemap.cellBounds.min).x + width / 2;
        xMax = tilemap.CellToWorld(tilemap.cellBounds.max).x - width / 2;

        yMin = tilemap.CellToWorld(tilemap.cellBounds.min).y + height / 2;
        yMax = tilemap.CellToWorld(tilemap.cellBounds.max).y - height / 2;
    }

    // Update is called once per frame
}
