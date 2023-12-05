using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;

namespace Scripts.CustomCompositeInput
{
    public struct TouchInput
    {
        public int TouchId;
        public Vector2 Position;
    }

    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public class TouchInputComposite : InputBindingComposite<TouchInput>
    {
        [InputControl(layout = "Vector2")]
        public int Position;

        [InputControl(layout = "int")]
        public int TouchId;

        public override TouchInput ReadValue(ref InputBindingCompositeContext context)
        {
            var touchId = context.ReadValue<int>(TouchId);
            var position = context.ReadValue<Vector2, Vector2MagnitudeComparer>(Position);


            return new TouchInput {
                TouchId = touchId,
                Position = position
            };
        }

        #if UNITY_EDITOR
        static TouchInputComposite()
        {
            Register();
        }
        #endif

        [RuntimeInitializeOnLoadMethod]
        private static void Register()
        {
            InputSystem.RegisterBindingComposite<TouchInputComposite>();
        }
    }
}