using Godot;
using System;

namespace Gmtk.Utils;
public static class Utils
{
    public enum EntityGroups { ENEMY, PLAYER };

    public static bool CheckTimerComplete(float timer_start_time, int duration)
    {
        if (Time.GetTicksMsec() - timer_start_time >= duration)
        {
            return true;
        }
        return false;
    }
}
