#define SUPPORT_2D //allows switching between 2D and 3D. No idea how to make this switchable in a more accessible way though. When needed, casts are done to Vector3 to make sure there is no data loss.

using UnityEngine;

namespace UnitySteer.Behaviors
{
    /// <summary>
    /// Parent class for objects that vehicles can aim for, be it other vehicles or
    /// static objects.
    /// </summary>
    [AddComponentMenu("UnitySteer/Detectables/DetectableObject")]
    public class DetectableObject : MonoBehaviour
    {
        private Transform _transform;

        [SerializeField] protected bool _drawGizmos = false;

        /// <summary>
        /// The vehicle's center in the transform
        /// </summary>
#if SUPPORT_2D
        [SerializeField] private Vector2 _center;
#else
        [SerializeField] private Vector3 _center;
#endif

        /// <summary>
        /// The vehicle's radius.
        /// </summary>
        [SerializeField] private float _radius = 1;


        /// <summary>
        /// Collider attached to this object. The GameObject that the DetectableObject
        /// is attached to is expected to have at most one collider.
        /// </summary>
#if SUPPORT_2D
        public Collider2D Collider { get; private set; }
#else
        public Collider Collider { get; private set; }
#endif

        /// <summary>
        /// Vehicle's position
        /// </summary>
        /// <remarks>The vehicle's position is the transform's position displaced 
        /// by the vehicle center</remarks>
#if SUPPORT_2D
        public Vector2 Position
        {
            get { return (Vector2)Transform.position + _center; }
        }
#else
        public Vector3 Position
        {
            get { return Transform.position + _center; }
        }
#endif

        /// <summary>
        /// Vehicle center on the transform
        /// </summary>
        /// <remarks>
        /// This property's setter recalculates a temporary value, so it's
        /// advised you don't re-scale the vehicle's transform after it has been set
        /// </remarks>
#if SUPPORT_2D
        public Vector2 Center
        {
            get { return _center; }
            set { _center = value; }
        }
#else
        public Vector3 Center
        {
            get { return _center; }
            set { _center = value; }
        }
#endif

        /// <summary>
        /// Vehicle radius
        /// </summary>
        /// <remarks>
        /// This property's setter recalculates a temporary value, so it's
        /// advised you don't re-scale the vehicle's transform after it has been set
        /// </remarks>
        public float Radius
        {
            get { return _radius; }
            set
            {
                _radius = Mathf.Clamp(value, 0.01f, float.MaxValue);
                SquaredRadius = _radius * _radius;
            }
        }

        /// <summary>
        /// Calculated squared object radius
        /// </summary>
        public float SquaredRadius { get; private set; }

        /// <summary>
        /// Cached transform for this behaviour
        /// </summary>
        public Transform Transform
        {
            get
            {
                // While this could be done using a ?? operator, that assignment/return
                // fails on Unity 4.3.4
                if (_transform == null)
                {
                    _transform = transform;
                }
                return _transform;
            }
        }

#region Methods

        protected virtual void Awake()
        {
#if SUPPORT_2D
            Collider = GetComponent<Collider2D>();
#else
            Collider = GetComponent<Collider>();
#endif
            SquaredRadius = _radius * _radius;
        }

        protected virtual void OnEnable()
        {
            if (Collider)
            {
                Radar.AddDetectableObject(this);
            }
        }

        protected virtual void OnDisable()
        {
            if (Collider)
            {
                Radar.RemoveDetectableObject(this);
            }
        }

        /// <summary>
        /// Predicts where the vehicle will be at a point in the future
        /// </summary>
        /// <param name="predictionTime">
        /// A time in seconds for the prediction <see cref="System.Single"/>. 
        /// Disregarded on the base function since obstacles do not move.
        /// </param>
        /// <returns>
        /// Object position<see cref="Vector2"/>
        /// </returns>
#if SUPPORT_2D
        public virtual Vector2 PredictFuturePosition(float predictionTime)
        {
            return Transform.position;
        }
#else
        public virtual Vector3 PredictFuturePosition(float predictionTime)
        {
            return Transform.position;
        }
#endif

        /// <summary>
        /// Recalculates the object's radius based on the transform's scale,
        /// using the largest of x/y/z as the scale value and multiplying it
        /// by a base.
        /// </summary>
        /// <param name="baseRadius">Base radius the object would have if the scale was 1</param>
        public void ScaleRadiusWithTransform(float baseRadius)
        {
            var scale = Transform.lossyScale;
            _radius = baseRadius * Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
        }

        protected virtual void OnDrawGizmos()
        {
            if (!_drawGizmos) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Position, Radius);
        }

#endregion
    }
}