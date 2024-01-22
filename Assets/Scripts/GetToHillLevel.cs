using UnityEngine;
using UnityEngine.Playables;
public class GetToHillLevel : GetToDestinationLevel
{
    [SerializeField] PlayableDirector animator;

    private void Awake()
    {
        animator.gameObject.SetActive(false);
    }
    public override bool levelCompleted()
    {
        if (completed) {
            animator.gameObject.SetActive(true);
            animator.Play();
        }
        return completed;
        
    }
}
