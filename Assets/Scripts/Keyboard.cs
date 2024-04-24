using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Keyboard : MonoBehaviour
{
    public TMP_InputField m_inputField;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InsertChar(string c){

        m_inputField.text +=c;
    }
    public void DeleteChar(){
        if(m_inputField.text.Length>0){
            m_inputField.text = m_inputField.text.Substring(0, m_inputField.text.Length-1);
        }

    }
}
