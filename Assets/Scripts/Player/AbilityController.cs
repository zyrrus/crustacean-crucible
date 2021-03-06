using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClawBrawl
{
    public class AbilityController : MonoBehaviour
    {
        private Rigidbody rb;
        private PlayerMovement player;
        [SerializeField] private WeaponController weapon;

        [Header("Dash")]
        [SerializeField] private float dashStrength;
        [SerializeField] private float dashCooldown;
        private float dashTimer;
        private bool canDash = true;
        [SerializeField] private IconCooldown dashIcon;

        [Header("Throw")]
        [SerializeField] private float throwCooldown;
        [SerializeField] private float throwTimer;
        public bool doneThrowing { get; private set; }
        [SerializeField] private IconCooldown throwIcon;

        [Header("Spin")]
        [SerializeField] private float spinSpeed;
        [SerializeField] private float spinDuration;
        [SerializeField] private float spinCooldown;
        private float spinTimer;
        private bool isSpinning;
        [SerializeField] private IconCooldown spinIcon;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            player = GetComponent<PlayerMovement>();
        }

        private void Start()
        {
            doneThrowing = true;
        }

        private void Update()
        {
            /// === Dash ==================================================

            // Update timer
            if (dashTimer >= 0)
                dashTimer -= Time.deltaTime;
            else
                dashIcon.Color();

            /// === Throw =================================================
            if (throwTimer >= 0)
                throwTimer -= Time.deltaTime;
            else if (doneThrowing && weapon.CanThrow())
                throwIcon.Color();

            if (!doneThrowing && weapon.CanThrow())
            {
                doneThrowing = true;
                throwTimer = throwCooldown;
            }

            /// === Spin ==================================================

            // Update timer
            if (spinTimer >= -spinCooldown)
                spinTimer -= Time.deltaTime;
            else
                spinIcon.Color();

            // Stop spin
            if (spinTimer <= 0)
            {
                isSpinning = false;
                player.doFacing = true;
            }

            // Perform spin
            if (isSpinning)
                transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (isSpinning || !canDash || dashTimer > 0) return;

            dashIcon.Dim();

            rb.AddForce(transform.forward * dashStrength, ForceMode.Impulse);
            dashTimer = dashCooldown;
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (isSpinning || !weapon.CanThrow() || throwTimer > 0) return;

            throwIcon.Dim();

            doneThrowing = false;
            weapon.Throw();
        }

        public void OnSpin(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (!doneThrowing || isSpinning || spinTimer > -spinCooldown) return;

            spinIcon.Dim();

            spinTimer = spinDuration;
            player.doFacing = false;
            isSpinning = true;
        }
    }
}
