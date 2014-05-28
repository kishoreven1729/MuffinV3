using UnityEngine;
using System.Collections;

public class ScoreItem : MonoBehaviour {

    public UILabel name;
    public UILabel score;

    public void SetScore(string n, int s)
    {
        name.text = n;
        score.text = s.ToString();
    }
}
