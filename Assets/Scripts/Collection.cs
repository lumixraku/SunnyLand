using UnityEngine;
using System.Collections;

public class Collection : MonoBehaviour
{
    // Use this for initialization
    private AudioSource feedback;
    protected Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        //feedback = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayFeedback()
    {
        animator.SetTrigger("feedback");
    }

    private void DestroyAfterAnim()
    {
        Destroy(gameObject);
    }

}
