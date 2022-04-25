using UnityEngine;

public static class GyroUtils
{
    /**
     * Checks wether the the phone is laying flat down with the screen up and with a given threshold.<br/>
     * The threadshold defaults to 0.05;
     */
    public static bool IsLayingFlatUp(Quaternion q, float threashold = 0.05f)
    {
        return (q.x <= threashold && q.x >= -threashold) &&
            (q.y <= threashold && q.y >= -threashold);
    }
}
