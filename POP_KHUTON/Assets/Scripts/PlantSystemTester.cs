using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantSystemTester : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PlantsManager plantsManager;
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private Transform testResultsPanel;
    [SerializeField] private TextMeshProUGUI logText;
    
    [Header("테스트 설정")]
    [SerializeField] private int numberOfPlantsToSpawn = 10;
    [SerializeField] private float spawnDelay = 0.5f;
    [SerializeField] private bool automaticallyRunTests = false;
    
    [Header("UI")]
    [SerializeField] private Button runTestButton;
    [SerializeField] private Button clearLogButton;
    
    private void Start()
    {
        // UI 버튼 이벤트 연결
        if (runTestButton != null)
            runTestButton.onClick.AddListener(RunAllTests);
            
        if (clearLogButton != null)
            clearLogButton.onClick.AddListener(ClearLog);
            
        // 참조 확인
        ValidateReferences();
        
        // 자동 테스트 실행
        if (automaticallyRunTests)
            StartCoroutine(RunAllTestsWithDelay(1.0f));
    }
    
    private void ValidateReferences()
    {
        if (plantsManager == null)
        {
            plantsManager = FindObjectOfType<PlantsManager>();
            if (plantsManager == null)
            {
                LogMessage("오류: PlantsManager를 찾을 수 없습니다!", Color.red);
                return;
            }
        }
        
        if (plantPrefab == null)
        {
            LogMessage("오류: Plant 프리팹이 설정되지 않았습니다!", Color.red);
        }
        
        // Farm 컴포넌트 확인
        Farm farm = plantsManager.GetComponent<Farm>();
        if (farm == null)
        {
            LogMessage("오류: PlantsManager에 연결된 Farm 컴포넌트를 찾을 수 없습니다!", Color.red);
        }
        else
        {
            LogMessage($"Farm 크기 확인: {farm.farmWidth} x {farm.farmBreadth}", Color.green);
        }
    }
    
    private IEnumerator RunAllTestsWithDelay(float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);
        RunAllTests();
    }
    
    public void RunAllTests()
    {
        LogMessage("===== 식물 시스템 테스트 시작 =====", Color.cyan);
        StartCoroutine(RunTestSequence());
    }
    
    private IEnumerator RunTestSequence()
    {
        yield return StartCoroutine(TestPlantCreation());
        yield return StartCoroutine(TestGridFilling());
        yield return StartCoroutine(TestExceptionHandling());
        
        LogMessage("===== 모든 테스트 완료 =====", Color.cyan);
    }
    
    private IEnumerator TestPlantCreation()
    {
        LogMessage("테스트 1: 식물 생성 및 이벤트 시스템 테스트", Color.yellow);
        
        if (plantPrefab == null)
        {
            LogMessage("테스트 실패: Plant 프리팹이 없습니다.", Color.red);
            yield break;
        }
        
        // 단일 식물 생성 테스트
        LogMessage("단일 식물 생성 중...", Color.white);
        GameObject plantObj = Instantiate(plantPrefab, Vector3.zero, Quaternion.identity);
        
        // 이벤트가 작동하여 Plant가 그리드에 추가되었는지 확인하기 위한 시간
        yield return new WaitForSeconds(0.5f);
        
        // 식물 컴포넌트 확인
        Plant plant = plantObj.GetComponent<Plant>();
        if (plant == null)
        {
            LogMessage("테스트 실패: 생성된 객체에 Plant 컴포넌트가 없습니다.", Color.red);
        }
        else
        {
            LogMessage($"Plant 생성 확인", Color.green);
        }
        
        yield return new WaitForSeconds(spawnDelay);
    }
    
    private IEnumerator TestGridFilling()
    {
        LogMessage("테스트 2: 그리드 채우기 테스트", Color.yellow);
        
        if (plantPrefab == null)
        {
            LogMessage("테스트 실패: Plant 프리팹이 없습니다.", Color.red);
            yield break;
        }
        
        LogMessage($"{numberOfPlantsToSpawn}개의 식물을 순차적으로 생성합니다...", Color.white);
        
        // 여러 식물 생성
        for (int i = 0; i < numberOfPlantsToSpawn; i++)
        {
            GameObject plantObj = Instantiate(plantPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Plant plant = plantObj.GetComponent<Plant>();
            
            if (plant != null)
            {
                // 식별을 위해 이름 설정
                plant.name = $"Plant_{i}";
                if (plant.TryGetComponent(out PlantInfo plantInfo))
                {
                    // 테스트를 위한 모니터링
                    StartCoroutine(MonitorPlantPosition(plant, plantInfo, i));
                }
            }
            
            yield return new WaitForSeconds(spawnDelay);
        }
        
        LogMessage("그리드 채우기 테스트 완료", Color.green);
        yield return new WaitForSeconds(1.0f);
    }
    
    private IEnumerator MonitorPlantPosition(Plant plant, PlantInfo plantInfo, int index)
    {
        // 식물이 그리드에 배치될 때까지 짧게 대기
        yield return new WaitForSeconds(0.2f);
        
        // 위치 확인
        if (plantInfo != null)
        {
            LogMessage($"Plant_{index} 위치: 그리드 ({plantInfo.currentCoordinate.x}, {plantInfo.currentCoordinate.y}), 월드 좌표 {plant.transform.position}", Color.white);
        }
    }
    
    private IEnumerator TestExceptionHandling()
    {
        LogMessage("테스트 3: 예외 처리 테스트", Color.yellow);
        
        Farm farm = plantsManager.GetComponent<Farm>();
        if (farm == null)
        {
            LogMessage("테스트 실패: Farm 컴포넌트를 찾을 수 없습니다.", Color.red);
            yield break;
        }
        
        // 그리드 용량 계산
        int totalCapacity = (int)(farm.farmWidth * farm.farmBreadth);
        LogMessage($"그리드 총 용량: {totalCapacity}칸", Color.white);
        
        // 용량을 초과하는 식물 생성 시도
        int additionalPlants = totalCapacity + 2;
        LogMessage($"용량을 초과하는 {additionalPlants}개의 추가 식물을 생성합니다...", Color.white);
        
        for (int i = 0; i < additionalPlants; i++)
        {
            GameObject plantObj = Instantiate(plantPrefab, Vector3.zero, Quaternion.identity);
            plantObj.name = $"ExcessPlant_{i}";
            
            yield return new WaitForSeconds(0.2f);
        }
        
        // 잠시 기다린 후 결과 확인
        yield return new WaitForSeconds(1.0f);
        LogMessage("예외 처리 테스트 완료. 로그에서 예외 메시지를 확인하세요.", Color.green);
    }
    
    private void LogMessage(string message, Color color)
    {
        Debug.Log(message);
        
        // UI 텍스트에 로그 추가 (있는 경우)
        if (logText != null)
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(color);
            logText.text += $"\n<color=#{colorHex}>{message}</color>";
            
            // 자동 스크롤 (Canvas의 ScrollRect 컴포넌트가 있는 경우)
            Canvas.ForceUpdateCanvases();
            if (testResultsPanel != null && testResultsPanel.TryGetComponent(out ScrollRect scrollRect))
            {
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }
    }
    
    public void ClearLog()
    {
        if (logText != null)
        {
            logText.text = "";
        }
    }
}