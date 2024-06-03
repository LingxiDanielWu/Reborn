using System.Collections.Generic;
using UnityEngine;

namespace EC.Manager
{
    public enum InputState
    {
        None,
        Down,
        Hold,
        Up
    }
    
    public enum InputType
    {
        None,
        Mouse,
        Key
    }

    public enum InputValue
    {
        None,
        MouseLeft,
        MouseMiddle,
        MouseRight,
        KeyW,
        KeyS,
        KeyA,
        KeyD,
        KeySpace
    }

    public class InputData
    {
        public readonly InputValue Value;
        public readonly InputType Type;
        public InputState State;

        public InputData(InputValue value, InputType type, InputState state)
        {
            Value = value;
            Type = type;
            State = state;
        }
    }

    public class InputManager : Singleton<InputManager>
    {
        private static Dictionary<InputValue, InputData> Inputs = new Dictionary<InputValue, InputData>
        {
            { InputValue.MouseLeft , new InputData(InputValue.MouseLeft, InputType.Mouse, InputState.None)},
            { InputValue.MouseRight , new InputData(InputValue.MouseRight, InputType.Mouse, InputState.None)},
            { InputValue.MouseMiddle , new InputData(InputValue.MouseMiddle, InputType.Mouse, InputState.None)},

            { InputValue.KeyW , new InputData(InputValue.KeyW, InputType.Key, InputState.None)},
            { InputValue.KeyS , new InputData(InputValue.KeyS, InputType.Key, InputState.None)},
            { InputValue.KeyA , new InputData(InputValue.KeyA, InputType.Key, InputState.None)},
            { InputValue.KeyD , new InputData(InputValue.KeyD, InputType.Key, InputState.None)},
            { InputValue.KeySpace , new InputData(InputValue.KeySpace, InputType.Key, InputState.None)},
        };

        private readonly List<KeyCode> USABLE_KEY_INPUT_VALUES = new List<KeyCode>
        {
            KeyCode.W,
            KeyCode.S,
            KeyCode.A,
            KeyCode.D,
            KeyCode.Space,
        };

        public bool HasInput
        {
            get;
            private set;
        }
        
        public float MouseMoveX
        {
            get;
            private set;
        }
        
        public float MouseMoveY
        {
            get;
            private set;
        }

        public override void Init()
        {
            MouseMoveX = 0;
            MouseMoveY = 0;
        }

        public override void Dispose()
        {

        }

        public Dictionary<InputValue, InputData> GetInputs()
        {
            return Inputs;
        }

        private bool IsTriggerAnyInput()
        {
            bool anyKeyHold = Input.anyKey;
            bool anyKeyDown = Input.anyKeyDown;
            bool anyKeyUp = IsAnyKeyUp();
            return anyKeyHold || anyKeyDown || anyKeyUp;
        }

        private bool IsAnyKeyUp()
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
            {
                return true;
            }

            for (int i = 0; i < USABLE_KEY_INPUT_VALUES.Count; i++)
            {
                if (Input.GetKeyUp(USABLE_KEY_INPUT_VALUES[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public override void Tick(float deltaTime)
        {
            if (IsTriggerAnyInput())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Inputs[InputValue.MouseLeft].State = InputState.Down;
                }
                
                if (Input.GetMouseButtonDown(1))
                {
                    Inputs[InputValue.MouseRight].State = InputState.Down;
                }
                
                if (Input.GetMouseButtonDown(2))
                {
                    Inputs[InputValue.MouseMiddle].State = InputState.Down;
                }
                
                if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
                {
                    Inputs[InputValue.MouseLeft].State = InputState.Hold;
                }
                
                if (Input.GetMouseButton(1) && !Input.GetMouseButtonDown(1))
                {
                    Inputs[InputValue.MouseRight].State = InputState.Hold;
                }
                
                if (Input.GetMouseButton(2) && !Input.GetMouseButtonDown(2))
                {
                    Inputs[InputValue.MouseMiddle].State = InputState.Hold;
                }
                
                if (Input.GetMouseButtonUp(0))
                {
                    Inputs[InputValue.MouseLeft].State = InputState.Up;
                }
                
                if (Input.GetMouseButtonUp(1))
                {
                    Inputs[InputValue.MouseRight].State = InputState.Up;
                }
                
                if (Input.GetMouseButtonUp(2))
                {
                    Inputs[InputValue.MouseMiddle].State = InputState.Up;
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    Inputs[InputValue.KeyW].State = InputState.Down;
                }
                
                if (Input.GetKeyDown(KeyCode.S))
                {
                    Inputs[InputValue.KeyS].State = InputState.Down;
                }
                
                if (Input.GetKeyDown(KeyCode.A))
                {
                    Inputs[InputValue.KeyA].State = InputState.Down;
                }
                
                if (Input.GetKeyDown(KeyCode.D))
                {
                    Inputs[InputValue.KeyD].State = InputState.Down;
                }
                
                if (Input.GetKey(KeyCode.W) && !Input.GetKeyDown(KeyCode.W))
                {
                    Inputs[InputValue.KeyW].State = InputState.Hold;
                }
                
                if (Input.GetKey(KeyCode.S) && !Input.GetKeyDown(KeyCode.S))
                {
                    Inputs[InputValue.KeyS].State = InputState.Hold;
                }
                
                if (Input.GetKey(KeyCode.A) && !Input.GetKeyDown(KeyCode.A))
                {
                    Inputs[InputValue.KeyA].State = InputState.Hold;
                }
                
                if (Input.GetKey(KeyCode.D) && !Input.GetKeyDown(KeyCode.D))
                {
                    Inputs[InputValue.KeyD].State = InputState.Hold;
                }
                
                if (Input.GetKeyUp(KeyCode.W))
                {
                    Inputs[InputValue.KeyW].State = InputState.Up;
                }
                
                if (Input.GetKeyUp(KeyCode.S))
                {
                    Inputs[InputValue.KeyS].State = InputState.Up;
                }
                
                if (Input.GetKeyUp(KeyCode.A))
                {
                    Inputs[InputValue.KeyA].State = InputState.Up;
                }
                
                if (Input.GetKeyUp(KeyCode.D))
                {
                    Inputs[InputValue.KeyD].State = InputState.Up;
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    Inputs[InputValue.KeySpace].State = InputState.Hold;
                }
                
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Inputs[InputValue.KeySpace].State = InputState.Down;
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    Inputs[InputValue.KeySpace].State = InputState.Up;
                }

                HasInput = true;
            }
            else
            {
                ResetInputs();
                HasInput = false;
            }
            
            MouseMoveX = Input.mousePositionDelta.x;
            MouseMoveY = Input.mousePositionDelta.y;
        }

        private void ResetInputs()
        {
            //todo:理论上把有改动的键位缓存起来更好
            foreach (var pair in Inputs)
            {
                pair.Value.State = InputState.None;
            }
        }

        //todo:键位查询方式感觉有些低效，需要优化
        public bool IsTriggerOpt(List<InputData> optInputs)
        {
            for (int i = 0; i < optInputs.Count; i++)
            {
                var input = optInputs[i];
                if (Inputs[input.Value].State != input.State)
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}