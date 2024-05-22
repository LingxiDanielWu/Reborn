using UnityEngine;

//先借用MonoSingleton，方便调试
public class GameConfig : MonoSingleton<GameConfig>
{
    [Range(0, 20)]
    public float NORMAL_RUNNING_SPEED = 3f;
    [Range(-20, 9.8f)]
    public float GRAVITY_ACCELATION = -9.8f;
    [Range(0, 20)]
    public float JUMP_SPEED = 5f;
    [Range(0, 1f)]
    public float HORIZONTAL_MOVE_DRAG_DURING_JUMP = 0.8f;
    [Range(0, 100f)] 
    public float JUMPING_CHANGE_HORIZONTAL_DIR_FACTOR = 1.6f;

    [Range(0, 100f)] public float CAMERA_ROTATE_SPEED = 50f;
    [Range(0, 100f)] public float CAMERA_FOLLOW_ROTATE_SPEED = 0.5f;
    [Range(0, 10f)] public float CHARACTER_TURN_DIR_SPEED = 0.12f;
    [Range(0, 50f)] public float MIN_DISTANCE_FROM_CAMERA_TO_CHARACTER = 1.3f;
    [Range(-90f, 0f)] public float MIN_CAMERA_PIVOT_Y = -3.5f;
    [Range(0f, 90f)] public float MAX_CAMERA_PIVOT_Y = 60f;
    [Range(0f, 10f)] public float CAMERA_ROTATE_LERP_SPEED = 8f;

    [Range(0f, 1f)] public float COMMON_ACTION_DELAY_TIME = 0.15f;
    [Range(0f, 1f)] public float SPEED_ATTACK_MOVE = 0.15f;
    public static float FRAME = 1 / 60f; 
    
    public override void Init()
    {
        DontDestroyOnLoad(this);
    }

    public override void Dispose()
    {
    }
}
