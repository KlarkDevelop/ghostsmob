using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public List<PlayerControler> players = new List<PlayerControler>();

    static public PlayersManager Singleton { get; set; }

    private void Start()
    {
        Singleton = this;
    }

    public void AddPlayer(PlayerControler player)
    {
        if (players.Count == 0)
        {
            player.inGameId = 1;
        }
        else
        {
            player.inGameId = players[players.Count - 1].inGameId + 1;
        }
        players.Add(player);
    }

    public void RemovePlayer(PlayerControler player)
    {
        players.Remove(player);
    }
}
