using UnityEngine;
using System.IO;

public class QuizLoader : MonoBehaviour
{
    public QuestionManager quizManager;

    void Start()
    {
        string path = Application.persistentDataPath + "/questions.json";
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            RunTimeQuestionList data = JsonUtility.FromJson<RunTimeQuestionList>(json);
            quizManager.LoadQuestions(data.questions);
        }
    }
}
