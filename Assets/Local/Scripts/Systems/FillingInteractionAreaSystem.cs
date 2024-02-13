using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class FillingInteractionAreaSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public void EventCatch(PreparationForInteractionEvent newEvent) 
        {
            FillingZone(newEvent.MeshRenderer, newEvent.Disable);
        }

        public void FillingZone(MeshRenderer meshRenderer, bool disable)
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(materialPropertyBlock);
            var valueID = Shader.PropertyToID("_Value");
            materialPropertyBlock.SetFloat(valueID, disable ? -100f : Time.timeSinceLevelLoad);
            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}
