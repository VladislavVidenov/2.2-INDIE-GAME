using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

    int maxHealth;
    float health;
    float stamina;
    int maxStamina;
    int bits;

    int bitsBoost;

    int screwUpgBoost;
    int hammerUpgBoost;
    int wrenchUpgBoost;

    float healthRegenRate = 100f;
    float healthRegenDelay = 10.0f;
    float timeNotHit;
    float healthRegenAmount;

    float staminaRegenRate = 100f;
    float staminaRegenDelay = 5.0f;
    [HideInInspector] public float timeNotRun; //public to set it in PlayerMovement
    float staminaRegenAmount;

    [SerializeField] Image hp75;
    [SerializeField] Image hp50;
    [SerializeField] Image hp25;
    [SerializeField] Image hitIndicator;

    Vector3 originalPos;
    [HideInInspector]
    public float IndicatorAlpha = 0.01f;
    [HideInInspector]
    public GameObject hitter;

    Camera mainCamera;

    SceneChangeManager sceneManager;

    [Tooltip("Reference to the Bits HUD")]
    [SerializeField] GameObject bitsHud;
    Text bitsText;

    [Tooltip("Reference to the Ingame HUD")]
    [SerializeField] HudScript inGameHud;

    Tool currentTool;

    float cutOffBits;

    [SerializeField]
    Transform respawnTransform;

    [SerializeField]
    string levelName = "";

    AudioSource audioSource;
    [SerializeField]AudioClip[] playerSounds;

    // Use this for initialization
    void Start () {
        originalPos = hitIndicator.transform.position;

        audioSource = GetComponent<AudioSource>();

        bitsText = bitsHud.GetComponentInChildren<Text>();
        DeactiveBitsHud();
        mainCamera = Camera.main;
        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneChangeManager>();

        GetPlayerStatsFromGameManager();
        UpdateHealthHud();
        UpdateStaminaHud();
    }

    void Update() {
        RegenHealth();
        RegenStamina();
        DeathHud();
        UpdateIndicator();
    }

    void UpdateIndicator() {
        if (hitter != null) {
            ShowHitCircle(hitter);
        }
        if (IndicatorAlpha > 0) {

            IndicatorAlpha -= 0.01f;
            hitIndicator.color = new Color(1, 1, 1, IndicatorAlpha);
        } else {
            hitter = null;
        }
    }

    void DeathHud() {
        hp75.color = new Color(1, 1, 1, ((float)maxHealth - (float)health) / 25);
        hp50.color = new Color(1, 1, 1, ((float)75 - (float)health) / 25);
        hp25.color = new Color(1, 1, 1, ((float)50 - (float)health) / 25);
    }

    void RegenHealth() {
        timeNotHit += Time.deltaTime;

        if (timeNotHit > healthRegenDelay) {
            healthRegenAmount += 0.2f;
            if (healthRegenAmount >= 1f) {
                ChangeHealth(1 * healthRegenRate * Time.deltaTime);
                healthRegenAmount = 0;
            }
        }
    }

    void RegenStamina() {
        timeNotRun += Time.deltaTime;
        if (timeNotRun > staminaRegenDelay) {
            staminaRegenAmount += 0.2f;
            if (staminaRegenAmount >= 1f) {
                ChangeStamina(1 * staminaRegenRate * Time.deltaTime);
                staminaRegenAmount = 0;
            }
        }
    }

    public void ChangeHealth(float amount) {
        health += amount;
        if (health > maxHealth) health = maxHealth;

        inGameHud.PlayerHealth = health;

        if (health <= 0) { health = 0; inGameHud.PlayerHealth = 0; Died(); }
    }

    public void ChangeStamina(float amount) {
        //Debug.Log("STAMINA: " + stamina);
        stamina += amount;
        if (stamina > maxStamina) stamina = maxStamina;

        inGameHud.PlayerStamina = stamina;

        if (stamina <= 0) { stamina = 0; inGameHud.PlayerStamina = 0; }
    }

    public void ShowHitCircle(GameObject hit) {
        Vector3 difVector = hit.transform.position - this.transform.position;

        difVector.y = 0;
        float angleForward = Vector3.Angle(this.transform.forward, difVector);
        float angelRight = Vector3.Angle(this.transform.right, difVector);
        if (angelRight < 90) {
            hitIndicator.gameObject.transform.eulerAngles = new Vector3(0, 0, -angleForward + 45);
            hitIndicator.gameObject.transform.position = originalPos + (hitIndicator.gameObject.transform.up * 50 + hitIndicator.gameObject.transform.right * 50);
        } else {
            hitIndicator.gameObject.transform.eulerAngles = new Vector3(0, 0, angleForward + 45);
            hitIndicator.gameObject.transform.position = originalPos - (-hitIndicator.gameObject.transform.up * 50 - hitIndicator.gameObject.transform.right * 50);
        }

    }

    void UpdateHealthHud() {
        inGameHud.PlayerHealth = health;
        inGameHud.PlayerHealthCap = maxHealth;
    }

    void UpdateStaminaHud() {
        inGameHud.PlayerStamina = stamina;
        inGameHud.PlayerStaminaCap = maxStamina;
    }

    public void TakeDamage(int amount){
		ChangeHealth (-amount);
        timeNotHit = 0;
		//show direction
		//knockback?
	}

    public void Died() {
        PlayDeathSound();
        Invoke("Restart", 1f);
    }

    void Restart() {
        sceneManager.SetState(GameState.InMenu);
        Application.LoadLevel(levelName);
    }

    void Respawn() {
        sceneManager.SetState(GameState.InGame);
    }

    public bool HasStamina() {
        if (stamina > 0) return true;
        else return false;
    }
  
    void GetPlayerStatsFromGameManager() {
        maxHealth = GameManager.Instance.maxHealth;
        health = GameManager.Instance.health;
        stamina = GameManager.Instance.stamina;
        maxStamina = GameManager.Instance.maxStamina;
        bits = GameManager.Instance.bits;
    }

  
    public void GetCurrencyStats( out int pBits) {
        pBits = bits;
    }

    public void SetCurrencyStats( int pBits) {
        bits = pBits;
    }

    public void GetHealthStats(out float pHealth, out int pMaxHealth) {
        pHealth = health;
        pMaxHealth = maxHealth;
    }

    public void SetHealthStats(float pHealth, int pMaxHealth) {
        health = pHealth;
        maxHealth = pMaxHealth;
    }

    public void GetStaminaStats(out float pStamina, out int pMaxStamina) {
        pStamina = stamina;
        pMaxStamina = maxStamina;
    }

    public void SetStaminaStats(float pStamina, int pMaxStamina) {
        stamina = pStamina;
        maxStamina = pMaxStamina;
    }

    public void IncreasePlayerStats(float pHealth, int pMaxHealth, float pStamina, int pMaxStamina, int pBits) {
        if(pBits>0) IncreaseBitsHud(pBits);
        maxHealth += pMaxHealth;
        health += pHealth;
        stamina += pStamina;
        maxStamina += pMaxStamina;
        bits += pBits + bitsBoost;

        UpdateHealthHud();
        UpdateStaminaHud();
    }

    public void SetRegenDelay(float newRegenDelay) {
        healthRegenDelay = newRegenDelay;
    }

    void IncreaseBitsHud(int Amount) {
        if (bitsHud.gameObject.activeInHierarchy) {
            StopAllCoroutines();
            CancelInvoke("DeactiveBitsHud");
            StartCoroutine(IncreaseHud(cutOffBits, Amount + (bits - cutOffBits)));
        }
        else {
            bitsHud.SetActive(true);
            StartCoroutine(IncreaseHud(bits, Amount));
           
        }
    }

    public void DeactiveBitsHud() {
        bitsHud.SetActive(false);
    }

    public void ActivateBitsHud() {  //activates the bits hud with up2date amount
        StopAllCoroutines();
        bitsText.text = bits.ToString();
        bitsHud.SetActive(true);
    }

    public void SetCurrentTool(Tool tool)
    {
        currentTool = tool;
        SetBoostUpgrades();
    }
    public void PlayGotHitSound() {
        if(!audioSource.isPlaying){
            audioSource.volume = 0.2f;
            audioSource.PlayOneShot(playerSounds[0]);
        }
    }
    public void PlayDeathSound() {
        if (audioSource.isPlaying)
            audioSource.volume = 0.6f;
            audioSource.Stop();
        audioSource.PlayOneShot(playerSounds[1]);
    }
    void SetBoostUpgrades()
    {
        switch (currentTool.Type)
        {
            case ToolTypes.Screwdriver:
                break;

            case ToolTypes.Hammer:
                bitsBoost = currentTool.bitsBoost + hammerUpgBoost;
                break;

            case ToolTypes.Wrench:
                bitsBoost = currentTool.bitsBoost + wrenchUpgBoost;
                break;
        }
    }

    public void Respawning() {
        this.transform.position = respawnTransform.position;
    }

    IEnumerator IncreaseHud(float text, float Amount) {

        float stepTime = 0.02f;
        float addAmount = (float)Amount / 100;
        
        for (int i = 0; i < 100+1; i++) {
            yield return new WaitForSeconds(stepTime);
            float adding = addAmount * i;
            cutOffBits = (text + Mathf.Floor(adding));
            bitsText.text = cutOffBits.ToString();
            if (i > 60) stepTime = 0.03f;
            if (i > 70) stepTime = 0.04f;
            if (i > 80) stepTime = 0.05f;

            if (i > 99) Invoke("DeactiveBitsHud", 5);
        }
    }

}
