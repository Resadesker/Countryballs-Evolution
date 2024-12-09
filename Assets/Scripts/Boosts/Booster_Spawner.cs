using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Booster_Spawner : MonoBehaviour
{
    [SerializeField] private Booster chest;
    [SerializeField] private Booster clock;
    [SerializeField] private Booster ship;
    [SerializeField] private Booster car;
    [SerializeField] private Booster plane;

    [SerializeField] private Sun sun;
    [SerializeField] private GameObject supersun;
    [SerializeField] private RectTransform canvasRect;  // Reference to the Canvas RectTransform

    [SerializeField] private Image mapBackground;
    [SerializeField] private Sprite mapWithRoad;

    [SerializeField] private SoundEmitter boosterCollectedSoundEmitter; // passed to each booster
    [SerializeField] private SoundEmitter clockTickingSoundEmitter; // passed to clock

    public static Booster_Spawner Instance { get; private set; }

    private float marginPercentage = 0.05f; // 5% margin from the edges

    private int shipsLevel = -1;
    private int roadsLevel = -1;
    private int planesLevel = -1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(SpawnChest());
        StartCoroutine(SpawnClock());
        StartCoroutine(SpawnSuns());

        StartCoroutine(SpawnCar());
        StartCoroutine(SpawnShip());
        StartCoroutine(SpawnPlane());
    }

    public void EnableRoad(int level)
    {
        roadsLevel = level;
        mapBackground.sprite = mapWithRoad;
    }

    public void EnablePlane(int level)
    {
        planesLevel = level;
    }
    
    public void EnableShips(int level)
    {
        shipsLevel = level;
    }
    
    private IEnumerator SpawnSuns()
    {
        // Wait for a random amount of time before spawning the next sun
        yield return new WaitForSeconds(Random.Range(2, 16));

        // Check if the level is not zero
        if (AgeManager.Instance.GetLevel() != 0)
        {
            // Calculate the canvas width and the margins
            float canvasWidth = canvasRect.rect.width;
            float margin = canvasWidth * marginPercentage;

            // Determine the left and right boundaries for spawning
            float leftBoundary = -canvasWidth / 2 + margin;
            float rightBoundary = canvasWidth / 2 - margin;

            // Generate a random X position within the boundaries
            float randomX = Random.Range(leftBoundary, rightBoundary);

            // Set the Y position just above the top of the canvas
            float spawnY = canvasRect.rect.height / 1.9f;

            // Create a new sun instance
            Sun sunInstance = Instantiate(sun, canvasRect);
            sunInstance.soundEmitter = boosterCollectedSoundEmitter;

            // Set the position of the new sun instance
            RectTransform sunRectTransform = sunInstance.GetComponent<RectTransform>();
            sunInstance.transform.localScale = sun.transform.localScale;
            sunRectTransform.anchoredPosition = new Vector3(randomX, spawnY, 0);
        }

        // Start the coroutine again for continuous spawning
        StartCoroutine(SpawnSuns());
    }

    private IEnumerator SpawnShip()
    {
        yield return new WaitForSeconds(Random.Range(2f, 50));
        if (shipsLevel != -1)
        {
            Booster clockInstance = Instantiate(ship);
            clockInstance.transform.parent = transform;
            clockInstance.transform.localScale = new Vector3(ship.transform.localScale.x*1.3f, ship.transform.localScale.y*1.3f, ship.transform.localScale.z*1.3f);
            clockInstance.SetLevel(shipsLevel);
            clockInstance.soundEmitter = boosterCollectedSoundEmitter;
        }

        StartCoroutine(SpawnShip());
    }
    
    private IEnumerator SpawnPlane()
    {
        yield return new WaitForSeconds(Random.Range(15, 60));
        if (planesLevel != -1)
        {
            Booster clockInstance = Instantiate(plane);
            clockInstance.transform.parent = transform;
            clockInstance.transform.localScale = plane.transform.localScale;
            clockInstance.SetLevel(planesLevel);
            clockInstance.soundEmitter = boosterCollectedSoundEmitter;
        }

        StartCoroutine(SpawnPlane());
    }
    
    private IEnumerator SpawnCar()
    {
        yield return new WaitForSeconds(Random.Range(2f, 25));
        if (roadsLevel != -1)
        {
            Booster clockInstance = Instantiate(car);
            clockInstance.transform.parent = transform;
            clockInstance.transform.localScale = new Vector3(car.transform.localScale.x*1.5f, car.transform.localScale.y*1.5f, car.transform.localScale.z*1.5f);
            clockInstance.SetLevel(roadsLevel);
            clockInstance.soundEmitter = boosterCollectedSoundEmitter;
        }

        StartCoroutine(SpawnCar());
    }
    
    private IEnumerator SpawnSupersuns()
    {
        yield return new WaitForSeconds(Random.Range(5, 15));
        GameObject sunInstance = Instantiate(supersun);
        sunInstance.transform.parent = transform;
        sunInstance.transform.localPosition = new Vector3(Random.Range(-274, 274), 715, 0);
        StartCoroutine(SpawnSupersuns());
    }
    
    private IEnumerator SpawnClock()
    {
        yield return new WaitForSeconds(Random.Range(15, 181));
        Booster clockInstance = Instantiate(clock);
        clockInstance.transform.parent = transform;
        clockInstance.transform.localScale = clock.transform.localScale;
        clockInstance.soundEmitter = boosterCollectedSoundEmitter;
        clockInstance.clockTickSoundEmitter = clockTickingSoundEmitter;
        StartCoroutine(SpawnClock());
    }

    private IEnumerator SpawnChest()
    {
        yield return new WaitForSeconds(Random.Range(15, 121));
        Booster chestInstance = Instantiate(chest);
        chestInstance.transform.parent = transform;
        chestInstance.transform.localScale = chest.transform.localScale;
        chestInstance.soundEmitter = boosterCollectedSoundEmitter;
        StartCoroutine(SpawnChest());
    }
}
