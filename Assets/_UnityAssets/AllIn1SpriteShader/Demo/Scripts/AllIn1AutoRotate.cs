using UnityEngine;

namespace AllIn1SpriteShader.Demo.Scripts
{
    public class AllIn1AutoRotate : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private bool useLocalRotation;

        private void Update()
        {
            transform.Rotate(rotationAxis * (rotationSpeed * Time.deltaTime), useLocalRotation ? Space.Self : Space.World);
        }
    }
}