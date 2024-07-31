using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Añadir esta línea
using Photon.Pun; // Añadir Photon

public class GestionUsuario : MonoBehaviourPunCallbacks
{
    public InputField txtUsuario, txtContraseña;
    public string nombreUsuario;
    public int scoreUsuario;
    public bool sesionIniciada = false;

    public void conectarUsuario()
    {
        StartCoroutine(Login());
    }

    public void registrarUsuario()
    {
        StartCoroutine(Registrar());
    }

    IEnumerator Login()
    {
        WWW conexion = new WWW("http://localhost/JUEGO/login.php?uss=" + txtUsuario.text + "&pss=" + txtContraseña.text);
        yield return (conexion);

        if (conexion.text == "200")
        {
            print("Usuario correcto");
            sesionIniciada = true;
            nombreUsuario = txtUsuario.text;
            PlayerPrefs.SetString("nombreUsuario", nombreUsuario); // Guardar el nombre de usuario
            PhotonNetwork.NickName = nombreUsuario; // Configurar el nombre de usuario en Photon
            StartCoroutine(Mostrar()); // Mostrar la información del usuario
            SceneManager.LoadScene("Multiplayer"); // Cambiar a la escena del multijugador
        }
        else
        {
            if (conexion.text == "401")
            {
                print("Usuario o contraseña incorrectos!!");
            }
            else
            {
                print("ERROR!! en la conexión");
            }
        }
    }

    IEnumerator Mostrar()
    {
        WWW conexion = new WWW("http://localhost/juego/mostrar.php?uss=" + txtUsuario.text);
        yield return (conexion);
        //Debug.Log(conexion.text);
        if (conexion.text == "401")
        {
            print("Usuario incorrectos!!");
        }
        else
        {
            string[] datos = conexion.text.Split('|');
            if (datos.Length != 2)
            {
                print("Error en la conexión!!");
            }
            else
            {
                nombreUsuario = datos[0];
                scoreUsuario = int.Parse(datos[1]);
            }
        }
    }

    IEnumerator Registrar()
    {
        WWW conexion = new WWW("http://localhost/JUEGO/registrar.php?uss="
            + txtUsuario.text
            + "&pss="
            + txtContraseña.text);
        yield return (conexion);

        if (conexion.text == "402")
        {
            print("Usuario ya existe!!");
        }
        else
        {
            if (conexion.text == "201")
            {
                nombreUsuario = txtUsuario.text;
                scoreUsuario = 0;
                sesionIniciada = true;
                PlayerPrefs.SetString("nombreUsuario", nombreUsuario); // Guardar el nombre de usuario
                PhotonNetwork.NickName = nombreUsuario; // Configurar el nombre de usuario en Photon
                print("Usuario Registrado!!");
            }
            else
            {
                Debug.LogError("ERROR!! en la conexion");
            }
        }
    }
}
