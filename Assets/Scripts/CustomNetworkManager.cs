using Mirror;
using System.Linq;
using UnityEngine;

/// <summary>
/// NetworkManager with some overriden methods.
/// </summary>
public class CustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        Transform startPos = GetStartPosition();
        UnRegisterStartPosition(startPos);
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        // Reserve a start position for the new player
        if (player.TryGetComponent(out NetworkPlayer networkPlayer)) {
            networkPlayer.SpawnPosition = startPos;
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        // Release the position of the disconnected player.
        if (conn.identity.TryGetComponent(out NetworkPlayer networkPlayer)) {
            networkPlayer.SpawnPosition = null;
        }

        base.OnServerDisconnect(conn);
    }

    public override Transform GetStartPosition() {
        // first remove any dead transforms
        startPositions.RemoveAll(t => t == null);

        if (startPositions.Count == 0)
            return null;

        if (playerSpawnMethod == PlayerSpawnMethod.Random) {
            return startPositions[Random.Range(0, startPositions.Count)];
        }
        else {
            return startPositions.FirstOrDefault();
        }
    }
}
