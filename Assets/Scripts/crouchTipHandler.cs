using UnityEngine;

public class crouchTipHandler : MonoBehaviour
{
    bool showedTip;
    [SerializeField] GameObject tip;
    [SerializeField] float range;
    [SerializeField] Transform rangeCenter;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            hideTip();
            Destroy(this);
        }

        if (showedTip)
        {
            return;
        }

        if (Vector3.Distance(PlayerManager.instance.transform.position, rangeCenter.position) <= range)
        {
            showTip();
        }
    }

    void showTip() {
        showedTip = true;
        tip.SetActive(true);
    }
    void hideTip() {
        tip.SetActive(false);
    }
}
