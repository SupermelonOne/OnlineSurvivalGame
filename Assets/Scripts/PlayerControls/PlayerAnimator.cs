using System;
using System.Runtime.CompilerServices;
//using Unity.Services.Multiplay.Authoring.Core.MultiplayApi;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private Animator animator;

    [SerializeField] private Vector2 direction;
    [SerializeField] private float speed;

    private void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }
    private void FixedUpdate()
    {
        if (playerBody == null)
            return;

        if (direction != Vector2.zero)
            playerBody.forward = Vector3.Slerp(playerBody.forward, new Vector3(direction.x, 0, direction.y), 10 * Time.deltaTime);

        if (animator == null)
            return;
        animator.SetFloat("MoveSpeed", speed);
    }

    public void SetVariables(Vector2 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
    }

    internal void Attack(bool value)
    {
        animator.SetBool("Attacking", value);
    }
}
