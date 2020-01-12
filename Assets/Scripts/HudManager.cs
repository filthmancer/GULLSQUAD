using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gull;
public class HudManager : MonoBehaviour
{
    public TextMeshProUGUI chipsRemaining;
    // Start is called before the first frame update
    void Start()
    {
        Gull.Posters.Level.Attach<Gull.Events.Level.EatItem>(e => EatItem(e.Item));
        EatItem(null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator EatItem(Chip item)
    {
        chipsRemaining.text = LevelManager.instance.chips.Count + " CHIPS LEFT";
        return null;
    }
}


