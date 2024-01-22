using UnityEngine;

public class Level : MonoBehaviour
{
    public bool active;
    public Vector3 startingPlayerPos;
    public Vector3 startingPlayerRot;

    
    public virtual void startLevel() {
        active = true;
        startingPlayerPos = PlayerManager.instance.transform.position;
        startingPlayerRot = PlayerManager.instance.transform.eulerAngles;
    }

    public virtual void onLevelEnd() {
        active = false;
    }

    public virtual bool levelCompleted() {
        return false;
    }

    public virtual void restartLevel() {
        resetPlayerPos();
    }

    void resetPlayerPos() {
        PlayerManager.instance.transform.position = startingPlayerPos;
        PlayerManager.instance.transform.eulerAngles = startingPlayerRot;
    }

}
