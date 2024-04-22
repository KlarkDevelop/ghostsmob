using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public List<Player> players = new List<Player>();

    static public PlayersManager Singleton { get; set; }

    private void Start()
    {
        Singleton = this;
    }

    public void AddPlayer(Player player)
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

    public void RemovePlayer(Player player)
    {
        players.Remove(player);
    }
}
