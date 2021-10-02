using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{

    [SerializeField] float _spendWoodSpeed = 50f;
    [SerializeField] float _buildSpeed = .5f;
    [SerializeField] GameObject _shineFX;
    
    ParticleSystem _particle;
    Transform _player;
    GameObject _woodPrefab;

    List<GameObject> _woodPieces = new List<GameObject>();
    int _activeWood = 0;

    bool _startBuilding = false;
    bool _levelDone = false;

    private void OnEnable()
    {
        _startBuilding = false;
        _woodPieces.Clear();
        _particle = _shineFX.GetComponent<ParticleSystem>();
        _activeWood = 0;
        BusSystem.OnNewLevelStart += HideAllPieces;
    }

    private void OnDisable()
    {
        BusSystem.OnNewLevelStart -= HideAllPieces;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Const.TAG_PLAYER))
        {
            if (GetComponentInChildren<Build>() != null)
            {
                if(!_levelDone)
                {
                    _levelDone = true;
                    BusSystem.CallLevelDone(true);
                    BusSystem.CallPerfectRound();
                    BusSystem.CallAddCash(100);
                    ShowPieces(true);
                }       
            }
        }
    }

    private void Update()
    {
        MoveWood(_activeWood);
    }

    public void ShowPieces(bool isWin)
    {
        if(isWin)
        {
            _player = GameObject.Find("Player").transform;
            _woodPrefab = Resources.Load<GameObject>("WoodPiece");
            _startBuilding = true;

            for(int i = 0; i < 24; i++)
            {
                CreateWood(); 
            }

            float time = _buildSpeed;

            foreach (Transform child in transform)
            {
                if (child.GetComponent<ParticleSystem>())
                    continue;

                child.gameObject.SetActive(true);

                foreach (Transform piece in child)
                {
                    FunctionTimer.Create(() => piece.gameObject.SetActive(true), time);
                    FunctionTimer.Create(() => _shineFX.transform.position = piece.position, time);
                    FunctionTimer.Create(() => _particle.Play(), time);
                    FunctionTimer.Create(() => BusSystem.CallSoundPlay(SoundEffects.PickIngot), time);
                    FunctionTimer.Create(() => GameManager.Instance.AddCash(-1), time);
                    time += _buildSpeed;
                } 
            }
        }
    }

    void HideAllPieces()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ParticleSystem>())
                continue;

            child.gameObject.SetActive(false);

            foreach (Transform piece in child)
                piece.gameObject.SetActive(false);
        }      
    }

    void CreateWood()
    {
        GameObject copyWood = Instantiate(_woodPrefab, transform.localPosition, Quaternion.identity);
        copyWood.GetComponent<Rigidbody>().isKinematic = true;
        copyWood.GetComponent<Rigidbody>().useGravity = false;
        copyWood.transform.localScale = Vector3.one * 3f;
        copyWood.transform.localPosition += new Vector3(0,.8f, -10);
        copyWood.transform.localEulerAngles += new Vector3(0, 90, 0);
        copyWood.transform.parent = GameManager.Instance.GetCurrentPlayzone().transform;
        _woodPieces.Add(copyWood);
    }

    void MoveWood(int wood)
    {
        if(_startBuilding)
        {
            if (Vector3.Distance(_woodPieces[wood].transform.position, transform.position) < .05f)
            {
                Destroy(_woodPieces[wood]);
                //_woodPieces.Remove();

                if (_activeWood < _woodPieces.Count - 1)
                    _activeWood++;
                else   
                    _startBuilding = false;
            }
            else
                _woodPieces[wood].transform.localPosition = Vector3.MoveTowards(_woodPieces[wood].transform.localPosition, transform.localPosition, _spendWoodSpeed * Time.deltaTime);
        }  
    }
}
