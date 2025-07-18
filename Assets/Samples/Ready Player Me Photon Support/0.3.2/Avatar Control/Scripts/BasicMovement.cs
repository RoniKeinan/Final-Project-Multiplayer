#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using UnityEngine;

namespace ReadyPlayerMe.PhotonSupport
{
    public class BasicMovement : MonoBehaviour
    {
        private Animator animator;
        private PhotonView photonView;
        private Rigidbody rigidbody; // Ensure there's a Rigidbody component attached
        public GameObject m_camera;
        private readonly static int WALK_ANIM = Animator.StringToHash("Walking");

        private bool isGrounded = true; // To check if the player is on the ground
        public Vector3[] spawnPoints;
        public int spawnIndex = 0;
        private void Awake()
        {
            animator = GetComponent<Animator>();
            photonView = GetComponent<PhotonView>();
            rigidbody = GetComponent<Rigidbody>(); // Get the Rigidbody component
        }

        private void Start()
        {
            if (photonView.IsMine)
            {
                GameObject cam = FindAnyObjectByType<PhotonSetup>().convaiCamera.gameObject;
                cam.transform.SetParent(transform);
                cam.transform.position = m_camera.transform.position;
                cam.transform.rotation = m_camera.transform.rotation;

                photonView.RPC("RequestSpawnPosFromMaster", RpcTarget.MasterClient, photonView.ViewID);
            }
          

            
        }
        [PunRPC]
        public void RequestSpawnPosFromMaster(int viewID)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (spawnIndex >= spawnPoints.Length)
                {
                    Debug.LogWarning("Not enough spawn points! Using default position.");
                    spawnIndex = 0; // Or handle differently
                }

                Vector3 spawnPos = spawnPoints[spawnIndex];
                photonView.RPC(nameof(SetSpawnPos), RpcTarget.All, viewID, spawnPos);
                spawnIndex++;
            }
        }

        [PunRPC]
        public void SetSpawnPos(int viewID, Vector3 pos)
        {
            PhotonView view = PhotonView.Find(viewID);
            if (view != null && view.IsMine)
            {
                view.gameObject.transform.position = pos;
                Debug.Log($"Set spawn position for {viewID} to {pos}");
            }
        }
        private void Update()
        {
            if (photonView.IsMine)
            {
                HandleMovement();
                HandleJump();
                HandleDancing();
              

            }
        }

        private void HandleMovement()
        {
            var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
            var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

            transform.Rotate(0, x, 0);
            transform.Translate(0, 0, z);

            animator.SetBool(WALK_ANIM, z != 0);
        }

        private void HandleJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                isGrounded = false; // Reset in OnCollisionEnter
                animator.SetTrigger("Jump");

            }
        }

        private void HandleDancing()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                animator.SetTrigger("Dancing");
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Check if we collide with the ground
            if (collision.contacts[0].normal.y > 0.5) // Ensures it's mostly vertical
            {
                isGrounded = true;
            }
        }
    }
}
#endif
