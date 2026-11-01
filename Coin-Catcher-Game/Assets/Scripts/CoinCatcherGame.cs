using UnityEngine;
using System.Collections.Generic;

public class CoinCatcherGame : MonoBehaviour
{
    private GameObject basket;
    private readonly List<GameObject> coins = new List<GameObject>();
    private int score = 0;
    private int lives = 3;
    private float timer = 0f;
    private bool gameOver = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartGame()
    {
        if (FindObjectOfType<CoinCatcherGame>() == null)
        {
            GameObject manager = new GameObject("CoinCatcherGame");
            manager.AddComponent<CoinCatcherGame>();
        }
    }

    private void Start()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cam = cameraObject.AddComponent<Camera>();
        }

        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.transform.position = new Vector3(0, 0, -10);
        cam.backgroundColor = new Color(0.08f, 0.12f, 0.24f);

        basket = GameObject.CreatePrimitive(PrimitiveType.Cube);
        basket.name = "Basket";
        basket.transform.position = new Vector3(0, -3.8f, 0);
        basket.transform.localScale = new Vector3(1.8f, 0.45f, 0.5f);
        basket.GetComponent<Renderer>().material.color = new Color(0.1f, 0.8f, 1f);
    }

    private void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R)) RestartGame();
            return;
        }

        float move = 0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) move = -1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) move = 1f;

        Vector3 p = basket.transform.position;
        p.x = Mathf.Clamp(p.x + move * 8f * Time.deltaTime, -6.5f, 6.5f);
        basket.transform.position = p;

        timer += Time.deltaTime;
        if (timer >= 0.8f)
        {
            SpawnCoin();
            timer = 0f;
        }

        for (int i = coins.Count - 1; i >= 0; i--)
        {
            GameObject coin = coins[i];
            if (coin == null) { coins.RemoveAt(i); continue; }

            coin.transform.position += Vector3.down * 4f * Time.deltaTime;

            if (coin.transform.position.y <= -3.35f &&
                Mathf.Abs(coin.transform.position.x - basket.transform.position.x) < 1.25f)
            {
                score++;
                Destroy(coin);
                coins.RemoveAt(i);
            }
            else if (coin.transform.position.y < -4.8f)
            {
                lives--;
                Destroy(coin);
                coins.RemoveAt(i);
                if (lives <= 0) gameOver = true;
            }
        }
    }

    private void SpawnCoin()
    {
        GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        coin.name = "Coin";
        coin.transform.position = new Vector3(Random.Range(-6.5f, 6.5f), 5.5f, 0);
        coin.transform.localScale = Vector3.one * 0.45f;
        coin.GetComponent<Renderer>().material.color = new Color(1f, 0.8f, 0.05f);
        coins.Add(coin);
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 26;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 20, 250, 45), "Score: " + score, style);
        GUI.Label(new Rect(20, 60, 250, 45), "Lives: " + lives, style);
        GUI.Label(new Rect(20, 105, 500, 40), "Move: A/D or Arrow Keys", style);

        if (gameOver)
        {
            GUIStyle center = new GUIStyle(style);
            center.alignment = TextAnchor.MiddleCenter;
            center.fontSize = 36;
            GUI.Label(new Rect(0, Screen.height / 2 - 70, Screen.width, 55), "GAME OVER", center);
            GUI.Label(new Rect(0, Screen.height / 2 - 20, Screen.width, 45), "Final Score: " + score, center);
            GUI.Label(new Rect(0, Screen.height / 2 + 25, Screen.width, 45), "Press R to Restart", center);
        }
    }

    private void RestartGame()
    {
        foreach (GameObject coin in coins)
            if (coin != null) Destroy(coin);

        coins.Clear();
        score = 0;
        lives = 3;
        timer = 0f;
        gameOver = false;
        basket.transform.position = new Vector3(0, -3.8f, 0);
    }
}
