using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    private SensorManager _sensorManager;

    [SerializeField]
    private Button _startButton;
    [SerializeField]
    private TextMeshProUGUI _startButtonText;
    [SerializeField]
    private TextMeshProUGUI _sensorDataRotation;
    [SerializeField]
    private TextMeshProUGUI _sensorDataAcceleration;
    [SerializeField]
    private TextMeshProUGUI _sensorStates;
    [SerializeField]
    private GameObject _successImage;

    private bool isShowingMessage = false;

    private bool isWritingData = false;

    private int dataLimit = 2000;

    private int totalWrittenData = 0;
    
    private Quaternion attitude;
    
    private Vector3 accelaration;

    private long startTime;

    private CSVWriter fileWriter;

    private void Awake()
    {
        _sensorManager = SensorManager.Instance;
    }

    void Start()
    {
        Time.fixedDeltaTime = 0.002f; // Set the update time for the FixedUpdate loop to 0.002 seconds between calls (500 calls/second)
        _startButton.onClick.AddListener(StartWritingData); // Add a listener to the start button

        _sensorManager.OnAttitudeChanged += UpdateAttitude; // Subscribes to the OnAttitudeChanged event
        _sensorManager.OnAccelerationChanged += UpdateAcceleration;
    }

    private void OnDisable()
    {
        _sensorManager.OnAttitudeChanged -= UpdateAttitude; // Unsubscribes to the OnAttitudeChanged event to prevent memory leak
        _sensorManager.OnAccelerationChanged -= UpdateAcceleration;
    }

    private void FixedUpdate()
    {
        
        if(isWritingData || totalWrittenData > dataLimit)
        {
            SensorData data = new SensorData(
                DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime,
                accelaration.x,
                accelaration.y,
                accelaration.z
            );

            fileWriter.serilize(data);
            totalWrittenData++;
        }
    }
    /**
     * <summary>
     * Makes the necessary checks for the attitude values
     * from the Gyroscope sensor.
     * </summary> 
     */
    private void UpdateAttitude(Quaternion q)
    {
        this.attitude = q;

        if (isWritingData && GyroUtils.IsLayingFlatUp(attitude))
        {
            StopWritingData();
        }

        _sensorDataRotation.text = $"Gyroscope:" + Environment.NewLine +
                                  $"X: {q.x}" + Environment.NewLine +
                                  $"Y: {q.y}" + Environment.NewLine +
                                  $"Z: {q.z}";
    }
    /**
     * Makes the necessary checks for the acceleration values from the accelerometer sensor.
     */
    private void UpdateAcceleration(Vector3 v)
    {
        this.accelaration = v;
        _sensorDataAcceleration.text = $"Accelerometer:" + Environment.NewLine +
                                      $"X: {v.x}" + Environment.NewLine +
                                      $"Y: {v.y}" + Environment.NewLine +
                                      $"Z: {v.z}";
    }
    /**
     * Setting everything up to write the data. The method will not write start writing the data if the
     * phone is already lying down or if a message is still in view.
     */
    private void StartWritingData()
    {
        if (isShowingMessage) return;
        if(GyroUtils.IsLayingFlatUp(attitude))
        {
            StartCoroutine(AlreadyLayingDownMessage());
            return;
        }
        fileWriter = new CSVWriter(DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
        startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        isWritingData = true;
        _startButtonText.text = "Reading...";
        _startButton.image.color = Color.yellow;
    }

    /**
     * Stop the writing process and emptys everything that needs to be emptied 
     */
    private void StopWritingData()
    {
        StartCoroutine(ShowSuccessMessage());
        isWritingData = false;
        totalWrittenData = 0;
        fileWriter.writer.Close(); // Closes the writer object to prevent memory leaks
    }

    /**
     * Shows feedback about the phone already being laid down
     */
    IEnumerator AlreadyLayingDownMessage()
    {
        int amountOfSecondsToDisplay = 2;
        isShowingMessage = true;
        _startButtonText.text = "Already laying down";
        _startButton.image.color = Color.red;

        yield return new WaitForSeconds(amountOfSecondsToDisplay);

        _startButtonText.text = "Start";
        _startButton.image.color = Color.green;
        isShowingMessage = false;
    }

    /**
     * Shows feedback about the successful data writing including the amount of data written
     */
    IEnumerator ShowSuccessMessage()
    {
        int amountOfSecondsToDisplay = 5;
        isShowingMessage = true; 
        _startButtonText.text = "Wrote: " + totalWrittenData.ToString();
        _successImage.SetActive(true);

        yield return new WaitForSeconds(amountOfSecondsToDisplay);

        _startButtonText.text = "Start";
        _startButton.image.color = Color.green;
        _successImage.SetActive(false);
        isShowingMessage = false;
    }
}
