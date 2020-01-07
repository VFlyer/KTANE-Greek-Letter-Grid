using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using KModkit;

public class greekLetterGridsScript : MonoBehaviour
{
    public KMAudio audio;
    public KMBombInfo bomb;

    public KMSelectable upButton;
    public KMSelectable downButton;
    public KMSelectable leftButton;
    public KMSelectable rightButton;
    public KMSelectable resetButton;
    public KMSelectable submitButton;
    public KMSelectable[] letters;
    private KMSelectable selectedLetter;
    string[] possibleLetters = { "A", "α", "B", "β", "Γ", "γ", "Δ", "δ", "Θ", "θ", "Λ", "λ", "Π", "π", "Σ", "σ", "Ω", "ω" };
    Color[] possibleColors = { new Color(1, 1, 1, 1), new Color(1, 0, 1, 1), new Color(1, 1, 0, 1), new Color(0, 1, 0, 1), new Color(0, 1, 1, 1) }; //Order of Colors: White, Magenta, Yellow, Green, Cyan
    float[] possibleXorZ = { -0.0375f, -0.0125f, 0.0125f, 0.0375f };
    /*
     *         X Axis Reference
     * -0.0375f, -0.0125f, 0.0125f, 0.0375f
     *    A          B        C        D
     *    
     *         Z Axis Reference
     * -0.0375f, -0.0125f, 0.0125f, 0.0375f
     *    4          3        2        1
     */
    float letter1InitialX;
    float letter1InitialZ;
    float letter2InitialX;
    float letter2InitialZ;
    float letter3InitialX;
    float letter3InitialZ;
    float letter1CurrentX;
    float letter1CurrentZ;
    float letter2CurrentX;
    float letter2CurrentZ;
    float letter3CurrentX;
    float letter3CurrentZ;
    float letter1CorrectX;
    float letter1CorrectZ;
    float letter2CorrectX;
    float letter2CorrectZ;
    float letter3CorrectX;
    float letter3CorrectZ;
    float selectedLetterX;
    float selectedLetterZ;
    string[] oddDigits = { "1", "3", "5", "7", "9" };
    string[] evenDigits = { "2", "4", "6", "8", "0" };
    string[] primeNumbers = { "2", "3", "5", "7" };
    string[] uppercaseLetters = { "A", "B", "Γ", "Δ", "Θ", "Λ", "Π", "Σ", "Ω" };
    string[] lowercaseLetters = { "α", "β", "γ", "δ", "θ", "λ", "π", "σ", "ω" };

    //Logging
    static int moduleIdCounter = 1;
    private int moduleId;
    private bool moduleSolved = false;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable letter in letters)
        {
            KMSelectable pressedLetter = letter;
            letter.OnInteract += delegate () { PressLetter(pressedLetter); return false; };
        }
        upButton.OnInteract += delegate () { PressUpButton(); return false; };
        downButton.OnInteract += delegate () { PressDownButton(); return false; };
        leftButton.OnInteract += delegate () { PressLeftButton(); return false; };
        rightButton.OnInteract += delegate () { PressRightButton(); return false; };
        resetButton.OnInteract += delegate () { PressResetButton(); return false; };
        submitButton.OnInteract += delegate () { Submit(); return false; };
    }
    // Use this for initialization
    void Start()
    {
        //Setting Randomized Colors & Letters
        //Holiday rules before full random
        if (System.DateTime.Now.Month == 1 && System.DateTime.Now.Day == 31) //The Mega Man X OVA "The Day of Sigma" was released on 01/31/06. All letters are now uppercase sigma. I'm sorry. I'm a Mega Man fanboy.
        {
            letters[0].GetComponent<TextMesh>().text = possibleLetters[14];
            letters[0].GetComponent<TextMesh>().color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            letters[1].GetComponent<TextMesh>().text = possibleLetters[14];
            letters[1].GetComponent<TextMesh>().color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            letters[2].GetComponent<TextMesh>().text = possibleLetters[14];
            letters[2].GetComponent<TextMesh>().color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
        }
        else if (System.DateTime.Now.Month == 2 && System.DateTime.Now.Day == 14) //Happy Valentine's Day! All letters are now magenta.
        {
            letters[0].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            letters[0].GetComponent<TextMesh>().color = possibleColors[1];
            letters[1].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            letters[1].GetComponent<TextMesh>().color = possibleColors[1];
            letters[2].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            letters[2].GetComponent<TextMesh>().color = possibleColors[1];
        }
        else if (System.DateTime.Now.Month == 3 && System.DateTime.Now.Day == 14) //Happy Pi Day! All letters are now upper/lowercase pi.
        {
            letters[0].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(12, 14)]; //Change the 12s back to possibleLetters.Length, as they are temporary.
            letters[0].GetComponent<TextMesh>().color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            letters[1].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(12, 14)];
            letters[1].GetComponent<TextMesh>().color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            letters[2].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(12, 14)];
            letters[2].GetComponent<TextMesh>().color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
        }
        else if (System.DateTime.Now.Month == 3 && System.DateTime.Now.Day == 17) //Happy St. Patrick's Day! All letters are now green.
        {
            letters[0].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            letters[0].GetComponent<TextMesh>().color = possibleColors[3];
            letters[1].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            letters[1].GetComponent<TextMesh>().color = possibleColors[3];
            letters[2].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            letters[2].GetComponent<TextMesh>().color = possibleColors[3];
        }
        else //Sigh, nothing special today... Everything is randomized. :(
        {
            letters[0].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            letters[0].GetComponent<TextMesh>().color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            letters[1].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            letters[1].GetComponent<TextMesh>().color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            letters[2].GetComponent<TextMesh>().text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            letters[2].GetComponent<TextMesh>().color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
        }

        //Setting Random Positions
        letters[0].transform.localPosition = new Vector3(possibleXorZ[UnityEngine.Random.Range(0, possibleXorZ.Length)], 0.013f, possibleXorZ[UnityEngine.Random.Range(0, possibleXorZ.Length)]);
        letters[1].transform.localPosition = new Vector3(possibleXorZ[UnityEngine.Random.Range(0, possibleXorZ.Length)], 0.013f, possibleXorZ[UnityEngine.Random.Range(0, possibleXorZ.Length)]);
        letters[2].transform.localPosition = new Vector3(possibleXorZ[UnityEngine.Random.Range(0, possibleXorZ.Length)], 0.013f, possibleXorZ[UnityEngine.Random.Range(0, possibleXorZ.Length)]);

        //Storing Initial Positions for Reset
        letter1InitialX = letters[0].transform.localPosition.x;
        letter1InitialZ = letters[0].transform.localPosition.z;
        letter2InitialX = letters[1].transform.localPosition.x;
        letter2InitialZ = letters[1].transform.localPosition.z;
        letter3InitialX = letters[2].transform.localPosition.x;
        letter3InitialZ = letters[2].transform.localPosition.z;

        //Determine Solution
        //Get Edgework Vars
        string serialNumber = bomb.GetSerialNumber();
        string serialNumberLastChar = serialNumber.Substring(serialNumber.Length - 1);
        Debug.Log("The last digit of the serial # is " + serialNumberLastChar);
        if (oddDigits.Contains(serialNumberLastChar))
        {
            Debug.Log("The last digit of the serial # is ODD");
        }
        else
        {
            Debug.Log("The last digit of the serial # is EVEN");
        }

        if (primeNumbers.Contains(serialNumberLastChar))
        {
            Debug.Log("The last digit of the serial # is PRIME");
        }
        else
        {
            Debug.Log("The last digit of the serial # is COMPOSITE");
        }

        //Check for each letter (e.g. if there is an uppercase alpha)
        for (int i = 0; i < letters.Length; i++)
        {
            switch (letters[i].GetComponent<TextMesh>().text)
            {
                case "A":
                    //If there is a lowercase sigma...
                    if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "σ" || letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text == "σ")
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #1 (Lowercase Sigma Detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #1 (Lowercase Sigma Detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #1 (Lowercase Sigma Detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the letter is yellow...
                    else if (letters[i].GetComponent<TextMesh>().color == new Color(1, 1, 0, 1))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #2 (This letter is yellow)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #2 (This letter is yellow)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #2 (This letter is yellow)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise of there is an unlit SND or an unlit IND...
                    else if (bomb.IsIndicatorOff("SND") || bomb.IsIndicatorOff("IND"))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #3 (Unlit SND or IND detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #3 (Unlit SND or IND detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #3 (Unlit SND or IND detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise if the other 2 letters are identical colors...
                    else if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color == letters[(i + 2) % letters.Length].GetComponent<TextMesh>().color)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #4 (Other 2 letters are identical colors)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #4 (Other 2 letters are identical colors)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #4 (Other 2 letters are identical colors)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = letter1InitialX;
                                letter1CorrectZ = letter1InitialZ;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = letter2InitialX;
                                letter2CorrectZ = letter2InitialZ;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = letter3InitialX;
                                letter3CorrectZ = letter3InitialZ;
                                Debug.Log("UPPERCASE ALPHA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "α":
                    //If initially in column D...
                    if ((i == 0 && letter1InitialX == 0.0375f) || (i == 1 && letter2InitialX == 0.0375f) || (i == 2 && letter3InitialX == 0.0375f))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = letter1InitialX -= 0.05f;
                                letter1CorrectZ = letter1InitialZ;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #1 (Initially in column D)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = letter2InitialX -= 0.05f;
                                letter2CorrectZ = letter2InitialZ;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #1 (Initially in column D)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = letter3InitialX -= 0.05f;
                                letter3CorrectZ = letter3InitialZ;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #1 (Initially in column D)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise if 3 or more batteries...
                    else if (bomb.GetBatteryCount() >= 3)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #2 (3+ batteries detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #2 (3+ batteries detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #2 (3+ batteries detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if this letter is green and an anycase delta is on the module...
                    else if (letters[i].GetComponent<TextMesh>().color == new Color(0, 0, 1, 1) && (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "Δ" || letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "δ" || letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text == "Δ" || letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text == "δ"))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #3 (this letter is green & delta detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #3 (this letter is green & delta detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #3 (this letter is green & delta detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there's a lit CLR...
                    else if (bomb.IsIndicatorOn("CLR"))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #4 (lit CLR detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #4 (lit CLR detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #4 (lit CLR detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE ALPHA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "B":
                    //If this letter is cyan...
                    if (letters[i].GetComponent<TextMesh>().color == new Color(0, 1, 1, 1))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE BETA CONDITION: #1 (this letter is cyan)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE BETA CONDITION: #1 (this letter is cyan)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE BETA CONDITION: #1 (this letter is cyan)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if this letter was initially found in row 3...
                    else if ((i == 0 && letter1InitialZ == -0.0125f) || (i == 1 && letter2InitialZ == -0.0125f) || (i == 2 && letter3InitialZ == -0.0125f))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = letter1InitialX;
                                letter1CorrectZ = letter1InitialZ += 0.025f;
                                Debug.Log("UPPERCASE BETA CONDITION: #2 (this letter was found in row 3)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = letter2InitialX;
                                letter2CorrectZ = letter2InitialZ += 0.025f;
                                Debug.Log("UPPERCASE BETA CONDITION: #2 (this letter was found in row 3)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = letter3InitialX;
                                letter3CorrectZ = letter3InitialZ += 0.025f;
                                Debug.Log("UPPERCASE BETA CONDITION: #2 (this letter was found in row 3)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the last char of serial # is odd & there is a lowercase letter...
                    else if (oddDigits.Contains(serialNumberLastChar) && (lowercaseLetters.Contains(letters[i].GetComponent<TextMesh>().text) || lowercaseLetters.Contains(letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text) || lowercaseLetters.Contains(letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text)))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE BETA CONDITION: #3 (last digit of the serial number is odd and there is a lowercase letter)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE BETA CONDITION: #3 (last digit of the serial number is odd and there is a lowercase letter)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE BETA CONDITION: #3 (last digit of the serial number is odd and there is a lowercase letter)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there is a a green uppercase letter...
                    else if ((letters[i].GetComponent<TextMesh>().color == new Color(0, 1, 0, 1)) || (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color == new Color(0, 1, 0, 1) && uppercaseLetters.Contains(letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text)) || (letters[(i + 2) % letters.Length].GetComponent<TextMesh>().color == new Color(0, 1, 0, 1) && uppercaseLetters.Contains(letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text)))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE BETA CONDITION: #4 (green uppercase detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE BETA CONDITION: #4 (green uppercase detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE BETA CONDITION: #4 (green uppercase detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE BETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE BETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE BETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "β":
                    //If this letter starts in A3...
                    if ((i == 0 && letter1InitialX == -0.0375f && letter1InitialZ == -0.0125f) || (i == 1 && letter2InitialX == -0.0375f && letter2InitialZ == -0.0125f) || (i == 2 && letter3InitialX == -0.0375f && letter3InitialZ == -0.0125f))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE BETA CONDITION: #1 (this letter started in A3)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE BETA CONDITION: #1 (this letter started in A3)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE BETA CONDITION: #1 (this letter started in A3)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if a letter is white and was initially found in the A column...
                    else if ((letter1InitialX == -0.0375f && letters[0].GetComponent<TextMesh>().color == new Color(1, 1, 1, 1)) || (letter2InitialX == -0.0375f && letters[1].GetComponent<TextMesh>().color == new Color(1, 1, 1, 1)) || (letter3InitialX == -0.0375f && letters[2].GetComponent<TextMesh>().color == new Color(1, 1, 1, 1)))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE BETA CONDITION: #2 (white letter was found in the A column)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE BETA CONDITION: #2 (white letter was found in the A column)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE BETA CONDITION: #2 (white letter was found in the A column)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if this letter is magenta or cyan...
                    else if (letters[i].GetComponent<TextMesh>().color == new Color(1, 0, 1, 1) || letters[i].GetComponent<TextMesh>().color == new Color(0, 1, 1, 1))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE BETA CONDITION: #3 (this letter is magenta/cyan)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE BETA CONDITION: #3 (this letter is magenta/cyan)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE BETA CONDITION: #3 (this letter is magenta/cyan)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there is a DVI and no RJ45...
                    else if (bomb.IsPortPresent(Port.DVI) && !bomb.IsPortPresent(Port.RJ45))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE BETA CONDITION: #4 (DVI and no RJ45 detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE BETA CONDITION: #4 (DVI and no RJ45 detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE BETA CONDITION: #4 (DVI and no RJ45 detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE BETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE BETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE BETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "Γ":
                    //If the serial number contains a letter that is in the first half of the alphabet...
                    if (bomb.GetSerialNumberLetters().Any(x => x == 'A' || x == 'B' || x == 'C' || x == 'D' || x == 'E' || x == 'F' || x == 'G' || x == 'H' || x == 'I' || x == 'J' || x == 'K' || x == 'L' || x == 'M'))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #1 (A letter in the serial number is found in the first half of the English Alphabet)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #1 (A letter in the serial number is found in the first half of the English Alphabet)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #1 (A letter in the serial number is found in the first half of the English Alphabet)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there is an empty port plate...
                    else if (bomb.GetPortPlates().Any(x => x.Length == 0))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #2 (empty port plate detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #2 (empty port plate detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #2 (empty port plate detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if all letters are magenta...
                    else if (letters[i].GetComponent<TextMesh>().color == new Color(1, 0, 1, 1) && letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color == new Color(1, 0, 1, 1) && letters[(i + 2) % letters.Length].GetComponent<TextMesh>().color == new Color(1, 0, 1, 1))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #3 (all letters are magenta)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #3 (all letters are magenta)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #3 (all letters are magenta)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if this letter is cyan and starts in row 4...
                    else if (letters[i].GetComponent<TextMesh>().color == new Color(0, 1, 1, 1) && ((i == 0 && letter1CurrentZ == -0.0375f) || (i == 1 && letter2CurrentZ == -0.0375f) || (i == 2 && letter3CurrentZ == -0.0375f)))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = letter1InitialX;
                                letter1CorrectZ = letter1InitialZ += 0.05f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #1 (this letter is cyan and started in row 4)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = letter2InitialX;
                                letter2CorrectZ = letter2InitialZ += 0.05f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #1 (this letter is cyan and started in row 4)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = letter3InitialX;
                                letter3CorrectZ = letter3InitialZ += 0.05f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #1 (this letter is cyan and started in row 4)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE GAMMA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "γ":
                    //If anycase anycolor theta is present...
                    if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "Θ" || letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "θ" || letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text == "Θ" || letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text == "θ")
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #1 (theta detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #1 (theta detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #1 (theta detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if not cyan and serial number contains vowel...
                    else if (letters[i].GetComponent<TextMesh>().color != new Color(0, 1, 1, 1) && bomb.GetSerialNumberLetters().Any(x => x == 'A' || x == 'E' || x == 'I' || x == 'O' || x == 'U'))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #2 (this letter is NOT cyan and vowel detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #2 (this letter is NOT cyan and vowel detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #2 (this letter is NOT cyan and vowel detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if this letter starts in column C...
                    else if ((i == 0 && letter1InitialX == 0.0125f) || (i == 1 && letter2InitialX == 0.0125f) || (i == 2 && letter3InitialX == 0.0125f))
                    {
                        if (int.Parse(serialNumberLastChar) <= 4) //If the last digit of the serial number is less than or equal to 4...
                        {
                            switch (i)
                            {
                                case 0:
                                    letter1CorrectX = letter1InitialX -= 0.05f;
                                    letter1CorrectZ = letter1InitialZ;
                                    Debug.Log("LOWERCASE GAMMA CONDITION: #2A (this letter starts in column C & the last digit of the serial number is less than or equal to 4.)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    letter2CorrectX = letter2InitialX -= 0.05f;
                                    letter2CorrectZ = letter2InitialZ;
                                    Debug.Log("LOWERCASE GAMMA CONDITION: #2A (this letter starts in column C & the last digit of the serial number is less than or equal to 4.)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    letter3CorrectX = letter3InitialX -= 0.05f;
                                    letter3CorrectZ = letter3InitialZ;
                                    Debug.Log("LOWERCASE GAMMA CONDITION: #2A (this letter starts in column C & the last digit of the serial number is less than or equal to 4.)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                        else //If the last digit of the serial number is greater than 4...
                        {
                            switch (i)
                            {
                                case 0:
                                    letter1CorrectX = letter1InitialX -= 0.025f;
                                    letter1CorrectZ = letter1InitialZ;
                                    Debug.Log("LOWERCASE GAMMA CONDITION: #2B (this letter starts in column C & the last digit of the serial number is greater than 4.)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    letter2CorrectX = letter2InitialX -= 0.025f;
                                    letter2CorrectZ = letter2InitialZ;
                                    Debug.Log("LOWERCASE GAMMA CONDITION: #2B (this letter starts in column C & the last digit of the serial number is greater than 4.)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    letter3CorrectX = letter3InitialX -= 0.025f;
                                    letter3CorrectZ = letter3InitialZ;
                                    Debug.Log("LOWERCASE GAMMA CONDITION: #2B (this letter starts in column C & the last digit of the serial number is greater than 4.)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                    }
                    //Otherwise, if the last digit of the serial number is prime...
                    else if (primeNumbers.ToString().Contains(serialNumberLastChar))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #4 (last digit of the serial number is prime)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #4 (last digit of the serial number is prime)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #4 (last digit of the serial number is prime)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE GAMMA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "Δ":
                    //If there is a stereo RCA port...
                    if (bomb.IsPortPresent(Port.StereoRCA))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #1 (stereo RCA detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #1 (stereo RCA detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #1 (stereo RCA detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the number of batteries is greater than the last digit of the serial number...
                    else if (bomb.GetBatteryCount() > int.Parse(serialNumberLastChar))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #2 (battery count > last digit in serial number)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #2 (battery count > last digit in serial number)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #2 (battery count > last digit in serial number)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if you have solved half or over half of the modules on this bomb...
                    else if (letters[i].GetComponent<TextMesh>().color != new Color(1, 0, 1, 1) && letters[i].GetComponent<TextMesh>().color != new Color(1, 1, 0, 1))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #3 (this letter is neither yellow nor magenta)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #3 (this letter is neither yellow nor magenta)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #3 (this letter is neither yellow nor magenta)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there is a green lowercase omega...
                    else if ((letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "ω" && letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color == new Color(0, 1, 0, 1)) || (letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text == "ω" && letters[(i + 2) % letters.Length].GetComponent<TextMesh>().color == new Color(0, 1, 0, 1)))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #4 (green lowercase omega detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #4 (green lowercase omega detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #4 (green lowercase omega detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE DELTA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "δ":
                    //If this is the only lowercase letter on the module...
                    if (uppercaseLetters.Contains(letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text) && uppercaseLetters.Contains(letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #1 (this letter is the only lowercase letter on the module)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #1 (this letter is the only lowercase letter on the module)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #1 (this letter is the only lowercase letter on the module)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the number of solved modules + the last digit of the serial number > 10...
                    else if (bomb.GetSolvedModuleNames().Count + int.Parse(serialNumberLastChar) > 10)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #2 (solved modules + last digit > 10)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #2 (solved modules + last digit > 10)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #2 (solved modules + last digit > 10)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if any letter starts in A3...
                    else if ((letter1InitialX == -0.0375f && letter1InitialZ == -0.0125f) || (letter2InitialX == -0.0375f && letter2InitialZ == -0.0125f) || (letter3InitialX == -0.0375f && letter3InitialZ == -0.0125f))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #3 (a letter starts in A3)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #3 (a letter starts in A3)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #3 (a letter starts in A3)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if this letter is the only cyan letter on the module...
                    else if (letters[i].GetComponent<TextMesh>().color == new Color(0, 1, 1, 1) && (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color != new Color(0, 1, 1, 1)) && (letters[(i + 2) % letters.Length].GetComponent<TextMesh>().color != new Color(0, 1, 1, 1)))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #4 (this letter is cyan and the other letters are not)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #4 (this letter is cyan and the other letters are not)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #4 (this letter is cyan and the other letters are not)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE DELTA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "Θ":
                    //If the number of D batteries is greater than the number of AA batteries...
                    if (bomb.GetBatteryCount(Battery.D) > bomb.GetBatteryCount(Battery.AA))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE THETA CONDITION: #1 (D batteries outnumber AA)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE THETA CONDITION: #1 (D batteries outnumber AA)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE THETA CONDITION: #1 (D batteries outnumber AA)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there is an unlit CAR indicator...
                    else if (bomb.IsIndicatorOff("CAR"))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                switch (int.Parse(serialNumberLastChar) % 4)
                                {
                                    case 0:
                                        letter1CorrectZ = 0.0375f;
                                        break;
                                    case 1:
                                        letter1CorrectZ = 0.0125f;
                                        break;
                                    case 2:
                                        letter1CorrectZ = -0.0125f;
                                        break;
                                    case 3:
                                        letter1CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("UPPERCASE THETA CONDITION: #2 (unlit CAR detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                switch (int.Parse(serialNumberLastChar) % 4)
                                {
                                    case 0:
                                        letter2CorrectZ = 0.0375f;
                                        break;
                                    case 1:
                                        letter2CorrectZ = 0.0125f;
                                        break;
                                    case 2:
                                        letter2CorrectZ = -0.0125f;
                                        break;
                                    case 3:
                                        letter2CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("UPPERCASE THETA CONDITION: #2 (unlit CAR detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                switch (int.Parse(serialNumberLastChar) % 4)
                                {
                                    case 0:
                                        letter3CorrectZ = 0.0375f;
                                        break;
                                    case 1:
                                        letter3CorrectZ = 0.0125f;
                                        break;
                                    case 2:
                                        letter3CorrectZ = -0.0125f;
                                        break;
                                    case 3:
                                        letter3CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("UPPERCASE THETA CONDITION: #2 (unlit CAR detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the colors of all 3 letters are unique...
                    else if (letters[i].GetComponent<TextMesh>().color != letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color && letters[i].GetComponent<TextMesh>().color != letters[(i + 2) % letters.Length].GetComponent<TextMesh>().color && letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color != letters[(i + 2) % letters.Length].GetComponent<TextMesh>().color)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                switch (bomb.GetPortPlateCount() % 4)
                                {
                                    case 0:
                                        letter1CorrectZ = 0.0375f;
                                        break;
                                    case 1:
                                        letter1CorrectZ = 0.0125f;
                                        break;
                                    case 2:
                                        letter1CorrectZ = -0.0125f;
                                        break;
                                    case 3:
                                        letter1CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("UPPERCASE THETA CONDITION: #3 (all colors are unique)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                switch (bomb.GetPortPlateCount() % 4)
                                {
                                    case 0:
                                        letter2CorrectZ = 0.0375f;
                                        break;
                                    case 1:
                                        letter2CorrectZ = 0.0125f;
                                        break;
                                    case 2:
                                        letter2CorrectZ = -0.0125f;
                                        break;
                                    case 3:
                                        letter2CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("UPPERCASE THETA CONDITION: #3 (all colors are unique)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                switch (bomb.GetPortPlateCount() % 4)
                                {
                                    case 0:
                                        letter3CorrectZ = 0.0375f;
                                        break;
                                    case 1:
                                        letter3CorrectZ = 0.0125f;
                                        break;
                                    case 2:
                                        letter3CorrectZ = -0.0125f;
                                        break;
                                    case 3:
                                        letter3CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("UPPERCASE THETA CONDITION: #3 (all colors are unique)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there is also a lowercase theta on the module...
                    else if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "θ" || letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text == "θ")
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE THETA CONDITION: #4 (lowercase theta detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE THETA CONDITION: #4 (lowercase theta detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE THETA CONDITION: #4 (lowercase theta detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE THETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE THETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE THETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "θ":
                    //If this letter starts in a row unique to the other 2 letters...
                    if ((i == 0 && letter1InitialZ != letter2InitialZ && letter1InitialZ != letter3InitialZ) || (i == 1 && letter2InitialZ != letter1InitialZ && letter2InitialZ != letter3InitialZ) || (i == 2 && letter3InitialZ != letter1InitialZ && letter3InitialZ != letter2InitialZ))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE THETA CONDITION: #1 (initial row is unique)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE THETA CONDITION: #1 (initial row is unique)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE THETA CONDITION: #1 (initial row is unique)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if this letter is white...
                    else if (letters[i].GetComponent<TextMesh>().color == new Color(1, 1, 1, 1))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                switch (bomb.GetStrikes() % 4)
                                {
                                    case 0:
                                        letter1CorrectZ = 0.0375f;
                                        break;
                                    case 1:
                                        letter1CorrectZ = 0.0125f;
                                        break;
                                    case 2:
                                        letter1CorrectZ = -0.0125f;
                                        break;
                                    case 3:
                                        letter1CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE THETA CONDITION: #2 (this letter is white)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                switch (bomb.GetStrikes() % 4)
                                {
                                    case 0:
                                        letter2CorrectZ = 0.0375f;
                                        break;
                                    case 1:
                                        letter2CorrectZ = 0.0125f;
                                        break;
                                    case 2:
                                        letter2CorrectZ = -0.0125f;
                                        break;
                                    case 3:
                                        letter2CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE THETA CONDITION: #2 (this letter is white)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                switch (bomb.GetStrikes() % 4)
                                {
                                    case 0:
                                        letter3CorrectZ = 0.0375f;
                                        break;
                                    case 1:
                                        letter3CorrectZ = 0.0125f;
                                        break;
                                    case 2:
                                        letter3CorrectZ = -0.0125f;
                                        break;
                                    case 3:
                                        letter3CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE THETA CONDITION: #2 (this letter is white)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the number of solved modules is less than or equal to the number of current strikes...
                    else if (bomb.GetSolvedModuleNames().Count <= bomb.GetStrikes())
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = letter1InitialX;
                                letter1CorrectZ = letter1InitialZ;
                                Debug.Log("LOWERCASE THETA CONDITION: #3 (solved modules <= strikes)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = letter2InitialX;
                                letter2CorrectZ = letter2InitialZ;
                                Debug.Log("LOWERCASE THETA CONDITION: #3 (solved modules <= strikes)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = letter3InitialX;
                                letter3CorrectZ = letter3InitialZ;
                                Debug.Log("LOWERCASE THETA CONDITION: #3 (solved modules <= strikes)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there is a PS/2 port or any duplicate ports of any type...
                    else if (bomb.IsPortPresent(Port.PS2) || bomb.IsDuplicatePortPresent())
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE THETA CONDITION: #4 (PS/2 detected OR duplicate port detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE THETA CONDITION: #4 (PS/2 detected OR duplicate port detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE THETA CONDITION: #4 (PS/2 detected OR duplicate port detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                if ((Mathf.Floor(bomb.GetTime()) / 60) % 2 == 1) //Only correct if minute is odd
                                {
                                    letter1CorrectX = -0.375f;
                                    letter1CorrectZ = 0.0375f;
                                }
                                Debug.Log("LOWERCASE THETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                if ((Mathf.Floor(bomb.GetTime()) / 60) % 2 == 1) //Only correct if minute is odd
                                {
                                    letter2CorrectX = -0.375f;
                                    letter2CorrectZ = 0.0375f;
                                }
                                Debug.Log("LOWERCASE THETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                if ((Mathf.Floor(bomb.GetTime()) / 60) % 2 == 1) //Only correct if minute is odd
                                {
                                    letter3CorrectX = -0.375f;
                                    letter3CorrectZ = 0.0375f;
                                }
                                Debug.Log("LOWERCASE THETA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "Λ":
                    //If the serial number contains an A...
                    if (bomb.GetSerialNumberLetters().Any(x => x == 'A'))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #1 (letter A detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #1 (letter A detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #1 (letter A detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the serial number contains a B...
                    else if (bomb.GetSerialNumberLetters().Any(x => x == 'B'))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #2 (letter B detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #2 (letter B detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #2 (letter B detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the serial number contains a D...
                    else if (bomb.GetSerialNumberLetters().Any(x => x == 'D'))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #3 (letter D detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #3 (letter D detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #3 (letter D detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise if the serial number contains either an L or an M...
                    else if (bomb.GetSerialNumberLetters().Any(x => x == 'L' || x == 'M'))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #4 (letter L/M detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #4 (letter L/M detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = 0.0375f;

                                Debug.Log("UPPERCASE LAMBDA CONDITION: #4 (letter L/M detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE LAMBDA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = -0.0375f;

                                Debug.Log("UPPERCASE LAMBDA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "λ":
                    //If there are 2 or more lit indicators...
                    if (bomb.GetOnIndicators().Count() >= 2)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #1 (lit indicator count >= 2)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #1 (lit indicator count >= 2)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #1 (lit indicator count >= 2)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if this letter shares a color with only 1 other letter on the module...
                    else if (letters[i].GetComponent<TextMesh>().color == letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color || letters[i].GetComponent<TextMesh>().color == letters[(i + 2) % letters.Length].GetComponent<TextMesh>().color)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #2 (shared color with only 1 letter)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #2 (shared color with only 1 letter)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #2 (shared color with only 1 letter)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there are more batteries than port plates...
                    else if (bomb.GetBatteryCount() > bomb.GetPortPlateCount())
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #3 (battery count > port plate count)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #3 (battery count > port plate count)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #3 (battery count > port plate count)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if one letter on the module is yellow (not this letter)...
                    else if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color == new Color(1, 1, 0, 1) || letters[(i + 2) % letters.Length].GetComponent<TextMesh>().color == new Color(1, 1, 0, 1))
                    {
                        switch (i)
                        {
                            case 0:
                                if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color == new Color(1, 1, 0, 1))
                                {
                                    letter1CorrectX = letter2CorrectX;
                                    letter1CorrectZ = letter2CorrectZ;
                                }
                                else
                                {
                                    letter1CorrectX = letter3CorrectX;
                                    letter1CorrectZ = letter3CorrectZ;
                                }
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #4 (one letter is yellow)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color == new Color(1, 1, 0, 1))
                                {
                                    letter2CorrectX = letter3CorrectX;
                                    letter2CorrectZ = letter3CorrectZ;
                                }
                                else
                                {
                                    letter2CorrectX = letter1CorrectX;
                                    letter2CorrectZ = letter1CorrectZ;
                                }
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #4 (one letter is yellow)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color == new Color(1, 1, 0, 1))
                                {
                                    letter3CorrectX = letter1CorrectX;
                                    letter3CorrectZ = letter1CorrectZ;
                                }
                                else
                                {
                                    letter3CorrectX = letter2CorrectX;
                                    letter3CorrectZ = letter2CorrectZ;
                                }
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #4 (one letter is yellow)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("LOWERCASE LAMBDA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = 0.0375f;

                                Debug.Log("LOWERCASE LAMBDA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "Π":
                    //If it's Pi Day...
                    if (System.DateTime.Now.Month == 3 && System.DateTime.Now.Day == 14)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = letter1InitialX;
                                letter1CorrectZ = letter1InitialZ;
                                Debug.Log("UPPERCASE PI CONDITION: #1 (Happy Pi Day!)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = letter2InitialX;
                                letter2CorrectZ = letter2InitialZ;
                                Debug.Log("UPPERCASE PI CONDITION: #1 (Happy Pi Day!)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = letter3InitialX;
                                letter3CorrectZ = letter3InitialZ;
                                Debug.Log("UPPERCASE PI CONDITION: #1 (Happy Pi Day!)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there is either a P or an I in the serial number...
                    else if (bomb.GetSerialNumberLetters().Any(x => x == 'P' || x == 'I'))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE PI CONDITION: #2 (serial number P or I detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE PI CONDITION: #2 (serial number P or I detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE PI CONDITION: #2 (serial number P or I detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if this letter starts in the same row as another letter...
                    else if ((i == 0 && letter1InitialZ == letter2InitialZ || letter1InitialZ == letter3InitialZ) || (i == 1 && letter2InitialZ == letter1InitialZ || letter2InitialZ == letter3InitialZ) || (i == 2 && letter3InitialZ == letter2InitialZ || letter3InitialZ == letter1InitialZ))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE PI CONDITION: #3 (initial row is shared)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE PI CONDITION: #3 (initial row is shared)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE PI CONDITION: #3 (initial row is shared)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if this letter starts in one of the corners of the grid...
                    else if ((i == 0 && Math.Abs(letter1InitialX) == 0.0375f && Math.Abs(letter1InitialZ) == 0.0375f) || (i == 1 && Math.Abs(letter2InitialX) == 0.0375f && Math.Abs(letter2InitialZ) == 0.0375f) || (i == 2 && Math.Abs(letter3InitialX) == 0.0375f && Math.Abs(letter3InitialZ) == 0.0375f))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE PI CONDITION: #4 (initially in corner)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE PI CONDITION: #4 (initially in corner)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE PI CONDITION: #4 (initially in corner)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    letter1CorrectX = -0.375f;
                                    switch (((int)bomb.GetTime() / 60) % 4)
                                    {
                                        case 0:
                                            letter1CorrectZ = 0.0375f;
                                            break;
                                        case 1:
                                            letter1CorrectZ = 0.0125f;
                                            break;
                                        case 2:
                                            letter1CorrectZ = -0.0125f;
                                            break;
                                        case 3:
                                            letter1CorrectZ = -0.0375f;
                                            break;
                                    }
                                }
                                Debug.Log("UPPERCASE PI CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                if ((Mathf.Floor(bomb.GetTime()) / 60) % 2 == 1) //Only correct if minute is odd
                                {
                                    letter2CorrectX = -0.375f;
                                    switch (((int)bomb.GetTime() / 60) % 4)
                                    {
                                        case 0:
                                            letter2CorrectZ = 0.0375f;
                                            break;
                                        case 1:
                                            letter2CorrectZ = 0.0125f;
                                            break;
                                        case 2:
                                            letter2CorrectZ = -0.0125f;
                                            break;
                                        case 3:
                                            letter2CorrectZ = -0.0375f;
                                            break;
                                    }
                                }
                                Debug.Log("UPPERCASE PI CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                if ((Mathf.Floor(bomb.GetTime()) / 60) % 2 == 1) //Only correct if minute is odd
                                {
                                    letter3CorrectX = -0.375f;
                                    switch (((int)bomb.GetTime() / 60) % 4)
                                    {
                                        case 0:
                                            letter3CorrectZ = 0.0375f;
                                            break;
                                        case 1:
                                            letter3CorrectZ = 0.0125f;
                                            break;
                                        case 2:
                                            letter3CorrectZ = -0.0125f;
                                            break;
                                        case 3:
                                            letter3CorrectZ = -0.0375f;
                                            break;
                                    }
                                }
                                Debug.Log("UPPERCASE PI CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "π":
                    //If the last digit of the serial number is either 3, 1, or 4...
                    if (int.Parse(serialNumberLastChar) == 3 || int.Parse(serialNumberLastChar) == 1 || int.Parse(serialNumberLastChar) == 4)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = letter1InitialX;
                                letter1CorrectZ = letter1InitialZ;
                                Debug.Log("LOWERCASE PI CONDITION: #1 (serial number is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = letter2InitialX;
                                letter2CorrectZ = letter2InitialZ;
                                Debug.Log("LOWERCASE PI CONDITION: #1 (serial number is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = letter3InitialX;
                                letter3CorrectZ = letter3InitialZ;
                                Debug.Log("LOWERCASE PI CONDITION: #1 (serial number is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the number of battery holders is either 3, 1, or 4...
                    else if (bomb.GetBatteryHolderCount() == 3 || bomb.GetBatteryHolderCount() == 1 || bomb.GetBatteryHolderCount() == 4)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                switch (bomb.GetBatteryHolderCount())
                                {
                                    case 1:
                                        letter1CorrectZ = 0.0375f;
                                        break;
                                    case 3:
                                        letter1CorrectZ = -0.0125f;
                                        break;
                                    case 4:
                                        letter1CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE PI CONDITION: #2 (battery holder count is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                switch (bomb.GetBatteryHolderCount())
                                {
                                    case 1:
                                        letter2CorrectZ = 0.0375f;
                                        break;
                                    case 3:
                                        letter2CorrectZ = -0.0125f;
                                        break;
                                    case 4:
                                        letter2CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE PI CONDITION: #2 (battery holder count is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                switch (bomb.GetBatteryHolderCount())
                                {
                                    case 1:
                                        letter3CorrectZ = 0.0375f;
                                        break;
                                    case 3:
                                        letter3CorrectZ = -0.0125f;
                                        break;
                                    case 4:
                                        letter3CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE PI CONDITION: #2 (battery holder count is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the number of indicators (both lit and unlit) is either 3, 1, or 4...
                    else if (bomb.GetIndicators().Count() == 3 || bomb.GetIndicators().Count() == 1 || bomb.GetIndicators().Count() == 4)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                switch (bomb.GetBatteryHolderCount())
                                {
                                    case 1:
                                        letter1CorrectZ = 0.0375f;
                                        break;
                                    case 3:
                                        letter1CorrectZ = -0.0125f;
                                        break;
                                    case 4:
                                        letter1CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE PI CONDITION: #3 (indicator count is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                switch (bomb.GetBatteryHolderCount())
                                {
                                    case 1:
                                        letter2CorrectZ = 0.0375f;
                                        break;
                                    case 3:
                                        letter2CorrectZ = -0.0125f;
                                        break;
                                    case 4:
                                        letter2CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE PI CONDITION: #3 (indicator count is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                switch (bomb.GetBatteryHolderCount())
                                {
                                    case 1:
                                        letter3CorrectZ = 0.0375f;
                                        break;
                                    case 3:
                                        letter3CorrectZ = -0.0125f;
                                        break;
                                    case 4:
                                        letter3CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE PI CONDITION: #3 (indicator count is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the number of solved modules is either 3, 1, or 4...
                    else if (bomb.GetSolvedModuleNames().Count() == 3 || bomb.GetSolvedModuleNames().Count() == 1 || bomb.GetSolvedModuleNames().Count() == 4)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                switch (bomb.GetBatteryHolderCount())
                                {
                                    case 1:
                                        letter1CorrectZ = 0.0375f;
                                        break;
                                    case 3:
                                        letter1CorrectZ = -0.0125f;
                                        break;
                                    case 4:
                                        letter1CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE PI CONDITION: #4 (solved module count is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                switch (bomb.GetBatteryHolderCount())
                                {
                                    case 1:
                                        letter2CorrectZ = 0.0375f;
                                        break;
                                    case 3:
                                        letter2CorrectZ = -0.0125f;
                                        break;
                                    case 4:
                                        letter2CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE PI CONDITION: #4 (solved module count is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                switch (bomb.GetBatteryHolderCount())
                                {
                                    case 1:
                                        letter3CorrectZ = 0.0375f;
                                        break;
                                    case 3:
                                        letter3CorrectZ = -0.0125f;
                                        break;
                                    case 4:
                                        letter3CorrectZ = -0.0375f;
                                        break;
                                }
                                Debug.Log("LOWERCASE PI CONDITION: #4 (solved module count is 3, 1, or 4)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE PI CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE PI CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE PI CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "Σ":
                    if (!bomb.IsIndicatorOn("SIG"))
                    {
                        //If the number of batteries plus the number of current strikes is greater than or equal to 5...
                        if (bomb.GetBatteryCount() + bomb.GetStrikes() >= 5)
                        {
                            switch (i)
                            {
                                case 0:
                                    letter1CorrectX = -0.0375f;
                                    letter1CorrectZ = -0.0375f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #1 (battery count + strike count >= 5)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    letter2CorrectX = -0.0375f;
                                    letter2CorrectZ = -0.0375f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #1 (battery count + strike count >= 5)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    letter3CorrectX = -0.0375f;
                                    letter3CorrectZ = -0.0375f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #1 (battery count + strike count >= 5)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                        //Otherwise, if the number of lit indicators plus the number of port plates is greater than or equal to 5...
                        else if (bomb.GetOnIndicators().Count() + bomb.GetPortPlateCount() >= 5)
                        {
                            switch (i)
                            {
                                case 0:
                                    letter1CorrectX = 0.0125f;
                                    letter1CorrectZ = -0.0125f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #2 (lit indicator count + port plate count >= 5)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    letter2CorrectX = 0.0125f;
                                    letter2CorrectZ = -0.0125f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #2 (lit indicator count + port plate count >= 5)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    letter3CorrectX = 0.0125f;
                                    letter3CorrectZ = -0.0125f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #2 (lit indicator count + port plate count >= 5)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                        //Otherwise, if the number of unlit indicators plus the last digit of the serial number is greater than or equal to 5...
                        else if (bomb.GetOffIndicators().Count() + int.Parse(serialNumberLastChar) >= 5)
                        {
                            switch (i)
                            {
                                case 0:
                                    letter1CorrectX = -0.0125f;
                                    letter1CorrectZ = 0.0375f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #3 (unlit indicator count + last digit of serial number >= 5)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    letter2CorrectX = -0.0125f;
                                    letter2CorrectZ = 0.0375f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #3 (unlit indicator count + last digit of serial number >= 5)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    letter3CorrectX = -0.0125f;
                                    letter3CorrectZ = 0.0375f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #3 (unlit indicator count + last digit of serial number >= 5)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                        //Otherwise, if the number of solved modules plus the number of battery holders is greater than or equal to 5...
                        else if (bomb.GetSolvedModuleNames().Count() + bomb.GetBatteryHolderCount() >= 5)
                        {
                            switch (i)
                            {
                                case 0:
                                    letter1CorrectX = 0.0375f;
                                    letter1CorrectZ = 0.0375f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #4 (solved module count + battery holder count >= 5)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    letter2CorrectX = 0.0375f;
                                    letter2CorrectZ = 0.0375f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #4 (solved module count + battery holder count >= 5)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    letter3CorrectX = 0.0375f;
                                    letter3CorrectZ = 0.0375f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #4 (solved module count + battery holder count >= 5)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                        //Otherwise, if nothing applies...
                        else
                        {
                            switch (i)
                            {
                                case 0:
                                    letter1CorrectX = -0.0125f;
                                    letter1CorrectZ = -0.0125f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #5 (N/A)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    letter2CorrectX = -0.0125f;
                                    letter2CorrectZ = -0.0125f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #5 (N/A)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    letter3CorrectX = -0.0125f;
                                    letter3CorrectZ = -0.0125f;
                                    Debug.Log("UPPERCASE SIGMA CONDITION: #5 (N/A)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                        break;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = letter1InitialX;
                                letter1CorrectZ = letter1InitialZ;
                                Debug.Log("UPPERCASE SIGMA CONDITION: #EX (SIG is present and on)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = letter2InitialX;
                                letter2CorrectZ = letter2InitialZ;
                                Debug.Log("UPPERCASE SIGMA CONDITION: #EX (SIG is present and on)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = letter3InitialX;
                                letter3CorrectZ = letter3InitialZ;
                                Debug.Log("UPPERCASE SIGMA CONDITION: #EX (SIG is present and on)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                        break;
                    }

                case "σ":
                    if (!bomb.IsIndicatorOn("SIG"))
                    {
                        //If this letter is green...
                        if (letters[i].GetComponent<TextMesh>().color == new Color(0, 1, 0, 1))
                        {
                            switch (i)
                            {
                                case 0:
                                    letter1CorrectX = -0.0125f;
                                    letter1CorrectZ = 0.0375f;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #1 (this letter is green)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    letter2CorrectX = -0.0125f;
                                    letter2CorrectZ = 0.0375f;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #1 (this letter is green)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    letter3CorrectX = -0.0125f;
                                    letter3CorrectZ = 0.0375f;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #1 (this letter is green)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                        //Otherwise, if there is a parallel port...
                        else if (bomb.IsPortPresent(Port.Parallel))
                        {
                            switch (i)
                            {
                                case 0:
                                    letter1CorrectX = -0.0125f;
                                    letter1CorrectZ = -0.0375f;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #2 (parallel port detected)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    letter2CorrectX = -0.0125f;
                                    letter2CorrectZ = -0.0375f;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #2 (parallel port detected)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    letter3CorrectX = -0.0125f;
                                    letter3CorrectZ = -0.0375f;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #2 (parallel port detected)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                        //Otherwise, if one and only one of the other letters is an uppercase lambda on the module...
                        else if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "Λ" ^ letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text == "Λ")
                        {
                            switch (i)
                            {
                                case 0:
                                    if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "Λ")
                                    {
                                        letter1CorrectX = letter2InitialX;
                                        letter1CorrectZ = letter2InitialZ;
                                    }
                                    else
                                    {
                                        letter1CorrectX = letter3InitialX;
                                        letter1CorrectZ = letter3InitialZ;
                                    }
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #3 (only 1 uppercase lambda detected)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "Λ")
                                    {
                                        letter2CorrectX = letter3InitialX;
                                        letter2CorrectZ = letter3InitialZ;
                                    }
                                    else
                                    {
                                        letter2CorrectX = letter1InitialX;
                                        letter2CorrectZ = letter1InitialZ;
                                    }
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #3 (only 1 uppercase lambda detected)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "Λ")
                                    {
                                        letter3CorrectX = letter1InitialX;
                                        letter3CorrectZ = letter1InitialZ;
                                    }
                                    else
                                    {
                                        letter3CorrectX = letter2InitialX;
                                        letter3CorrectZ = letter2InitialZ;
                                    }
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #3 (only 1 uppercase lambda detected)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                        //Otherwise, if this letter starts in the fourth row...
                        else if ((i == 0 && letter1InitialZ == -0.0375f) || (i == 1 && letter2InitialZ == -0.0375f) || (i == 2 && letter3InitialZ == -0.0375f))
                        {
                            switch (i)
                            {
                                case 0:
                                    letter1CorrectX = letter1InitialX;
                                    letter1CorrectZ = letter1InitialZ;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #4 (initial row is row 4)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    letter2CorrectX = letter2InitialX;
                                    letter2CorrectZ = letter2InitialZ;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #4 (initial row is row 4)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    letter3CorrectX = letter3InitialX;
                                    letter3CorrectZ = letter3InitialZ;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #4 (initial row is row 4)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                        //Otherwise, if nothing else applies...
                        else
                        {
                            switch (i)
                            {
                                case 0:
                                    letter1CorrectX = -0.0375f;
                                    letter1CorrectZ = 0.0375f;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #5 (N/A)");
                                    Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                    break;
                                case 1:
                                    letter2CorrectX = -0.0375f;
                                    letter2CorrectZ = 0.0375f;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #5 (N/A)");
                                    Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                    break;
                                case 2:
                                    letter3CorrectX = -0.0375f;
                                    letter3CorrectZ = 0.0375f;
                                    Debug.Log("LOWERCASE SIGMA CONDITION: #5 (N/A)");
                                    Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                    break;
                            }
                        }
                        break;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = letter1InitialX;
                                letter1CorrectZ = letter1InitialZ;
                                Debug.Log("LOWERCASE SIGMA CONDITION: #EX (SIG is present and on)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = letter2InitialX;
                                letter2CorrectZ = letter2InitialZ;
                                Debug.Log("LOWERCASE SIGMA CONDITION: #EX (SIG is present and on)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = letter3InitialX;
                                letter3CorrectZ = letter3InitialZ;
                                Debug.Log("LOWERCASE SIGMA CONDITION: #EX (SIG is present and on)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                        break;
                    }

                case "Ω":
                    //If this letter is magenta...
                    if (letters[i].GetComponent<TextMesh>().color == new Color(1, 0, 1, 1))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #1 (letter is magenta)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #1 (letter is magenta)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #1 (letter is magenta)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the serial number contains a Z...
                    else if (bomb.GetSerialNumberLetters().Contains('Z'))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #2 (serial number Z detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #2 (serial number Z detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #2 (serial number Z detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there is an uppercase alpha on the module...
                    else if (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().text == "A" || letters[(i + 2) % letters.Length].GetComponent<TextMesh>().text == "A")
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #3 (uppercase alpha detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #3 (uppercase alpha detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #3 (uppercase alpha detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the last digit of the serial number is composite...
                    else if (!primeNumbers.Contains(serialNumberLastChar))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #4 (serial number last digit is composite)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #4 (serial number last digit is composite)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #4 (serial number last digit is composite)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = 0.0375f;
                                Debug.Log("UPPERCASE OMEGA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

                case "ω":
                    //If this letter starts in C4...
                    if ((i == 0 && letter1InitialX == 0.0125f && letter1InitialZ == -0.0375f) || (i == 1 && letter2InitialX == 0.0125f && letter2InitialZ == -0.0375f) || (i == 2 && letter3InitialX == 0.0125f && letter3InitialZ == -0.0375f))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0125f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #1 (this letter starts in C4)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0125f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #1 (this letter starts in C4)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0125f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #1 (this letter starts in C4)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if the serial number contains a W...
                    else if (bomb.GetSerialNumberLetters().Contains('W'))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = -0.0375f;
                                letter1CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #2 (serial letter contains W)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = -0.0375f;
                                letter2CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #2 (serial letter contains W)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = -0.0375f;
                                letter3CorrectZ = 0.0125f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #2 (serial letter contains W)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if there are 0 batteries...
                    else if (bomb.GetBatteryCount() == 0)
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0125f;
                                letter1CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #3 (0 batteries detected)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0125f;
                                letter2CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #3 (0 batteries detected)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0125f;
                                letter3CorrectZ = -0.0125f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #3 (0 batteries detected)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if this letter is cyan AND only one of the other letters is white...
                    else if (letters[i].GetComponent<TextMesh>().color == new Color(0, 1, 1, 1) && (letters[(i + 1) % letters.Length].GetComponent<TextMesh>().color == new Color(1, 1, 1, 1) ^ letters[(i + 2) % letters.Length].GetComponent<TextMesh>().color == new Color(1, 1, 1, 1)))
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = 0.0375f;
                                letter1CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #4 (this letter is cyan and only 1 of the other letters is white)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = 0.0375f;
                                letter2CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #4 (this letter is cyan and only 1 of the other letters is white)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = 0.0375f;
                                letter3CorrectZ = -0.0375f;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #4 (this letter is cyan and only 1 of the other letters is white)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                letter1CorrectX = letter1InitialX;
                                letter1CorrectZ = letter1InitialZ;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter1CorrectX.ToString() + " and the correct Z is " + letter1CorrectZ.ToString());
                                break;
                            case 1:
                                letter2CorrectX = letter2InitialX;
                                letter2CorrectZ = letter2InitialZ;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter2CorrectX.ToString() + " and the correct Z is " + letter2CorrectZ.ToString());
                                break;
                            case 2:
                                letter3CorrectX = letter3InitialX;
                                letter3CorrectZ = letter3InitialZ;
                                Debug.Log("LOWERCASE OMEGA CONDITION: #5 (N/A)");
                                Debug.Log("The correct X is " + letter3CorrectX.ToString() + " and the correct Z is " + letter3CorrectZ.ToString());
                                break;
                        }
                    }
                    break;

            }
        }
    }

    //Arrow and Resest Button Interaction Methods
    void PressUpButton()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        upButton.AddInteractionPunch();
        if (selectedLetter != null && moduleSolved == false)
        {
            Debug.Log("You pressed up! You deserve a cookie!");
            if (selectedLetterZ != 0.0375f)
            {
                selectedLetter.transform.localPosition = new Vector3(selectedLetterX, 0.013f, selectedLetterZ += 0.025f);
            }
        }
    }

    void PressDownButton()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        downButton.AddInteractionPunch();
        if (selectedLetter != null && moduleSolved == false)
        {
            Debug.Log("You pressed down! You deserve a cookie!");
            if (selectedLetterZ != -0.0375f)
            {
                selectedLetter.transform.localPosition = new Vector3(selectedLetterX, 0.013f, selectedLetterZ -= 0.025f);
            }
        }
    }

    void PressLeftButton()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        leftButton.AddInteractionPunch();
        if (selectedLetter != null && moduleSolved == false)
        {
            Debug.Log("You pressed left! You deserve a cookie!");
            if (selectedLetterX != -0.0375f)
            {
                selectedLetter.transform.localPosition = new Vector3(selectedLetterX -= 0.025f, 0.013f, selectedLetterZ);
            }
        }
    }

    void PressRightButton()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        rightButton.AddInteractionPunch();
        if (selectedLetter != null && moduleSolved == false)
        {
            Debug.Log("You pressed right! You deserve a cookie!");
            if (selectedLetterX != 0.0375f)
            {
                selectedLetter.transform.localPosition = new Vector3(selectedLetterX += 0.025f, 0.013f, selectedLetterZ);
            }
        }
    }

    void PressResetButton()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        resetButton.AddInteractionPunch();
        if (moduleSolved)
        {
            return;
        }
        else
        {
            Debug.Log("You reset the module.");
            letters[0].transform.localPosition = new Vector3(letter1InitialX, 0.013f, letter1InitialZ);
            letters[1].transform.localPosition = new Vector3(letter2InitialX, 0.013f, letter2InitialZ);
            letters[2].transform.localPosition = new Vector3(letter3InitialX, 0.013f, letter3InitialZ);
            selectedLetter = null;
        }
    }

    void Submit()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        submitButton.AddInteractionPunch();

        if (moduleSolved)
        {
            return;
        }

        if ((letter1CurrentX.ToString().Equals(letter1CorrectX.ToString()) && letter1CurrentZ.ToString().Equals(letter1CorrectZ.ToString())) && (letter2CurrentX.ToString().Equals(letter2CorrectX.ToString()) && letter2CurrentZ.ToString().Equals(letter2CorrectZ.ToString())) && (letter3CurrentX.ToString().Equals(letter3CorrectX.ToString()) && letter3CurrentZ.ToString().Equals(letter3CorrectZ.ToString()))) //Add letters 2 & 3 when done.
        {
            moduleSolved = true;
            GetComponent<KMBombModule>().HandlePass();
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
            Debug.Log("Module solved. Gj!");
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
            Debug.Log("You failed. Try again, but do better! The correct X for Letter 1 was " + letter1CorrectX.ToString() + " and the correct Z was " + letter1CorrectZ.ToString() + ". Your X was " + letter1CurrentX.ToString() + " and your Z was " + letter1CurrentZ.ToString());
            Debug.Log("The correct X for Letter 2 was " + letter2CorrectX.ToString() + " and the correct Z was " + letter2CorrectZ.ToString() + ". Your X was " + letter2CurrentX.ToString() + " and your Z was " + letter2CurrentZ.ToString());
            Debug.Log("The correct X for Letter 3 was " + letter3CorrectX.ToString() + " and the correct Z was " + letter3CorrectZ.ToString() + ". Your X was " + letter3CurrentX.ToString() + " and your Z was " + letter3CurrentZ.ToString());
            Start(); //Reset module with new letters in new positions.

        }
    }

    //Letter Button Interaction Methods
    void PressLetter(KMSelectable letter)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        Debug.Log("You pressed " + letter.GetComponent<TextMesh>().text);
        selectedLetter = letter;
        selectedLetterX = selectedLetter.transform.localPosition.x;
        selectedLetterZ = selectedLetter.transform.localPosition.z;
        selectedLetter.AddInteractionPunch();
    }

    // Update is called once per frame
    void Update()
    {
        letter1CurrentX = letters[0].transform.localPosition.x;
        letter1CurrentZ = letters[0].transform.localPosition.z;
        letter2CurrentX = letters[1].transform.localPosition.x;
        letter2CurrentZ = letters[1].transform.localPosition.z;
        letter3CurrentX = letters[2].transform.localPosition.x;
        letter3CurrentZ = letters[2].transform.localPosition.z;
    }

    //TP Support
    int TwitchModuleScore = 1;
    public string TwitchHelpMessage = "Select a letter to move with !{0} select [1 | 2 | 3]. Use the arrow buttons with !{0} [press | move] [u | l | r | d]. You can string as many movements together in one command so long as there is no separation between movements. Reset with !{0} reset. Submit with !{0} submit.";
    public KMSelectable[] ProcessTwitchCommand(string command)
    {
        //Select Letter
        if (command.Equals("select 1", StringComparison.InvariantCultureIgnoreCase))
        {
            return new KMSelectable[] { letters[0] };
        }
        else if (command.Equals("select 2", StringComparison.InvariantCultureIgnoreCase))
        {
            return new KMSelectable[] { letters[1] };
        }
        else if (command.Equals("select 3", StringComparison.InvariantCultureIgnoreCase))
        {
            return new KMSelectable[] { letters[2] };
        }
        //Submit & Reset
        else if (command.Equals("reset", StringComparison.InvariantCultureIgnoreCase))
        {
            return new KMSelectable[] { resetButton };
        }
        else if (command.Equals("submit", StringComparison.InvariantCultureIgnoreCase))
        {
            return new KMSelectable[] { submitButton };
        }
        //Arrow buttons - Credit to the 'Mouse in the Maze' module and River for most of the following code
        else if (command.StartsWith("move ", StringComparison.InvariantCultureIgnoreCase) || command.StartsWith("press ", StringComparison.InvariantCultureIgnoreCase))
        {
            var btns = new List<KMSelectable>();
            var pieces = command.Trim().ToLowerInvariant().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < pieces.Length; i++)
            {
                switch (pieces[i])
                {
                    case "u": btns.Add(upButton); break;
                    case "l": btns.Add(leftButton); break;
                    case "r": btns.Add(rightButton); break;
                    case "d": btns.Add(downButton); break;
                    default:
                        if (pieces[i].All(c => new[] { 'u', 'l', 'r', 'd' }.Contains(c)))
                        {
                            foreach(char c in pieces[i])
                            {
                                switch(c)
                                {
                                    case 'u': btns.Add(upButton); break;
                                    case 'l': btns.Add(leftButton); break;
                                    case 'r': btns.Add(rightButton); break;
                                    case 'd': btns.Add(downButton); break;
                                }
                            }
                        }
                        break;
                }
                return btns.ToArray();
            }
        }

        return null;
    }
}