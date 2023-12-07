using System.Collections.Generic;
using Scripts.BehaviorTree;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class SoundSystem: ISystem
    {
        public EventBus EventBus;
        public SoundCollection SoundCollection;
        public Scenario Scenario;
        public AudioListener AudioListener;
        public UIView UIView;

        public List<Character> Characters;

        private LinkedList<AudioSourceHolder> SourceCollection = new LinkedList<AudioSourceHolder>();
        private Character _player = null;
        
        public void EventCatch(StartEvent newEvent)
        {
            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.PLAYER)
                {
                    _player = character;
                }
            }

            EventBus.CallEvent(new PlaySoundEvent() { SoundId = 0, IsMusic = true, AttachedTo = _player.transform, Position = _player.transform.position });
        }

        public void EventCatch(FixedUpdateEvent newEvent) {
            var node = SourceCollection.First;
            while (node != null)
            {
                var nodeNext = node.Next;
                var source = node.Value;
                if (source.AttachedTo != null)
                {
                    source.Source.transform.position = source.AttachedTo.position;
                }
                node = nodeNext;
            }

            AudioListener.transform.position = _player.transform.position;
        }

        public void EventCatch(SetSoundVolumeEvent newEvent)
        {
            var internalState = (ScenarioState) Scenario.BehaviorTreeRunner.InternalState;

            if (newEvent.IsMusic)
            {
                internalState.MusicVolume = newEvent.Volume;
            }
            else
            {
                internalState.SoundVolume = newEvent.Volume;
            }

            UIView.SetVolume(internalState.SoundVolume, internalState.MusicVolume);

            var node = SourceCollection.First;
            while (node != null)
            {
                var nodeNext = node.Next;
                var source = node.Value;
                if (source.IsMusic == newEvent.IsMusic) {
                    source.Source.volume = newEvent.Volume;
                }
                node = nodeNext;
            }
        }

        public void EventCatch(PlaySoundEvent newEvent)
        {
            var internalState = (ScenarioState) Scenario.BehaviorTreeRunner.InternalState;

            var node = SourceCollection.First;
            while (node != null)
            {
                var nodeNext = node.Next;
                var source = node.Value;
                if (source.SoundId == newEvent.SoundId && source.IsMusic == newEvent.IsMusic && !source.Source.isPlaying) {
                    source.Source.transform.position = newEvent.Position;
                    source.AttachedTo = newEvent.AttachedTo;
                    source.Source.Play();
                    return;
                }
                node = nodeNext;
            }

            var clipDescription = SoundCollection.GetClip(newEvent.SoundId, newEvent.IsMusic);

            if (clipDescription == null)
            {
                return;
            }

            var newSource = new AudioSourceHolder()
            {
                Source = new GameObject("AudioSource").AddComponent<AudioSource>(),
                SoundId = newEvent.SoundId,
                AttachedTo = newEvent.AttachedTo,
                IsMusic = newEvent.IsMusic
            };
            newSource.Source.transform.position = newEvent.Position;
            newSource.Source.clip = clipDescription.Clip;
            newSource.Source.volume = clipDescription.IsMusic ? internalState.MusicVolume : internalState.SoundVolume;
            newSource.Source.loop = newSource.IsMusic;
            newSource.Source.spatialBlend = newSource.IsMusic ? 0f : 1f;
            newSource.Source.Play();
            SourceCollection.AddLast(newSource);
        }

        public void EventCatch(RemoveAttachedSoundsEvent newEvent)
        {
            ClearAttachedSounds(newEvent.AttachedTo);
        }

        private void ClearAttachedSounds(Transform transform)
        {
            if (transform != null)
            {
                return;
            }

            var node = SourceCollection.First;
            while (node != null)
            {
                var nodeNext = node.Next;
                var source = node.Value;
                if (source.AttachedTo.Equals(transform))
                {
                    SourceCollection.Remove(node);
                }
                node = nodeNext;
            }
        }
    }
}