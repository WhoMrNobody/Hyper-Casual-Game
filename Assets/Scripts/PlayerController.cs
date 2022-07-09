using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    public static PlayerController Current;
    public float limitX;
    public float runningSpeed;
    public float speedX;
    float _currentRunningSpeed;
    public List<RidingCyclinder> cyclinders;
    public GameObject RidingCylinderPrefab;
    public Animator animator;
    public GameObject bridgePiecePrefab;
    public AudioSource cylinderAudioSource, triggerAudioSource, itemAudioSource;
    public AudioClip gatherAudioClip, dropAudioClip, coinAudioClip, buyItemClip, equipItemAudioClip, unequipItemAudioClip;
    public List<GameObject> wearSpots;
    private bool _spawningBridge;
    private BridgeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;
    private bool _finished;
    private float _scoreTimer=0f;
    private float _lastTouchedX;
    private float _dropSoundTimer;
    
    void Update()
    {   
        if(LevelController.Current== null || !LevelController.Current.gameActive){
            return;
        }

        float newX = 0f;
        float touchDeltaX=0f;

        if(Input.touchCount > 0){

            if(Input.GetTouch(0).phase == TouchPhase.Began){

                _lastTouchedX = Input.GetTouch(0).position.x;

            }else if(Input.GetTouch(0).phase == TouchPhase.Moved){
                
                touchDeltaX = 5 * ( Input.GetTouch(0).position.x - _lastTouchedX) / Screen.width;
                _lastTouchedX=Input.GetTouch(0).position.x;
            }

            touchDeltaX = Input.GetTouch(0).deltaPosition.x / Screen.width;

        }else if(Input.GetMouseButton(0)){

            touchDeltaX = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + speedX * touchDeltaX * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);

        Vector3 newPos = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position=newPos;

        if(_spawningBridge){

            PlayDropSound();
            _creatingBridgeTimer -= Time.deltaTime;
            if(_creatingBridgeTimer < 0){

                _creatingBridgeTimer = 0.01f;
                IncrementCyclinderVolume(-0.01f);
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab, this.transform);
                createdBridgePiece.transform.SetParent(null);
                Vector3 direction = _bridgeSpawner.endRefence.transform.position - _bridgeSpawner.startRefence.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                createdBridgePiece.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.startRefence.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.startRefence.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                createdBridgePiece.transform.position = newPiecePosition;

                if(_finished){

                    _scoreTimer -= Time.deltaTime;
                    if(_scoreTimer < 0){

                        _scoreTimer = 0.3f;
                        LevelController.Current.ChangeScore(1);
                    }
                }
            }

        }
    }

    public void ChangeCharacterSpeed(float value){

        _currentRunningSpeed=value;
    }

    void OnTriggerEnter(Collider other) {
        
        if(other.tag == "AddCyclinder"){

            cylinderAudioSource.PlayOneShot(gatherAudioClip, 0.1f);
            IncrementCyclinderVolume(0.1f);
            Destroy(other.gameObject);

        }else if(other.tag=="SpawnBridge"){

            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());

        }else if(other.tag == "StopSpawnBridge"){
            
            StopSpawningBridge();

            if(_finished){
                LevelController.Current.FinishGame();
            }
        }else if(other.tag == "Finish"){

            _finished=true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }else if(other.tag == "Coin"){

            triggerAudioSource.PlayOneShot(coinAudioClip, 0.1f);
            other.tag="Untagged";
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other) {
        
        if(LevelController.Current.gameActive){
            
            if(other.tag == "Trap"){
            PlayDropSound();
            IncrementCyclinderVolume(-Time.fixedDeltaTime);
        }
        }
        
    }

    public void IncrementCyclinderVolume(float value){

        if(cyclinders.Count == 0){

            if(value > 0){
                CreateCylinder(value);
            }else{
                
                if(_finished){
                    LevelController.Current.FinishGame();
                }else{
                   Die();
                }
            }

        }else{
            cyclinders[cyclinders.Count - 1].IncrementCyclinderVolume(value);
        }
    }

    public void Die(){

        animator.SetBool("dead", true);
        gameObject.layer=6;
        Camera.main.transform.SetParent(null);
        LevelController.Current.GameOver();
    }

    public void CreateCylinder(float value){

        RidingCyclinder createdCylinder = Instantiate(RidingCylinderPrefab, transform).GetComponent<RidingCyclinder>();
        cyclinders.Add(createdCylinder);
        createdCylinder.IncrementCyclinderVolume(value);
    }

    public void DestroyCylinder(RidingCyclinder cyclinder){

        cyclinders.Remove(cyclinder);
        Destroy(cyclinder.gameObject);
    }

    public void StartSpawningBridge(BridgeSpawner spawner){

        _bridgeSpawner=spawner;
        _spawningBridge=true;

    }
    public void StopSpawningBridge(){
        _spawningBridge=false;
    }

    public void PlayDropSound(){

        _dropSoundTimer -= Time.deltaTime;
        if(_dropSoundTimer<0){

            _dropSoundTimer = 0.15f;
            cylinderAudioSource.PlayOneShot(dropAudioClip, 0.1f);
        }
    }
}
