using System.Collections.Generic;

[System.Serializable]
public class BeatSaberNote
{
    public float _time;
    public int _lineIndex;
    public int _lineLayer;
    public int _type;
    public int _cutDirection;
}

[System.Serializable]
public class BeatSaberMap
{
    public List<BeatSaberNote> _notes;
}
