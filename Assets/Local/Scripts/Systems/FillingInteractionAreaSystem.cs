using System.Collections.Generic;
using Unity.VisualScripting;
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
            FillingZone(newEvent.MeshRenderer);
        }

        public void FillingZone(MeshRenderer meshRenderer)
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(materialPropertyBlock);
            var valueID = Shader.PropertyToID("_Value");
            materialPropertyBlock.SetFloat(valueID, Time.time);
            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}
