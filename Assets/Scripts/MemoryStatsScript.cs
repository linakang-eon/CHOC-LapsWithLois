using System.Text;
using Unity.Profiling;
using UnityEngine;

public class MemoryStatsScript : MonoBehaviour
{
    string statsText;
    ProfilerRecorder totalUsedMemoryRecorder;
    ProfilerRecorder totalReservedMemoryRecorder;
    ProfilerRecorder gcReservedMemoryRecorder;
    ProfilerRecorder systemUsedMemoryRecorder;

    void OnEnable()
    {
        totalUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
        totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");
        gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        systemUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
    }

    void OnDisable()
    {
        totalUsedMemoryRecorder.Dispose();
        totalReservedMemoryRecorder.Dispose();
        gcReservedMemoryRecorder.Dispose();
        systemUsedMemoryRecorder.Dispose();
    }

    void Update()
    {
        var sb = new StringBuilder(500);
        if (totalUsedMemoryRecorder.Valid)
            sb.AppendLine($"Total Used Memory: {totalUsedMemoryRecorder.LastValue}");
        if (totalReservedMemoryRecorder.Valid)
            sb.AppendLine($"Total Reserved Memory: {totalReservedMemoryRecorder.LastValue}");
        if (gcReservedMemoryRecorder.Valid)
            sb.AppendLine($"GC Reserved Memory: {gcReservedMemoryRecorder.LastValue}");
        if (systemUsedMemoryRecorder.Valid)
            sb.AppendLine($"System Used Memory: {systemUsedMemoryRecorder.LastValue}");
        statsText = sb.ToString();
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(10, 50, 250, 100), statsText);
    }
}