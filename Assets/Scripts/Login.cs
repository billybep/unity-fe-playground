using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField] private string _authEndpointLogin = "http://127.0.0.1:13765/account/login";
    [SerializeField] private string _authEndpointRegister = "http://127.0.0.1:13765/account/register";

    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _registerButton;
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;

    public void OnLoginClick()
    {
        _statusText.text = "Signing in ...";
        _loginButton.interactable = false;
        Debug.Log(_authEndpointLogin);

        StartCoroutine(TryLogin());
    }

    public void OnRegisterClick()
    {
        _statusText.text = "Creating account ...";
        _registerButton.interactable = false;
        Debug.Log(_authEndpointRegister);

        StartCoroutine(TryRegister());
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

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest request = UnityWebRequest.Post(_authEndpointLogin, form);
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

                Debug.Log(request.downloadHandler.text + "loginresponse");
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                if (response.code == 0)
                {
                    _loginButton.interactable = false;
                    _statusText.text = $"Welcome ...";
                }
                else
                {
                    switch (response.code)
                    {
                        case 1:
                            _statusText.text = "Invalid credentials";
                            _loginButton.interactable = true;
                            break;
                        default:
                            _statusText.text = "Corruption detected";
                            _loginButton.interactable = false;
                            break;
                    }
                }

                // Clear
                _usernameInputField.text = null;
                _passwordInputField.text = null;
                break;
        }

        yield return null;
    }

    private IEnumerator TryRegister()
    {
        string username = _usernameInputField.text;
        string password = _passwordInputField.text;

        // check input
        if (username.Length < 3 || username.Length > 24)
        {
            _statusText.text = "Invalid username";
            _registerButton.interactable = true;
            yield break;
        }

        if (password.Length < 3 || password.Length > 24)
        {
            _statusText.text = "Invalid password";
            _registerButton.interactable = true;
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest request = UnityWebRequest.Post(_authEndpointRegister, form);
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

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                _statusText.text = $"Error connection to the server... Error: {request.error}";
                _registerButton.interactable = true;
                break;
            case UnityWebRequest.Result.ProtocolError:
                _statusText.text = $": HTTP Error: {request.error}";
                break;
            case UnityWebRequest.Result.Success:

                Debug.Log(request.downloadHandler.text + "registerresponse");
                RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(request.downloadHandler.text);

                if (response.code == 0)
                {
                    _registerButton.interactable = false;
                    _statusText.text = "Account has been created";
                }
                else
                {
                    switch (response.code)
                    {
                        case 1:
                            _statusText.text = "Invalid credentials";
                            break;
                        case 2:
                            _statusText.text = "Username has been registered";
                            break;
                        case 3:
                            _statusText.text = "Password is unsafe";
                            break;
                        default:
                            _statusText.text = "Corruption detected";
                            break;
                    }
                    _registerButton.interactable = true;
                }

                // Clear
                _usernameInputField.text = null;
                _passwordInputField.text = null;
                break;
        }
        yield return null;
    }
}
