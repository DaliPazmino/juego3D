using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerLogic1 : MonoBehaviourPun
{
    public float moveVelocity = 5.0f;
    public float rotationVelocity = 200.0f;
    private Animator anim;
    public float x, y;

    public Rigidbody rb;
    private Canvas canvas;
    public float fuerzaDeSalto = 8f;
    public bool puedoSaltar;
    public int score = 0;

    private new PhotonView photonView;
    public TextMeshProUGUI usernameLabel;
    private GameController gameController;
    private bool gameEnded = false;

    private TextMeshProUGUI contadorMonedas;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView component not found on player in Awake.");
        }
    }

    void Start()
    {
        if (photonView == null)
        {
            photonView = GetComponent<PhotonView>();
        }

        if (photonView == null)
        {
            Debug.LogError("PhotonView component not found on player in Start.");
            return;
        }

        if (!photonView.IsMine)
        {
            Debug.Log("Not the local player, destroying camera");
            Destroy(GetComponentInChildren<Camera>().gameObject);
            return;
        }

        puedoSaltar = false;
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator component not found on player.");
        }

        canvas = GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the children of the player object.");
        }
        else
        {
            Debug.Log("Canvas found.");
            canvas.enabled = true;
        }

        if (photonView.Owner != null)
        {
            string nombreUsuario = photonView.Owner.NickName;
            if (usernameLabel != null)
            {
                usernameLabel.text = nombreUsuario;
                Debug.Log("UsernameLabel text set to: " + nombreUsuario);
            }
            else
            {
                Debug.LogError("UsernameLabel not assigned in the inspector.");
            }
        }

        gameController = FindObjectOfType<GameController>();
        if (gameController == null)
        {
            Debug.LogError("GameController not found in the scene.");
        }

        // Find the ContadorMonedas in the scene
        GameObject panel = GameObject.Find("Panel");
        if (panel != null)
        {
            contadorMonedas = panel.transform.Find("ContadorMonedas").GetComponent<TextMeshProUGUI>();
            if (contadorMonedas == null)
            {
                Debug.LogError("ContadorMonedas not found in Panel.");
            }
        }
        else
        {
            Debug.LogError("Panel not found in the scene.");
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine || gameEnded) return;

        transform.Rotate(0, x * Time.deltaTime * rotationVelocity, 0);
        transform.Translate(0, 0, y * Time.deltaTime * moveVelocity);
    }

    void Update()
    {
        if (!photonView.IsMine || gameEnded) return;

        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        if (anim != null)
        {
            anim.SetFloat("VelX", x);
            anim.SetFloat("VelY", y);

            if (puedoSaltar)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    anim.SetBool("jump", true);
                    rb.AddForce(new Vector3(0, fuerzaDeSalto, 0), ForceMode.Impulse);
                }
                anim.SetBool("touchground", true);
            }
            else
            {
                EstoyCayendo();
            }
        }
    }

    public void EstoyCayendo()
    {
        if (anim != null)
        {
            anim.SetBool("touchground", false);
            anim.SetBool("jump", false);
        }
    }

    [PunRPC]
    public void EndGame()
    {
        gameEnded = true;
        if (anim != null)
        {
            anim.SetFloat("VelX", 0);
            anim.SetFloat("VelY", 0);
            anim.SetBool("touchground", true);
            anim.SetBool("jump", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView == null)
        {
            Debug.LogError("PhotonView is not initialized.");
            return;
        }

        if (!photonView.IsMine || gameEnded) return;

        Debug.Log("Trigger entered by: " + other.gameObject.name);

        if (other.gameObject.tag == "Item")
        {
            Debug.Log("Item collected");
            other.gameObject.SetActive(false);
            score += 100;
            Debug.Log("Score updated: " + score);

            if (contadorMonedas != null)
            {
                contadorMonedas.text = score.ToString();
            }

            if (score >= 600)
            {
                Debug.Log("Score threshold reached");
                if (gameController != null)
                {
                    gameController.photonView.RPC("EndGame", RpcTarget.All, photonView.Owner.NickName);
                    photonView.RPC("EndGame", RpcTarget.All);
                }
                else
                {
                    Debug.LogError("GameController is null.");
                }
            }
        }
    }
}