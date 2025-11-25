using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AuthoritativePlayer : NetworkBehaviour
{
    [SerializeField] private List<GameObject> localObjects = new List<GameObject>();

    [SerializeField] private float moveSpeed = 1;
    private NetworkVariable<Vector3> serverPosition = new(readPerm: NetworkVariableReadPermission.Everyone, writePerm:
        NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> serverAction1 = new(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> serverAction2 = new(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<Vector2> serverDirection = new(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    private NetworkVariable<float> serverSpeed = new(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    private Vector2 input;
    private float inputSendInterval = 1f / 60f; //60 updates per second
    private float inputSendTimer;
    private CharacterController characterController;

    private PlayerAnimator playerAnimator;

    private bool action1 = false;
    private bool action2 = false;

    public UnityEvent action1Event;
    public UnityEvent action2Event;

    private float yPosition;

    private float safeDistance = 0.35f;
    private struct Snapshot
    {
        public Vector3 pos;
        public float time;
        public Snapshot(Vector3 p, float t)
        {
            pos = p;
            time = t;
        }
    }

    private Queue<Snapshot> snapshots = new();

    private void Update()
    {
        if (snapshots.Count >= 2)
        {
            Interpolate();
        }
        if (playerAnimator != null)
        {
            playerAnimator.SetVariables(serverDirection.Value, serverSpeed.Value);
            playerAnimator.Attack(serverAction1.Value);
        }

        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            action1 = true;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            action2 = true;
        }

        inputSendTimer += Time.deltaTime;
        if (inputSendTimer >= inputSendInterval)
        {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            SendInputServerRpc(input, action1, action2);
            action1 = false;
            action2 = false;
            inputSendTimer = 0;
        }
        transform.position = Vector3.Lerp(transform.position, serverPosition.Value, Time.deltaTime * 10f);
    }

    private void Interpolate()
    {
        Snapshot[] array = snapshots.ToArray();
        Snapshot from = array[0];
        Snapshot to = array[1];

        float duration = to.time - from.time;

        float elapsed = Time.time - from.time;
        float t = Mathf.Clamp01(elapsed / duration);

        transform.position = Vector3.Lerp(from.pos, to.pos, t);

        if (t >= 1f) snapshots.Dequeue();
    }

    [ServerRpc]
    private void SendInputServerRpc(Vector2 input, bool _action1, bool _action2)
    {
        float deltaTime = NetworkManager.Singleton.ServerTime.FixedDeltaTime;
        Vector3 move = new Vector3(input.x, 0f, input.y) * moveSpeed * deltaTime;

        //Vector3 targetPos = transform.position + move;
        if (move.magnitude <= 0.5f)
        {
            //transform.position = targetPos;
            if (transform.position.y != yPosition)
            {
                move.y = yPosition - transform.position.y;
            }
            characterController.Move(move);
            serverPosition.Value = transform.position;
            serverDirection.Value = (new Vector2(-move.x, -move.z)).normalized;
            serverSpeed.Value = (new Vector2(move.x, move.z)).magnitude;
        }
        serverAction1.Value = _action1;
        if (_action1)
            action1Event.Invoke();
        serverAction2.Value = _action2;
        if (_action2)
            action2Event.Invoke();
    }


    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            serverPosition.OnValueChanged += (oldVal, newVal) =>
            {
                snapshots.Enqueue(new Snapshot(newVal, Time.time));

                while (snapshots.Count > 5)
                {
                    snapshots.Dequeue();
                }
            };
        }
        if (!IsOwner)
        {
            foreach (GameObject localObject in localObjects)
            {
                Destroy(localObject);
            }
        }
    }



    private void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        characterController = GetComponent<CharacterController>();
        yPosition = 0;

        if (!IsOwner) return;

        FollowLocalPlayer camera = FindFirstObjectByType<FollowLocalPlayer>();
        if (camera == null) return;
        camera.SetPlayer(this.transform);
    }

}
