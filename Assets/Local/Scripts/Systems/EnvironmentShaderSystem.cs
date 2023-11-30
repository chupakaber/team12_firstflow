using UnityEngine;

namespace Scripts
{
    public class EnvironmentShaderSystem: ISystem
    {
        public Camera Camera;

        private int CameraWorldPositionShaderPropertyKey = Shader.PropertyToID("_CameraWorldPosition");

        public void EventCatch(UpdateEvent newEvent)
        {
            Shader.SetGlobalVector(CameraWorldPositionShaderPropertyKey, Camera.transform.position);
        }
    }
}
