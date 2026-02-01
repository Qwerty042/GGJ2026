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
    public GameObject playerSprite;
    public GameObject luchadorSprite;

    private string selectedLuchador;
    private GameObject questionBox;
    private Vector3 questionLocation = new Vector3(-5.4f, -3.3f, 0);
    private GameObject[] answerButtons = new GameObject[4];
    private Vector3[] buttonLocations = { new Vector3(0.64f,-2.66f,0),
                                          new Vector3(6.04f,-2.66f,0),
                                          new Vector3(0.64f,-3.96f,0),
                                          new Vector3(6.04f,-3.96f,0)};

    private Luchador luchador;
    private float answeredPauseTime = 0.3f;
    private float deathPauseTime = 1.0f;
    private float timer;

    public AudioClip ringBell;
    private AudioSource flavourAudioSource;


    public AudioClip hoverSound;
    private AudioSource hoverAudioSource;

    public AudioClip[] goodHits;
    public AudioClip badHit;
    public AudioClip goodKO;
    public AudioClip badKO;
    private AudioSource hitAudioSource;

    private Vector3 playerSpriteStartPos;
    private Vector3 playerSpriteStartScale;
    private float playerJumpingAnimPos = 0.0f;
    private float playerJumpingAnimDir = 1.0f;
    private float playerJumpingAnimRate = 2.0f;

    private Vector3 luchadorSpriteStartPos;
    private Vector3 luchadorSpriteStartScale;
    private float luchadorJumpingAnimPos = 0.0f;
    private float luchadorJumpingAnimDir = 1.0f;
    private float luchadorJumpingAnimRate = 2.722993f;



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
        UpdateHeathBarPlayer(playerHealthBar, GlobalGameState.playerHealth/GlobalGameState.MAX_PLAYER_HEALTH);
        playerSpriteStartPos = playerSprite.transform.position;
        playerSpriteStartScale = playerSprite.transform.localScale;
        luchadorSpriteStartPos = luchadorSprite.transform.position;
        luchadorSpriteStartScale = luchadorSprite.transform.localScale;

        hoverAudioSource = gameObject.AddComponent<AudioSource>();
        hitAudioSource  = gameObject.AddComponent<AudioSource>();
        flavourAudioSource = gameObject.AddComponent<AudioSource>();

        flavourAudioSource.PlayOneShot(ringBell);
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

                Vector3 nextPos = new Vector3();
                nextPos.x = Mathf.Lerp(playerSpriteStartPos.x, playerSpriteStartPos.x + 0.5f, playerJumpingAnimPos);
                nextPos.y = playerSpriteStartPos.y + (-0.25f * Mathf.Sin(((playerSpriteStartPos.x * Mathf.PI)/(0.5f))-(((nextPos.x) * Mathf.PI)/(0.5f))));
                // Debug.Log(nextPos.y);
                
                playerSprite.transform.position = nextPos;

                playerJumpingAnimPos += playerJumpingAnimRate * Time.deltaTime * playerJumpingAnimDir;
                if (playerJumpingAnimPos >= 1.0f)
                {
                    playerJumpingAnimPos = 1.0f;
                    playerJumpingAnimDir = -1.0f;
                }
                else if (playerJumpingAnimPos <= 0.0f)
                {
                    playerJumpingAnimPos = 0.0f;
                    playerJumpingAnimDir = 1.0f;
                }

                nextPos = new Vector3();
                nextPos.x = Mathf.Lerp(luchadorSpriteStartPos.x, luchadorSpriteStartPos.x + 0.3f, luchadorJumpingAnimPos);
                nextPos.y = luchadorSpriteStartPos.y + (-0.15f * Mathf.Sin(((luchadorSpriteStartPos.x * Mathf.PI)/(0.3f))-(((nextPos.x) * Mathf.PI)/(0.3f))));
                // Debug.Log(nextPos.y);
                
                luchadorSprite.transform.position = nextPos;

                luchadorJumpingAnimPos += luchadorJumpingAnimRate * Time.deltaTime * luchadorJumpingAnimDir;
                if (luchadorJumpingAnimPos >= 1.0f)
                {
                    luchadorJumpingAnimPos = 1.0f;
                    luchadorJumpingAnimDir = -1.0f;
                }
                else if (luchadorJumpingAnimPos <= 0.0f)
                {
                    luchadorJumpingAnimPos = 0.0f;
                    luchadorJumpingAnimDir = 1.0f;
                }

                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                for (int i = 0; i < answerButtons.Length; i++)
                {
                    if(hit.transform != null && hit.transform.gameObject == answerButtons[i])
                    {
                        if (Input.GetMouseButtonDown(0))
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
                                playerSprite.transform.position = new Vector3(playerSpriteStartPos.x + 3.0f, playerSpriteStartPos.y + 0.6f, playerSpriteStartPos.z);
                                playerSprite.transform.localScale = playerSpriteStartScale * 0.9f;
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
                                luchadorSprite.transform.position = new Vector3(luchadorSpriteStartPos.x - 2.5f, playerSpriteStartPos.y - 0.1f, playerSpriteStartPos.z);
                                luchadorSprite.transform.localScale = luchadorSpriteStartScale * 1.2f;
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
                UpdateHeathBarPlayer(playerHealthBar, GlobalGameState.playerHealth/GlobalGameState.MAX_PLAYER_HEALTH);
                UpdateHeathBarLuchador(luchadorHealthBar, luchador.health/luchador.maxHealth);
                Debug.Log($"Remaining Player Health: {GlobalGameState.playerHealth}, Remaining Luchador Health: {luchador.health}");
                if(GlobalGameState.playerHealth <= 0)
                {
                    timer = deathPauseTime;
                    quizState = QuizState.qsEND_LOSS;
                }
                else if (luchador.health <= 0)
                {
                    timer = deathPauseTime;
                    quizState = QuizState.qsEND_WIN;
                }
                else
                {
                    luchadorSprite.transform.localScale = luchadorSpriteStartScale;
                    playerSprite.transform.localScale = playerSpriteStartScale;
                    quizState = QuizState.qsLOAD_QUESTION;
                }
                break;
            case QuizState.qsEND_WIN:
                timer -= Time.deltaTime;
                Vector3 npos = luchadorSprite.transform.position;
                luchadorSprite.transform.Rotate(0.0f, 0.0f, 2.0f);
                Debug.Log(luchadorSprite.transform.localScale * 0.99f);
                luchadorSprite.transform.localScale *= 0.99f;
                npos.y += 3.0f * Time.deltaTime;
                npos.x += 10.0f * Time.deltaTime;
                luchadorSprite.transform.position = npos;
                if(timer <= 0.0f)
                {
                    AudioManager.Instance.SetVolume(luchador.displayName, 0.0f);
                    GlobalGameState.prevLuchadorName = luchador.displayName;
                    GlobalGameState.playerScore++;
                    SceneManager.LoadScene("EnemySelect");
                }
                break;
            case QuizState.qsEND_LOSS:
                timer -= Time.deltaTime;
                npos = playerSprite.transform.position;
                playerSprite.transform.Rotate(0.0f, 0.0f, -2.0f);
                Debug.Log(playerSprite.transform.localScale * 1.006f);
                if (playerSprite.transform.localScale.x <= 20)
                {
                    playerSprite.transform.localScale *= 1.006f;
                }
                npos.y -= 3.0f * Time.deltaTime;
                npos.x -= 30.0f * Time.deltaTime;
                playerSprite.transform.position = npos;
                if(timer <= 0.0f)
                {
                    AudioManager.Instance.SetVolume(luchador.displayName, 0.0f);
                    AudioManager.Instance.SetVolume("Background", 0.0f);
                    SceneManager.LoadScene("Ded");
                }
                // else if (timer <= deathPauseTime - 1.0f)
                // {
                //     Vector3 pos = playerSprite.transform.position;
                //     pos.y -= 12.0f * Time.deltaTime;
                //     playerSprite.transform.position = pos;
                // }
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
        
        if (!LuchadorSprites.Map.TryGetValue(luchador.displayName, out var files))
        {
            Debug.LogError($"No sprite mapping for luchador: {luchador.displayName}");
            return;
        }
        Sprite thumb = Resources.Load<Sprite>($"Luchadors/{files[1]}");
        questionBox.transform.Find("HeadSprite").GetComponent<SpriteRenderer>().sprite = thumb;
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

    void UpdateHeathBarLuchador(GameObject healthBar, float healthRatio)
    {
        healthBar.GetComponentsInChildren<Transform>()[1].localScale = new Vector3(healthRatio, 1, 1);
        healthBar.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(+0.5f*(1.0f-healthRatio), 0, 0);
    }

    void UpdateHeathBarPlayer(GameObject healthBar, float healthRatio)
    {
        healthBar.GetComponentsInChildren<Transform>()[1].localScale = new Vector3(healthRatio, 1, 1);
        healthBar.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(-0.5f*(1.0f-healthRatio), 0, 0);
    }
}
