using UnityEngine;
using System.Collections;
 
public class healthBar : MonoBehaviour {
       
        public int maxHealth;
        public int currentHealth;
        private Vector3 screenPosition;
        private float healthBarLength;
        private float maxHealthBarLength;
        private GUIStyle maxStyle;
        private GUIStyle currentStyle;
 
        void Start ()
        {
                //initialize size of the bar
                
                maxHealthBarLength = 75;
				healthBarLength = maxHealthBarLength;
                maxStyle = new GUIStyle();
                currentStyle = new GUIStyle();
               
                //create color fill
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.red);
                tex.Apply();
               
                //style fullsize health bar
                maxStyle.normal.background = tex;
                maxStyle.alignment = TextAnchor.MiddleCenter;
                maxStyle.normal.textColor = Color.white;
               
                //style remaining health bar
                Texture2D tex2 = new Texture2D(1, 1);
                tex2.SetPixel(1, 1, Color.green);
                tex2.Apply();
                currentStyle.normal.background = tex2;
        }
       
        void OnGUI ()
        {
                //position of the health bar onscreen
        screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        screenPosition.y = Screen.height - screenPosition.y;
               
               
                GUI.Box(new Rect(screenPosition.x - (maxHealthBarLength / 2), screenPosition.y - 40, maxHealthBarLength, 10), "", maxStyle);
                GUI.Box(new Rect(screenPosition.x - (maxHealthBarLength / 2), screenPosition.y - 40, healthBarLength, 10), "", currentStyle);
    }
       
        void Update ()
        {
               
        }
       
        public void AdjustCurrentHealth (int adj)
        {
            currentHealth = adj;
               
            if(currentHealth < 1)
                currentHealth = 0;
            if(currentHealth > maxHealth)
                currentHealth = maxHealth;
            if(maxHealth < 1)
                maxHealth = 1;
               
            healthBarLength = (float)maxHealthBarLength * (currentHealth / (float)maxHealth);
    }
}