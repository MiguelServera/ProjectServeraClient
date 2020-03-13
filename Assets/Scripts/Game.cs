using System;

public class Game
{
    private string _id;
    public string Id
    {
        get { return _id; }
        set { _id = value; }
    }

    private string _nickname;
    public string Nickname
    {
        get { return _nickname; }
        set { _nickname = value; }
    }

    private int _score;
    public int Score
    {
        get { return _score; }
        set { _score = value; }
    }

    private int _difficulty;
    public int Difficulty
    {
        get { return _difficulty; }
        set { _difficulty = value; }
    }

    private DateTime _dateStarted;
    public DateTime DateStarted
    {
        get { return _dateStarted; }
        set { _dateStarted = value; }
    }

    private DateTime _dateFinished;
    public DateTime DateFinished
    {
        get { return _dateFinished; }
        set { _dateFinished = value; }
    }
}