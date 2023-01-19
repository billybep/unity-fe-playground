using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField] private string _authEndpoint = "http://127.0.0.1:13765/account";
    
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Button _loginButton;
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;

    public void OnLoginClick()
    {
        _statusText.text = "Signing in ...";
        _loginButton.interactable = false;
        Debug.Log(_authEndpoint);

        StartCoroutine(TryLogin());
    }

    private IEnumerator TryLogin()
    {
        string username = _usernameInputField.text;
        string password = _passwordInputField.text;

        // check input
        if (username.Length < 3 || username.Length > 24)
        {
            _statusText.text = "Invalid username";
            _loginButton.interactable = true;
            yield break;
        }

        if (password.Length < 3 || password.Length > 24)
        {
            _statusText.text = "Invalid password";
            _loginButton.interactable = true;
            yield break;
        }

        UnityWebRequest request = UnityWebRequest.Get($"{_authEndpoint}?username={username}&password={password}");
        var handler = request.SendWebRequest();

        float startTime = 0.0f;

        while (!handler.isDone)
        {
            startTime += Time.deltaTime;

            if (startTime > 10.0f)
            {
                break;
            }

            yield return null;
        }

        // if Web request successfull
        //if (request.result == UnityWebRequest.Result.Success)
        //{
        //    // if Success login
        //    if (request.downloadHandler.text != "Invalid credential")
        //    {
        //        _statusText.text = "Welcome";
        //        _loginButton.interactable = false;
        //    }
        //    else
        //    {
        //        _statusText.text = "Invalid credential";
        //        _loginButton.interactable = true;
        //    }

        //    _statusText.text = request.downloadHandler.text;
        //    _loginButton.interactable = true;
        //}
        //else
        //{
        //    _statusText.text = "Error connection to the server...";
        //    _loginButton.interactable = true;
        //}

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                _statusText.text = $"Error connection to the server... Error: {request.error}";
                _loginButton.interactable = true;
                break;
            case UnityWebRequest.Result.ProtocolError:
                _statusText.text = $": HTTP Error: {request.error}";
                break;
            case UnityWebRequest.Result.Success:
                if (request.downloadHandler.text != "Invalid credential")
                {
                    _loginButton.interactable = false;
                    GameAccount returnedAccount = JsonUtility.FromJson<GameAccount>(request.downloadHandler.text);
                    _statusText.text = $"Welcome {returnedAccount.username} : id: {returnedAccount._id}";
                }
                else
                {
                    _statusText.text = "Invalid credential";
                    _loginButton.interactable = true;
                }

                //_statusText.text = request.downloadHandler.text + "WELCOME";
                //_loginButton.interactable = true;

                // Clear
                _usernameInputField.text = null;
                _passwordInputField = null;
                break;
        }

        Debug.Log($"{username}:{password}");

        yield return null;
    }
}
