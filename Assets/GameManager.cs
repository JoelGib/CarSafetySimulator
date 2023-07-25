using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{

    delegate void callBackTimer();

    public bool startGame = false;
    public bool canDrive = false;
    public bool canChooseSeat = false;
    public bool isSeatBeltOn = false;
    public bool canCrossRoad = false;
    private float distanceTravelled = 0;

    public bool isLanguageDefault = false; // 0: English, 1: Arabic

    [System.Serializable]private struct QuestionText{
        public string question;
        public string[] options;
    }

    [System.Serializable]private struct GameEventTexts{
        public string[] welcomeText;
        public string[] backseatText;
        public string[] seatbeltText;
        public string[] roadCrossingText;

    }

    [System.Serializable]private class GamePanels{
        public GameObject welcomePanel;
        public GameObject carPanel;
        public GameObject EndPanel;
    }

    // private class WindSheildParent{
        
    // }

    [SerializeField]private Material[] BackSeatMaterials;

    [SerializeField]private QuestionText[] questionText;
    [SerializeField]private GameEventTexts gameEventTexts;
    [SerializeField]private GamePanels gamePanels;

    // private WindSheildParent windSheildParent;

    private GameObject textboxEnglish;
    private GameObject textBoxArabic;
    private GameObject seatbeltSign;
    private GameObject panel;
    private GameObject narrator;
    private GameObject seatButtonsPanel;

    [SerializeField]private AudioClip[] EnglishAudios;
    [SerializeField]private AudioClip[] ArabicAudios;
    

    [SerializeField]private int numberOfQuestions = 3;
    

    [SerializeField]private GameObject BabySeat;
    [SerializeField]private GameObject phone;
    [SerializeField]private GameObject SeatBelt;
    [SerializeField]private GameObject Pedestrian;

    [SerializeField]private AudioSource GameAudioSource;
    [SerializeField]private AudioClip SFX_Seatbelt;
    [SerializeField]private AudioClip SFX_CarIgnition;
    [SerializeField]private AudioClip SFX_CarEngine;
    [SerializeField]private GameObject Stars;
    

    
    public static GameManager Instance { get; private set; }

    private void Awake(){
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    // Start is called before the first frame update
    private void Start()
    {
        // DebugArray(gameEventTexts.welcomeText);
        // DebugArray(gameEventTexts.backseatText);
        // DebugArray(gameEventTexts.roadCrossingText);
        // DebugArray(gameEventTexts.seatbeltText);
        TogglePanels(true,false);
        InitializeWindShieldChildren();
        phone.SetActive(false);
        BabySeat.SetActive(false);
        // Debug.Log("Is Phone Active: "+phone.activeSelf);
        // Debug.Log("Is BabySeat Active: "+BabySeat.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {   
        if(canChooseSeat){
            ChooseBackSeatButton();
        }

        if(isSeatBeltOn){
            if(Input.GetKey(KeyCode.F)){
                FastenSeatbelts();
            }
        }

        if(distanceTravelled > 139){
            gamePanels.EndPanel.SetActive(true);
            distanceTravelled = 999;
            GameAudioSource.Stop();
            Destroy(phone);
            
        }
        
        // Debug.Log("CanDrive: "+canDrive);
    }

    private void TogglePanels(bool b_welcomePanel, bool b_carPanel){
        gamePanels.welcomePanel.SetActive(b_welcomePanel);
        gamePanels.carPanel.SetActive(b_carPanel);
        gamePanels.EndPanel.SetActive(false);
    }

    private void InitializeWindShieldChildren(){
        // windSheildParent = new WindSheildParent();
        // Panel
        panel = gamePanels.carPanel.transform.GetChild(0).gameObject;
        // Narrator
        narrator = gamePanels.carPanel.transform.GetChild(1).gameObject;
        // Textboxes
        textboxEnglish = gamePanels.carPanel.transform.GetChild(2).gameObject;
        textBoxArabic = gamePanels.carPanel.transform.GetChild(3).gameObject;
        // SeatbeltSign
        seatbeltSign = gamePanels.carPanel.transform.GetChild(4).gameObject;
        // SeatButtonsPanel
        seatButtonsPanel = gamePanels.carPanel.transform.GetChild(5).gameObject;

        // Debug.Log("Panel: " + panel.name);
        // Debug.Log("Narrator: " + narrator.name);
        // Debug.Log("TextboxEnglish: " + textboxEnglish.name);
        // Debug.Log("TextBoxArabic: " + textBoxArabic.name);
        // Debug.Log("SeatbeltSign: " + seatbeltSign.name);
        // Debug.Log("SeatButtonsPanel: " + seatButtonsPanel.name);
    }

    private void ToggleWindShieldBoxes(bool e_box, bool a_box){ // FALSE: English, TRUE: Arabic
        
        textboxEnglish.SetActive(e_box);
        textBoxArabic.SetActive(a_box);
    }

    private void SetWindSheildText(bool isDefault, string EnglishText, string ArabicText){ // FALSE: English, TRUE: Arabic
        if(!isDefault){
            Debug.Log("Set ENGLISH Textbox");
            textboxEnglish.GetComponent<TMP_Text>().text = EnglishText;
        }
        else{
            Debug.Log("Set ARABIC Textbox");
            textBoxArabic.GetComponent<TMP_Text>().text = ArabicText;
        }
    }

    void DebugArray(string[] arr){
        for(int i = 0; i < arr.Length; i++){
            Debug.Log("Array["+i+"]: "+arr[i]);
        }
    }

    private void MainGameLoop(){
        // WELCOME MESSAGE [1]
        WelcomeMessage();// [1]
        // WAIT X SECONDS [2]
        StartCoroutine(WaitForSec(getSpeechDuration(), ChooseBackSeat));// [2]
        // // CHOOSE BACKSEAT COLOR [3]
        
        // // FASTEN SEATBELTS [4]
        // FastenSeatbelts();
        // // PEDESTRIAN CROSSING ROAD [5]
        // PedestrianWalk();
    }

    private void WelcomeMessage(){
        ToggleWindShieldBoxes(!isLanguageDefault, isLanguageDefault);
        SetWindSheildText(isLanguageDefault, gameEventTexts.welcomeText[0], gameEventTexts.welcomeText[1]);
        //Audio Here 
        PlaySpeechAudio(isLanguageDefault, 0);
        startGame = true;
        
    }

    private void PlaySpeechAudio(bool isDefault, int index){
        GameAudioSource.Stop();
        GameAudioSource.loop = false;
        if(!isDefault){
            GameAudioSource.clip = EnglishAudios[index];
            GameAudioSource.Play();
        }
        else{
            GameAudioSource.clip = ArabicAudios[index];
            GameAudioSource.Play();
        }
    }

    private float getSpeechDuration(){
        return GameAudioSource.clip.length + 0.5f;
    }

    private void ChooseBackSeat(){
        seatButtonsPanel.SetActive(true);
        SetWindSheildText(isLanguageDefault, gameEventTexts.backseatText[0], gameEventTexts.backseatText[1]);
        // Audio Here
        PlaySpeechAudio(isLanguageDefault, 1);
        canChooseSeat = true;
    }

    public void setDistanceTravelled(float distance){ // 5
        distanceTravelled = distance;
        PedestrianWalk(); 
    }
    private void PedestrianWalk(){ //5.1
        PlaySpeechAudio(isLanguageDefault, 3);
        StartCoroutine(WaitForSec(getSpeechDuration(), ()=>{
            phone.SetActive(true);
            phone.GetComponent<AudioSource>().Play();
        }));
        if(distanceTravelled != 0 && distanceTravelled < 139){
            canDrive = false;
        }
        
        // Animator am = Pedestrian.GetComponent<Animator>();
        Pedestrian.GetComponent<Animator>().SetTrigger("canWalk");

        if(!canDrive){
            StartCoroutine(WaitForSec(4, ()=>{
                // GameObject car = BabySeat.transform.parent.GetChild(0).gameObject;
                // Debug.Log("CarName: "+car.name);
                // car.transform.position = new Vector3(0,0,0);
                // car.transform.rotation = Quaternion.identity;
                canDrive = true;
                phone.GetComponent<AudioSource>().Stop();
                phone.SetActive(false);
                GameAudioSource.Stop();
                GameAudioSource.clip = SFX_CarEngine;
                GameAudioSource.loop = true;
                GameAudioSource.Play();
                phone.GetComponent<AudioSource>().Stop();
                StopAllCoroutines();
            }));
        }
        
    }



    private void ShowSeatBeltInstruction(){ // 4
        SetWindSheildText(isLanguageDefault, gameEventTexts.seatbeltText[0], gameEventTexts.seatbeltText[1]);
        // Audio Here
        PlaySpeechAudio(isLanguageDefault, 2);
        seatbeltSign.SetActive(true);
        isSeatBeltOn = true;
        StartCoroutine(WaitForSec(getSpeechDuration(), FastenSeatbelts));

    }

    private void FastenSeatbelts(){ //4.1
        if(!isSeatBeltOn) return;
        StopCoroutine("WaitForSec");
        gamePanels.carPanel.SetActive(false);
        Destroy(gamePanels.carPanel); // Destroy Panel
        Animation ctx = SeatBelt.GetComponent<Animation>();
        ctx.Play(ctx.clip.name);

        GameAudioSource.clip = SFX_Seatbelt;
        GameAudioSource.Play();
        isSeatBeltOn = false;
        GameAudioSource.clip = SFX_CarIgnition;
        GameAudioSource.Play();
        StartCoroutine(WaitForSec(SFX_CarIgnition.length, StartCarEnigine));
    }

    private void StartCarEnigine(){ //4.2
        StopCoroutine("WaitForSec");
        GameAudioSource.clip = SFX_CarEngine;
        GameAudioSource.loop = true;
        GameAudioSource.Play();
        canDrive = true;
        Debug.Log("CanDrive!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! : "+canDrive);
    }

    private void ChooseBackSeatButton(){ // [3]
        if(!BabySeat.activeSelf){
            if(Input.GetKey(KeyCode.Alpha1)){
                SetSeatMaterial(0);
                
            }
            if(Input.GetKey(KeyCode.Alpha2)){
                SetSeatMaterial(1);
            }
            if(Input.GetKey(KeyCode.Alpha3)){
                SetSeatMaterial(2);
            }
            
        }
        
    }

    private void SetSeatMaterial(int index){ // [3.1]
        BabySeat.GetComponentInChildren<MeshRenderer>().material = BackSeatMaterials[index];
        BabySeat.SetActive(true);
        canChooseSeat = false;
        seatButtonsPanel.SetActive(false);

        ShowSeatBeltInstruction();
        
    }

    public void ClickedEnglish(){
        Debug.Log("English");
        isLanguageDefault = false;
        TogglePanels(false, true);
        MainGameLoop();
        
    }

    public void ClickedArabic(){
        Debug.Log("Arabic");
        isLanguageDefault = true;
        TogglePanels(false, true);
        MainGameLoop();
    }

    public void OnStarsClicked(){
        int name = (EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex());
        
        for(int i = 0; i < name + 1; i++){
            Stars.transform.GetChild(i).GetComponent<Image>().color = Color.yellow;
        }

        StartCoroutine(WaitForSec(2, ()=>{
            Application.Quit();
        }));
        
    }


    // Timers

    private IEnumerator WaitForSec(float timer, callBackTimer callBack){

        yield return new WaitForSeconds(timer);
        callBack?.Invoke();
    }
}
