using UnityEngine;

public class PeddleSpeed : MonoBehaviour
{
    private Animator animator;
    private CanoeMovement canoeMovement;

    public enum Direction
    {
        Left,
        Right
    }
    public Direction direction;

    public float speed;


    void Start()
    {
        animator = GetComponent<Animator>();
        canoeMovement = GetComponentInParent<CanoeMovement>();
        
    }
    void Update()
    {
        if (animator != null)
        {
            if (direction == Direction.Left)
            {
                animator.SetFloat("speed", canoeMovement.leftAnimation);
            }
            else if (direction == Direction.Right)
            {
                animator.SetFloat("speed", canoeMovement.rightAnimation);
            }
        }

        
    }
}
