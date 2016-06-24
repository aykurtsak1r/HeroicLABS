using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameUp;
using UnityEngine.UI;

public class ConnectionTest : MonoBehaviour {

    public InputField inputEmail;
    public InputField inputName;
    public InputField inputPoint;
    public Text leaderBoardText;
    public Button postButton;

    Client.ErrorCallback failure = (status, reason) =>
    {
        if (status >= 500)
        {
            Debug.LogError(status + ": " + reason);
        }
        else
        {
            Debug.LogWarning(status + ": " + reason);
        }
    };

    string password = "password";
    string passwordConfirmation = "password";
	// Use this for initialization
	void Start () {

        Client.ApiKey = "37b90266cebb4518af85a9e5f76c1092";

        Client.Ping((PingInfo server) =>
        {
            Debug.Log("Server Time: " + server.Time);
            postButton.interactable = true;
        }, failure);


        /*string uid = PlayerPrefs.GetString("UID");
        if (string.IsNullOrEmpty(uid))
        {
            uid = SystemInfo.deviceUniqueIdentifier;
            PlayerPrefs.SetString("UID", uid);
        }*/

        /*Client.LoginAnonymous(uid, (SessionClient sessionClient) =>
        {
            // we have a SessionClient to make requests with - let's check it's connected
            sessionClient.Ping(() =>
            {
                Debug.Log("Ping was successful with a valid apikey and token");

                Serialize(sessionClient);
                testGamer(sessionClient);

            }, (status, reason) =>
            {
                Debug.LogErrorFormat("The request failed with '{0}' and '{1}'.", status, reason);
            });
        }, (status, reason) =>
        {
            Debug.LogErrorFormat("Could not login user with ID: '{0}'.", uid);
        });*/
        
	}

    public void CheckAccount()
    {
        postButton.interactable = false;
        string email = inputEmail.text;
        string playerName = inputName.text;

        Client.CreateEmailAccount(email, password, passwordConfirmation, playerName, (SessionClient sessionClient) =>
        {
            // Store session just as before           
            SubmitOrUpdate(email);

        }, (status, reason) =>
        {
            //Debug.LogErrorFormat("Could not register account - got '{0}'.", reason);
            Login(email, password);
        });
    }

    void Login(string email, string password)
    {
        Client.LoginEmail(email, password, (SessionClient session) =>
        {
            // Store session just as before
            Debug.Log("Loggin was successful with a valid apikey");                     
            SubmitOrUpdate(email);
   
        }, (status, reason) =>
        {
            Debug.LogErrorFormat("Could not login - got '{0}'.", reason);
        });       

    }

    void SubmitOrUpdate(string email)
    {
        long newScore = long.Parse(inputPoint.text);
        
        Client.LoginEmail(email, password, (SessionClient sessionClient) =>
        {
            // The ID for the leaderboard is in Hub
            string leaderboardId = "b6a7804dce4f41738123bfd943b95479";
            sessionClient.UpdateLeaderboard(leaderboardId, newScore, (Rank rank) =>
            {
                Debug.LogFormat("The user's best score is '{0}'.", rank.Score);
                ShowLeaderBoard();
            }, (status, reason) =>
            {
                Debug.LogErrorFormat("The request failed with '{0}' and '{1}'.", status, reason);
            });
        }, (status, reason) =>
        {
            Debug.LogErrorFormat("Could not login - got '{0}'.", reason);
        });
    }

    void ShowLeaderBoard()
    {
        leaderBoardText.text = "";
        string leaderboardId = "b6a7804dce4f41738123bfd943b95479";
        Client.Leaderboard(leaderboardId, (Leaderboard leaderboard) =>
        {
            foreach (Leaderboard.Entry entry in leaderboard)
            {
                Debug.LogFormat("Name: '{0}' - Score: '{1}'.", entry.Name, entry.Score);
                leaderBoardText.text += entry.Name + "   " + entry.Score + "\n";
                // you could add each leaderboard entry to a UI element and show it
            }
            postButton.interactable = true;
        }, (status, reason) =>
        {
            Debug.LogErrorFormat("The request failed with '{0}' and '{1}'.", status, reason);
        });
    }
    void Serialize(SessionClient session)
    {
        string cachedSession = PlayerPrefs.GetString("SESSION");
        if (string.IsNullOrEmpty(cachedSession))
        {
            PlayerPrefs.SetString("SESSION", session.Serialize());
            Debug.Log(cachedSession);
        }
        else
        {
            SessionClient sessionClient = SessionClient.Deserialize(cachedSession);
            Debug.Log("Restored session: " + sessionClient.Token);
        }
    }

    void testGamer(SessionClient session)
    {
        session.Gamer((Gamer gamer) =>
        {
            Debug.Log("Gamer Name: " + gamer.Name);
            Debug.Log("Gamer ID: " + gamer.GamerId);
        }, failure);

        Client.CreateEmailAccount("unitysdk@gameup.io", "password", "password", "UnitySDK Test", session, (SessionClient gus) =>
        {
            session = gus;
            Debug.Log("Created GameUp Account: " + session.Token);
        }, (status, reason) =>
        {
            Client.LoginEmail("unitysdk@gameup.io", "password", (SessionClient gus) =>
            {
                Debug.Log("Logged in with GameUp Account: " + session.Token);
            }, failure);
        });

        session.UpdateGamer("UnitySDKTest", () =>
        {
            Debug.Log("Updated gamer's profile");
        }, failure);
    }
}
