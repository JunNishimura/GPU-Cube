using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUCube
{
    public class GPUCube3D : GPUCubeBase
    {
        /// <summary>
        /// Execute Init kernel
        /// </summary>
        protected override void ExecuteInitKernel()
        {
            int kernel = CubeCS.FindKernel("Init");

            CubeCS.SetVector("_CubeNumXYZ", CubeNumEachDir);
            CubeCS.SetVector("_PosStepXYZ", PosStep);
            CubeCS.SetVector("_ArangeCenterXYZ", ArangeCenter);
            CubeCS.SetBuffer(kernel, "_CubeBuffer", cubeBuffer);
            CubeCS.Dispatch(kernel, threadGroupSize.x, threadGroupSize.y, threadGroupSize.z);
        }

        /// <summary>
        /// Execute Update kernel
        /// </summary>
        protected override void ExecuteUpdateKernel()
        {
            int kernel = CubeCS.FindKernel("Update");

            CubeCS.SetFloat("_TimeSpeed", TimeSpeed);
            CubeCS.SetFloat("_ElapsedTime", elapsedTime);
            CubeCS.SetVector("_CubeNumXYZ", CubeNumEachDir);
            CubeCS.SetVector("_CubeSizeXYZ", CubeSize);
            CubeCS.SetBuffer(kernel, "_CubeBuffer", cubeBuffer);
            CubeCS.Dispatch(kernel, threadGroupSize.x, threadGroupSize.y, threadGroupSize.z);
        }
    }
}