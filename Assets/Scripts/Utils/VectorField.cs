using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace DefaultNamespace.Utils
{
    public struct VectorFieldConfig
    {
        public float m_Freq;
        public float m_RandomX;
        public float m_RandomY;

        public static VectorFieldConfig Default => new VectorFieldConfig(0.25f, 123.2412312f, -732.123123f);
        public VectorFieldConfig(float freq, float randomX, float randomY)
        {
            m_Freq = freq;
            m_RandomX = randomX;
            m_RandomY = randomY;
        }
    }
    public class VectorField
    {
        private static float GetNoise(float3 input, VectorFieldConfig config)
        {
            return Mathf.PerlinNoise(input.x * config.m_Freq, input.y * config.m_Freq);
        }
        public static float3 Sample(float3 input, VectorFieldConfig config)
        {
            var x = GetNoise(input, config);
            var y = GetNoise(input + new float3(config.m_RandomX, config.m_RandomY, 0), config);
            return new float3(x, y, 0);
        }
    }

    /*
    public static class VectorFieldTests
    {
        // create a menuitem
        // sample the points in the vector field, Debug.DrawRay the outputs
        [UnityEditor.MenuItem("Tests/231251231")]
        public static void Test()
        {
            var vectorFieldConfig = VectorFieldConfig.Default;
            for (float y = 0; y < 100; y += 1f)
            {
                for (float x = 0; x < 100; x += 1f)
                {
                    var input = new float3(x, y, 0);
                    var output = VectorField.Sample(input, vectorFieldConfig);
                    var inputAsVec2 = new UnityEngine.Vector2(input.x, input.y);
                    var outputAsVec2 = new UnityEngine.Vector3(output.x, output.y);
                    Debug.DrawRay(inputAsVec2, outputAsVec2 * 1f, Color.red, 5.1f);
                }
            }
        }
    }
    */
}