using UnityEngine;
using Unity.Netcode;
public class PlayerNetwork : NetworkBehaviour
{
    private Vector3 vel;
    private float rotVel;
    [SerializeField] private float InterTime = 0.1f;

    private NetworkVariable<NetData> _netData = new NetworkVariable<NetData>(writePerm: NetworkVariableWritePermission.Owner);
    void Update()
    {
        if (IsOwner)
        {
            _netData.Value = new NetData()
            {
                position = transform.position,
                rotation = transform.rotation.eulerAngles
            };
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, _netData.Value.position, ref vel, InterTime);
            transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, _netData.Value.rotation.y, ref rotVel, InterTime), 0);
        }
    }

    struct NetData : INetworkSerializable
    {
        private float _x, _z;
        private float _yRot;

        internal Vector3 position
        {
            get => new Vector3(_x, 0, _z);
            set { _x = value.x; _z = value.z; }
        }

        internal Vector3 rotation
        {
            get => new Vector3(0, _yRot, 0);
            set => _yRot = value.y;
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _x);
            serializer.SerializeValue(ref _z);
            serializer.SerializeValue(ref _yRot);
        }
    }
}
