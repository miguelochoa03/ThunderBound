using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBehavior : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    [TextArea] public string firstText;
    [TextArea] public string secondText;
    public float typingSpeed = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TypeText());
    }
    IEnumerator TypeText()
    {
        yield return new WaitForSeconds(2.5f);

        textComponent.text = "";
        foreach (char c in firstText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(1.5f);

        textComponent.text = "";
        foreach (char c in secondText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(1.8f);
        textComponent.text = "";
    }
}
