using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private readonly NetworkVariable<PlayerNetworkData> _netState = new(writePerm: NetworkVariableWritePermission.Owner);
    private Vector3 _vel;
    private float _rotVel;
    [SerializeField] private float cheapInterpolationTime = 0.1f;

    private void Update()
    {
        if (IsOwner)
        {
            _netState.Value = new PlayerNetworkData()
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles,
                ScaleY = transform.localScale.y,
            };
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, _netState.Value.Position, ref _vel, cheapInterpolationTime);
            transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, _netState.Value.Rotation.y, ref _rotVel, cheapInterpolationTime), 0);
            
            var scale = transform.localScale;
            scale.y = _netState.Value.ScaleY;
            transform.localScale = scale;
        }
    }

    struct PlayerNetworkData : INetworkSerializable
    {
        private float _x, _z;
        private short _rotY;
        private float _scaleY;

        internal Vector3 Position
        {
            get => new Vector3(_x, 0, _z);
            set
            {
                _x = value.x;
                _z = value.z;
            } 
        }

        internal Vector3 Rotation
        {
            get => new Vector3(0, _rotY, 0);
            set => _rotY = (short)value.y;
        }

        
        internal float ScaleY
        {
            get => _scaleY;
            set => _scaleY = value;
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _x);
            serializer.SerializeValue(ref _z);
            
            serializer.SerializeValue(ref _rotY);
            serializer.SerializeValue(ref _scaleY);
        }
    }
}
