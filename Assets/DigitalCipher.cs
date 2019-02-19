using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class DigitalCipher : MonoBehaviour {

    public KMAudio Audio;
    public KMBombInfo BombInfo;
    public KMBombModule BombModule;

    public KMSelectable[] Buttons;
    public TextMesh Input;
    public TextMesh Output;

    private static readonly Char[] Alphanumeric = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
   
    private string inputText;
    private string inputTextReversed;
    private string outputText;
    private string outputTextCalculated;

    private int timesPressed = 0;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].OnInteract += ButtonPressed(i);
        }
    }

    private KMSelectable.OnInteractHandler ButtonPressed(int buttonPressed)
    {
        return delegate ()
        {

            Buttons[buttonPressed].AddInteractionPunch();

            if (moduleSolved)
                return false;

            if (Array.IndexOf(Alphanumeric, Char.ToLowerInvariant(outputTextCalculated[timesPressed])) == buttonPressed)
            {
                Debug.LogFormat(@"[Digital Cipher #{0}] Tried to press {1} while {2} was expected", moduleId, Char.ToUpperInvariant(Alphanumeric[buttonPressed]), Char.ToUpperInvariant(Alphanumeric[Array.IndexOf(Alphanumeric, Char.ToLowerInvariant(outputTextCalculated[timesPressed]))]));
                outputText += Char.ToUpperInvariant(outputTextCalculated[timesPressed]).ToString();
                Output.text = outputText;
                timesPressed++;
                if (timesPressed == inputText.Length)
                {
                    BombModule.HandlePass();
                    Debug.LogFormat(@"[Digital Cipher #{0}] Module was solved. Well done", moduleId);
                    moduleSolved = true;
                }
            }
            else
            {
                BombModule.HandleStrike();
                Debug.LogFormat(@"[Digital Cipher #{0}] Tried to press {1} while {2} was expected. Strike", moduleId, Char.ToUpperInvariant(Alphanumeric[buttonPressed]), Char.ToUpperInvariant(Alphanumeric[Array.IndexOf(Alphanumeric, Char.ToLowerInvariant(outputTextCalculated[timesPressed]))]) );
            }

            return false;
        };
    }

    void Start () {

        moduleId = moduleIdCounter++;

        StartCoroutine(GenerateEverything());

    }

    private IEnumerator GenerateEverything()
    {

        for (int i = 0; i <= 14; i++)
        {
            inputText += Char.ToUpperInvariant(Alphanumeric[Random.Range(0, Alphanumeric.Length)]).ToString();
            yield return new WaitForSeconds(0.2f);
            Input.text = inputText;
        }
        
        char[] charArray = inputText.ToCharArray();
        Array.Reverse(charArray);
        inputTextReversed = new string(charArray);

        for (int i = 0; i < inputText.Length; i++)
        {
            int originalDigit = Array.IndexOf(Alphanumeric, Char.ToLowerInvariant(inputText[i])) + Array.IndexOf(Alphanumeric, Char.ToLowerInvariant(inputTextReversed[i]));
            int digitalRoot = 0;

            while (originalDigit > 0 || digitalRoot > 9)
            {
                if (originalDigit == 0)
                {
                    originalDigit = digitalRoot;
                    digitalRoot = 0;
                }

                digitalRoot += originalDigit % 10;
                originalDigit /= 10;
            }
            if(digitalRoot == 0)
                outputTextCalculated += Char.ToUpperInvariant(Alphanumeric[digitalRoot]).ToString();
            else
                outputTextCalculated += Char.ToUpperInvariant(Alphanumeric[digitalRoot-1]).ToString();
        }

        Debug.LogFormat(@"[Digital Cipher #{0}] Input String: {1} Expected String: {2}", moduleId, inputText, outputTextCalculated);

    }
}
