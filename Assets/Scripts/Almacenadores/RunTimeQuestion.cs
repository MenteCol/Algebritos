using UnityEngine;

[System.Serializable]
public class RunTimeQuestion
{
    public string questionText;
    public string[] options = new string[4];
    public int correctAnswerIndex;
}
