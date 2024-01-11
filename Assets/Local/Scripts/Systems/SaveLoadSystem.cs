using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scripts.Serialization;
using Scripts.BehaviorTree;

namespace Scripts
{
    public class SaveLoadSystem: ISystem
    {
        public EventBus EventBus;
        public Scenario Scenario;

        private ByteBuffer _byteBuffer = new ByteBuffer();
        private string _savePath;

        private const float SAVE_TIME_PERIOD = 5f;

        private float _lastSaveTime = 0f;

        public void EventCatch(StartEvent newEvent)
        {
            _savePath = $"{Application.persistentDataPath}/current.save";

            _byteBuffer = new ByteBuffer() { Pointer = 0 };

            Load();
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            if (Time.time - _lastSaveTime > SAVE_TIME_PERIOD)
            {
                _lastSaveTime = Time.time;

                Save();
            }
        }

/*
        public void EventCatch(SaveEvent newEvent)
        {
            Save();
        }

        public void EventCatch(LoadEvent newEvent)
        {
            Load();
        }
*/

        public void EventCatch(ClearGameProgressEvent newEvent)
        {
            ClearProgressAndRestart();
        }

        private void Load()
        {
            _lastSaveTime = Time.time;

            if (File.Exists(_savePath))
            {
                _byteBuffer.Data = File.ReadAllBytes(_savePath);
                _byteBuffer.Pointer = 0;
                var scenarioState = (ScenarioState) Scenario.BehaviorTreeRunner.InternalState;
                scenarioState.Unpack(_byteBuffer);
            }
        }

        private void Save()
        {
            var scenarioState = (ScenarioState) Scenario.BehaviorTreeRunner.InternalState;

            var dataLength = scenarioState.GetStructureLength();

            if (_byteBuffer.Data == null || _byteBuffer.Data.Length < dataLength)
            {
                _byteBuffer.Data = new byte[dataLength * 2];
            }

            _byteBuffer.Pointer = 0;

            scenarioState.Pack(_byteBuffer);

            File.WriteAllBytes(_savePath, _byteBuffer.Data);
        }

        private void ClearProgressAndRestart()
        {
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
            }

            SceneManager.LoadScene(0);
        }
    }
}