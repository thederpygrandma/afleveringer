using UnityEngine;
using UnityEngine.InputSystem;


// Inspired by https://www.youtube.com/watch?v=ERAN5KBy2Gs&t=503s
[DefaultExecutionOrder(-1)]
public class SensorManager : Singleton<SensorManager>
{
    // Creates a delegate and event for the attitude event
    public delegate void AttitudeChangedEvent(Quaternion quaternion);
    public event AttitudeChangedEvent OnAttitudeChanged;

    public delegate void AccelerationChangedEvent(Vector3 quaternion);
    public event AccelerationChangedEvent OnAccelerationChanged;

    private Input inputSensors;

    private void Awake()
    {
        inputSensors = new Input();
    }

    private void OnEnable()
    {
        inputSensors.Sensors.Enable();
    }

    private void OnDisable()
    {
        inputSensors.Sensors.Disable();
    }

    void Start()
    {
        // Turns on the gyroscope and accelerometer sensors if they exist
        if (Accelerometer.current != null) InputSystem.EnableDevice(Accelerometer.current);
        if (AttitudeSensor.current != null) InputSystem.EnableDevice(AttitudeSensor.current);

        // Subsribes to the "performed" event for both the Gyroscope and Accelerometer
        inputSensors.Sensors.Acceleration.performed += AccelerationChanged;
        inputSensors.Sensors.Attitude.performed += AttitudeChanged;
    }

    private void AccelerationChanged(InputAction.CallbackContext context)
    {
        // If any method is subribed to the method, send the acceleration data
        if (OnAccelerationChanged != null) OnAccelerationChanged(context.ReadValue<Vector3>());
    }

    private void AttitudeChanged(InputAction.CallbackContext context)
    {
        if (OnAttitudeChanged != null) OnAttitudeChanged(context.ReadValue<Quaternion>());
    }
}
