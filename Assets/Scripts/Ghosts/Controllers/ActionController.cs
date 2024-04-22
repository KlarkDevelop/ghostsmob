using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionController : MonoBehaviour
{
    [SerializeField] private GameObject EMPSource;
    public static UnityEvent<EMPSignalSource> onAction = new UnityEvent<EMPSignalSource>();
    private Ghost _ghost;
    private int layerItemsId;

    public bool showActionGizmos = true;

    public void Init(Ghost ghost)
    {
        layerItemsId = LayerMask.GetMask("interactableObject");
        _ghost = ghost;
    }

    public void TryDoAction()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, _ghost.propereties.actionRange, layerItemsId);
        if (col.Length != 0)
        {
            GameObject item = col[UnityEngine.Random.Range(0, col.Length)].gameObject;

            //TODO: проверка на то какой это предмет для высчета шанса взаимодействия (передача в DoAction переменной с типом класса для роботы конеретной перегрузки)

            DoAction(item);
        }
    }


    public void DoAction(GameObject obj) //TODO: разделить логику в зависимости от того что это за предмет (перегрузкой метода) и зарефакторить этот метод
    {
        Rigidbody item = obj.GetComponent<Rigidbody>();

        List<Transform> players = new List<Transform>();

        foreach (Player player in PlayersManager.Singleton.players)
        {
            if (_ghost.currentRoom == player.currentRoom)
            {
                players.Add(player.transform);
            }
        }

        Vector3 throwVector = new Vector3();
        if (UnityEngine.Random.Range(0f, 1) <= _ghost.propereties.agrasiveness && players.Count != 0)
        {
            Transform nearestPlayer = players[0];
            foreach (Transform anotherPlayer in players)
            {
                if (Vector3.Distance(nearestPlayer.position, transform.position) > Vector3.Distance(anotherPlayer.position, transform.position))
                {
                    nearestPlayer = anotherPlayer;
                }
            }
            throwVector = (nearestPlayer.position - item.transform.position).normalized;
        }
        else
        {
            throwVector = new Vector3(UnityEngine.Random.Range(-1f, 1), UnityEngine.Random.Range(0f, 1), UnityEngine.Random.Range(-1f, 1));
        }
        Vector3 throwDirection = throwVector * _ghost.propereties.throwForce;

        item.AddForce(throwDirection, ForceMode.Impulse);
        EMPSignalSource source;
        if (item.TryGetComponent<EMPSignalSource>(out source))
        {
            source.UpdateTimer();
        }
        else
        {
            source = Instantiate(EMPSource, item.transform).GetComponent<EMPSignalSource>();
        }
        onAction.Invoke(source);
    }
}
