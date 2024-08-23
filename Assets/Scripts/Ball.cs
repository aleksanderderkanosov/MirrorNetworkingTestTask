using UnityEngine;

public class Ball : MonoBehaviour
{
    private NetworkPlayer _owner;

    public NetworkPlayer Owner { get => _owner; set => _owner = value; }
}
