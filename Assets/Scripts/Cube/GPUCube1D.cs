using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUCube
{
    public class GPUCube1D : GPUCubeBase
    {
        public float WaveHeight = 5f;

        /// <summary>
        /// Execute Init kernel
        /// </summary>
        protected override void ExecuteInitKernel()
        {
            int kernel = CubeCS.FindKernel("Init");

            CubeCS.SetInt("_CubeNumX", (int)CubeNumEachDir.x);
            CubeCS.SetFloat("_PosStep", PosStep.x);
            CubeCS.SetFloat("_ArangeCenter", ArangeCenter.x);
            CubeCS.SetBuffer(kernel, "_CubeBuffer", cubeBuffer);
            CubeCS.Dispatch(kernel, threadGroupSize.x, threadGroupSize.y, threadGroupSize.z);
        }

        /// <summary>
        /// Execute Update kernel
        /// </summary>
        protected override void ExecuteUpdateKernel()
        {
            int kernel = CubeCS.FindKernel("Update");

            CubeCS.SetInt("_CubeNumX", (int)CubeNumEachDir.x);
            CubeCS.SetFloat("_TimeSpeed", TimeSpeed);
            CubeCS.SetFloat("_ElapsedTime", elapsedTime);
            CubeCS.SetFloat("_WaveHeight", WaveHeight);
            CubeCS.SetVector("_CubeSize", CubeSize);
            CubeCS.SetBuffer(kernel, "_CubeBuffer", cubeBuffer);
            CubeCS.Dispatch(kernel, threadGroupSize.x, threadGroupSize.y, threadGroupSize.z);
        }
    }
}