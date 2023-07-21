﻿using System;
using UnityEngine;

namespace Obi.MyScenes
{
    public class ObiCustomUpdater : ObiUpdater
    {
        /// <summary>
        /// Each FixedUpdate() call will be divided into several substeps. Performing more substeps will greatly improve the accuracy/convergence speed of the simulation. 
        /// Increasing the amount of substeps is more effective than increasing the amount of constraint iterations.
        /// </summary>
        [Tooltip("Amount of substeps performed per FixedUpdate. Increasing the amount of substeps greatly improves accuracy and convergence speed.")]
        public int substeps = 4;

        [NonSerialized] private float accumulatedTime;

        private void OnValidate()
        {
            substeps = Mathf.Max(1, substeps);
        }

        private void OnEnable()
        {
            accumulatedTime = 0;
        }

        private void OnDisable()
        {
            Physics.autoSimulation = true;
        }

        public void HandleFixedUpdate()
        {
            ObiProfiler.EnableProfiler();

            PrepareFrame();

            BeginStep(Time.fixedDeltaTime);

            float substepDelta = Time.fixedDeltaTime / (float)substeps;

            // Divide the step into multiple smaller substeps:
            for (int i = 0; i < substeps; ++i)
                Substep(Time.fixedDeltaTime, substepDelta, substeps-i);

            EndStep(substepDelta);

            ObiProfiler.DisableProfiler();

            accumulatedTime -= Time.fixedDeltaTime;
        }

        public void HandleUpdate()
        {
            accumulatedTime += Time.deltaTime;

            ObiProfiler.EnableProfiler();
            Interpolate(Time.fixedDeltaTime, accumulatedTime);
            ObiProfiler.DisableProfiler();
        }
    }
}