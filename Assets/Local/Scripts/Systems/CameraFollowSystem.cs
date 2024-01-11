using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class CameraFollowSystem: ISystem
    {
        public Camera Camera;
        public Scenario Scenario;
        public List<Character> Characters;

        public void EventCatch(UpdateEvent newEvent)
        {
            foreach (Character character in Characters) 
            {
                if (character.CharacterType == Enums.CharacterType.PLAYER && character.IsCutSceneActiv == false) 
                {
                    var newPosition = character.transform.position + Scenario.BaseCameraOffset;
                    newPosition = Vector3.Lerp(Camera.transform.position, newPosition, Time.deltaTime * 10f);
                    Camera.transform.position = newPosition;
                }
            }
        }

        public void EventCatch(StartEvent newEvent)
        {
            Scenario.BaseCameraOffset = Camera.transform.position;
        }
    }
}
