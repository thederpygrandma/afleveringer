public struct SensorData
{
    public SensorData(long miliseconds, float x, float y, float z)
    {
        Miliseconds = miliseconds;
        X = x;
        Y = y;
        Z = z;
    }

    public long Miliseconds { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}
