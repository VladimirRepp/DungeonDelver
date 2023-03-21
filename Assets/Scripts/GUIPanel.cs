using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIPanel : MonoBehaviour
{
    [Header("Settings")]
    public Dray _dray;
    public Sprite _healthEmpty;
    public Sprite _healthHalf;
    public Sprite _healthFull;

    Text _keyCountText;
    List<Image> _healthImages;

    void Start()
    {
        Transform trans = transform.Find("Key Count");
        _keyCountText = trans.GetComponent<Text>();

        Transform healthPanel = transform.Find("Health Panel");
        _healthImages = new List<Image>();
        if(healthPanel != null)
        {
            for(int i = 0; i<20; i++)
            {
                trans = healthPanel.Find("H_" + i);

                if (trans == null)
                    break;

                _healthImages.Add(trans.GetComponent<Image>());
            }
        }
    }

    void Update()
    {
        _keyCountText.text = _dray._numKeys.ToString();

        int health = _dray.health;
        for(int i = 0; i<_healthImages.Count; i++)
        {
            if(health > 1)
            {
                _healthImages[i].sprite = _healthFull;
            }
            else if(health == 1)
            {
                _healthImages[i].sprite = _healthHalf;
            }
            else
            {
                _healthImages[i].sprite = _healthEmpty;
            }

            health -= 2;
        }
    }
}
