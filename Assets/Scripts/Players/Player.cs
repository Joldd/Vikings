
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    [SerializeField] protected SOGameSetup gameSetup;
    
    public SOGameSetup GameSetup { get => gameSetup; set => gameSetup = value; }

}
