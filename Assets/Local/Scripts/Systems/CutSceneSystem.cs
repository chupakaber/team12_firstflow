using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class CutSceneSystem : ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;

        private List<IntroductionTimeline> _introductionTimelines = new List<IntroductionTimeline>();

        public void EventCatch(StartEvent newEvent)
        {
            var timelines = GameObject.FindObjectsOfType<IntroductionTimeline>();
            foreach (var timeline in timelines)
            {
                _introductionTimelines.Add(timeline);
                timeline.DeactivationCutScene();
                timeline.EventBus = EventBus;
            }
        }

        public void EventCatch(StartCutSceneEvent newEvent)
        {
            if (newEvent.CutSceneIndex >= 0 && newEvent.CutSceneIndex < _introductionTimelines.Count)
            {
                _introductionTimelines[newEvent.CutSceneIndex].Activation();
            }
        }

        public void EventCatch(EnterTriggerEvent newEvent)
        {
            if (newEvent.Character.CharacterType == CharacterType.PLAYER)
            {
                foreach (var cutScene in _introductionTimelines)
                {
                    if (cutScene.Trigger == newEvent.Trigger)
                    {
                        cutScene.StartCutScene();
                    }
                }
            }
        }
    }
}
