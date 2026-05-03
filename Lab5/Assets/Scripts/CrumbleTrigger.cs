using UnityEngine;

public class CrumbleTrigger : MonoBehaviour
{
    public CrumblingPlatform platform;

    void OnTriggerEnter(Collider other)
    {
        if (platform == null) return;
        if (other.GetComponent<CharacterController>() != null)
            platform.StartCrumbling();
    }
}
