using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace DefaultNamespace.Utils
{
    public class VectorField
    {
        private float m_Freq = 1;
        private float m_RandomX = 0;
        private float m_RandomY = 0;

        public VectorField(float freq)
        {
            m_Freq = freq;
            m_RandomX = UnityEngine.Random.Range(-1000f, 1000f);
            m_RandomY = UnityEngine.Random.Range(-1000f, 1000f);
        }

        private float GetNoise(float3 input)
        {
            return Mathf.PerlinNoise(input.x * m_Freq, input.y * m_Freq);
        }
        public float3 Sample(float3 input)
        {
            var x = GetNoise(input);
            var y = GetNoise(input + new float3(m_RandomX, m_RandomY, 0));
            return new float3(x, y, 0);
        }
    }

    public static class VectorFieldTests
    {
        // create a menuitem
        // sample the points in the vector field, Debug.DrawRay the outputs
        [UnityEditor.MenuItem("Tests/231251231")]
        public static void Test()
        {
            var field = new VectorField(0.25f);
            for (float y = 0; y < 100; y += 1f)
            {
                for (float x = 0; x < 100; x += 1f)
                {
                    var input = new float3(x, y, 0);
                    var output = field.Sample(input);
                    var inputAsVec2 = new UnityEngine.Vector2(input.x, input.y);
                    var outputAsVec2 = new UnityEngine.Vector3(output.x, output.y);
                    Debug.DrawRay(inputAsVec2, outputAsVec2 * 1f, Color.red, 5.1f);
                }
            }
        }
    }
}