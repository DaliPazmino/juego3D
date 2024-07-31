using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviourPun
{
    public GameObject scorePanel;
    public TextMeshProUGUI winnerLabel;
    private bool gameEnded = false;

    void Start()
    {
        if (scorePanel != null)
        {
            scorePanel.SetActive(false);
        }
    }

    [PunRPC]
    public void EndGame(string winner)
    {
        gameEnded = true;
        if (scorePanel != null)
        {
            scorePanel.SetActive(true);
            if (winnerLabel != null)
            {
                winnerLabel.text = "Winner: " + winner;
            }
            else
            {
                Debug.LogError("WinnerLabel not found in ScorePanel.");
            }
        }
        else
        {
            Debug.LogError("ScorePanel not assigned.");
        }
    }

    [PunRPC]
    public void RestartGame()
    {
        StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
            {
                yield return null;
            }
        }

        // Esperar un momento antes de recargar la escena para asegurar que todos se desconecten.
        yield return new WaitForSeconds(1f);

        // Recargar la escena actual
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);

        // Esperar a que la escena se recargue
        yield return null;

        // Reconectar a Photon
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnRestartButtonClicked()
    {
        Debug.Log("entrando al boton");
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Is master entrando");
            photonView.RPC("RestartGame", RpcTarget.All);
        }
    }
}
