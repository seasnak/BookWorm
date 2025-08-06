using Godot;

namespace Bookworm.Utils;

public partial class GameUtils : Node
{

    public static bool CheckTimerComplete(float timer_start_time, int duration)
    {
        if (Time.GetTicksMsec() - timer_start_time >= duration)
        {
            return true;
        }
        return false;
    }

}
