using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("카메라 설정")]
    [SerializeField] private float dragSpeed = 60f;
    [SerializeField] private Transform cameraTransform;
    
    [Header("이동 제한")]
    [SerializeField] private bool limitBoundaries = true;
    [SerializeField] private Vector2 minBoundary = new Vector2(-30f, -30f);
    [SerializeField] private Vector2 maxBoundary = new Vector2(30f, 30f);
    
    // 드래그 관련 변수
    private Vector3 dragOrigin;
    private Vector3 dragDifference;
    private bool isDragging = false;
    private Plane groundPlane;

    private void Start()
    {
        // 카메라가 설정되지 않았다면 현재 게임오브젝트의 카메라를 사용
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
        
        // 지면 평면 초기화 (Y=0 기준)
        groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // 이동 방향 벡터를 저장할 변수
    private Vector3 moveDirection = Vector3.zero;

    private void Update()
    {
        // 입력 처리는 Update에서 진행
        HandleDragInput();
    }
    
    private void FixedUpdate()
    {
        // 실제 카메라 이동은 FixedUpdate에서 처리
        if (moveDirection != Vector3.zero)
        {
            MoveCamera(moveDirection);
            
            // 이동 후 방향 벡터 초기화
            moveDirection = Vector3.zero;
        }
    }

    private void HandleDragInput()
    {
        // 마우스 버튼을 누를 때 드래그 시작점 설정
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            
            // 광선이 평면과 교차하는지 확인
            if (groundPlane.Raycast(ray, out entry))
            {
                // 광선과 평면의 교차점을 드래그 시작점으로 설정
                dragOrigin = ray.GetPoint(entry);
            }
        }
        
        // 마우스 버튼을 뗄 때 드래그 종료
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        
        // 드래그 중일 때 이동 방향 계산
        if (isDragging && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            
            if (groundPlane.Raycast(ray, out entry))
            {
                // 현재 광선과 평면의 교차점 계산
                Vector3 dragCurrentPosition = ray.GetPoint(entry);
                
                // 드래그 차이 계산 (시작점 - 현재점)
                dragDifference = dragCurrentPosition - dragOrigin;
                
                // 카메라가 기울어져 있을 때 월드 좌표를 기준으로 이동 방향 설정
                moveDirection = new Vector3(-dragDifference.x, 0, -dragDifference.z);
                
                // 이동 후 드래그 시작점 갱신
                dragOrigin = dragCurrentPosition;
            }
        }
    }
    
    private void MoveCamera(Vector3 direction)
    {
        // 카메라 이동 (FixedUpdate에서는 Time.fixedDeltaTime 사용)
        Vector3 newPosition = cameraTransform.position + direction * dragSpeed * Time.fixedDeltaTime;
        
        if (limitBoundaries)
        {
            // X, Z 좌표를 제한 범위 내로 제한
            newPosition.x = Mathf.Clamp(newPosition.x, minBoundary.x, maxBoundary.x);
            newPosition.z = Mathf.Clamp(newPosition.z, minBoundary.y, maxBoundary.y);
        }
        
        // 카메라 Y축 높이는 유지
        newPosition.y = cameraTransform.position.y;
        
        // 최종 위치로 카메라 이동
        cameraTransform.position = newPosition;
    }
    
    // 기즈모로 이동 제한 범위 시각화 (에디터에서만 표시)
    private void OnDrawGizmosSelected()
    {
        if (limitBoundaries)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3(
                (minBoundary.x + maxBoundary.x) * 0.5f,
                0,
                (minBoundary.y + maxBoundary.y) * 0.5f
            );
            Vector3 size = new Vector3(
                maxBoundary.x - minBoundary.x,
                0.1f,
                maxBoundary.y - minBoundary.y
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}