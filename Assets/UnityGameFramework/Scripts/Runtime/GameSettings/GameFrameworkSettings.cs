using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GlobalSettings", menuName = "Game Framework/GlobalSettings")]
public class GameFrameworkSettings : ScriptableObject
{
    [Header("Framework")] [SerializeField] private FrameworkGlobalSettings m_FrameworkGlobalSettings;

    public FrameworkGlobalSettings FrameworkGlobalSettings
    {
        get { return m_FrameworkGlobalSettings; }
    }

    [Header("HybridCLR")] [SerializeField] private HybridCLRCustomGlobalSettings m_HybridCLRCustomGlobalSettings;

    public HybridCLRCustomGlobalSettings HybridClrCustomGlobalSettings => m_HybridCLRCustomGlobalSettings;
}
