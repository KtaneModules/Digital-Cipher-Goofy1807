using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

    private bool activated = false;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].OnInteract += ButtonPressed(i);
        }
        BombModule.OnActivate += OnActivate;
    }

    private KMSelectable.OnInteractHandler ButtonPressed(int buttonPressed)
    {
        return delegate ()
        {

            Buttons[buttonPressed].AddInteractionPunch();

            if (moduleSolved || !activated)
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
                    StartCoroutine(solveAnim());
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

        //StartCoroutine(GenerateEverything());

    }

    void OnActivate()
    {
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

        activated = true;

    }

    private IEnumerator solveAnim()
    {
        yield return new WaitForSeconds(0.75f);
        for (int i = 1; i <= 15; i++)
        {
            Input.text = Input.text.Substring(0, Input.text.Length - 1);
            Output.text = Output.text.Substring(0, Output.text.Length - 1);
            yield return new WaitForSeconds(0.2f);
        }
    }

    //twitch plays
    private bool cmdIsValid(string s)
    {
        string[] things = { "A","B","C","D","E","F","G","H","I" };
        for(int i = 0; i < s.Length; i++)
        {
            string comp = s[i].ToString().ToUpper();
            bool good = false;
            for(int j = 0; j < things.Length; j++)
            {
                if (things[j].Equals(comp))
                {

                    good = true;
                    break;
                }
            }
            if(good == false)
            {
                return false;
            }
        }
        return true;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <letters> [Presses the specified string of letters] | Valid letters are A-I";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if(parameters.Length == 2)
            {
                if (cmdIsValid(parameters[1]))
                {
                    for(int i = 0; i < parameters[1].Length; i++)
                    {
                        if (parameters[1][i].Equals('A') || parameters[1][i].Equals('a'))
                        {
                            Buttons[0].OnInteract();
                        }
                        else if (parameters[1][i].Equals('B') || parameters[1][i].Equals('b'))
                        {
                            Buttons[1].OnInteract();
                        }
                        else if (parameters[1][i].Equals('C') || parameters[1][i].Equals('c'))
                        {
                            Buttons[2].OnInteract();
                        }
                        else if (parameters[1][i].Equals('D') || parameters[1][i].Equals('d'))
                        {
                            Buttons[3].OnInteract();
                        }
                        else if (parameters[1][i].Equals('E') || parameters[1][i].Equals('e'))
                        {
                            Buttons[4].OnInteract();
                        }
                        else if (parameters[1][i].Equals('F') || parameters[1][i].Equals('f'))
                        {
                            Buttons[5].OnInteract();
                        }
                        else if (parameters[1][i].Equals('G') || parameters[1][i].Equals('g'))
                        {
                            Buttons[6].OnInteract();
                        }
                        else if (parameters[1][i].Equals('H') || parameters[1][i].Equals('h'))
                        {
                            Buttons[7].OnInteract();
                        }
                        else if (parameters[1][i].Equals('I') || parameters[1][i].Equals('i'))
                        {
                            Buttons[8].OnInteract();
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else
                {
                    yield return "sendtochaterror The specified string of letters to press '" + parameters[1] + "' is invalid!";
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the string of letters to press!";
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (!activated) { yield return true; yield return new WaitForSeconds(0.1f); }
        char[] chars = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };
        for(int i = timesPressed; i < outputTextCalculated.Length; i++)
        {
            Buttons[Array.IndexOf(chars, outputTextCalculated[i])].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
