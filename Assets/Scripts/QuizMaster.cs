using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Luchador
{
    public List<Question> questions;
    public string name;
    public string metadata;
}

public class Question
{
    public string questionString;
    public string correctAnswerString;
    public string[] answers;
    public bool used;
}

public class QuizMaster : MonoBehaviour
{
    public GameObject questionBoxPrefab;
    public GameObject answerButtonPrefab;

    private GameObject questionBox;
    private Vector3 questionLocation = new Vector3(-4.5f, -2.75f, 0);
    private GameObject[] answerButtons = new GameObject[4];
    private Vector3[] buttonLocations = { new Vector3(2.0f,-2.0f,0),
                                          new Vector3(6.5f,-2.0f,0),
                                          new Vector3(2.0f,-3.5f,0),
                                          new Vector3(6.5f,-3.5f,0)};

    private int correctButtonIndex = 2;

    private Luchador luchador;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadLuchador("el_modismo");

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i] = Instantiate(answerButtonPrefab, buttonLocations[i], Quaternion.identity);
            answerButtons[i].GetComponentInChildren<TextMeshPro>().text = "Answer " + (i+1).ToString();
        }
        questionBox = Instantiate(questionBoxPrefab, questionLocation, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if(hit.transform != null && hit.transform.gameObject == answerButtons[i])
            {
                if (Input.GetMouseButton(0))
                {
                    answerButtons[i].GetComponent<SpriteRenderer>().color = (i == correctButtonIndex) ? Color.green : Color.red;
                }
                else
                {
                    answerButtons[i].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            else
            {
                answerButtons[i].GetComponent<SpriteRenderer>().color = Color.gray7;
            }
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
            Debug.Log($"Question {question.questionString}, a) {question.answers[0]} b) {question.answers[1]} c) {question.answers[2]} d) {question.answers[3]}, Correct answer {question.correctAnswerString}");
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

    void destroyQuestion()
    {
        
    }
}
