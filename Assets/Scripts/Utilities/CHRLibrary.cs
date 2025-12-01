using UnityEngine;

public static class CHRLibrary 
{
    
    // Right is Z, down is X according to the camera
    public static Vector3 ConvertVector(this Vector3 vector)
    {
        return new Vector3(vector.z, vector.y, vector.x);
    }
}
