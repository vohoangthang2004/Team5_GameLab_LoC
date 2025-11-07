using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBarScript : MonoBehaviour
{
    public Slider healthBarSlider;

    public int maxHealth;
    public int currHealth;

    void Start()
    {
        currHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthBarSlider.value = currHealth;
        healthBarSlider.maxValue = maxHealth;
    }
}
