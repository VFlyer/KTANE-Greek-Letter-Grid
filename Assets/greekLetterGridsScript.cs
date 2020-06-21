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
    public TextMesh[] lettersText;
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

    //Edgework
    private Edgework bombEdgework;
    class Edgework
    {
        public string serialNumber;

    }

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable letter in letters)
        {
            KMSelectable pressedLetter = letter;
            TextMesh text = letter.GetComponent<TextMesh>();
            letter.OnInteract += delegate () { PressLetter(pressedLetter, text); return false; };
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
            lettersText[0].text = possibleLetters[14];
            lettersText[0].color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            lettersText[1].text = possibleLetters[14];
            lettersText[1].color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            lettersText[2].text = possibleLetters[14];
            lettersText[2].color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
        }
        else if (System.DateTime.Now.Month == 2 && System.DateTime.Now.Day == 14) //Happy Valentine's Day! All letters are now magenta.
        {
            lettersText[0].text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            lettersText[0].color = possibleColors[1];
            lettersText[1].text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            lettersText[1].color = possibleColors[1];
            lettersText[2].text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            lettersText[2].color = possibleColors[1];
        }
        else if (System.DateTime.Now.Month == 3 && System.DateTime.Now.Day == 14) //Happy Pi Day! All letters are now upper/lowercase pi.
        {
            lettersText[0].text = possibleLetters[UnityEngine.Random.Range(12, 14)]; //Change the 12s back to possibleLetters.Length, as they are temporary.
            lettersText[0].color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            lettersText[1].text = possibleLetters[UnityEngine.Random.Range(12, 14)];
            lettersText[1].color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            lettersText[2].text = possibleLetters[UnityEngine.Random.Range(12, 14)];
            lettersText[2].color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
        }
        else if (System.DateTime.Now.Month == 3 && System.DateTime.Now.Day == 17) //Happy St. Patrick's Day! All letters are now green.
        {
            lettersText[0].text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            lettersText[0].color = possibleColors[3];
            lettersText[1].text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            lettersText[1].color = possibleColors[3];
            lettersText[2].text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            lettersText[2].color = possibleColors[3];
        }
        else //Sigh, nothing special today... Everything is randomized. :(
        {
            lettersText[0].text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            lettersText[0].color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            lettersText[1].text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            lettersText[1].color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
            lettersText[2].text = possibleLetters[UnityEngine.Random.Range(0, possibleLetters.Length)];
            lettersText[2].color = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
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
        DefineEdgework();
        Rules();

        //Verify the expected coordinate is the same as the reported coordinate (Debug code)
        Debug.LogFormat("<Greek Letter Grid #{0}> First:{1}, Second:{2}, Third:{3}", moduleId,
            CoordinateConversion(letter1CorrectX, letter1CorrectZ),
            CoordinateConversion(letter2CorrectX, letter2CorrectZ),
            CoordinateConversion(letter3CorrectX, letter3CorrectZ));
    }

    void DefineEdgework()
    {
        bombEdgework = new Edgework();
        string serialNumber = bomb.GetSerialNumber();
        string serialNumberLastChar = serialNumber.Substring(serialNumber.Length - 1);
        DebugLog("The last digit of the serial # is " + serialNumberLastChar);
        if (oddDigits.Contains(serialNumberLastChar))
        {
            DebugLog("The last digit of the serial # is ODD");
        }
        else
        {
            DebugLog("The last digit of the serial # is EVEN");
        }

        if (primeNumbers.Contains(serialNumberLastChar))
        {
            DebugLog("The last digit of the serial # is PRIME");
        }
        else
        {
            DebugLog("The last digit of the serial # is COMPOSITE");
        }
        bombEdgework.serialNumber = serialNumber;
    }

    void Rules()
    {
        string[] lettersCorrect = new string[] { "", "", "" };
        float[] lettersInitialX = new float[] { letter1InitialX, letter2InitialX, letter3InitialX };
        float[] lettersInitialZ = new float[] { letter1InitialZ, letter2InitialZ, letter3InitialZ };
        string serialNumber = bombEdgework.serialNumber;
        string serialNumberLastChar = serialNumber.Substring(serialNumber.Length - 1);

        //Check for each letter (e.g. if there is an uppercase alpha)
        for (int i = 0; i < letters.Length; i++)
        {
            switch (lettersText[i].text)
            {
                case "A":
                    //If there is a lowercase sigma...
                    if (lettersText[(i + 1) % letters.Length].text == "σ" || lettersText[(i + 2) % letters.Length].text == "σ")
                    {
                        lettersCorrect[i] = "A4";
                        DebugLog("UPPERCASE ALPHA CONDITION: #1 (Lowercase Sigma Detected)");
                    }
                    //Otherwise, if the letter is yellow...
                    else if (lettersText[i].color == new Color(1, 1, 0, 1))
                    {
                        lettersCorrect[i] = "B1";
                        DebugLog("UPPERCASE ALPHA CONDITION: #2 (This letter is yellow)");
                    }
                    //Otherwise of there is an unlit SND or an unlit IND...
                    else if (bomb.IsIndicatorOff("SND") || bomb.IsIndicatorOff("IND"))
                    {
                        lettersCorrect[i] = "D3";
                        DebugLog("UPPERCASE ALPHA CONDITION: #3 (Unlit SND or IND detected)");
                    }
                    //Otherwise if the other 2 letters are identical colors...
                    else if (lettersText[(i + 1) % letters.Length].color == lettersText[(i + 2) % letters.Length].color)
                    {
                        lettersCorrect[i] = "C3";
                        DebugLog("UPPERCASE ALPHA CONDITION: #4 (Other 2 letters are identical colors)");
                    }
                    //Otherwise if nothing applies...
                    else
                    {
                        lettersCorrect[i] = CoordinateConversion(lettersInitialX[i], lettersInitialZ[i]);
                        DebugLog("UPPERCASE ALPHA CONDITION: #5 (N/A)");
                    }
                    break;

                case "α":
                    //If initially in column D...
                    if (lettersInitialX[i] == 0.0375f)
                    {
                        lettersCorrect[i] = CoordinateConversion(lettersInitialX[i] - 0.05f, lettersInitialZ[i]);
                        DebugLog("LOWERCASE ALPHA CONDITION: #1 (Initially in column D)");
                    }
                    //Otherwise if 3 or more batteries...
                    else if (bomb.GetBatteryCount() >= 3)
                    {
                        lettersCorrect[i] = "D1";
                        DebugLog("LOWERCASE ALPHA CONDITION: #2 (3+ batteries detected)");
                    }
                    //Otherwise, if this letter is green and an anycase delta is on the module...
                    else if (lettersText[i].color == new Color(0, 0, 1, 1) && (lettersText[(i + 1) % letters.Length].text == "Δ" || lettersText[(i + 1) % letters.Length].text == "δ" || lettersText[(i + 2) % letters.Length].text == "Δ" || lettersText[(i + 2) % letters.Length].text == "δ"))
                    {
                        lettersCorrect[i] = "C2";
                        DebugLog("LOWERCASE ALPHA CONDITION: #3 (this letter is green & delta detected)");
                    }
                    //Otherwise, if there's a lit CLR...
                    else if (bomb.IsIndicatorOn("CLR"))
                    {
                        lettersCorrect[i] = "A1";
                        DebugLog("LOWERCASE ALPHA CONDITION: #4 (lit CLR detected)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "A3";
                        DebugLog("LOWERCASE ALPHA CONDITION: #5 (N/A)");
                    }
                    break;

                case "B":
                    //If this letter is cyan...
                    if (lettersText[i].color == new Color(0, 1, 1, 1))
                    {
                        lettersCorrect[i] = "D1";
                        DebugLog("UPPERCASE BETA CONDITION: #1 (this letter is cyan)");
                    }
                    //Otherwise, if this letter was initially found in row 3...
                    else if (lettersInitialZ[i] == -0.0125f)
                    {
                        lettersCorrect[i] = CoordinateConversion(lettersInitialX[i], lettersInitialZ[i] + 0.025f);
                        DebugLog("UPPERCASE BETA CONDITION: #2 (this letter was found in row 3)");
                    }
                    //Otherwise, if the last char of serial # is odd & there is a lowercase letter...
                    else if (oddDigits.Contains(serialNumberLastChar) && (lowercaseLetters.Contains(lettersText[(i + 1) % letters.Length].text) || lowercaseLetters.Contains(lettersText[(i + 2) % letters.Length].text)))
                    {
                        lettersCorrect[i] = "B4";
                        DebugLog("UPPERCASE BETA CONDITION: #3 (last digit of the serial number is odd and there is a lowercase letter)");
                    }
                    //Otherwise, if there is a a green uppercase letter...
                    else if ((lettersText[i].color == new Color(0, 1, 0, 1)) || (lettersText[(i + 1) % letters.Length].color == new Color(0, 1, 0, 1) && uppercaseLetters.Contains(lettersText[(i + 1) % letters.Length].text)) || (lettersText[(i + 2) % letters.Length].color == new Color(0, 1, 0, 1) && uppercaseLetters.Contains(lettersText[(i + 2) % letters.Length].text)))
                    {
                        lettersCorrect[i] = "C2";
                        DebugLog("UPPERCASE BETA CONDITION: #4 (green uppercase detected)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "C4";
                        DebugLog("UPPERCASE BETA CONDITION: #5 (N/A)");
                    }
                    break;

                case "β":
                    //If this letter starts in A3...
                    if (lettersInitialX[i] == -0.0375f && lettersInitialZ[i] == -0.0125f)
                    {
                        lettersCorrect[i] = "C4";
                        DebugLog("LOWERCASE BETA CONDITION: #1 (this letter started in A3)");
                    }
                    //Otherwise, if a letter is white and was initially found in the A column...
                    else if ((letter1InitialX == -0.0375f && lettersText[0].color == new Color(1, 1, 1, 1)) || (letter2InitialX == -0.0375f && lettersText[1].color == new Color(1, 1, 1, 1)) || (letter3InitialX == -0.0375f && lettersText[2].color == new Color(1, 1, 1, 1)))
                    {
                        lettersCorrect[i] = "A2";
                        DebugLog("LOWERCASE BETA CONDITION: #2 (white letter was found in the A column)");
                    }
                    //Otherwise, if this letter is magenta or cyan...
                    else if (lettersText[i].color == new Color(1, 0, 1, 1) || lettersText[i].color == new Color(0, 1, 1, 1))
                    {
                        lettersCorrect[i] = "D4";
                        DebugLog("LOWERCASE BETA CONDITION: #3 (this letter is magenta/cyan)");
                    }
                    //Otherwise, if there is a DVI and no RJ45...
                    else if (bomb.IsPortPresent(Port.DVI) && !bomb.IsPortPresent(Port.RJ45))
                    {
                        lettersCorrect[i] = "B2";
                        DebugLog("LOWERCASE BETA CONDITION: #4 (DVI and no RJ45 detected)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "B1";
                        DebugLog("LOWERCASE BETA CONDITION: #5 (N/A)");
                    }
                    break;

                case "Γ":
                    //If the serial number contains a letter that is in the first half of the alphabet...
                    if (bomb.GetSerialNumberLetters().Any(x => x == 'A' || x == 'B' || x == 'C' || x == 'D' || x == 'E' || x == 'F' || x == 'G' || x == 'H' || x == 'I' || x == 'J' || x == 'K' || x == 'L' || x == 'M'))
                    {
                        lettersCorrect[i] = "D2";
                        DebugLog("UPPERCASE GAMMA CONDITION: #1 (A letter in the serial number is found in the first half of the English Alphabet)");
                    }
                    //Otherwise, if there is an empty port plate...
                    else if (bomb.GetPortPlates().Any(x => x.Length == 0))
                    {
                        lettersCorrect[i] = "C2";
                        DebugLog("UPPERCASE GAMMA CONDITION: #2 (empty port plate detected)");
                    }
                    //Otherwise, if all letters are magenta...
                    else if (lettersText.All(x => x.color == new Color(1, 0, 1, 1)))
                    {
                        lettersCorrect[i] = "A1";
                        DebugLog("UPPERCASE GAMMA CONDITION: #3 (all letters are magenta)");
                    }
                    //Otherwise, if this letter is cyan and starts in row 4...
                    else if (lettersText[i].color == new Color(0, 1, 1, 1) && lettersInitialZ[i] == -0.0375f)
                    {
                        lettersCorrect[i] = CoordinateConversion(lettersInitialX[i], lettersInitialZ[i] + 0.05f);
                        DebugLog("UPPERCASE GAMMA CONDITION: #1 (this letter is cyan and started in row 4)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "A2";
                        DebugLog("UPPERCASE GAMMA CONDITION: #5 (N/A)");
                    }
                    break;

                case "γ":
                    //If anycase anycolor theta is present...
                    if (lettersText[(i + 1) % letters.Length].text == "Θ" || lettersText[(i + 1) % letters.Length].text == "θ" || lettersText[(i + 2) % letters.Length].text == "Θ" || lettersText[(i + 2) % letters.Length].text == "θ")
                    {
                        lettersCorrect[i] = "C1";
                        DebugLog("LOWERCASE GAMMA CONDITION: #1 (theta detected)");
                    }
                    //Otherwise, if not cyan and serial number contains vowel...
                    else if (lettersText[i].color != new Color(0, 1, 1, 1) && bomb.GetSerialNumberLetters().Any(x => x == 'A' || x == 'E' || x == 'I' || x == 'O' || x == 'U'))
                    {
                        lettersCorrect[i] = "B3";
                        DebugLog("LOWERCASE GAMMA CONDITION: #2 (this letter is NOT cyan and vowel detected)");
                    }
                    //Otherwise, if this letter starts in column C...
                    else if (lettersInitialX[i] == 0.0125f)
                    {
                        DebugLog("LOWERCASE GAMMA CONDITION: #2 (this letter starts in column C)");
                        if (int.Parse(serialNumberLastChar) <= 4) //If the last digit of the serial number is less than or equal to 4...
                        {
                            lettersCorrect[i] = CoordinateConversion(lettersInitialX[i] - 0.05f, lettersInitialZ[i]);
                            DebugLog("The last digit of the serial number is less than or equal to 4.");
                        }
                        else //If the last digit of the serial number is greater than 4...
                        {
                            lettersCorrect[i] = CoordinateConversion(lettersInitialX[i] - 0.025f, lettersInitialZ[i]);
                            DebugLog("The last digit of the serial number is greater than 4.");
                        }
                    }
                    //Otherwise, if the last digit of the serial number is prime...
                    else if (primeNumbers.ToString().Contains(serialNumberLastChar))
                    {
                        lettersCorrect[i] = "D3";
                        DebugLog("LOWERCASE GAMMA CONDITION: #4 (last digit of the serial number is prime)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "D4";
                        DebugLog("LOWERCASE GAMMA CONDITION: #5 (N/A)");
                    }
                    break;

                case "Δ":
                    //If there is a stereo RCA port...
                    if (bomb.IsPortPresent(Port.StereoRCA))
                    {
                        lettersCorrect[i] = "B2";
                        DebugLog("UPPERCASE DELTA CONDITION: #1 (stereo RCA detected)");
                    }
                    //Otherwise, if the number of batteries is greater than the last digit of the serial number...
                    else if (bomb.GetBatteryCount() > int.Parse(serialNumberLastChar))
                    {
                        lettersCorrect[i] = "A3";
                        DebugLog("UPPERCASE DELTA CONDITION: #2 (battery count > last digit in serial number)");
                    }
                    //Otherwise, if you have solved half or over half of the modules on this bomb...
                    else if (lettersText[i].color != new Color(1, 0, 1, 1) && lettersText[i].color != new Color(1, 1, 0, 1))
                    {
                        lettersCorrect[i] = "D4";
                        DebugLog("UPPERCASE DELTA CONDITION: #3 (this letter is neither yellow nor magenta)");
                    }
                    //Otherwise, if there is a green lowercase omega...
                    else if ((lettersText[(i + 1) % letters.Length].text == "ω" && lettersText[(i + 1) % letters.Length].color == new Color(0, 1, 0, 1)) || (lettersText[(i + 2) % letters.Length].text == "ω" && lettersText[(i + 2) % letters.Length].color == new Color(0, 1, 0, 1)))
                    {
                        lettersCorrect[i] = "A1";
                        DebugLog("UPPERCASE DELTA CONDITION: #4 (green lowercase omega detected)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "C1";
                        DebugLog("UPPERCASE DELTA CONDITION: #5 (N/A)");
                    }
                    break;

                case "δ":
                    //If this is the only lowercase letter on the module...
                    if (uppercaseLetters.Contains(lettersText[(i + 1) % letters.Length].text) && uppercaseLetters.Contains(lettersText[(i + 2) % letters.Length].text))
                    {
                        lettersCorrect[i] = "C3";
                        DebugLog("LOWERCASE DELTA CONDITION: #1 (this letter is the only lowercase letter on the module)");
                    }
                    //Otherwise, if the number of solved modules + the last digit of the serial number > 10...
                    else if (bomb.GetSolvedModuleNames().Count + int.Parse(serialNumberLastChar) > 10)
                    {
                        lettersCorrect[i] = "B3";
                        DebugLog("LOWERCASE DELTA CONDITION: #2 (solved modules + last digit > 10)");
                    }
                    //Otherwise, if any letter starts in A3...
                    else if ((letter1InitialX == -0.0375f && letter1InitialZ == -0.0125f) || (letter2InitialX == -0.0375f && letter2InitialZ == -0.0125f) || (letter3InitialX == -0.0375f && letter3InitialZ == -0.0125f))
                    {
                        lettersCorrect[i] = "A3";
                        DebugLog("LOWERCASE DELTA CONDITION: #3 (a letter starts in A3)");
                    }
                    //Otherwise, if this letter is the only cyan letter on the module...
                    else if (lettersText[i].color == new Color(0, 1, 1, 1) && (lettersText[(i + 1) % letters.Length].color != new Color(0, 1, 1, 1)) && (lettersText[(i + 2) % letters.Length].color != new Color(0, 1, 1, 1)))
                    {
                        lettersCorrect[i] = "B4";
                        DebugLog("LOWERCASE DELTA CONDITION: #4 (this letter is cyan and the other letters are not)");
                    }
                    //Otherwise if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "C2";
                        DebugLog("LOWERCASE DELTA CONDITION: #5 (N/A)");
                    }
                    break;

                case "Θ":
                    //If the number of D batteries is greater than the number of AA batteries...
                    if (bomb.GetBatteryCount(Battery.D) > (bomb.GetBatteryCount(Battery.AA) + bomb.GetBatteryCount(Battery.AAx3) + bomb.GetBatteryCount(Battery.AAx4)))
                    {
                        lettersCorrect[i] = "A4";
                        DebugLog("UPPERCASE THETA CONDITION: #1 (D batteries outnumber AA)");
                    }
                    //Otherwise, if there is an unlit CAR indicator...
                    else if (bomb.IsIndicatorOff("CAR"))
                    {
                        //zIndex should be the position in possibleXorZ that matches our desired value
                        //Serial 0: index 3 - shown 1, Serial 3: index 0 - shown 4, Serial 9: index 2 - shown 2
                        int zIndex = 3 - (int.Parse(serialNumberLastChar) % 4);
                        lettersCorrect[i] = CoordinateConversion(possibleXorZ[1], possibleXorZ[zIndex]);
                        DebugLog("UPPERCASE THETA CONDITION: #2 (unlit CAR detected)");
                    }
                    //Otherwise, if the colors of all 3 letters are unique...
                    else if (lettersText[i].color != lettersText[(i + 1) % letters.Length].color && lettersText[i].color != lettersText[(i + 2) % letters.Length].color && lettersText[(i + 1) % letters.Length].color != lettersText[(i + 2) % letters.Length].color)
                    {
                        int zIndex = 3 - (bomb.GetPortPlateCount() % 4);
                        lettersCorrect[i] = CoordinateConversion(possibleXorZ[3], possibleXorZ[zIndex]);
                        DebugLog("UPPERCASE THETA CONDITION: #3 (all colors are unique)");
                    }
                    //Otherwise, if there is also a lowercase theta on the module...
                    else if (lettersText[(i + 1) % letters.Length].text == "θ" || lettersText[(i + 2) % letters.Length].text == "θ")
                    {
                        lettersCorrect[i] = "D1";
                        DebugLog("UPPERCASE THETA CONDITION: #4 (lowercase theta detected)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "B2";
                        DebugLog("UPPERCASE THETA CONDITION: #5 (N/A)");
                    }
                    break;

                case "θ":
                    //If this letter starts in a row unique to the other 2 letters...
                    if (lettersInitialZ[(i + 1) % letters.Length] != lettersInitialZ[i] && lettersInitialZ[(i + 2) % letters.Length] != lettersInitialZ[i])
                    {
                        lettersCorrect[i] = "D3";
                        DebugLog("LOWERCASE THETA CONDITION: #1 (initial row is unique)");
                    }
                    //Otherwise, if this letter is white...
                    else if (lettersText[i].color == new Color(1, 1, 1, 1))
                    {
                        int zIndex = 3 - (bomb.GetStrikes() % 4);
                        lettersCorrect[i] = CoordinateConversion(possibleXorZ[2], possibleXorZ[zIndex]);
                        DebugLog("LOWERCASE THETA CONDITION: #2 (this letter is white)");
                    }
                    //Otherwise, if the number of solved modules is less than or equal to the number of current strikes...
                    else if (bomb.GetSolvedModuleNames().Count <= bomb.GetStrikes())
                    {
                        lettersCorrect[i] = CoordinateConversion(lettersInitialX[i], lettersInitialZ[i]);
                        DebugLog("LOWERCASE THETA CONDITION: #3 (solved modules <= strikes)");
                    }
                    //Otherwise, if there is a PS/2 port or any duplicate ports of any type...
                    else if (bomb.IsPortPresent(Port.PS2) || bomb.IsDuplicatePortPresent())
                    {
                        lettersCorrect[i] = "B1";
                        DebugLog("LOWERCASE THETA CONDITION: #4 (PS/2 detected OR duplicate port detected)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "A2";
                        DebugLog("LOWERCASE THETA CONDITION: #5 (N/A)");
                    }
                    break;

                case "Λ":
                    //If the serial number contains an A...
                    if (bomb.GetSerialNumberLetters().Any(x => x == 'A'))
                    {
                        lettersCorrect[i] = "A1";
                        DebugLog("UPPERCASE LAMBDA CONDITION: #1 (letter A detected)");
                    }
                    //Otherwise, if the serial number contains a B...
                    else if (bomb.GetSerialNumberLetters().Any(x => x == 'B'))
                    {
                        lettersCorrect[i] = "B1";
                        DebugLog("UPPERCASE LAMBDA CONDITION: #2 (letter B detected)");
                    }
                    //Otherwise, if the serial number contains a D...
                    else if (bomb.GetSerialNumberLetters().Any(x => x == 'D'))
                    {
                        lettersCorrect[i] = "D1";
                        DebugLog("UPPERCASE LAMBDA CONDITION: #3 (letter D detected)");
                    }
                    //Otherwise if the serial number contains either an L or an M...
                    else if (bomb.GetSerialNumberLetters().Any(x => x == 'L' || x == 'M'))
                    {
                        lettersCorrect[i] = "C1";
                        DebugLog("UPPERCASE LAMBDA CONDITION: #4 (letter L/M detected)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "C4";
                        DebugLog("UPPERCASE LAMBDA CONDITION: #5 (N/A)");
                    }
                    break;

                case "λ":
                    //If there are 2 or more lit indicators...
                    if (bomb.GetOnIndicators().Count() >= 2)
                    {
                        lettersCorrect[i] = "D3";
                        DebugLog("LOWERCASE LAMBDA CONDITION: #1 (lit indicator count >= 2)");
                    }
                    //Otherwise, if this letter shares a color with only 1 other letter on the module...
                    else if (lettersText[(i + 1) % letters.Length].color != lettersText[(i + 2) % letters.Length].color && (lettersText[i].color == lettersText[(i + 1) % letters.Length].color || lettersText[i].color == lettersText[(i + 2) % letters.Length].color))
                    {
                        lettersCorrect[i] = "B2";
                        DebugLog("LOWERCASE LAMBDA CONDITION: #2 (shared color with only 1 letter)");
                    }
                    //Otherwise, if there are more batteries than port plates...
                    else if (bomb.GetBatteryCount() > bomb.GetPortPlateCount())
                    {
                        lettersCorrect[i] = "A1";
                        DebugLog("LOWERCASE LAMBDA CONDITION: #3 (battery count > port plate count)");
                    }
                    //Otherwise, if one letter on the module is yellow (not this letter)...
                    else if (lettersText[i].color != new Color(1, 1, 0, 1) && (lettersText[(i + 1) % letters.Length].color == new Color(1, 1, 0, 1) ^ lettersText[(i + 2) % letters.Length].color == new Color(1, 1, 0, 1)))
                    {
                        int yellowPosition;
                        if (lettersText[(i + 1) % letters.Length].color == new Color(1, 1, 0, 1))
                            yellowPosition = (i + 1) % letters.Length;
                        else
                            yellowPosition = (i + 2) % letters.Length;
                        lettersCorrect[i] = "Yellow" + yellowPosition;
                        DebugLog("LOWERCASE LAMBDA CONDITION: #4 (one letter is yellow)");
                        DebugLog("The correct coordinate is based on the correct coordinate of the yellow letter.");
                        continue;
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "D1";
                        DebugLog("LOWERCASE LAMBDA CONDITION: #5 (N/A)");
                    }
                    break;

                case "Π":
                    //If it's Pi Day...
                    if (System.DateTime.Now.Month == 3 && System.DateTime.Now.Day == 14)
                    {
                        lettersCorrect[i] = CoordinateConversion(lettersInitialX[i], lettersInitialZ[i]);
                        DebugLog("UPPERCASE PI CONDITION: #1 (Happy Pi Day!)");
                    }
                    //Otherwise, if there is either a P or an I in the serial number...
                    else if (bomb.GetSerialNumberLetters().Any(x => x == 'P' || x == 'I'))
                    {
                        lettersCorrect[i] = "B4";
                        DebugLog("UPPERCASE PI CONDITION: #2 (serial number P or I detected)");
                    }
                    //Otherwise, if this letter starts in the same row as another letter...
                    else if (lettersInitialZ[i] == lettersInitialZ[(i + 1) % letters.Length] || lettersInitialZ[i] == lettersInitialZ[(i + 2) % letters.Length])
                    {
                        lettersCorrect[i] = "B2";
                        DebugLog("UPPERCASE PI CONDITION: #3 (initial row is shared)");
                    }
                    //Otherwise, if this letter starts in one of the corners of the grid...
                    else if (Math.Abs(lettersInitialX[i]) == 0.0375f && Math.Abs(lettersInitialZ[i]) == 0.0375f)
                    {
                        lettersCorrect[i] = "A3";
                        DebugLog("UPPERCASE PI CONDITION: #4 (initially in corner)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        int zIndex = 3 - (((int)bomb.GetTime() / 60) % 4);
                        lettersCorrect[i] = CoordinateConversion(possibleXorZ[0], possibleXorZ[zIndex]);
                        DebugLog("UPPERCASE PI CONDITION: #5 (N/A)");
                    }
                    break;

                case "π":
                    //If the last digit of the serial number is either 3, 1, or 4...
                    if (int.Parse(serialNumberLastChar) == 3 || int.Parse(serialNumberLastChar) == 1 || int.Parse(serialNumberLastChar) == 4)
                    {
                        lettersCorrect[i] = CoordinateConversion(lettersInitialX[i], lettersInitialZ[i]);
                        DebugLog("LOWERCASE PI CONDITION: #1 (serial number is 3, 1, or 4)");
                    }
                    //Otherwise, if the number of battery holders is either 3, 1, or 4...
                    else if (bomb.GetBatteryHolderCount() == 3 || bomb.GetBatteryHolderCount() == 1 || bomb.GetBatteryHolderCount() == 4)
                    {
                        //4 holders -> 3 index -> 0 pos, 1 holder -> 0 index -> 3 pos, 3 holders -> 2 index -> 1 pos
                        int zIndex = 4 - (bomb.GetBatteryHolderCount() % 4);
                        lettersCorrect[i] = CoordinateConversion(possibleXorZ[3], possibleXorZ[zIndex]);
                        DebugLog("LOWERCASE PI CONDITION: #2 (battery holder count is 3, 1, or 4)");
                    }
                    //Otherwise, if the number of indicators (both lit and unlit) is either 3, 1, or 4...
                    else if (bomb.GetIndicators().Count() == 3 || bomb.GetIndicators().Count() == 1 || bomb.GetIndicators().Count() == 4)
                    {
                        int zIndex = 4 - (bomb.GetIndicators().Count() % 4);
                        lettersCorrect[i] = CoordinateConversion(possibleXorZ[1], possibleXorZ[zIndex]);
                        DebugLog("LOWERCASE PI CONDITION: #3 (indicator count is 3, 1, or 4)");
                    }
                    //Otherwise, if the number of solved modules is either 3, 1, or 4...
                    else if (bomb.GetSolvedModuleNames().Count() == 3 || bomb.GetSolvedModuleNames().Count() == 1 || bomb.GetSolvedModuleNames().Count() == 4)
                    {
                        int zIndex = 4 - (bomb.GetSolvableModuleNames().Count() % 4);
                        lettersCorrect[i] = CoordinateConversion(possibleXorZ[2], possibleXorZ[zIndex]);
                        DebugLog("LOWERCASE PI CONDITION: #4 (solved module count is 3, 1, or 4)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "C3";
                        DebugLog("LOWERCASE PI CONDITION: #5 (N/A)");
                    }
                    break;

                case "Σ":
                case "σ":
                    if (bomb.IsIndicatorOn("SIG"))
                    {
                        lettersCorrect[i] = CoordinateConversion(lettersInitialX[i], lettersInitialZ[i]);
                        DebugLog("SIGMA CONDITION: #EX (SIG is present and on)");
                    }
                    else if (lettersText[i].text == "Σ")
                    {
                        //If the number of batteries plus the number of current strikes is greater than or equal to 5...
                        if (bomb.GetBatteryCount() + bomb.GetStrikes() >= 5)
                        {
                            lettersCorrect[i] = "A4";
                            DebugLog("UPPERCASE SIGMA CONDITION: #1 (battery count + strike count >= 5)");
                        }
                        //Otherwise, if the number of lit indicators plus the number of port plates is greater than or equal to 5...
                        else if (bomb.GetOnIndicators().Count() + bomb.GetPortPlateCount() >= 5)
                        {
                            lettersCorrect[i] = "C2";
                            DebugLog("UPPERCASE SIGMA CONDITION: #2 (lit indicator count + port plate count >= 5)");
                        }
                        //Otherwise, if the number of unlit indicators plus the last digit of the serial number is greater than or equal to 5...
                        else if (bomb.GetOffIndicators().Count() + int.Parse(serialNumberLastChar) >= 5)
                        {
                            lettersCorrect[i] = "B1";
                            DebugLog("UPPERCASE SIGMA CONDITION: #3 (unlit indicator count + last digit of serial number >= 5)");
                        }
                        //Otherwise, if the number of solved modules plus the number of battery holders is greater than or equal to 5...
                        else if (bomb.GetSolvedModuleNames().Count() + bomb.GetBatteryHolderCount() >= 5)
                        {
                            lettersCorrect[i] = "D1";
                            DebugLog("UPPERCASE SIGMA CONDITION: #4 (solved module count + battery holder count >= 5)");
                        }
                        //Otherwise, if nothing applies...
                        else
                        {
                            lettersCorrect[i] = "B3";
                            DebugLog("UPPERCASE SIGMA CONDITION: #5 (N/A)");
                        }
                    }

                    //Lowercase Sigma
                    else
                    {
                        //If this letter is green...
                        if (lettersText[i].color == new Color(0, 1, 0, 1))
                        {
                            lettersCorrect[i] = "B1";
                            DebugLog("LOWERCASE SIGMA CONDITION: #1 (this letter is green)");
                        }
                        //Otherwise, if there is a parallel port...
                        else if (bomb.IsPortPresent(Port.Parallel))
                        {
                            lettersCorrect[i] = "B4";
                            DebugLog("LOWERCASE SIGMA CONDITION: #2 (parallel port detected)");
                        }
                        //Otherwise, if one and only one of the other letters is an uppercase lambda on the module...
                        else if (lettersText[(i + 1) % letters.Length].text == "Λ" ^ lettersText[(i + 2) % letters.Length].text == "Λ")
                        {
                            int lambdaPosition;
                            if (lettersText[(i + 1) % letters.Length].text == "Λ")
                                lambdaPosition = (i + 1) % letters.Length;
                            else
                                lambdaPosition = (i + 2) % letters.Length;
                            lettersCorrect[i] = CoordinateConversion(lettersInitialX[lambdaPosition], lettersInitialZ[lambdaPosition]);
                            DebugLog("LOWERCASE SIGMA CONDITION: #3 (only 1 uppercase lambda detected)");
                        }
                        //Otherwise, if this letter starts in the fourth row...
                        else if (lettersInitialZ[i] == -0.0375f)
                        {
                            lettersCorrect[i] = CoordinateConversion(lettersInitialX[i], lettersInitialZ[i]);
                            DebugLog("LOWERCASE SIGMA CONDITION: #4 (initial row is row 4)");
                        }
                        //Otherwise, if nothing else applies...
                        else
                        {
                            lettersCorrect[i] = "A1";
                            DebugLog("LOWERCASE SIGMA CONDITION: #5 (N/A)");
                        }
                    }
                    break;

                case "Ω":
                    //If this letter is magenta...
                    if (lettersText[i].color == new Color(1, 0, 1, 1))
                    {
                        lettersCorrect[i] = "C4";
                        DebugLog("UPPERCASE OMEGA CONDITION: #1 (letter is magenta)");
                    }
                    //Otherwise, if the serial number contains a Z...
                    else if (bomb.GetSerialNumberLetters().Contains('Z'))
                    {
                        lettersCorrect[i] = "C2";
                        DebugLog("UPPERCASE OMEGA CONDITION: #2 (serial number Z detected)");
                    }
                    //Otherwise, if there is an uppercase alpha on the module...
                    else if (lettersText[(i + 1) % letters.Length].text == "A" || lettersText[(i + 2) % letters.Length].text == "A")
                    {
                        lettersCorrect[i] = "A3";
                        DebugLog("UPPERCASE OMEGA CONDITION: #3 (uppercase alpha detected)");
                    }
                    //Otherwise, if the last digit of the serial number is composite...
                    else if (!primeNumbers.Contains(serialNumberLastChar))
                    {
                        lettersCorrect[i] = "B3";
                        DebugLog("UPPERCASE OMEGA CONDITION: #4 (serial number last digit is composite)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = "D1";
                        DebugLog("UPPERCASE OMEGA CONDITION: #5 (N/A)");
                    }
                    break;

                case "ω":
                    //If this letter starts in C4...
                    if (lettersInitialX[i] == 0.0125f && lettersInitialZ[i] == -0.0375f)
                    {
                        lettersCorrect[i] = "B2";
                        DebugLog("LOWERCASE OMEGA CONDITION: #1 (this letter starts in C4)");
                    }
                    //Otherwise, if the serial number contains a W...
                    else if (bomb.GetSerialNumberLetters().Contains('W'))
                    {
                        lettersCorrect[i] = "A2";
                        DebugLog("LOWERCASE OMEGA CONDITION: #2 (serial letter contains W)");
                    }
                    //Otherwise, if there are 0 batteries...
                    else if (bomb.GetBatteryCount() == 0)
                    {
                        lettersCorrect[i] = "C3";
                        DebugLog("LOWERCASE OMEGA CONDITION: #3 (0 batteries detected)");
                    }
                    //Otherwise, if this letter is cyan AND only one of the other letters is white...
                    else if (lettersText[i].color == new Color(0, 1, 1, 1) && (lettersText[(i + 1) % letters.Length].color == new Color(1, 1, 1, 1) ^ lettersText[(i + 2) % letters.Length].color == new Color(1, 1, 1, 1)))
                    {
                        lettersCorrect[i] = "D4";
                        DebugLog("LOWERCASE OMEGA CONDITION: #4 (this letter is cyan and only 1 of the other letters is white)");
                    }
                    //Otherwise, if nothing applies...
                    else
                    {
                        lettersCorrect[i] = CoordinateConversion(lettersInitialX[i], lettersInitialZ[i]);
                        DebugLog("LOWERCASE OMEGA CONDITION: #5 (N/A)");
                    }
                    break;

            }
            DebugLog("The correct coordinate is {0}", lettersCorrect[i]);
        }
        for (int i = 0; i < letters.Length; i++)
        {
            if (lettersCorrect[i].StartsWith("Yellow"))
            {
                int index = int.Parse(lettersCorrect[i].Replace("Yellow", ""));
                lettersCorrect[i] = lettersCorrect[index];
                DebugLog("The correct coordinate for λ condition #4 is " + lettersCorrect[i]);
                break;
            }
        }
        letter1CorrectX = possibleXorZ[lettersCorrect[0][0] - 'A'];
        letter2CorrectX = possibleXorZ[lettersCorrect[1][0] - 'A'];
        letter3CorrectX = possibleXorZ[lettersCorrect[2][0] - 'A'];
        //1 -> 3, 2 -> 2, 3 -> 1, 4 -> 0
        letter1CorrectZ = possibleXorZ[4 - (lettersCorrect[0][1] - '0')];
        letter2CorrectZ = possibleXorZ[4 - (lettersCorrect[1][1] - '0')];
        letter3CorrectZ = possibleXorZ[4 - (lettersCorrect[2][1] - '0')];
    }

    string CoordinateConversion(float coordinateX, float coordinateZ)
    {
         Dictionary<float, char> conversion = new Dictionary<float, char>
        {
            { possibleXorZ[0], 'A' },
            { possibleXorZ[1], 'B' },
            { possibleXorZ[2], 'C' },
            { possibleXorZ[3], 'D' }
        };
        char X = conversion[coordinateX];
        //'A' - 'A' = 0, 4 - 0 = 4 // 'D' - 'A' = 3, 4 - 3 = 1
        int Z = 4 - (conversion[coordinateZ] - 'A');
        return X.ToString() + Z.ToString();
    }

    //Arrow and Resest Button Interaction Methods
    void PressUpButton()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        upButton.AddInteractionPunch();
        if (selectedLetter != null && moduleSolved == false)
        {
            DebugLog("You pressed up! You deserve a cookie!");
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
            DebugLog("You pressed down! You deserve a cookie!");
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
            DebugLog("You pressed left! You deserve a cookie!");
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
            DebugLog("You pressed right! You deserve a cookie!");
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
            DebugLog("You reset the module.");
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

        //Rules based on user action
        Rules();

        if ((letter1CurrentX.ToString().Equals(letter1CorrectX.ToString()) && letter1CurrentZ.ToString().Equals(letter1CorrectZ.ToString())) && (letter2CurrentX.ToString().Equals(letter2CorrectX.ToString()) && letter2CurrentZ.ToString().Equals(letter2CorrectZ.ToString())) && (letter3CurrentX.ToString().Equals(letter3CorrectX.ToString()) && letter3CurrentZ.ToString().Equals(letter3CorrectZ.ToString()))) //Add letters 2 & 3 when done.
        {
            moduleSolved = true;
            GetComponent<KMBombModule>().HandlePass();
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
            DebugLog("Module solved. Gj!");
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
            DebugLog("You failed. Try again, but do better! The correct coordinate for Letter 1 was " + CoordinateConversion(letter1CorrectX, letter1CorrectZ) + ". Your submitted coordinate was " + CoordinateConversion(letter1CurrentX, letter1CurrentZ));
            DebugLog("The correct coordinate for Letter 2 was " + CoordinateConversion(letter2CorrectX, letter2CorrectZ) + ". Your submitted coordinate was " + CoordinateConversion(letter2CurrentX, letter2CurrentZ));
            DebugLog("The correct coordinate for Letter 3 was " + CoordinateConversion(letter3CorrectX, letter3CorrectZ) + ". Your submitted coordinate was " + CoordinateConversion(letter3CurrentX, letter3CurrentZ));
            Start(); //Reset module with new letters in new positions.

        }
    }

    //Letter Button Interaction Methods
    void PressLetter(KMSelectable letter, TextMesh text)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        DebugLog("You pressed " + text);
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

    void DebugLog(string log, params object[] args)
    {
        string logData = string.Format(log, args);
        Debug.LogFormat("[Greek Letter Grid #{0}] {1}", moduleId, logData);
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