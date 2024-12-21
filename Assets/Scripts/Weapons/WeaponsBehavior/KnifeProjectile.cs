using UnityEngine;

public class KnifeProjectile : Projectile
{
    private Transform playerTransform; // V? tr� c?a ng??i ch?i
    private float rotationAngle; // G�c quay hi?n t?i
    private float orbitRadius; // B�n k�nh quay dao
    private float orbitSpeed; // T?c ?? quay quanh ng??i ch?i
    public float selfRotationSpeed = 200f; // T?c ?? t? xoay c?a dao

    public void SetOrbit(Transform player, float angle, float radius, float speed)
    {
        playerTransform = player; // L?y transform ng??i ch?i ?? dao theo d�i
        rotationAngle = angle; // G�c quay ban ??u
        orbitRadius = radius; // B�n k�nh qu? ??o
        orbitSpeed = speed; // T?c ?? quay quanh ng??i ch?i
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Quay quanh ng??i ch?i
        rotationAngle += orbitSpeed * Time.deltaTime;
        rotationAngle %= 360f; // Gi? g�c trong kho?ng [0, 360]

        float radians = rotationAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * orbitRadius;

        transform.position = playerTransform.position + offset;

        // T? xoay quanh tr?c z
        transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + (selfRotationSpeed * Time.deltaTime));
    }
}
