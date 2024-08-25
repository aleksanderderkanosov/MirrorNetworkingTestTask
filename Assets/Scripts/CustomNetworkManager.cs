using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        if (player.TryGetComponent(out NetworkPlayer networkPlayer)) {
            networkPlayer.SpawnPosition = startPos;
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        if (conn.identity.TryGetComponent(out NetworkPlayer networkPlayer)) {
            networkPlayer.SpawnPosition = null;
        }

        base.OnServerDisconnect(conn);
    }
}
