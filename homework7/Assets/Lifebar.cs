using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lifebar : MonoBehaviour
{
    float health;
    float transitionHealth;
    public Slider healthSlider;

    // Start is called before the first frame update
    void Start() {
        health = 100;
        transitionHealth = health;
    }

    void OnGUI() {
        if (GUI.Button(new Rect(50, 50, 50, 50), "+"))
        {
            transitionHealth += 10;
            if(transitionHealth > 100)
                transitionHealth=100;
        }
        if (GUI.Button(new Rect(50, 100, 50, 50), "-"))
        {
            transitionHealth -= 10;
            if(transitionHealth < 0)
                transitionHealth = 0;
        }
        Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + 2.2f, transform.position.z );
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Rect rect = new Rect(screenPos.x - 50, Screen.height - screenPos.y + 50,100,200);

        GUI.color = Color.red;
        health = Mathf.Lerp(health, transitionHealth, 0.5f);
        GUI.HorizontalScrollbar(rect, 0, health, 0, 100);
        healthSlider.value = health / 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
