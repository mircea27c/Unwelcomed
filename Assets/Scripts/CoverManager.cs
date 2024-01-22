using UnityEngine;

public class CoverManager : MonoBehaviour
{
    public static CoverManager instance;

    Cover[] allCoverPoints;
    int coversCount;
    private void Awake()
    {
        instance = this;
        getAllCoverPoints();
    }

    void getAllCoverPoints() {
        allCoverPoints = FindObjectsOfType<Cover>();
        coversCount = allCoverPoints.Length;
    }

    int cov_i;
    public bool findCoverInRange(Vector3 position, float range, out Cover availableCover) {
        for (cov_i = 0; cov_i < coversCount; cov_i++) {
            if (!allCoverPoints[cov_i].busy && (position - allCoverPoints[cov_i].coverPosition).sqrMagnitude <= range * range) {
                availableCover = allCoverPoints[cov_i];
                availableCover.busy = true;
                return true;
            }
        }
        availableCover = null;
        return false;
    }
}
