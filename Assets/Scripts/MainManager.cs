using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;
    public GameObject inputField;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    public static MainManager Instance;
    public string PlayerName;
    public int HighScoreP;
    public InputField iField;
    public string BestPlayer;
    public Text BestPlayerText;
    public Text HighScoreText;

    void Awake()
    {
        m_Started = false;
        m_GameOver = false;
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadHighScore();
        DontDestroyOnLoad(gameObject);
    }

    private void GetPlayerName()
    {
        Debug.Log(iField.text);
        PlayerName = iField.text;

    }

    [System.Serializable]
    class SaveData
    {
        public string BestPlayer;
        public int HighScoreP;

    }

    public void SaveHighScore()
    {
        SaveData data = new SaveData();
        data.BestPlayer = BestPlayer;
        data.HighScoreP = HighScoreP;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadHighScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            BestPlayer = data.BestPlayer;
            HighScoreP = data.HighScoreP;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        LoadHighScore();
        BestPlayerText.text = "Best Player: " + "BestPlayer";
        HighScoreText.text = "High score: " + HighScoreP;
        
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0); //SceneManager.GetActiveScene().buildIndex);
                SaveHighScore();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GetPlayerName();
                inputField.SetActive(false);
                if (m_Points > HighScoreP)
                {
                    HighScoreP = m_Points;
                    BestPlayer = PlayerName;
                    SaveHighScore();
                }

            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        inputField.SetActive(true);
        
    }

    public void ResetHighScore()
    {
        HighScoreP = 0;
        BestPlayer = "";
        HighScoreText.text = "";
        BestPlayerText.text = "";
        SaveHighScore();
    }

}
