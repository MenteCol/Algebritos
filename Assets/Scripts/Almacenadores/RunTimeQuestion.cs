using UnityEngine;

[System.Serializable]
public class RunTimeQuestion
{
    public string questionText;
    public string[] options;
    public int correctAnswerIndex;
    public int level; 
}

[System.Serializable]

public class RunTimeQuestionList
{
    public System.Collections.Generic.List<RunTimeQuestion> questions;
}
