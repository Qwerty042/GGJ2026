using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Luchador
{
    public List<Question> questions;
    public string name;
    public string displayName;
    public string metadata;
    public int currentQuestionIndex;
    public float health;
    public float maxHealth;
}

public class Question
{
    public string questionString;
    public string correctAnswerString;
    public string[] answers;
}

public class QuizMaster : MonoBehaviour
{
    public GameObject playerHealthBar;
    public GameObject luchadorHealthBar;
    public GameObject questionBoxPrefab;
    public GameObject answerButtonPrefab;

    private string selectedLuchador;
    private GameObject questionBox;
    private Vector3 questionLocation = new Vector3(-4.5f, -2.75f, 0);
    private GameObject[] answerButtons = new GameObject[4];
    private Vector3[] buttonLocations = { new Vector3(2.0f,-2.0f,0),
                                          new Vector3(6.5f,-2.0f,0),
                                          new Vector3(2.0f,-3.5f,0),
                                          new Vector3(6.5f,-3.5f,0)};

    private Luchador luchador;
    private float answeredPauseTime = 0.3f;
    private float timer;


    public AudioClip hoverSound;
    private AudioSource hoverAudioSource;

    public AudioClip[] goodHits;
    public AudioClip badHit;
    public AudioClip goodKO;
    public AudioClip badKO;
    private AudioSource hitAudioSource;



    private enum QuizState
    {
        qsLOAD_QUESTION,
        qsANSWERING,
        qsCORRECT,
        qsINCORRECT,
        qsUNLOAD_QUESTION,
        qsEND_WIN,
        qsEND_LOSS
    }
    private QuizState quizState = QuizState.qsLOAD_QUESTION;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectedLuchador = GlobalGameState.nextLuchadorCsvFileName;
        LoadLuchador(selectedLuchador);
        UpdateHeathBar(playerHealthBar, GlobalGameState.playerHealth/GlobalGameState.MAX_PLAYER_HEALTH);

        hoverAudioSource = gameObject.AddComponent<AudioSource>();
        hitAudioSource  = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (quizState)
        {
            case QuizState.qsLOAD_QUESTION:
                LoadNextQuestion();
                quizState = QuizState.qsANSWERING;
                break;
            case QuizState.qsANSWERING:
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                for (int i = 0; i < answerButtons.Length; i++)
                {
                    if(hit.transform != null && hit.transform.gameObject == answerButtons[i])
                    {
                        if (Input.GetMouseButton(0))
                        {
                            if(answerButtons[i].GetComponentInChildren<TextMeshPro>().text == luchador.questions[luchador.currentQuestionIndex].correctAnswerString)
                            {
                                quizState = QuizState.qsCORRECT;
                                answerButtons[i].GetComponent<SpriteRenderer>().color = Color.green;
                                luchador.health -= 19.0f;
                                Debug.Log("You struck the luchador!");
                                if (luchador.health >= 80.0f)
                                {
                                    hitAudioSource.PlayOneShot(goodHits[0]);
                                }
                                else if (luchador.health >= 60.0f)
                                {
                                    hitAudioSource.PlayOneShot(goodHits[1]);
                                }
                                else if (luchador.health >= 40.0f)
                                {
                                    hitAudioSource.PlayOneShot(goodHits[2]);
                                }
                                else if (luchador.health >= 20.0f)
                                {
                                    hitAudioSource.PlayOneShot(goodHits[3]);
                                }
                                else if (luchador.health > 0)
                                {
                                    hitAudioSource.PlayOneShot(goodHits[4]);
                                }
                                else
                                {
                                    hitAudioSource.PlayOneShot(goodKO);
                                }
                                FindFirstObjectByType<LuchadorShake>().StartCoroutine(FindFirstObjectByType<LuchadorShake>().Shake(0.2f, 0.5f));
                            }
                            else
                            {
                                quizState = QuizState.qsINCORRECT;
                                answerButtons[i].GetComponent<SpriteRenderer>().color = Color.red;
                                GlobalGameState.playerHealth -= 10.0f;
                                Debug.Log("You were struck by the luchador!");
                                if (GlobalGameState.playerHealth <= 0)
                                {
                                    hitAudioSource.PlayOneShot(badKO);
                                }
                                else
                                {
                                    hitAudioSource.PlayOneShot(badHit);
                                }
                                FindFirstObjectByType<CameraShake>().StartCoroutine(FindFirstObjectByType<CameraShake>().Shake(0.2f, 0.5f));
                                FindFirstObjectByType<RedDamageOverlay>().StartCoroutine(FindFirstObjectByType<RedDamageOverlay>().Pulse());
                            }
                            timer = answeredPauseTime;
                        }
                        else
                        {
                            if (answerButtons[i].GetComponent<SpriteRenderer>().color == Color.gray7)
                            {
                                hoverAudioSource.PlayOneShot(hoverSound);
                            } 
                            answerButtons[i].GetComponent<SpriteRenderer>().color = Color.white;
                        }
                    }
                    else
                    {
                        answerButtons[i].GetComponent<SpriteRenderer>().color = Color.gray7;
                    }
                }
                break;
            case QuizState.qsCORRECT:
                timer -= Time.deltaTime;
                if(timer <= 0.0f)
                {
                    quizState = QuizState.qsUNLOAD_QUESTION;
                }
                break;
            case QuizState.qsINCORRECT:
                timer -= Time.deltaTime;
                if(timer <= 0.0f)
                {
                    quizState = QuizState.qsUNLOAD_QUESTION;
                }
                break;
            case QuizState.qsUNLOAD_QUESTION:
                DestroyQuestion();
                Debug.Log($"Remaining Player Health: {GlobalGameState.playerHealth}, Remaining Luchador Health: {luchador.health}");
                if(GlobalGameState.playerHealth <= 0)
                {
                    quizState = QuizState.qsEND_LOSS;
                }
                else if (luchador.health <= 0)
                {
                    quizState = QuizState.qsEND_WIN;
                }
                else
                {
                    quizState = QuizState.qsLOAD_QUESTION;
                    UpdateHeathBar(playerHealthBar, GlobalGameState.playerHealth/GlobalGameState.MAX_PLAYER_HEALTH);
                    UpdateHeathBar(luchadorHealthBar, luchador.health/luchador.maxHealth);
                }
                break;
            case QuizState.qsEND_WIN:
                GlobalGameState.prevLuchadorName = luchador.displayName;
                SceneManager.LoadScene("EnemySelect");
                GlobalGameState.playerScore++;
                break;
            case QuizState.qsEND_LOSS:
                SceneManager.LoadScene("Ded");
                break;
            default:
                Debug.LogError("quizState has gone rouge");
                break;
        }
    }

    void LoadLuchador(string luchadorName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Luchadors/" + luchadorName);
        if (csvFile == null)
        {
            Debug.LogError($"Failed to load {luchadorName}.csv from Resources/Luchadors");
            return;
        }

        string[] lines = csvFile.text.Split("\n");
        if (lines.Length <= 0)
        {
            Debug.LogError("levels.csv is empty!");
            return;
        }

        luchador = new Luchador();
        luchador.questions = new List<Question>();
        luchador.name = luchadorName;
        luchador.displayName = GlobalGameState.nextLuchadorName;
        luchador.maxHealth = 100;
        luchador.health = luchador.maxHealth;
        luchador.metadata = lines[0]; // TODO: parse this to get the visual and sound assets for this luchador

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] cols = line.Split('|');
            Question question = new Question();
            question.questionString = cols[0];
            question.correctAnswerString = cols[1];
            question.answers = cols[1..];
            shuffleArray(question.answers);
            // Debug.Log($"Question {question.questionString}, a) {question.answers[0]} b) {question.answers[1]} c) {question.answers[2]} d) {question.answers[3]}, Correct answer {question.correctAnswerString}");
            luchador.questions.Add(question);
        }
    }

    void shuffleArray(string[] texts)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < texts.Length; t++ )
        {
            string tmp = texts[t];
            int r = Random.Range(t, texts.Length);
            texts[t] = texts[r];
            texts[r] = tmp;
        }
    }

    void LoadNextQuestion()
    {

        Debug.Log($"Questions remaining {luchador.questions.Count}");
        if(luchador.questions.Count == 0)
        {
            Debug.Log("No more questions!");
            return;
        }

        luchador.currentQuestionIndex = Random.Range(0, luchador.questions.Count);

        questionBox = Instantiate(questionBoxPrefab, questionLocation, Quaternion.identity);
        questionBox.GetComponentInChildren<TextMeshPro>().text = luchador.questions[luchador.currentQuestionIndex].questionString.Replace("[MASK]", "[\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0]");
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i] = Instantiate(answerButtonPrefab, buttonLocations[i], Quaternion.identity);
            answerButtons[i].GetComponentInChildren<TextMeshPro>().text = luchador.questions[luchador.currentQuestionIndex].answers[i];
            answerButtons[i].GetComponent<SpriteRenderer>().color = Color.gray7;
        }
    }

    void DestroyQuestion()
    {
        luchador.questions.RemoveAt(luchador.currentQuestionIndex);
        Destroy(questionBox);
        foreach (GameObject answerButton in answerButtons)
        {
            Destroy(answerButton);
        }
    }

    void UpdateHeathBar(GameObject healthBar, float healthRatio)
    {
        healthBar.GetComponentsInChildren<Transform>()[1].localScale = new Vector3(healthRatio, 1, 1);
        healthBar.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(-0.5f*(1.0f-healthRatio), 0, 0);
    }
}
