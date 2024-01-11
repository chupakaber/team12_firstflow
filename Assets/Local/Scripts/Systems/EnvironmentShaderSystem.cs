using UnityEngine;

namespace Scripts
{
    public class EnvironmentShaderSystem: ISystem
    {
        public Camera Camera;
        
        private Light _light;
        private int CameraWorldPositionShaderPropertyKey = Shader.PropertyToID("_CameraWorldPosition");
        private int WorldSpaceLightPos0ShaderPropertyKey = Shader.PropertyToID("_WorldSpaceLightDirectionTest");

        public void EventCatch(StartEvent newEvent)
        {
            _light = Object.FindObjectOfType<Light>();
        }

        public void EventCatch(UpdateEvent newEvent)
        {
            Shader.SetGlobalVector(CameraWorldPositionShaderPropertyKey, Camera.transform.position);
            Shader.SetGlobalVector(WorldSpaceLightPos0ShaderPropertyKey, -_light.transform.forward);
        }
    }
}
